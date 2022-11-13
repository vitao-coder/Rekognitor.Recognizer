using DlibDotNet;
using DlibDotNet.Dnn;
using Recognizer.IOC;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Drawing;

namespace Recognizer.Dlib.Wrapper
{

     public class FacialDetection : IFacialDetection, IDisposable
    {
        ShapePredictor _shapePredictor;
        LossMetric _lossMetric;
        string _appFolder;
        bool _enableJittering;
        readonly IModelLoader _modelLoader;
        readonly FrontalFaceDetector _frontalFaceDetector;

        public FacialDetection(IModelLoader modelLoader, FrontalFacialDetector detector, ShapePrediction predictor, LossMetrics lossMetrics) {
            _appFolder = Environment.CurrentDirectory;
            _modelLoader = modelLoader;

            _shapePredictor = predictor.GetShapePredictor();
            _frontalFaceDetector = detector.GetFrontalFacialDetector();
            _lossMetric = lossMetrics.GetLossMetrics();
            _enableJittering = false;            
        }
        

        public float[]? FacialDetector(byte[] image, out string processMessage) {           
            var tempFileName = Guid.NewGuid().ToString();
            var filePath = _appFolder +@"/temp/"+ tempFileName + ".jpg";

            using (var fileStr = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write))
            {
                using (var fs = new BinaryWriter(fileStr))
                {
                    fs.Write(image);
                    fs.Close();
                }
                fileStr.Close();
            }

            Matrix<RgbPixel> faceExtracted;
            DlibDotNet.Rectangle faceDetected;

            try
            {
                float[]? returnFaceDesc = new float[128];
       
                using (var img = DlibDotNet.Dlib.LoadImageAsMatrix<RgbPixel>(filePath))
                {

                    DlibDotNet.Rectangle[] facesDetector;
                    lock (_frontalFaceDetector)
                    {
                        facesDetector = _frontalFaceDetector.Operator(img);                        
                    }

                    if (!facesDetector.Any())
                    {
                        processMessage = "face not encountered";
                        return null;
                    }

                    if (facesDetector.Length != 1)
                    {
                        processMessage = "multiple  encountered";
                        return null;
                    }

                    faceDetected = facesDetector[0];
                    FullObjectDetection shape;
                    lock (_shapePredictor) {
                        shape = _shapePredictor.Detect(img, faceDetected);                        
                    }
                    var faceChipDetail = DlibDotNet.Dlib.GetFaceChipDetails(shape, 150, 0.24);

                    faceExtracted = DlibDotNet.Dlib.ExtractImageChip<RgbPixel>(img, faceChipDetail);

                    faceChipDetail.Dispose();
                    shape.Dispose();
                }

                OutputLabels<Matrix<float>> faceDescriptors;
                lock (_lossMetric) {
                    faceDescriptors = _lossMetric.Operator(faceExtracted);
                    _lossMetric.Clean();
                }
                var faceDescriptor = faceDescriptors[0];

                if (_enableJittering)
                {
                    using (var jittered = DlibDotNet.Dlib.Trans(faceDescriptor))
                    {
                        var jitterImage = JitterImage(faceExtracted);
                        var ret = _lossMetric.Operator(jitterImage);
                        var m = DlibDotNet.Dlib.Mat(ret);
                        var faceDescriptoMat = DlibDotNet.Dlib.Mean<float>(m);

                        using (var t = DlibDotNet.Dlib.Trans(faceDescriptoMat))
                        {
                            jitterImage.Dispose();
                            faceDescriptor.Dispose();
                        }
                        ret.Dispose();
                        m.Dispose();
                        faceDescriptoMat.Dispose();
                    }
                }
                returnFaceDesc = faceDescriptor.ToImmutableArray().ToArray();
                faceDescriptor.Dispose();
                faceDescriptors.Dispose();                
                faceExtracted.Dispose();
                processMessage = "sucess";
                return returnFaceDesc;
            }
            catch (Exception ex)
            {
                throw;

            }
            finally
            {
                File.Delete(filePath);                
            }     
        }

        private static Matrix<RgbPixel> JitterImage(Matrix<RgbPixel> img)
        {      
            var rnd = new Rand();
            var crop = new Matrix<RgbPixel>();
            crop = DlibDotNet.Dlib.JitterImage(img, rnd);
            return crop;
        }

        public void Dispose()
        {
            GC.Collect();
        }
    }
}
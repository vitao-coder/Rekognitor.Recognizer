using DlibDotNet;
using DlibDotNet.Dnn;
using Recognizer.IOC;
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
        

        public float[]? FacialDetector(byte[] image) {
           // Stopwatch timer = new Stopwatch();
           // timer.Start();
            var tempFileName = Guid.NewGuid().ToString();
            var filePath = _appFolder +@"\temp\"+ tempFileName + ".jpg";

            var fileStr = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write);
            using (var fs = new BinaryWriter(fileStr))
            {
                fs.Write(image);
            }
            fileStr.Close();

            Matrix<RgbPixel> faceExtracted;
            DlibDotNet.Rectangle faceDetected;

            try
            {
                
                using (var img = DlibDotNet.Dlib.LoadImageAsMatrix<RgbPixel>(filePath))
                {
                    var facesDetector = _frontalFaceDetector.Operator(img);

                    if (!facesDetector.Any())
                    {
                        return null;
                    }

                    if (facesDetector.Length != 1)
                    {
                        return null;
                    }

                    faceDetected = facesDetector[0];                    
                    var shape = _shapePredictor.Detect(img, faceDetected);
                    
                    var faceChipDetail = DlibDotNet.Dlib.GetFaceChipDetails(shape,150,0.24);
                    
                    faceExtracted = DlibDotNet.Dlib.ExtractImageChip<RgbPixel>(img, faceChipDetail);
                    
                    faceChipDetail.Dispose();
                    shape.Dispose();                    
                }
                
                var faceDescriptors = _lossMetric.Operator(faceExtracted);
                var faceDescriptor = faceDescriptors[0];                

                var finalDescriptorWithoutJittering = DlibDotNet.Dlib.Trans(faceDescriptor);
                
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
                faceDescriptor.Dispose();
                faceDescriptors.Dispose();
                finalDescriptorWithoutJittering.Dispose();                               

                return new float[] { };
            }
            catch (Exception ex)
            {
                throw;

            }
            finally
            {   
                File.Delete(filePath);
                //Console.WriteLine("Detection total time " + timer.Elapsed.ToString() + "\n\n");
               // timer.Stop();
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
            /*_shapePredictor.Dispose();
            _frontalFaceDetector.Dispose();
            _lossMetric.Dispose();*/
        }

    }
}
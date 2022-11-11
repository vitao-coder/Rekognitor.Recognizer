using DlibDotNet;
using DlibDotNet.Dnn;
using Recognizer.IOC;
using System.Diagnostics;
using System.Drawing;

namespace Recognizer.Dlib.Wrapper
{

     public class FacialDetection : IFacialDetection, IDisposable
    {

        FrontalFaceDetector _frontalFaceDetector;
        ShapePredictor _shapePredictor;
        LossMetric _lossMetric;
        string _appFolder;
        bool _enableJittering;
        readonly IModelLoader _modelLoader;

        public FacialDetection(IModelLoader modelLoader) {
            _appFolder = Environment.CurrentDirectory;
            _modelLoader = modelLoader;

            _frontalFaceDetector = DlibDotNet.Dlib.GetFrontalFaceDetector();            
            _enableJittering = false;

            var shapePredictor = modelLoader.GetModel((int)AVAIABLE_MODELS.ShapePredictor68MarksGTX);
            _shapePredictor = ShapePredictor.Deserialize(shapePredictor.Data);

            var lossMetric = modelLoader.GetModel((int)AVAIABLE_MODELS.RessNet);
            _lossMetric = LossMetric.Deserialize(lossMetric.Data);
        }
        

        public float[]? FacialDetector(byte[] image) {
            Stopwatch timer = new Stopwatch();
            timer.Start();
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
                Console.WriteLine("Detection total time " + timer.Elapsed.ToString() + "\n\n");
                timer.Stop();
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
            _frontalFaceDetector.Dispose();
            _shapePredictor.Dispose();
            _lossMetric.Dispose();
        }

    }
}
using DlibDotNet;
using DlibDotNet.Dnn;
using Recognizer.IOC;
using Recognizer.IOC.Shared;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Recognizer.Dlib.Wrapper
{

     public class FacialDetection : IFacialDetection, IDisposable
    {
        readonly ShapePrediction _shapePrediction;
        readonly LossMetrics _lossMetrics;
        string _appFolder;
        bool _enableJittering;        
        readonly FrontalFacialDetector _frontalFacialDetector;

        public FacialDetection(FrontalFacialDetector detector, ShapePrediction predictor, LossMetrics lossMetrics) {
            _appFolder = Environment.CurrentDirectory;            
            _shapePrediction = predictor;
            _frontalFacialDetector = detector;
            _lossMetrics = lossMetrics;
            _enableJittering = false;            
        }
        

        public float[]? FacialDetector(byte[] image, out string processMessage) {

            var bitmapImage = getBitmapFromBytes(image);

            Matrix<RgbPixel> faceExtracted;
            DlibDotNet.Rectangle faceDetected;

            try
            {
                var _frontalFaceDetector = _frontalFacialDetector.GetFrontalFacialDetector();
                var _shapePredictor = _shapePrediction.GetShapePredictor();
                var _lossMetric = _lossMetrics.GetLossMetrics();

                float[]? returnFaceDesc = new float[128];
                               
                
                using (var img = LoadImageAsMatrixFromBitmap(bitmapImage))
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
                        processMessage = "multiple encountered";
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
                processMessage = "success";
                return returnFaceDesc;
            }
            catch (Exception ex)
            {
                throw;

            }
            finally
            {
                bitmapImage.Dispose();                
                //Console.WriteLine("Memory Load Bytes:" + GC.GetGCMemoryInfo().MemoryLoadBytes);
                //Console.WriteLine("Total Avaible Memory Bytes:" + GC.GetGCMemoryInfo().TotalAvailableMemoryBytes);
                //Console.WriteLine("Total Commited Bytes:" + GC.GetGCMemoryInfo().TotalCommittedBytes);
                if (GC.GetGCMemoryInfo().MemoryLoadBytes > 0) 
                    GC.AddMemoryPressure(GC.GetGCMemoryInfo().MemoryLoadBytes);
            }     
        }

        private static Matrix<RgbPixel> JitterImage(Matrix<RgbPixel> img)
        {      
            var rnd = new Rand();
            var crop = new Matrix<RgbPixel>();
            crop = DlibDotNet.Dlib.JitterImage(img, rnd);
            return crop;
        }

        private Bitmap? getBitmapFromBytes(byte[] image)
        {
            using (var ms = new MemoryStream(image))
            {
                return new Bitmap(ms);
            }

            return null;
        }

        private unsafe static Matrix<RgbPixel>? LoadImageAsMatrixFromBitmap(Bitmap bitmap)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, width, height);
            PixelFormat pixelFormat = bitmap.PixelFormat;
            Mode mode;
            int num;
            int num2;
            switch (pixelFormat)
            {
                case PixelFormat.Format8bppIndexed:
                    mode = Mode.Greyscale;
                    num = 1;
                    num2 = 1;
                    break;
                case PixelFormat.Format24bppRgb:
                    mode = Mode.Rgb;
                    num = 3;
                    num2 = 3;
                    break;
                case PixelFormat.Format32bppRgb:
                case PixelFormat.Format32bppArgb:
                    mode = Mode.Rgb;
                    num = 4;
                    num2 = 3;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("bitmap", "The specified PixelFormat is not supported.");
            }

            BitmapData bitmapData = null;
            try
            {
                bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, pixelFormat);
                byte[] array = new byte[width * height * num2];         
                fixed (byte* ptr = &array[0])
                {
                    byte* ptr2 = ptr;
                    switch (num)
                    {
                        case 1:
                            {
                                IntPtr scan = bitmapData.Scan0;
                                int stride2 = bitmapData.Stride;
                                for (int k = 0; k < height; k++)
                                {
                                    Marshal.Copy(IntPtr.Add(scan, k * stride2), array, k * width, width * num2);
                                }

                                break;
                            }
                        case 3:
                        case 4:
                            {
                                byte* ptr3 = (byte*)(void*)bitmapData.Scan0;
                                int stride = bitmapData.Stride;
                                for (int i = 0; i < height; i++)
                                {
                                    int num3 = i * stride;
                                    int num4 = i * width * num2;
                                    for (int j = 0; j < width; j++)
                                    {
                                        ptr2[num4 + j * num2] = ptr3[num3 + j * num + 2];
                                        ptr2[num4 + j * num2 + 1] = ptr3[num3 + j * num + 1];
                                        ptr2[num4 + j * num2 + 2] = ptr3[num3 + j * num];
                                    }
                                }

                                break;
                            }
                    }

                    IntPtr array2 = (IntPtr)ptr;
                    switch (mode)
                    {
                        case Mode.Rgb:
                            return new Matrix<RgbPixel>(array2, height, width, width * 3);
                        case Mode.Greyscale:
                            return null;
                    }
                }
           
            }
            finally
            {
                if (bitmapData != null)
                {
                    bitmap.UnlockBits(bitmapData);
                }
            }

            return null;
        }

        public enum Mode
        {
            Rgb,        
            Greyscale
        }

        public void Dispose()
        {
            
        }
    }
}
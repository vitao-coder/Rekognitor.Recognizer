namespace Recognizer.FaceRecognition.Wrapper;

using FaceRecognitionDotNet;
using Recognizer.IOC;
using Recognizer.IOC.Shared;
using System.Drawing;
using System.Runtime.Intrinsics.X86;
using System.Text;

public class FaceComparison : IFaceComparison, IDisposable
{
    /*readonly ModelParameters _modelParameters;
    readonly FaceRecognition _recognition;
    readonly FaceRecognitionDotNet.Model _mlModel;
    readonly PredictorModel _predictorModel;*/

    public FaceComparison(/*ModelParameters modelParameters*/)
    {
        //_modelParameters = modelParameters;
        //_recognition = _modelParameters.GetRecognition();
    }


    /*public bool FacialComparator(byte[] image1, byte[] image2, out string processMessage)
    {
        bool faceMathched = false;
        try
        {
            var img1 = getImageFromByte(image1);
            var img2 = getImageFromByte(image2);
            if (img1 != null && img2 != null)
            {
                var encodings1 = getFaceEncodings(img1).FirstOrDefault();
                img1.Dispose();
                var encodings2 = getFaceEncodings(img2).FirstOrDefault();                
                img2.Dispose();

                faceMathched = FaceRecognition.CompareFace(encodings1, encodings2, 0.75);
                encodings1.Dispose();
                encodings2.Dispose();
            }else
            {
                processMessage = "error - null images";
            }

            processMessage = "success";
            return faceMathched;
        }
        catch (ArgumentNullException)
        {
            processMessage = "error - argument exception";
            return false;
        }
        catch (ObjectDisposedException)
        {
            processMessage = "error - object is disposed ";
            return false;
        }
    }*/


    public bool FacialComparator(double[] descriptor1, double[] descriptor2, out string processMessage)
    {
        bool faceMathched = false;
        try
        {
            var encodings1 = FaceRecognition.LoadFaceEncoding(descriptor1);
            var encodings2 = FaceRecognition.LoadFaceEncoding(descriptor2);

            faceMathched = FaceRecognition.CompareFace(encodings1, encodings2, 0.75);
            encodings1.Dispose();
            encodings2.Dispose();    

            processMessage = "success";
            return faceMathched;
        }
        catch (ArgumentNullException)
        {
            processMessage = "error - argument exception";
            return false;
        }
        catch (ObjectDisposedException)
        {
            processMessage = "error - object is disposed ";
            return false;
        }
    }
    /*
    private FaceRecognitionDotNet.Image? getImageFromByte(byte[] image)
    {        
        using (var ms = new MemoryStream(image))
        {
            using (var bmp = new Bitmap(ms))
            {
                return FaceRecognition.LoadImage(bmp);
            }            
        }

        return null;
    }

    private IEnumerable<Location> getFaceLocations(FaceRecognitionDotNet.Image image)
    {
        return _recognition.FaceLocations(image, 1, _mlModel);
    }

    private IEnumerable<FaceEncoding> getFaceEncoding(FaceRecognitionDotNet.Image image, Location location)
    {
        return _recognition.FaceEncodings(image, new[] { location },1, _predictorModel, _mlModel);
    }

    private IEnumerable<FaceEncoding>? getFaceEncodings(FaceRecognitionDotNet.Image image)
    {
        var locations1 = getFaceLocations(image);
        foreach (var location in locations1)
        {
            return getFaceEncoding(image, location);
        }
        return null;
    }

    private double[]? getFaceDescriptor(FaceRecognitionDotNet.Image image)
    {
        var locations = getFaceLocations(image);
        foreach (var location in locations)
        {
            var encodings = getFaceEncoding(image, location);
            foreach (var encoding in encodings)
            {
                var rawEncode = encoding.GetRawEncoding();
                encoding.Dispose();
                return rawEncode;
            }
            
        }
        return null;
    }*/

    public void Dispose()
    {
        
    }
}

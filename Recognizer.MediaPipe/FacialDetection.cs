using Recognizer.IOC;

namespace Recognizer.MediaPipe;
public class FacialDetection : IFacialDetection
{
    public float[]? FacialDetector(byte[] image, out string processMessage)
    {
        processMessage = "";

        

        return new float[]
            { 0, 0, 0,image.Length };
    }
}

namespace Recognizer.IOC
{
    public interface IFacialDetection
    {
        float[]? FacialDetector(byte[] image, out string processMessage);
    }
}
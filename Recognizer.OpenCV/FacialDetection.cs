namespace Recognizer.OpenCV
{
    public interface FacialDetection
    {
        public float[]? FacialDetector(byte[] image);
    }
}
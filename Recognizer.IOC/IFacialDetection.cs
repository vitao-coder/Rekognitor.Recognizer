namespace Recognizer.IOC
{
    public interface IFacialDetection
    {
        public float[]? FacialDetector(byte[] image);
        void Dispose();
    }
}
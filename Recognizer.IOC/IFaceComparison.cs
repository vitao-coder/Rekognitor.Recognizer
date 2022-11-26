namespace Recognizer.IOC
{
    public interface IFaceComparison
    {
        //bool FacialComparator(byte[] image1, byte[] image2, out string processMessage);
        bool FacialComparator(double[] descriptor1, double[] descriptor2, out string processMessage);
    }
}
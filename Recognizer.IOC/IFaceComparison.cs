namespace Recognizer.IOC
{
    public interface IFaceComparison
    {        
        bool FacialComparator(double[] descriptor1, double[] descriptor2, out string processMessage);
    }
}
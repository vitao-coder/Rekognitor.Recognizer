using DlibDotNet;
using DlibDotNet.Dnn;
using Recognizer.IOC;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Recognizer.Dlib.Wrapper
{

    public class FrontalFacialDetector : IDisposable
    {

        readonly FrontalFaceDetector _detector;
        public FrontalFacialDetector()
        {
            _detector = DlibDotNet.Dlib.GetFrontalFaceDetector();
        }

        public FrontalFaceDetector GetFrontalFacialDetector() => _detector;

        public void Dispose()
        {
           _detector.Dispose();
        }
    }
}

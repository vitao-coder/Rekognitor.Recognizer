using DlibDotNet;
using DlibDotNet.Dnn;
using Recognizer.IOC;
using Recognizer.IOC.Shared;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Recognizer.Dlib.Wrapper
{

    public class LossMetrics : IDisposable
    {
        readonly IModelLoader _modelLoader;

        readonly LossMetric? _lossMetric;
        public LossMetrics(IModelLoader modelLoader)
        {
            _modelLoader = modelLoader;
            var lossMetric = modelLoader.GetModel((int)AVAIABLE_MODELS.RessNet);
            if (lossMetric != null)
                _lossMetric = LossMetric.Deserialize(lossMetric.Data);
        }

        public LossMetric GetLossMetrics() {
            return _lossMetric;
        }

        public void Dispose()
        {
            _lossMetric.Dispose();
        }
    }
}

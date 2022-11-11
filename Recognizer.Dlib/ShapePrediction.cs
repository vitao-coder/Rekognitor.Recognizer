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

    public class ShapePrediction : IDisposable
    {
        readonly IModelLoader _modelLoader;
        readonly ShapePredictor _shapePredictor;

        public ShapePrediction(IModelLoader modelLoader)
        {
            _modelLoader = modelLoader;
            var shapePredictor = _modelLoader.GetModel((int)AVAIABLE_MODELS.ShapePredictor68MarksGTX);
            if (shapePredictor != null)
                _shapePredictor = ShapePredictor.Deserialize(shapePredictor.Data);
        }

        public ShapePredictor GetShapePredictor() 
        {
            return _shapePredictor;
        }

        public void Dispose()
        {
            _shapePredictor.Dispose();
        }
    }
}

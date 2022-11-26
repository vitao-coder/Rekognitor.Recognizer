namespace Recognizer.FaceRecognition.Wrapper;

using FaceRecognitionDotNet;
using Recognizer.IOC;
using Recognizer.IOC.Shared;
using System.Drawing;
using System.Runtime.Intrinsics.X86;
using System.Text;

public class ModelParameters : IDisposable
{
    readonly IModelLoader _modelLoader;
    readonly FaceRecognition _recognition;
    readonly FaceRecognitionDotNet.Model _mlModel;
    readonly PredictorModel _predictorModel;

    public ModelParameters(IModelLoader modelLoader)
    {
        _modelLoader = modelLoader;

        ModelParameter model = new ModelParameter();

        var shape5Landmarks = _modelLoader.GetModel((int)AVAIABLE_MODELS.ShapePredictor5Marks);
        var shape68Landmarks = _modelLoader.GetModel((int)AVAIABLE_MODELS.ShapePredictor68MarksGTX);
        var faceRecognitionModel = _modelLoader.GetModel((int)AVAIABLE_MODELS.RessNet);
        var cnnFaceDetectorModel = _modelLoader.GetModel((int)AVAIABLE_MODELS.Mmod);

        if (shape5Landmarks != null && shape68Landmarks != null)
        {
            model.PosePredictor5FaceLandmarksModel = shape5Landmarks.Data;
            model.PosePredictor68FaceLandmarksModel = shape68Landmarks.Data;
            model.CnnFaceDetectorModel = cnnFaceDetectorModel.Data; //mmod
            model.FaceRecognitionModel = faceRecognitionModel.Data; ;//resnet

            try
            {
                _mlModel = FaceRecognitionDotNet.Model.Cnn;
                _predictorModel = PredictorModel.Large;
                _recognition = FaceRecognition.Create(model);
            }
            catch (ArgumentNullException)
            {

                throw;
            }
            catch (NullReferenceException)
            {

                throw;
            }
            
        }

    }

    public FaceRecognition GetRecognition()
    {
        return _recognition;
    }



    public void Dispose()
    {
        _modelLoader.Dispose();
        _recognition.Dispose();
    }
}

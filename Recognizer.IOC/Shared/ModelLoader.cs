using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Recognizer.IOC.Shared;

public enum AVAIABLE_MODELS
{
    RessNet = 0,
    ShapePredictor5Marks = 1,
    ShapePredictor68Marks = 2,
    ShapePredictor68MarksGTX = 3,
    AgePredictor = 4,
    Mmod = 5,
}

public class Model : IModel
{
    public AVAIABLE_MODELS modelType;
    public string description;
    public byte[] data;

    public int AVAIABLE_MODELS
    {
        get
        {
            return (int)modelType;
        }
    }

    string IModel.Description
    {
        get
        {
            return description;
        }
    }

    byte[] IModel.Data
    {
        get
        {
            return data;
        }
    }
}

public class ModelLoader : IModelLoader, IDisposable
{
    List<Model> models = new List<Model>();
    public ModelLoader()
    {
        var _appFolder = Environment.CurrentDirectory;
        string pathModels = _appFolder + "/trainedmodels/";

        var dlib_face_recognition_resnet_model_v1 = File.ReadAllBytes(pathModels + "dlib_face_recognition_resnet_model_v1.dat");
        var mmod_human_face_detector = File.ReadAllBytes(pathModels + "mmod_human_face_detector.dat");
        var shape_predictor_5_face_landmarks = File.ReadAllBytes(pathModels + "shape_predictor_5_face_landmarks.dat");
        var shape_predictor_68_face_landmarks = File.ReadAllBytes(pathModels + "shape_predictor_68_face_landmarks.dat");
        var shape_predictor_68_face_landmarksGTX = File.ReadAllBytes(pathModels + "shape_predictor_68_face_landmarks_GTX.dat");


        models.Add(new Model()
        {
            modelType = AVAIABLE_MODELS.Mmod,
            description = "MMOD Model",
            data = mmod_human_face_detector,
        });

        models.Add(new Model()
        {
            modelType = AVAIABLE_MODELS.RessNet,
            description = "Ress Net Model",
            data = dlib_face_recognition_resnet_model_v1,
        });

        models.Add(new Model()
        {
            modelType = AVAIABLE_MODELS.ShapePredictor5Marks,
            description = "5 Landmarks",
            data = shape_predictor_5_face_landmarks,
        });

        models.Add(new Model()
        {
            modelType = AVAIABLE_MODELS.ShapePredictor68Marks,
            description = "68 Landmarks",
            data = shape_predictor_68_face_landmarks,
        });

        models.Add(new Model()
        {
            modelType = AVAIABLE_MODELS.ShapePredictor68MarksGTX,
            description = "68 Landmarks GTX",
            data = shape_predictor_68_face_landmarksGTX,
        });

    }

    public IModel? GetModel(int modelType)
    {
        return models.FirstOrDefault(it => it.modelType == (AVAIABLE_MODELS)modelType);
    }


    public void Dispose()
    {
        models.Clear();
    }
}

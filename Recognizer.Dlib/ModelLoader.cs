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
    public enum AVAIABLE_MODELS
    {
        RessNet = 0,
        ShapePredictor5Marks = 1,
        ShapePredictor68Marks = 2,
        ShapePredictor68MarksGTX = 3,
        AgePredictor = 4,
    }

    public class Model : IModel
    {
        public AVAIABLE_MODELS modelType;        
        public string description;
        public byte[] data;

        public int AVAIABLE_MODELS { 
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
            models.Add(new Model() { 
                modelType = AVAIABLE_MODELS.RessNet,
                description = Properties.Resources.ressnet_lossmetrics,
                data = Properties.Resources.dlib_face_recognition_resnet_model_v1,
            });

            models.Add(new Model()
            {
                modelType = AVAIABLE_MODELS.ShapePredictor5Marks,
                description = Properties.Resources.shape_predictor_5,
                data = Properties.Resources.shape_predictor_5_face_landmarks,
            });

            models.Add(new Model()
            {
                modelType = AVAIABLE_MODELS.ShapePredictor68Marks,
                description = Properties.Resources.shape_predictor_68,
                data = Properties.Resources.shape_predictor_68_face_landmarks,
            });

            models.Add(new Model()
            {
                modelType = AVAIABLE_MODELS.ShapePredictor68MarksGTX,
                description = Properties.Resources.shape_predictor_68_gtx,
                data = Properties.Resources.shape_predictor_68_face_landmarks_GTX,
            });

            models.Add(new Model()
            {
                modelType = AVAIABLE_MODELS.AgePredictor,
                description = Properties.Resources.age_predictor,
                data = Properties.Resources.dnn_age_predictor_v1,
            });

        }

        public IModel? GetModel(int modelType)
        {
            return models.FirstOrDefault(it=>it.modelType == (AVAIABLE_MODELS)modelType);
        }

        public void Dispose()
        {
            models.Clear();
        }
    }
}

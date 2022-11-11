using System.Dynamic;
using System.Reflection;

namespace Recognizer.IOC
{

    public interface IModel
    {
        int AVAIABLE_MODELS { get; }
        string Description { get; }
        byte[] Data { get; }
    }

    public interface IModelLoader
    {
        IModel? GetModel(int modelType);
        void Dispose();
    }
}
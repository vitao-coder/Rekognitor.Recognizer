using Recognizer.Grpc.Services;
using Recognizer.Dlib.Wrapper;
using Recognizer.IOC;
using Recognizer.IOC.Shared;
using Recognizer.FaceRecognition.Wrapper;
using FacialDetection = Recognizer.Dlib.Wrapper.FacialDetection;

namespace Recognizer.Grpc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddScoped<IFacialDetection, FacialDetection>();

            builder.Services.AddScoped<IFaceComparison, FaceComparison>();

            builder.Services.AddSingleton<FrontalFacialDetector>();

            builder.Services.AddSingleton<IModelLoader, ModelLoader>();
            //builder.Services.AddSingleton<ModelParameters>();
            builder.Services.AddSingleton<ShapePrediction>(); 
            builder.Services.AddSingleton<LossMetrics>();


            builder.Services.AddGrpc();
            var app = builder.Build();

            app.Services.GetService<IModelLoader>();
            app.Services.GetService<ShapePrediction>();
            app.Services.GetService<LossMetrics>();
            app.Services.GetService<FrontalFacialDetector>();


            app.MapGrpcService<Services.FacialDetector>();
            app.MapGrpcService<Services.FacialComparator>();


            app.Run();
        }
    }
}
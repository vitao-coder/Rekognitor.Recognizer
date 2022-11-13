using Recognizer.Grpc.Services;
using Recognizer.Dlib.Wrapper;
using Recognizer.IOC;

namespace Recognizer.Grpc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddScoped<IFacialDetection, FacialDetection>();
            builder.Services.AddSingleton<FrontalFacialDetector>();

            builder.Services.AddSingleton<IModelLoader, ModelLoader>();            
            builder.Services.AddSingleton<ShapePrediction>(); 
            builder.Services.AddSingleton<LossMetrics>();


            builder.Services.AddGrpc();
            var app = builder.Build();

            app.Services.GetService<IModelLoader>();
            app.Services.GetService<ShapePrediction>();
            app.Services.GetService<LossMetrics>();
            app.Services.GetService<FrontalFacialDetector>();


            app.MapGrpcService<FacialRecognizer>();
            

            app.Run();
        }
    }
}
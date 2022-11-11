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
            builder.Services.AddScoped<FrontalFacialDetector>();

            builder.Services.AddSingleton<IModelLoader, ModelLoader>();            
            builder.Services.AddSingleton<ShapePrediction>(); 
            builder.Services.AddSingleton<LossMetrics>();


            builder.Services.AddGrpc();
            var app = builder.Build();

            app.Services.GetService<IModelLoader>();
            app.Services.GetService<ShapePrediction>();
            app.Services.GetService<LossMetrics>();


            app.MapGrpcService<FacialRecognizer>();
            //app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

            app.Run();
        }
    }
}
using Recognizer.Grpc.Services;
using Recognizer.Dlib.Wrapper;
using Recognizer.IOC;
using Recognizer.IOC.Shared;
using FacialDetection = Recognizer.Dlib.Wrapper.FacialDetection;

namespace Recognizer.Grpc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddScoped<IFacialDetection, FacialDetection>();

            builder.Services.AddSingleton<IModelLoader, ModelLoader>();
            builder.Services.AddSingleton<FrontalFacialDetector>();
            builder.Services.AddSingleton<ShapePrediction>(); 
            builder.Services.AddSingleton<LossMetrics>();
            builder.Services.AddGrpc();

            // Configure Kestrel to listen on a specific HTTP port 
            builder.WebHost.ConfigureKestrel(options =>
            {   
                options.ListenAnyIP(8082, listenOptions =>
                {                    
                    listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
                });
            });

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
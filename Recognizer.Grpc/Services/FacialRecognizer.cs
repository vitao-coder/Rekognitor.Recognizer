using Grpc.Core;
using Recognizer.Grpc;
using Recognizer.Dlib.Wrapper;
using Recognizer.IOC;

namespace Recognizer.Grpc.Services
{
    public class FacialRecognizer : Recognizer.RecognizerBase
    {
        private readonly ILogger<FacialRecognizer> _logger;
        private readonly IFacialDetection _detection;
        public FacialRecognizer(ILogger<FacialRecognizer> logger, IFacialDetection detection)
        {
            _logger = logger;
            _detection = detection;
        }

        public override Task<DetectionReply> FacialDetection(DetectionRequest request, ServerCallContext context)
        {                        
            if (request.ImageBytes == null) throw new Exception("image bytes null");

            var descriptor = _detection.FacialDetector(request.ImageBytes.ToByteArray());
            
            return Task.FromResult(new DetectionReply
            {
                RequestId = descriptor.ToString()
            });            
        }


        public override Task<ComparisonReply> FacialComparison(ComparisonRequest request, ServerCallContext context)
        {
            return Task.FromResult(new ComparisonReply
            {
                RequestId = Guid.NewGuid().ToString(),
            });
        }
    }
}
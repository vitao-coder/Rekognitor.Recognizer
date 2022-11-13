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
            string outMessage;
            var descriptor = _detection.FacialDetector(request.ImageBytes.ToByteArray(), out outMessage);
            if (descriptor != null)
            {
                array128D array128D = new array128D();
                array128D.Array.AddRange(descriptor);
                
                return Task.FromResult(new DetectionReply
                {
                    RequestId = request.RequestId,
                    Array128D = array128D,
                    StatusMessage = outMessage,
                });
            }

            return Task.FromResult(
                new DetectionReply
                {
                    RequestId = request.RequestId,
                    StatusMessage = outMessage,
                });
        }
    }
}
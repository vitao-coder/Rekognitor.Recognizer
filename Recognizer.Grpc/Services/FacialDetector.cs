using Grpc.Core;
using RecognizerGrpc;
using Recognizer.IOC;


namespace Recognizer.Grpc.Services
{
    public class FacialDetector : Detector.DetectorBase
    {
        private readonly ILogger<FacialDetector> _logger;
        private readonly IFacialDetection _detection;        

        public FacialDetector(ILogger<FacialDetector> logger, IFacialDetection detection)
        {
            _logger = logger;
            _detection = detection;            
        }

        public override Task<DetectionReply> FacialDetection(DetectionRequest request, ServerCallContext context)
        {
            if (request.ImageBytes == null) throw new Exception("image bytes null");
            string outMessage;
            var bytArr = request.ImageBytes.ToByteArray();
            var descriptor = _detection.FacialDetector(bytArr, out outMessage);
            bytArr = null;
            if (descriptor != null)
            {
                array128D array128D = new array128D();
                array128D.Array.AddRange(descriptor);
                descriptor = null;
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
using Grpc.Core;
using RecognizerGrpc;
using Recognizer.IOC;
using Recognizer.Grpc.Services.Math;

namespace Recognizer.Grpc.Services
{
    public class FacialComparator : Comparator.ComparatorBase
    {
        private readonly ILogger<FacialComparator> _logger;                
        private readonly IFacialDetection _detector;

        public FacialComparator(ILogger<FacialComparator> logger, IFacialDetection detector)
        {
            _logger = logger;               
            _detector = detector;
        }

        public override Task<ComparisonReply> FacialComparison(ComparisonRequest request, ServerCallContext context)
        {
            if (request.ImageBytes1 == null) throw new ArgumentNullException("image bytes 1 null");
            if (request.ImageBytes2 == null) throw new ArgumentNullException("image bytes 2 null");

            string outMessage="";

            var detect1 = Task.Run<float[]?>(() =>
            {
                return _detector.FacialDetector(request.ImageBytes1.ToByteArray(), out outMessage);
            });

            var detect2 = Task.Run<float[]?>(() =>
            {
                return _detector.FacialDetector(request.ImageBytes2.ToByteArray(), out outMessage);
            });

            Task.WhenAll(detect1, detect2);

            var detectt1 = detect1.Result.ToList();
            var detectt2 = detect2.Result.ToList();
            
            var cosine = Cosine.Distance<float>(detectt1, detectt2);
            if (outMessage == "success") outMessage = "";
            return Task.FromResult(
                new ComparisonReply
                {
                    Score = cosine,
                    Error = outMessage,
                });
        }
    }
}
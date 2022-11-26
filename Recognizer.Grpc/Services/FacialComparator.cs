using Grpc.Core;
using RecognizerGrpc;
using Recognizer.IOC;

namespace Recognizer.Grpc.Services
{
    public class FacialComparator : Comparator.ComparatorBase
    {
        private readonly ILogger<FacialComparator> _logger;        
        private readonly IFaceComparison _comparison;
        private readonly IFacialDetection _detector;

        public FacialComparator(ILogger<FacialComparator> logger, IFaceComparison comparison, IFacialDetection detector)
        {
            _logger = logger;   
            _comparison = comparison;
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

            var detectt1 = detect1.Result.Select(it => (double)it).ToArray();
            var detectt2 = detect2.Result.Select(it => (double)it).ToArray();

            var sameFace = _comparison.FacialComparator(detectt1, detectt2, out outMessage);

            return Task.FromResult(
                new ComparisonReply
                {
                    RequestId = request.RequestId,
                    StatusMessage = outMessage + " - result: " + sameFace.ToString(),
                });
        }
    }
}
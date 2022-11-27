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

            var detectt1 = detect1.Result.Select(it => (double)it);
            var detectt2 = detect2.Result.Select(it => (double)it);

            var euclidean = LrNorm.Euclidean<double>(detectt1.ToList(), detectt2.ToList());
            var manhattan = LrNorm.Manhattan<double>(detectt1.ToList(), detectt2.ToList());
            var cosine = Cosine.Distance<double>(detectt1.ToList(), detectt2.ToList());

            var confidencePercentage = (100 - (cosine + euclidean));


            return Task.FromResult(
                new ComparisonReply
                {
                    RequestId = request.RequestId,
                    StatusMessage = outMessage 
                    + " - euclidean: " + euclidean.ToString() 
                    + " - manhattan: " + manhattan.ToString() 
                    + " - cosine: " + cosine.ToString()
                    + " - confidencePercentage: " + confidencePercentage.ToString(),
                });
        }
    }
}
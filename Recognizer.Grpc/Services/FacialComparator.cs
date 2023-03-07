using Grpc.Core;
using RecognizerGrpc;
using Recognizer.IOC;
using Recognizer.Grpc.Services.Similarity;
using System.Linq;

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

            var detectt1 = detect1.Result;
            var detectt2 = detect2.Result;

            var cosine = Similarity.Similarity.CosineSimilarity(detectt1, detectt2);
            var euclidean = Similarity.Similarity.EuclideanDistance(detectt1, detectt2);

            var score = Compare(detectt1, detectt2);
            if (outMessage == "success") outMessage = "";
            return Task.FromResult(
                new ComparisonReply
                {
                    Score = score,
                    Error = outMessage,     
                    CosineDistance = cosine,
                    EuclideanDistance = euclidean
                });
        }
        static private double Compare(float[] faceA, float[] faceB)
        {
            double result = 0;
            for (int i = 0; i < faceA.Length; i++)
                result = result + System.Math.Pow(faceA[i] - faceB[i], 2);
            return result;
        }
    }
}
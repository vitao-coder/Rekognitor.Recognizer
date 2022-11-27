namespace Recognizer.Grpc.Services.Math;
using System;

/// <summary>
/// Calculate cosine distance between two vectors
/// </summary>
public class Cosine
{
    /// <summary>
    /// Calculates the distance between frequency distributions calculated from lists of items
    /// </summary>
    /// <typeparam name="T">Type of the list item, e.g. int or string</typeparam>
    /// <param name="l1">First list of items</param>
    /// <param name="l2">Second list of items</param>
    /// <returns>Distance in degrees. 90 is totally different, 0 exactly the same</returns>
    public static double Distance<T>(List<T> l1, List<T> l2)
    {
        if (l1.Count() == 0 || l2.Count() == 0)
        {
            throw new Exception("Cosine Distance: lists cannot be zero length");
        }

        // find distinct list of items from two lists, used to align frequency distributions from two lists
        List<T> dvs = FrequencyDist<T>.GetDistinctValues(l1, l2);
        // calculate frequency distributions for each list.
        FrequencyDist<T> fd1 = new FrequencyDist<T>(l1, dvs);
        FrequencyDist<T> fd2 = new FrequencyDist<T>(l2, dvs);

        if (fd1.ItemFreq.Count() != fd2.ItemFreq.Count)
        {
            throw new Exception("Cosine Distance: Frequency count vectors must be same length");
        }
        double dotProduct = 0.0;
        double l2norm1 = 0.0;
        double l2norm2 = 0.0;
        for (int i = 0; i < fd1.ItemFreq.Values.Count(); i++)
        {
            if (!EqualityComparer<T>.Default.Equals(fd1.ItemFreq.Values[i].Value, fd2.ItemFreq.Values[i].Value))
                throw new Exception("Mismatched values in frequency distribution for Cosine distance calculation");

            dotProduct += fd1.ItemFreq.Values[i].Count * fd2.ItemFreq.Values[i].Count;
            l2norm1 += fd1.ItemFreq.Values[i].Count * fd1.ItemFreq.Values[i].Count;
            l2norm2 += fd2.ItemFreq.Values[i].Count * fd2.ItemFreq.Values[i].Count;
        }
        double cos = dotProduct / (Math.Sqrt(l2norm1) * Math.Sqrt(l2norm2));
        // convert cosine value to radians then to degrees
        return Math.Acos(cos) * 180.0 / Math.PI;
    }
}


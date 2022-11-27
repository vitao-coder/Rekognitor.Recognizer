namespace Recognizer.Grpc.Services.Math;

using System;

public class LrNorm
{
    /// <summary>
    /// Returns Euclidean distance between frequency distributions of two lists
    /// </summary>
    /// <typeparam name="T">Type of the item, e.g. int or string</typeparam>
    /// <param name="l1">First list of items</param>
    /// <param name="l2">Second list of items</param>
    /// <returns>Distance, 0 - identical</returns>
    public static double Euclidean<T>(List<T> l1, List<T> l2)
    {
        return DoLrNorm(l1, l2, 2);
    }

    /// <summary>
    /// Returns Manhattan distance between frequency distributions of two lists
    /// </summary>
    /// <typeparam name="T">Type of the item, e.g. int or string</typeparam>
    /// <param name="l1">First list of items</param>
    /// <param name="l2">Second list of items</param>
    /// <returns>Distance, 0 - identical</returns>
    public static double Manhattan<T>(List<T> l1, List<T> l2)
    {
        return DoLrNorm(l1, l2, 1);
    }

    /// <summary>
    /// Returns LrNorm distance between frequency distributions of two lists
    /// </summary>
    /// <typeparam name="T">Type of the item, e.g. int or string</typeparam>
    /// <param name="l1">First list of items</param>
    /// <param name="l2">Second list of items</param>
    /// <param name="r">Power to use 2 = Euclidean, 1 = Manhattan</param>
    /// <returns>Distance, 0 - identical</returns>
    public static double DoLrNorm<T>(List<T> l1, List<T> l2, int r)
    {
        // find distinct list of values from both lists.
        List<T> dvs = FrequencyDist<T>.GetDistinctValues(l1, l2);

        // create frequency distributions aligned to list of descrete values
        FrequencyDist<T> fd1 = new FrequencyDist<T>(l1, dvs);
        FrequencyDist<T> fd2 = new FrequencyDist<T>(l2, dvs);

        if (fd1.ItemFreq.Count != fd2.ItemFreq.Count)
        {
            throw new Exception("Lists of different length for LrNorm calculation");
        }
        double sumsq = 0.0;

        for (int i = 0; i < fd1.ItemFreq.Count; i++)
        {
            if (!EqualityComparer<T>.Default.Equals(fd1.ItemFreq.Values[i].Value, fd2.ItemFreq.Values[i].Value))
                throw new Exception("Mismatched values in frequency distribution for LrNorm calculation");

            if (r == 1)   // Manhattan optimization
            {
                sumsq += Math.Abs((fd1.ItemFreq.Values[i].Count - fd2.ItemFreq.Values[i].Count));
            }
            else
            {
                sumsq += Math.Pow((double)Math.Abs((fd1.ItemFreq.Values[i].Count - fd2.ItemFreq.Values[i].Count)), r);
            }
        }
        if (r == 1)    // Manhattan optimization
        {
            return sumsq;
        }
        else
        {
            return Math.Pow(sumsq, 1.0 / r);
        }
    }
}
namespace Recognizer.Grpc.Services.Math;



/// <summary>
/// Manages Frequency Distributions for items of type T
/// </summary>
/// <typeparam name="T">Type for item</typeparam>
public class FrequencyDist<T>
{
    /// <summary>
    /// Construct Frequency Distribution for the given list of items
    /// </summary>
    /// <param name="li">List of items to calculate for</param>
    public FrequencyDist(IReadOnlyCollection<T> li)
    {
        CalcFreqDist(li);
    }

    /// <summary>
    /// Construct Frequency Distribution for the given list of items, across all keys in itemValues
    /// </summary>
    /// <param name="li">List of items to calculate for</param>
    /// <param name="itemValues">Entire list of itemValues to include in the frequency distribution</param>
    public FrequencyDist(IReadOnlyCollection<T> li, IReadOnlyCollection<T> itemValues)
    {
        if (li.First() == null)
        {
            throw new NullReferenceException();
        }
        CalcFreqDist(li);
        // add items to frequency distribution that are in itemValues but missing from the frequency distribution
        foreach (var v in itemValues.Where(v => !ItemFreq.Keys.Contains(v)))
        {
            ItemFreq.Add(v, new Item { Value = v, Count = 0 });
        }
        // check that all values in li are in the itemValues list
        //foreach (var v in li.Where(v => !itemValues.Contains(v)))
        //{
        //    throw new Exception(
        //        $"FrequencyDist: Value in list for frequency distribution not in supplied list of values: '{v}'.");
        //}
    }

    /// <summary>
    /// Calculate the frequency distribution for the values in list
    /// </summary>
    /// <param name="li">List of items to calculate for</param>
    void CalcFreqDist(IReadOnlyCollection<T> li)
    {
        //CHANGED from HERE
        _itemFreq = new SortedList<T, Item>((from item in li
                                             group item by item into theGroup
                                             select new Item { Value = theGroup.FirstOrDefault(), Count = theGroup.Count() }).ToDictionary(q => q.Value, q => q));
    }

    SortedList<T, Item> _itemFreq = new SortedList<T, Item>();

    /// <summary>
    /// Getter for the Item Frequency list
    /// </summary>
    public SortedList<T, Item> ItemFreq => _itemFreq;
    //IReadOnlyCollection<KeyValuePair<T, Item>>
    //unused
    public int Freq(T value) => _itemFreq.Keys.Contains(value) ? _itemFreq[value].Count : 0;

    /// <summary>
    /// Returns the list of distinct values between two lists
    /// </summary>
    /// <param name="l1"></param>
    /// <param name="l2"></param>
    /// <returns></returns>
    public static List<T> GetDistinctValues(IReadOnlyCollection<T> l1, IReadOnlyCollection<T> l2) => l1.Concat(l2).ToList().Distinct().ToList();

    /// <summary>
    /// Manages a count of items (int, string etc) for frequency counts
    /// </summary>
    /// <typeparam name="T">The type for item</typeparam>
    public class Item
    {
        /// <summary>
        /// The value of the item, e.g. int or string
        /// </summary>
        public T Value { get; set; }
        /// <summary>
        /// The count of the item
        /// </summary>
        public int Count { get; set; }
    }
}


namespace CoyposServer.Utils.Extensions;

public static class ListExtensions
{
    /// <summary>
    /// Filter type
    /// </summary>
    public enum FilterType 
    {
        /// <summary> All items need to match the criteria</summary>
        AND,
        /// <summary> Any of the items need to match the criteria</summary>
        OR,
        /// <summary> None of the items need to match the criteria</summary>
        NOR
    }

    /// <summary>
    /// Filters a List of T by a reference item of type T
    /// </summary>
    /// <param name="list">Input list</param>
    /// <param name="filter">Reference item to filter by</param>
    /// <param name="filterType">Filter type to apply (see FilterType enum) in string form</param>
    /// <returns>Filtered List</returns>
    /// <exception cref="ArgumentException">Thrown when filterType is not parsable into an enum of type FilterType</exception>
    public static List<T>? Filter<T>(this List<T>? list, T filter, string filterType)
    {
        if (!Enum.TryParse<FilterType>(filterType.ToUpper(), true, out var parsedType))
        {
            throw new ArgumentException(
                $"Unknown filter: {filterType.ToUpper()}. Known filters: {string.Join(",", (from object? val in Enum.GetValues(typeof(FilterType)) select val.ToString()).ToList())}");
        }

        return list.Filter(filter, parsedType);
    }
    
    /// <summary>
    /// Filters a List of T by a reference item of type T
    /// </summary>
    /// <param name="list">Input list</param>
    /// <param name="filter">Reference item to filter by</param>
    /// <param name="filterType">Filter type to apply (see FilterType enum)</param>
    /// <returns>Filtered List</returns>
    public static List<T>? Filter<T>(this List<T>? list, T filter, FilterType filterType)
    {
        if (list is null || filter is null)
            return list;

        var propertyInfos = typeof(T).GetProperties();
        var outList = new List<T>();
        
        foreach (var item in list)
        {
            var results = new List<bool>();
            foreach (var propertyInfo in propertyInfos)
            {
                var filterVal = propertyInfo.GetValue(filter);
                if (filterVal == null)
                    continue;
                var listVal = propertyInfo.GetValue(item);
                results.Add(Equals(filterVal, listVal));
            }
            
            switch (filterType)
            {
                case FilterType.AND:
                    if (results.All(result => result))
                        outList.Add(item);
                    break;
                case FilterType.OR:
                    if (results.Any(result => result))
                        outList.Add(item);
                    break;
                case FilterType.NOR:
                    if (results.All(result => !result))
                        outList.Add(item);
                    break;
            }
        }

        return outList;
    }

    /// <summary>
    /// Pagefies a list.
    /// </summary>
    /// <param name="list">Input list</param>
    /// <param name="itemsPerPage">Number of items on each page</param>
    /// <param name="pageNumber">Page number to display</param>
    /// <returns>Pagefied list</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when itemsPerPage or pageNumber is 0 or lower</exception>
    public static List<T>? Pagefy<T>(this List<T>? list, int itemsPerPage, int pageNumber)
    {
        if (list == null)
            return list;

        if (itemsPerPage <= 0)
            throw new ArgumentOutOfRangeException(nameof(itemsPerPage), "Items per page must be greater than zero.");

        if (pageNumber <= 0)
            throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number must be greater than zero.");
        
        int startIndex = (pageNumber - 1) * itemsPerPage;
        int endIndex = Math.Min(startIndex + itemsPerPage, list.Count);

        // out of range? return empty.
        if (startIndex >= list.Count)
            return new List<T>();

        return list.GetRange(startIndex, endIndex - startIndex);
    }
}
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
        NOR,
        /// <summary> None of the items need to match the criteria</summary>
        ISNULL
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
        
        // if all properties are null, don't filter anything.
        if (propertyInfos.All(p => p.GetValue(filter) == null))
            return list;
        
        // filter:
        var outList = new List<T>();
        foreach (var item in list)
        {
            var results = new List<bool>();
            foreach (var propertyInfo in propertyInfos)
            {
                object? filterVal;
                object? listVal;
                
                if (filterType == FilterType.ISNULL && propertyInfo.GetValue(filter) != null && propertyInfo.GetValue(item) == null)
                {
                    outList.Add(item);
                    goto finishIteration;
                }
                
                // reference hack: compare IDs instead!
                var idProperty = propertyInfo.PropertyType.GetProperties().FirstOrDefault(p => p.Name == "ID");
                if (propertyInfo.PropertyType.AssemblyQualifiedName.Contains(".Sql.") && idProperty is not null)
                {
                    if (propertyInfo.GetValue(filter) == null || propertyInfo.GetValue(item) == null)
                        continue;

                    var filterInnerVal = propertyInfo.GetValue(filter);
                    filterVal = idProperty.GetValue(filterInnerVal);

                    var listInnerVal = propertyInfo.GetValue(item);
                    listVal = idProperty.GetValue(listInnerVal);
                }
                // </hack>
                else
                {
                    filterVal = propertyInfo.GetValue(filter);
                    if (filterVal == null)
                        continue;
                    listVal = propertyInfo.GetValue(item);
                    
                    if (filterType != FilterType.ISNULL && filterVal == null)
                        continue;
                }

                results.Add(Equals(filterVal, listVal));
            }

            if (results.Count == 0)
                continue;
            
            // got our results, now let's see what we care about
            switch (filterType)
            {
                case FilterType.AND:
                    if (results.All(result => result))
                        outList.Add(item);
                    break;
                case FilterType.OR:
                case FilterType.ISNULL:
                    if (results.Any(result => result))
                        outList.Add(item);
                    break;
                case FilterType.NOR:
                    if (results.All(result => !result))
                        outList.Add(item);
                    break;
            }
            finishIteration: ;
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
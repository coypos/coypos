namespace CoyposServer.Utils.Extensions;

public static class ObjectHelpers
{
    public static T CopyNonNullValues<T>(T targetItem, T itemWithNullValues)
    {
        foreach (var propertyInfo in typeof(T).GetProperties())
        {
            var targetValue = propertyInfo.GetValue(itemWithNullValues);
            if (targetValue is not null)
                propertyInfo.SetValue(targetItem, targetValue);
        }

        return targetItem;
    }
}
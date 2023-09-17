namespace CoyposServer.Utils.Extensions;

public static class DatabaseExtensions
{
    public static void AttachVirtualProperties(this DatabaseContext? dbContext, object? model)
    {
        if (dbContext is null || model is null)
            return;
        
        var virtualProperties = model.GetType().GetProperties().Where(p =>
        {
            var getMethod = p.GetGetMethod();
            return getMethod is not null && getMethod.IsVirtual;
        });
        
        foreach (var virtualProperty in virtualProperties)
        {
            var val = virtualProperty.GetValue(model);
            if (val is null)
                continue;
            dbContext.Attach(val);
        }
    }
}
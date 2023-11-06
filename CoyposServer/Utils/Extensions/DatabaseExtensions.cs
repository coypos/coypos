using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using NUnit.Framework;

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
    
    public static bool IsRealDatabase(this DatabaseContext? dbContext) =>
        !dbContext.GetService<IDatabaseProvider>().Name.ToLower().Contains("inmemory");

    public static async Task ForceSaveChangesAsync(this DatabaseContext? dbContext, string tableName)
    {
        // temp fix:
        await dbContext.SaveChangesAsync();
        return;
        
        if (!dbContext.IsRealDatabase())
        {
            await dbContext.SaveChangesAsync();
            return;
        }

        await dbContext.Database.ExecuteSqlRawAsync($"alter table {tableName} disable trigger all;");
        await dbContext.SaveChangesAsync();
        await dbContext.Database.ExecuteSqlRawAsync($"alter table {tableName} enable trigger all;");
    }
}
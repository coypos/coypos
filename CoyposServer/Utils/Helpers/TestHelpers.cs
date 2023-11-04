using CoyposServer.Models.Sql;
using Microsoft.EntityFrameworkCore;

namespace CoyposServer.Utils.Extensions;

public static class TestHelpers
{
    public static async Task<DatabaseContext> GenerateDatabaseContext()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        var databaseContext = new DatabaseContext(options);
        await databaseContext.Database.EnsureCreatedAsync();
        for (int i = 0; i < 10; i++)
        {
            databaseContext.Categories.Add(new Category()
            {
                ID = i,
                Name = Guid.NewGuid().ToString(),
                ParentCategory = databaseContext.Categories.FirstOrDefault(),
                UpdateDate = DateTime.Now,
                CreateDate = DateTime.Now
            });
            await databaseContext.SaveChangesAsync();
        }

        for (int i = 0; i < 100; i++)
        {
            var random = new Random();
            databaseContext.Products.Add(new Product()
            {
                ID = i,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                Enabled = random.Next() > (int.MaxValue / 2),
                Name = Guid.NewGuid().ToString(),
                Barcode = random.Next().ToString(),
                Price = (decimal) Math.Round(random.NextDouble() * random.Next(1, 10), 2),
                IsLoose = false,
                Weight = random.Next(10, 1000),
                Category = databaseContext.Categories.ToList()[random.Next(0, databaseContext.Categories.Count() - 1)],
            });
            await databaseContext.SaveChangesAsync();
        }

        for (int i = 0; i < 50; i++)
        {
            databaseContext.Settings.Add(new Setting()
            {
                ID = i,
                Key = Guid.NewGuid().ToString(),
                Value = Guid.NewGuid().ToString()
            });
            await databaseContext.SaveChangesAsync();
        }

        return databaseContext;
    }
}
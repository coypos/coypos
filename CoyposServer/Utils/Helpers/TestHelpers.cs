using CoyposServer.Models.Sql;
using Microsoft.EntityFrameworkCore;
using System;

namespace CoyposServer.Utils.Extensions;

public static class TestHelpers
{
    public static string FirstUserPassword = "";
    
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
                IsVisible = true,
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

        for (int i = 0; i < 2; i++)
        {
            var rawPassword = Guid.NewGuid().ToString();
            var salt = BCrypt.Net.BCrypt.GenerateSalt();
            var password = BCrypt.Net.BCrypt.HashPassword(rawPassword, salt);
            if (i == 0)
             FirstUserPassword = rawPassword;
            var random = new Random();
            var minValue = 100000000000; // 10^11
            var maxValue = 999999999999; // 10^12 - 1

            var cardNumber =  random.Next((int)(minValue / 1000000), (int)(maxValue / 1000000 + 1)) * 1000000 + random.Next(1000000);
            var user = new User()
            {
                ID = i,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                Name = Guid.NewGuid().ToString(),
                Role = "",
                CardNumber = cardNumber.ToString(),
                PhoneNumber = string.Concat("+", cardNumber.ToString().AsSpan(0, cardNumber.ToString().Length - 1)),
                Points = random.Next(0, 1001),
                Email = Guid.NewGuid().ToString().Replace("-", "") + "@gmail.com",
                Password = password,
                Salt = salt,
                LoginToken = Guid.NewGuid().ToString(),
                LoginTokenValidDate = DateTime.Now
            };

            databaseContext.Users.Add(user);
            await databaseContext.SaveChangesAsync();
        }

        for (int i = 0; i < 2; i++)
        {
            var random = new Random();
            
            databaseContext.Promotions.Add(new Promotion()
            {
                ID = i,
                Ids = i.ToString(),
                DiscountPercentage = random.Next(1, 99),
                StartDate = DateTime.Now - TimeSpan.FromDays(1),
                EndDate = DateTime.Now + TimeSpan.FromDays(1),
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
            });
            await databaseContext.SaveChangesAsync();
        }

        return databaseContext;
    }
}
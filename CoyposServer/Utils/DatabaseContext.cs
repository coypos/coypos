﻿using CoyposServer.Models.Sql;
using Microsoft.EntityFrameworkCore;

namespace CoyposServer.Utils;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies();
    }

    //public DbSet<LoyaltyCard> LoyaltyCards { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Promotion> Promotions { get; set; }
    //public DbSet<Promotion> Transactions { get; set; }
    //public DbSet<TransactionLog> TransactionLogs { get; set; }
    public DbSet<User> Users { get; set; }
    
    public DbSet<Image> Images { get; set; }
    public DbSet<Setting> Settings { get; set; }
    
    public DbSet<Receipt> Receipts { get; set; }
    
    public DbSet<Transaction> Transactions { get; set; }
    
    public DbSet<PaymentMethod> PaymentMethods { get; set; }
    
    public DbSet<Language> Languages { get; set; }
    
    public DbSet<Employee> Employees { get; set; }
}
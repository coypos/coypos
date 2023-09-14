using CoyposServer.Models.Sql;
using Microsoft.EntityFrameworkCore;

namespace CoyposServer.Utils;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base (options) {}

    //public DbSet<LoyaltyCard> LoyaltyCards { get; set; }
    public DbSet<Product> Products { get; set; }
    //public DbSet<Promotion> Promotions { get; set; }
    //public DbSet<Promotion> Transactions { get; set; }
    //public DbSet<TransactionLog> TransactionLogs { get; set; }
    //public DbSet<User> Users { get; set; }
}
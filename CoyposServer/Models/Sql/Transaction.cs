using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CoyposServer.Models.Sql;
using CoyposServer.Utils.Extensions;

public class Transaction
{
    [Key] [Column("ID")]
    public int? ID { get; set; }
    
    [Column("Product")] 
    public virtual Product? Product { get; set; }

    [Column("Quantity")]
    public decimal? Quantity { get; set; }
    
    [Column("TotalPrice")]
    public decimal? TotalPrice { get; set; }
    
    [Column("OriginalPrice")]
    public decimal? OriginalPrice { get; set; }
    
    [Column("Receipt")]
    public virtual Receipt? Receipt { get; set; }

    [Column("CreateDate")] [Skipped]
    public DateTime? CreateDate { get; set; }

    [Column("UpdateDate")] [Skipped]
    public DateTime? UpdateDate { get; set; }
}
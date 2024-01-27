using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using CoyposServer.Utils.Extensions;

namespace CoyposServer.Models.Sql;

using System;
using System.ComponentModel.DataAnnotations;

public class Product
{
    [Key] [Column("ID")]
    public int? ID { get; set; }

    [Column("CreateDate")] [Skipped]
    public DateTime? CreateDate { get; set; }

    [Column("UpdateDate")] [Skipped]
    public DateTime? UpdateDate { get; set; }

    [Column("Enabled")]
    public bool? Enabled { get; set; } 

    [MaxLength(255)] [Column("Name")]
    public string? Name { get; set; }

    [MaxLength(20)] [Column("Barcode")]
    public string? Barcode { get; set; }

    [Column("Price")]
    public decimal? Price { get; set; }

    [Column("isLoose")]
    public bool? IsLoose { get; set; }
    
    [Column("Weight")]
    public int? Weight { get; set; }

    [Column("Description")]
    public string? Description { get; set; }
    
    [Column("CategoryID")]
    public virtual Category? Category { get; set; }

    [MaxLength(255)]
    [Column("Image")]
    public string? Image { get; set; }
    
    [Column]
    public bool? AgeRestricted { get; set; }
    
    [NotMapped]
    public decimal? DiscountedPrice;
    
    [NotMapped]
    public Promotion? AppliedPromotion;
}
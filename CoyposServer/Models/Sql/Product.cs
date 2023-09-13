using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoyposServer.Models.Sql;

using System;
using System.ComponentModel.DataAnnotations;

public class Product
{
    [Key] [Column("ID")]
    public int? ID { get; set; }

    [Column("CREATEDATE")]
    public DateTime? CreateDate { get; set; }

    [Column("UPDATEDATE")]
    public DateTime? UpdatedDate { get; set; }

    [Column("Status")]
    public bool? Status { get; set; } 

    [MaxLength(255)] [Column("Name")]
    public string? Name { get; set; }

    [MaxLength(20)] [Column("Barcode")]
    public string? Barcode { get; set; }

    [Column("Price")]
    public decimal? Price { get; set; }

    [Column("Quantity")]
    public int? Quantity { get; set; }

    [Column("isWeight")]
    public bool? IsWeight { get; set; }

    [Column("isLoose")]
    public bool? IsLoose { get; set; }

    [Column("Description")]
    public string? Description { get; set; }

    [MaxLength(50)]
    [Column("Category")]
    public string? Category { get; set; }

    [MaxLength(255)]
    [Column("Image")]
    public string? Image { get; set; }
}
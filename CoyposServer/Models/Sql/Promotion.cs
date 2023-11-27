using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CoyposServer.Models.Sql;

public class Promotion
{
    [Column("ID")] [Key]
    public int? ID { get; set; }

    [Column("IDs")]
    public string? Ids { get; set; }

    [Column("DiscountPercentage")] 
    public int? DiscountPercentage { get; set; }

    [Column("StartDate")] 
    public DateTime? StartDate { get; set; }

    [Column("EndDate")] 
    public DateTime? EndDate { get; set; }
    
    [Column("CreateDate")]
    public DateTime? CreateDate { get; set; }

    [Column("UpdateDate")]
    public DateTime? UpdateDate { get; set; }
}
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CoyposServer.Models.Sql;

public class Promotion
{
    [Key]
    public int ID { get; set; }

    [Required]
    public DateTime CreatedDate { get; set; }

    [Required]
    public DateTime UpdatedDate { get; set; }

    [ForeignKey("Product")]
    public int ProductID { get; set; }

    public virtual Product Product { get; set; } 

    public decimal DiscountPercentage { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }
}
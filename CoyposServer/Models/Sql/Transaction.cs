using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CoyposServer.Models.Sql;

public class Transaction
{
    [Key] 
    public int ID { get; set; }

    [Required]
    public DateTime CreatedDate { get; set; }

    [Required]
    public DateTime UpdatedDate { get; set; }

    [ForeignKey("User")]
    public int UserID { get; set; }

    public virtual User User { get; set; } 

    [ForeignKey("Product")] 
    public int ProductID { get; set; }

    public virtual Product Product { get; set; }

    public int Quantity { get; set; }

    public decimal TotalPrice { get; set; }

    [Required]
    public DateTime TransactionTime { get; set; }
}
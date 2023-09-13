namespace CoyposServer.Models.Sql;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class TransactionLog
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

    [MaxLength(50)]
    public string Action { get; set; }

    [Required]
    public DateTime Timestamp { get; set; }
}
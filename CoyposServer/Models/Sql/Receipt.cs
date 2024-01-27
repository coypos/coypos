using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoyposServer.Models.Sql;

public class Receipt
{
	[Key] [Column("ID")]
	public int? ID { get; set; }
	
	[Column("User")]
	public virtual User? User { get; set; }
	
	[Column("Action")]
	public string? Action { get; set; }
	
	[Column("TransactionId")]
	public string? TransactionId { get; set; }

	[Column("CreateDate")] 
	public DateTime? CreateDate { get; set; }

	[Column("UpdateDate")] 
	public DateTime? UpdateDate { get; set; }
	
	[NotMapped]
	public List<Transaction> Transactions { get; set; }
}
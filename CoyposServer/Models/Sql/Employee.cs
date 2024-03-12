using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace CoyposServer.Models.Sql;

public class Employee
{
	[Key] [Column("ID")]
	public int? ID { get; set; }
	
	[MaxLength(255)] [Column("Name")]
	public string? Name { get; set; }
	
	[Column("CardID")]
	public string? CardId { get; set; }
	
	[Column("PIN")]
	public string? PIN { get; set; }
	
	[Column("Enabled")]
	public bool? Enabled { get; set; }
	
	[Column("Admin")]
	public bool? Admin { get; set; }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoyposServer.Models.Sql;

public class PaymentMethod
{
	[Key] [Column("ID")]
	public int? ID { get; set; }
	
	[MaxLength(255)] [Column("Name")]
	public string Name { get; set; }
	
	[Column("Image")]
	public string? Image { get; set; }
	
	[Column("AuthData")]
	public string? AuthData { get; set; }
	
	[Column("Enabled")]
	public bool? Enabled { get; set; }
}
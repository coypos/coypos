using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoyposServer.Models.Sql;

public class Language
{
	[Key] [Column("ID")]
	public int? ID { get; set; }
	
	[MaxLength(255)] [Column("Name")]
	public string? Name { get; set; }
	
	[Column("CountryCode")]
	public string? CountryCode { get; set; }
	
	[Column("Enabled")]
	public bool? Enabled { get; set; }
	
	[Column("Image")]
	public string? Image { get; set; }
}
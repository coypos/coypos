using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoyposServer.Models.Sql;

public class Image
{
	[Key] [Column("id")]
	public int? ID { get; set; }
    
	[Column("image")]
	public string? Img { get; set; }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoyposServer.Models.Sql;

public class Setting
{
    [Key] [Column("ID")]
    public int? ID { get; set; }
    
    [Column("_key")]
    public string? Key { get; set; }
    
    [Column("_value")]
    public string? Value { get; set; }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoyposServer.Models.Sql;

public class Category
{
    [Key] [Column("ID")]
    public int? ID { get; set; }
    
    [Column("Name")]
    public string? Name { get; set; }
    
    [Column("ParentCategoryID")]
    public virtual Category? ParentCategory { get; set; }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CoyposServer.Utils.Extensions;

namespace CoyposServer.Models.Sql;

public class Category
{
    [Key] [Column("ID")]
    public int? ID { get; set; }
    
    [Column("Name")]
    public string? Name { get; set; }

    [Column("isVisible")]
    public bool? IsVisible { get; set; }
    
    [Column("ParentCategoryID")]
    public virtual Category? ParentCategory { get; set; }
    
    [Column("UpdateDate")] [Skipped]
    public DateTime? UpdateDate { get; set; }
    
    [Column("CreateDate")] [Skipped]
    public DateTime? CreateDate { get; set; }
}

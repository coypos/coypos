using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoyposServer.Models.Sql;

public class User
{
    [Key] [Column("ID")]
    public int? ID { get; set; }

    [Column("CreateDate")]
    public DateTime? CreateDate { get; set; }

    [Column("UpdateDate")]
    public DateTime? UpdateDate { get; set; }

    [Column("Name")]
    public string? Name { get; set; }

    [Column("Role")]
    public string? Role { get; set; }

    [Column("CardNumber")]
    public string? CardNumber { get; set; }

    [Column("PhoneNumber")]
    public string? PhoneNumber { get; set; }
    
    [Column("Points")]
    public int? Points { get; set; }
    
    [Column("Email")]
    public string? Email { get; set; }
    
    [Column("Password")]
    public string? Password { get; set; }
    
    [Column("Salt")]
    public string? Salt { get; set; }
    
    [Column("LoginToken")]
    public string? LoginToken { get; set; }

    [Column("LoginTokenValidDate")]
    public DateTime? LoginTokenValidDate { get; set; }
}
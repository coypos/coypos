using System;
using System.ComponentModel.DataAnnotations;

public class User
{
    [Key]
    public int ID { get; set; }

    [Required]
    public DateTime CreatedDate { get; set; }

    [Required]
    public DateTime UpdatedDate { get; set; }

    [MaxLength(255)]
    public string Name { get; set; }

    [MaxLength(50)]
    public string Role { get; set; }

    [MaxLength(20)]
    public string CardNumber { get; set; }

    [MaxLength(15)]
    public string PhoneNumber { get; set; }
}
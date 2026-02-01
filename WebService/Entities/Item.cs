using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebService.Entities;

[Table("items")]
public class Item
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("code")]
    public int Code { get; set; }

    [Column("value")]
    public string Value { get; set; } = string.Empty;
}

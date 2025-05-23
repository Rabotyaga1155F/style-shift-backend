using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StyleShiftBackend.Models;

[Table("style_card_statuses")]
public class StyleCardStatuses
{
    [Key]
    [Column("id")]
    public string ID { get; set; }= Guid.NewGuid().ToString();

    [Required]
    [MaxLength(50)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;
    

    public List<StyleCard> StyleCards { get; set; } = new();
}
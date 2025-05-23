using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace StyleShiftBackend.Models;
[Table("style_cards")]
public class StyleCard
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public string StyleCardID { get; set; }= Guid.NewGuid().ToString();
    [Column("user_id")]
    public string UserID { get; set; }

    [Column("style_card_status_id")]
    public string StyleCardStatusID { get; set; }

    [Column("quiz", TypeName = "jsonb")]
    public string Quiz { get; set; } = "{}";
    
    [ForeignKey(nameof(UserID))]
    public CustomUser? User { get; set; } = null!;

    [ForeignKey(nameof(StyleCardStatusID))]
    public StyleCardStatuses? Status { get; set; } = null!;
}
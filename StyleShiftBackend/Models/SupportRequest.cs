using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace StyleShiftBackend.Models;

[Table("support_requests")]
public class SupportRequest
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public string SupportRequestId{ get; set; }= Guid.NewGuid().ToString();

    [Required]
    [Column("user_id")]
    public string UserId { get; set; }
    
    [JsonIgnore]
    public CustomUser? User { get; set; }

    [Required]
    [MaxLength(200)]
    [Column("subject")]
    public string Subject { get; set; }

    [Required]
    [Column("email")] 
    public string Email { get; set; }

    [Required]
    [Column("message")] 
    public string Message { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    [Column("status_id")] 
    public string StatusId { get; set; }
    
    public SupportRequestStatus? Status { get; set; }
}
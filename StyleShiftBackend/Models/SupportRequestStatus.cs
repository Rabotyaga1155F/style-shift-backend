using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StyleShiftBackend.Models;

[Table("support_request_statuses")]
public class SupportRequestStatus
{

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public string SupportRequestStatusId{ get; set; }= Guid.NewGuid().ToString();

    [Required]
    [MaxLength(50)]
    [Column("name")]
    public string Name { get; set; }
}

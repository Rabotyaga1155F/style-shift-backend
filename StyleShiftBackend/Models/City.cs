using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StyleShiftBackend.Models
{
    [Table("cities")]
    public class City
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public string CityId { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [Column("name")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public ICollection<PickupPoint> PickupPoints { get; set; } = new List<PickupPoint>();
    }
}

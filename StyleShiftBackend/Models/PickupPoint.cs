using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace StyleShiftBackend.Models
{
    [Table("pickup_points")]
    public class PickupPoint
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public string PickupPointId { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [Column("address")]
        public string Address { get; set; }

        [Required]
        [Column("city_id")]
        public string CityId { get; set; }

        [ForeignKey(nameof(CityId))]
        public City City { get; set; } = null!;

        [Required]
        [Column("latitude")]
        public double Latitude { get; set; }

        [Required]
        [Column("longitude")]
        public double Longitude { get; set; }

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}

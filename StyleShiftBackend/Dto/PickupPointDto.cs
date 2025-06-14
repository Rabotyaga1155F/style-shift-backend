namespace StyleShiftBackend.Dto
{
    public class PickupPointDto
    {
        public string Address { get; set; } = string.Empty;

        public string CityId { get; set; } = string.Empty;

        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }
}

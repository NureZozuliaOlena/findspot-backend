using System.Text.Json.Serialization;

namespace findspot_backend.Models.DTO
{
    public class TouristObjectDto
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public TimeSpan OpeningTime { get; set; }
        public TimeSpan ClosingTime { get; set; }
    }
}

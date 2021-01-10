using System.ComponentModel.DataAnnotations;

namespace API.Dtos
{
    public class RegisterOrUpdateVesselDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Imo { get; set; }
        [Required]
        public string Flag { get; set; }
        [Required]
        public double Deadweight { get; set; }
        [Required]
        public double LengthOverall { get; set; }
        [Required]
        public double Beam { get; set; }
        [Required]
        public double Depth { get; set; }
    }
}
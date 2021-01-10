namespace API.Dtos
{
    public class VesselDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Imo { get; set; }
        public string Flag { get; set; }
        public double Deadweight { get; set; }
        public double LengthOverall { get; set; }
        public double Beam { get; set; }
        public double Depth { get; set; }
    }
}
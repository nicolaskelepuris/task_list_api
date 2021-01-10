using System.Collections.Generic;

namespace API.Dtos
{
    public class VesselsDto
    {
        public IReadOnlyList<VesselDto> Vessels { get; set; }
    }
}
using System.Collections.Generic;

namespace API.Dtos
{
    public class UsersDto
    {
        public IReadOnlyList<UserDto> Users { get; set; }
    }
}
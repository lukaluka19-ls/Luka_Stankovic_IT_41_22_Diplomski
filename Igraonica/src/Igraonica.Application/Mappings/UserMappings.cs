using Igraonica.Application.DTOs;
using Igraonica.Domain.Entities;

namespace Igraonica.Application.Mappings;

public static class UserMappings
{
    public static UserDto ToDto(this User user) => new(
        user.Id, user.Email, user.FirstName, user.LastName,
        user.PhoneNumber, user.Role.ToString(), user.IsEmailVerified);
}

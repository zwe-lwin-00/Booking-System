using BookingSystem.Application.DTOs;

namespace BookingSystem.Application.Services;

public interface IUserService
{
    Task<UserDto?> GetByIdAsync(Guid id);
    Task<UserDto> UpdateAsync(Guid id, CreateUserDto updateUserDto);
}

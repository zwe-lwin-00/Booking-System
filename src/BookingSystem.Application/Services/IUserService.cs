using BookingSystem.Application.DTOs;

namespace BookingSystem.Application.Services;

public interface IUserService
{
    Task<UserDto?> GetByIdAsync(Guid id);
    Task<UserDto?> GetByEmailAsync(string email);
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task<UserDto> CreateAsync(CreateUserDto createUserDto);
    Task<UserDto> UpdateAsync(Guid id, CreateUserDto updateUserDto);
    Task DeleteAsync(Guid id);
}

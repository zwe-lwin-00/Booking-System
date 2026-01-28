using BookingSystem.Application.Common.Exceptions;
using BookingSystem.Application.DTOs;
using BookingSystem.Domain.Entities;
using BookingSystem.Domain.Interfaces;

namespace BookingSystem.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto?> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user == null ? null : MapToDto(user);
    }

    public async Task<UserDto?> GetByEmailAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        return user == null ? null : MapToDto(user);
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(MapToDto);
    }

    public async Task<UserDto> CreateAsync(CreateUserDto createUserDto)
    {
        // Check if email already exists
        var existingUser = await _userRepository.GetByEmailAsync(createUserDto.Email);
        if (existingUser != null)
            throw new InvalidOperationException($"User with email {createUserDto.Email} already exists");

        var user = new User
        {
            FirstName = createUserDto.FirstName,
            LastName = createUserDto.LastName,
            Email = createUserDto.Email,
            PhoneNumber = createUserDto.PhoneNumber
        };

        var createdUser = await _userRepository.AddAsync(user);
        return MapToDto(createdUser);
    }

    public async Task<UserDto> UpdateAsync(Guid id, CreateUserDto updateUserDto)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            throw new NotFoundException(nameof(User), id);

        // Check if email is being changed and if it conflicts
        if (user.Email != updateUserDto.Email)
        {
            var existingUser = await _userRepository.GetByEmailAsync(updateUserDto.Email);
            if (existingUser != null)
                throw new InvalidOperationException($"User with email {updateUserDto.Email} already exists");
        }

        user.FirstName = updateUserDto.FirstName;
        user.LastName = updateUserDto.LastName;
        user.Email = updateUserDto.Email;
        user.PhoneNumber = updateUserDto.PhoneNumber;

        await _userRepository.UpdateAsync(user);
        return MapToDto(user);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _userRepository.DeleteAsync(id);
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber
        };
    }
}

using BookingSystem.Application.DTOs;
using BookingSystem.Domain.Interfaces;

namespace BookingSystem.Application.Services;

public class ClassService : IClassService
{
    private readonly IClassRepository _classRepository;

    public ClassService(IClassRepository classRepository)
    {
        _classRepository = classRepository;
    }

    public async Task<ClassDto?> GetByIdAsync(Guid id)
    {
        var classEntity = await _classRepository.GetByIdAsync(id);
        return classEntity == null ? null : MapToDto(classEntity);
    }

    public async Task<IEnumerable<ClassDto>> GetByCountryIdAsync(Guid countryId)
    {
        var classes = await _classRepository.GetByCountryIdAsync(countryId);
        return classes.Where(c => c.StartTime > DateTime.UtcNow).Select(MapToDto);
    }

    public async Task<IEnumerable<ClassDto>> GetUpcomingAsync()
    {
        var classes = await _classRepository.GetUpcomingAsync(DateTime.UtcNow);
        return classes.Select(MapToDto);
    }

    private static ClassDto MapToDto(Domain.Entities.Class classEntity)
    {
        return new ClassDto
        {
            Id = classEntity.Id,
            Name = classEntity.Name,
            Description = classEntity.Description,
            CountryId = classEntity.CountryId,
            CountryName = classEntity.Country.Name,
            CountryCode = classEntity.Country.Code,
            StartTime = classEntity.StartTime,
            EndTime = classEntity.EndTime,
            RequiredCredits = classEntity.RequiredCredits,
            MaxCapacity = classEntity.MaxCapacity,
            CurrentBookings = classEntity.CurrentBookings,
            AvailableSlots = classEntity.MaxCapacity - classEntity.CurrentBookings,
            IsFull = classEntity.IsFull
        };
    }
}

using BookingSystem.Application.DTOs;
using BookingSystem.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace BookingSystem.Application.Services;

public class ClassService : IClassService
{
    private readonly IClassRepository _classRepository;
    private readonly ILogger<ClassService> _logger;

    public ClassService(IClassRepository classRepository, ILogger<ClassService> logger)
    {
        _classRepository = classRepository;
        _logger = logger;
    }

    public async Task<ClassDto?> GetByIdAsync(Guid id)
    {
        _logger.LogInformation("Get class by id {ClassId}", id);
        var classEntity = await _classRepository.GetByIdAsync(id);
        if (classEntity == null)
        {
            _logger.LogWarning("Class {ClassId} not found", id);
            return null;
        }
        return MapToDto(classEntity);
    }

    public async Task<IEnumerable<ClassDto>> GetByCountryIdAsync(Guid countryId)
    {
        _logger.LogInformation("Get classes by country {CountryId}", countryId);
        var classes = await _classRepository.GetByCountryIdAsync(countryId);
        return classes.Where(c => c.StartTime > DateTime.UtcNow).Select(MapToDto);
    }

    public async Task<IEnumerable<ClassDto>> GetUpcomingAsync()
    {
        _logger.LogInformation("Get upcoming classes");
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

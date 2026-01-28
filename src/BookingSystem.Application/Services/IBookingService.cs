using BookingSystem.Application.DTOs;

namespace BookingSystem.Application.Services;

public interface IBookingService
{
    Task<BookingDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<BookingDto>> GetAllAsync();
    Task<PagedResultDto<BookingDto>> GetPagedAsync(BookingQueryDto query);
    Task<IEnumerable<BookingDto>> GetByUserIdAsync(Guid userId);
    Task<BookingDto> CreateAsync(CreateBookingDto createBookingDto);
    Task<BookingDto> UpdateAsync(Guid id, UpdateBookingDto updateBookingDto);
    Task<BookingDto> UpdateStatusAsync(Guid id, Domain.Enums.BookingStatus status);
    Task DeleteAsync(Guid id);
}

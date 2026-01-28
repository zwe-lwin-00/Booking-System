using BookingSystem.Application.DTOs;
using BookingSystem.Application.Services;
using BookingSystem.Domain.Entities;
using BookingSystem.Domain.Enums;
using BookingSystem.Domain.Interfaces;
using FluentValidation;
using Moq;
using Xunit;

namespace BookingSystem.UnitTests.Services;

public class BookingServiceTests
{
    private readonly Mock<IBookingRepository> _bookingRepositoryMock;
    private readonly Mock<IRoomRepository> _roomRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IValidator<CreateBookingDto>> _validatorMock;
    private readonly BookingService _bookingService;

    public BookingServiceTests()
    {
        _bookingRepositoryMock = new Mock<IBookingRepository>();
        _roomRepositoryMock = new Mock<IRoomRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _validatorMock = new Mock<IValidator<CreateBookingDto>>();
        _bookingService = new BookingService(
            _bookingRepositoryMock.Object,
            _roomRepositoryMock.Object,
            _userRepositoryMock.Object,
            _validatorMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsBooking_WhenBookingExists()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var booking = new Booking
        {
            Id = bookingId,
            User = new User { FirstName = "John", LastName = "Doe" },
            Room = new Room { RoomNumber = "101" },
            CheckInDate = DateTime.Now.AddDays(1),
            CheckOutDate = DateTime.Now.AddDays(3),
            Status = BookingStatus.Confirmed
        };

        _bookingRepositoryMock.Setup(x => x.GetByIdAsync(bookingId))
            .ReturnsAsync(booking);

        // Act
        var result = await _bookingService.GetByIdAsync(bookingId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(bookingId, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenBookingDoesNotExist()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        _bookingRepositoryMock.Setup(x => x.GetByIdAsync(bookingId))
            .ReturnsAsync((Booking?)null);

        // Act
        var result = await _bookingService.GetByIdAsync(bookingId);

        // Assert
        Assert.Null(result);
    }
}

using BookingSystem.Application.Common.Exceptions;
using BookingSystem.Application.DTOs;
using BookingSystem.Domain.Entities;
using BookingSystem.Domain.Enums;
using BookingSystem.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace BookingSystem.Application.Services;

public class ScheduleService : IScheduleService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IClassRepository _classRepository;
    private readonly IUserPackageRepository _userPackageRepository;
    private readonly IWaitlistRepository _waitlistRepository;
    private readonly IDatabase _redis;
    private readonly ILogger<ScheduleService> _logger;

    public ScheduleService(
        IBookingRepository bookingRepository,
        IClassRepository classRepository,
        IUserPackageRepository userPackageRepository,
        IWaitlistRepository waitlistRepository,
        IConnectionMultiplexer redis,
        ILogger<ScheduleService> logger)
    {
        _bookingRepository = bookingRepository;
        _classRepository = classRepository;
        _userPackageRepository = userPackageRepository;
        _waitlistRepository = waitlistRepository;
        _redis = redis.GetDatabase();
        _logger = logger;
    }

    public async Task<BookingDto> BookClassAsync(Guid userId, BookClassDto bookClassDto)
    {
        _logger.LogInformation("Book class {ClassId} by user {UserId}, UserPackageId: {UserPackageId}", bookClassDto.ClassId, userId, bookClassDto.UserPackageId);

        var classEntity = await _classRepository.GetByIdAsync(bookClassDto.ClassId);
        if (classEntity == null)
        {
            _logger.LogWarning("Book class failed: class {ClassId} not found", bookClassDto.ClassId);
            throw new NotFoundException(nameof(Class), bookClassDto.ClassId);
        }

        var userPackage = await _userPackageRepository.GetByIdAsync(bookClassDto.UserPackageId);
        if (userPackage == null || userPackage.UserId != userId)
        {
            _logger.LogWarning("Book class failed: user package {UserPackageId} not found or not owned by user {UserId}", bookClassDto.UserPackageId, userId);
            throw new NotFoundException(nameof(UserPackage), bookClassDto.UserPackageId);
        }

        if (userPackage.IsExpired)
        {
            _logger.LogWarning("Book class failed: user package {UserPackageId} has expired", bookClassDto.UserPackageId);
            throw new InvalidOperationException("Package has expired");
        }

        if (userPackage.RemainingCredits < classEntity.RequiredCredits)
        {
            _logger.LogWarning("Book class failed: insufficient credits for user {UserId}, required {RequiredCredits}", userId, classEntity.RequiredCredits);
            throw new InvalidOperationException("Insufficient credits in package");
        }

        // Check country match
        if (userPackage.Package.CountryId != classEntity.CountryId)
        {
            _logger.LogWarning("Book class failed: package country does not match class country for user {UserId}", userId);
            throw new InvalidOperationException($"Package is for {userPackage.Package.Country.Name}, but class is in {classEntity.Country.Name}");
        }

        // Check for overlapping bookings
        var overlappingBookings = await _bookingRepository.GetByUserIdAndTimeRangeAsync(
            userId, classEntity.StartTime, classEntity.EndTime);
        
        if (overlappingBookings.Any(b => b.Status != BookingStatus.Cancelled))
        {
            _logger.LogWarning("Book class failed: user {UserId} has overlapping booking for class {ClassId}", userId, bookClassDto.ClassId);
            throw new InvalidOperationException("You already have a booking that overlaps with this class time");
        }

        // Use Redis lock for concurrent booking prevention
        var lockKey = $"booking_lock:class:{classEntity.Id}";
        var lockValue = Guid.NewGuid().ToString();
        var lockAcquired = await _redis.StringSetAsync(lockKey, lockValue, TimeSpan.FromSeconds(10), When.NotExists);

        if (!lockAcquired)
        {
            _logger.LogWarning("Book class failed: could not acquire lock for class {ClassId}", classEntity.Id);
            throw new InvalidOperationException("Class is being booked by another user. Please try again.");
        }

        try
        {
            // Re-check capacity after acquiring lock
            var currentClass = await _classRepository.GetByIdAsync(classEntity.Id);
            if (currentClass == null || currentClass.IsFull)
            {
                _logger.LogWarning("Book class failed: class {ClassId} is full after lock", classEntity.Id);
                throw new InvalidOperationException("Class is full");
            }

            // Deduct credits
            userPackage.RemainingCredits -= classEntity.RequiredCredits;

            // Create booking
            var booking = new Booking
            {
                UserId = userId,
                ClassId = classEntity.Id,
                UserPackageId = userPackage.Id,
                CreditsUsed = classEntity.RequiredCredits,
                Status = BookingStatus.Booked
            };

            // Increment booking count
            await _classRepository.IncrementBookingCountAsync(classEntity.Id);

            var createdBooking = await _bookingRepository.AddAsync(booking);
            await _userPackageRepository.UpdateAsync(userPackage);

            // Get booking with navigation properties for mapping
            var result = await _bookingRepository.GetByIdAsync(createdBooking.Id);
            _logger.LogInformation("Class booked successfully. BookingId: {BookingId}, UserId: {UserId}, ClassId: {ClassId}", createdBooking.Id, userId, classEntity.Id);
            return MapToDto(result!);
        }
        finally
        {
            // Release lock using Lua script for atomic operation
            var script = "if redis.call('get', KEYS[1]) == ARGV[1] then return redis.call('del', KEYS[1]) else return 0 end";
            await _redis.ScriptEvaluateAsync(script, new RedisKey[] { lockKey }, new RedisValue[] { lockValue });
        }
    }

    public async Task CancelBookingAsync(Guid userId, Guid bookingId)
    {
        _logger.LogInformation("Cancel booking {BookingId} by user {UserId}", bookingId, userId);

        var booking = await _bookingRepository.GetByIdAsync(bookingId);
        if (booking == null || booking.UserId != userId)
        {
            _logger.LogWarning("Cancel booking failed: booking {BookingId} not found or not owned by user {UserId}", bookingId, userId);
            throw new NotFoundException(nameof(Booking), bookingId);
        }

        if (booking.Status == BookingStatus.Cancelled)
        {
            _logger.LogWarning("Cancel booking failed: booking {BookingId} is already cancelled", bookingId);
            throw new InvalidOperationException("Booking is already cancelled");
        }

        var classEntity = await _classRepository.GetByIdAsync(booking.ClassId);
        if (classEntity == null)
        {
            _logger.LogWarning("Cancel booking failed: class {ClassId} not found", booking.ClassId);
            throw new NotFoundException(nameof(Class), booking.ClassId);
        }

        // Check if cancellation is within 4 hours
        var hoursUntilClass = (classEntity.StartTime - DateTime.UtcNow).TotalHours;
        var shouldRefund = hoursUntilClass >= 4;

        // Refund credits if cancelled 4+ hours before
        if (shouldRefund)
        {
            var userPackage = await _userPackageRepository.GetByIdAsync(booking.UserPackageId);
            if (userPackage != null)
            {
                userPackage.RemainingCredits += booking.CreditsUsed;
                await _userPackageRepository.UpdateAsync(userPackage);
            }
        }

        booking.Status = BookingStatus.Cancelled;
        await _bookingRepository.UpdateAsync(booking);

        // Decrement booking count
        await _classRepository.DecrementBookingCountAsync(classEntity.Id);

        // Check if there's a waitlist and promote next user
        await PromoteWaitlistUserAsync(classEntity.Id);
        _logger.LogInformation("Booking {BookingId} cancelled successfully by user {UserId}, refund: {ShouldRefund}", bookingId, userId, shouldRefund);
    }

    public async Task AddToWaitlistAsync(Guid userId, Guid classId, Guid userPackageId)
    {
        _logger.LogInformation("Add user {UserId} to waitlist for class {ClassId}, UserPackageId: {UserPackageId}", userId, classId, userPackageId);

        var classEntity = await _classRepository.GetByIdAsync(classId);
        if (classEntity == null)
        {
            _logger.LogWarning("Add to waitlist failed: class {ClassId} not found", classId);
            throw new NotFoundException(nameof(Class), classId);
        }

        if (!classEntity.IsFull)
        {
            _logger.LogWarning("Add to waitlist failed: class {ClassId} is not full", classId);
            throw new InvalidOperationException("Class is not full. You can book directly.");
        }

        var userPackage = await _userPackageRepository.GetByIdAsync(userPackageId);
        if (userPackage == null || userPackage.UserId != userId)
        {
            _logger.LogWarning("Add to waitlist failed: user package {UserPackageId} not found or not owned by user {UserId}", userPackageId, userId);
            throw new NotFoundException(nameof(UserPackage), userPackageId);
        }

        if (userPackage.IsExpired)
        {
            _logger.LogWarning("Add to waitlist failed: user package {UserPackageId} has expired", userPackageId);
            throw new InvalidOperationException("Package has expired");
        }

        if (userPackage.RemainingCredits < classEntity.RequiredCredits)
        {
            _logger.LogWarning("Add to waitlist failed: insufficient credits for user {UserId}", userId);
            throw new InvalidOperationException("Insufficient credits in package");
        }

        // Check if already in waitlist
        var existingWaitlists = await _waitlistRepository.GetByClassIdAsync(classId);
        if (existingWaitlists.Any(w => w.UserId == userId && !w.IsPromoted))
        {
            _logger.LogWarning("Add to waitlist failed: user {UserId} already in waitlist for class {ClassId}", userId, classId);
            throw new InvalidOperationException("You are already in the waitlist for this class");
        }

        // Get next position
        var position = await _waitlistRepository.GetNextPositionAsync(classId);

        var waitlist = new Waitlist
        {
            UserId = userId,
            ClassId = classId,
            UserPackageId = userPackageId,
            CreditsReserved = classEntity.RequiredCredits,
            Position = position
        };

        await _waitlistRepository.AddAsync(waitlist);
        _logger.LogInformation("User {UserId} added to waitlist for class {ClassId}, position {Position}", userId, classId, position);
    }

    public async Task CheckInAsync(Guid userId, Guid bookingId)
    {
        _logger.LogInformation("Check-in for booking {BookingId} by user {UserId}", bookingId, userId);

        var booking = await _bookingRepository.GetByIdAsync(bookingId);
        if (booking == null || booking.UserId != userId)
        {
            _logger.LogWarning("Check-in failed: booking {BookingId} not found or not owned by user {UserId}", bookingId, userId);
            throw new NotFoundException(nameof(Booking), bookingId);
        }

        if (booking.Status == BookingStatus.Cancelled)
        {
            _logger.LogWarning("Check-in failed: booking {BookingId} is cancelled", bookingId);
            throw new InvalidOperationException("Cannot check in to a cancelled booking");
        }

        var classEntity = await _classRepository.GetByIdAsync(booking.ClassId);
        if (classEntity == null)
        {
            _logger.LogWarning("Check-in failed: class {ClassId} not found", booking.ClassId);
            throw new NotFoundException(nameof(Class), booking.ClassId);
        }

        // Check if class time has reached
        if (DateTime.UtcNow < classEntity.StartTime.AddMinutes(-15))
        {
            _logger.LogWarning("Check-in failed: too early for booking {BookingId}", bookingId);
            throw new InvalidOperationException("Check-in is only available 15 minutes before class starts");
        }

        if (DateTime.UtcNow > classEntity.EndTime)
        {
            _logger.LogWarning("Check-in failed: class has ended for booking {BookingId}", bookingId);
            throw new InvalidOperationException("Class has already ended");
        }

        booking.IsCheckedIn = true;
        booking.CheckInTime = DateTime.UtcNow;
        booking.Status = BookingStatus.CheckedIn;
        await _bookingRepository.UpdateAsync(booking);
        _logger.LogInformation("Check-in successful for booking {BookingId}, user {UserId}", bookingId, userId);
    }

    public async Task<IEnumerable<BookingDto>> GetUserBookingsAsync(Guid userId)
    {
        _logger.LogInformation("Get user bookings for user {UserId}", userId);
        var bookings = await _bookingRepository.GetByUserIdAsync(userId);
        return bookings.Select(MapToDto);
    }

    private async Task PromoteWaitlistUserAsync(Guid classId)
    {
        var nextWaitlist = await _waitlistRepository.GetNextInWaitlistAsync(classId);
        if (nextWaitlist == null)
            return;

        var classEntity = await _classRepository.GetByIdAsync(classId);
        if (classEntity == null || classEntity.IsFull)
            return;

        // Create booking from waitlist
        var booking = new Booking
        {
            UserId = nextWaitlist.UserId,
            ClassId = classId,
            UserPackageId = nextWaitlist.UserPackageId,
            CreditsUsed = nextWaitlist.CreditsReserved,
            Status = BookingStatus.Booked
        };

        // Deduct credits
        var userPackage = await _userPackageRepository.GetByIdAsync(nextWaitlist.UserPackageId);
        if (userPackage != null)
        {
            userPackage.RemainingCredits -= nextWaitlist.CreditsReserved;
            await _userPackageRepository.UpdateAsync(userPackage);
        }

        await _bookingRepository.AddAsync(booking);
        await _classRepository.IncrementBookingCountAsync(classId);

        // Mark waitlist as promoted
        nextWaitlist.IsPromoted = true;
        await _waitlistRepository.UpdateAsync(nextWaitlist);
    }

    private static BookingDto MapToDto(Booking booking)
    {
        return new BookingDto
        {
            Id = booking.Id,
            UserId = booking.UserId,
            UserName = $"{booking.User.FirstName} {booking.User.LastName}",
            ClassId = booking.ClassId,
            ClassName = booking.Class.Name,
            CheckInDate = booking.Class.StartTime,
            CheckOutDate = booking.Class.EndTime,
            NumberOfGuests = 1,
            TotalPrice = 0,
            Status = booking.Status,
            CreditsUsed = booking.CreditsUsed,
            IsCheckedIn = booking.IsCheckedIn,
            CheckInTime = booking.CheckInTime,
            CreatedAt = booking.CreatedAt
        };
    }
}

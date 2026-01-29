using BookingSystem.Domain.Interfaces;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace BookingSystem.Infrastructure.Jobs;

public class WaitlistRefundJob
{
    private readonly IWaitlistRepository _waitlistRepository;
    private readonly IUserPackageRepository _userPackageRepository;
    private readonly IClassRepository _classRepository;
    private readonly ILogger<WaitlistRefundJob> _logger;

    public WaitlistRefundJob(
        IWaitlistRepository waitlistRepository,
        IUserPackageRepository userPackageRepository,
        IClassRepository classRepository,
        ILogger<WaitlistRefundJob> logger)
    {
        _waitlistRepository = waitlistRepository;
        _userPackageRepository = userPackageRepository;
        _classRepository = classRepository;
        _logger = logger;
    }

    [AutomaticRetry(Attempts = 3)]
    public async Task ProcessWaitlistRefunds()
    {
        _logger.LogInformation("WaitlistRefundJob started: processing refunds for ended classes");
        try
        {
            var allClasses = await _classRepository.GetAllAsync();
            var endedClasses = allClasses.Where(c => c.EndTime < DateTime.UtcNow).ToList();
            _logger.LogInformation("WaitlistRefundJob: found {Count} ended classes to process", endedClasses.Count);

            foreach (var classEntity in endedClasses)
            {
                var waitlists = await _waitlistRepository.GetPendingByClassIdAsync(classEntity.Id);
                var pending = waitlists.Where(w => !w.IsPromoted).ToList();

                foreach (var waitlist in pending)
                {
                    var userPackage = await _userPackageRepository.GetByIdAsync(waitlist.UserPackageId);
                    if (userPackage != null)
                    {
                        userPackage.RemainingCredits += waitlist.CreditsReserved;
                        await _userPackageRepository.UpdateAsync(userPackage);
                        _logger.LogInformation("WaitlistRefundJob: refunded {Credits} credits to UserPackage {UserPackageId} for Waitlist {WaitlistId}", waitlist.CreditsReserved, userPackage.Id, waitlist.Id);
                    }
                    else
                    {
                        _logger.LogWarning("WaitlistRefundJob: UserPackage {UserPackageId} not found for Waitlist {WaitlistId}", waitlist.UserPackageId, waitlist.Id);
                    }

                    waitlist.IsPromoted = true;
                    await _waitlistRepository.UpdateAsync(waitlist);
                }
            }

            _logger.LogInformation("WaitlistRefundJob completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "WaitlistRefundJob failed with error");
            throw;
        }
    }
}

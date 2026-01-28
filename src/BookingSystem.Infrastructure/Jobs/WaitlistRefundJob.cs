using BookingSystem.Domain.Interfaces;
using Hangfire;

namespace BookingSystem.Infrastructure.Jobs;

public class WaitlistRefundJob
{
    private readonly IWaitlistRepository _waitlistRepository;
    private readonly IUserPackageRepository _userPackageRepository;
    private readonly IClassRepository _classRepository;

    public WaitlistRefundJob(
        IWaitlistRepository waitlistRepository,
        IUserPackageRepository userPackageRepository,
        IClassRepository classRepository)
    {
        _waitlistRepository = waitlistRepository;
        _userPackageRepository = userPackageRepository;
        _classRepository = classRepository;
    }

    [AutomaticRetry(Attempts = 3)]
    public async Task ProcessWaitlistRefunds()
    {
        var allClasses = await _classRepository.GetAllAsync();
        var endedClasses = allClasses.Where(c => c.EndTime < DateTime.UtcNow);

        foreach (var classEntity in endedClasses)
        {
            var waitlists = await _waitlistRepository.GetPendingByClassIdAsync(classEntity.Id);
            
            foreach (var waitlist in waitlists.Where(w => !w.IsPromoted))
            {
                // Refund credits to user package
                var userPackage = await _userPackageRepository.GetByIdAsync(waitlist.UserPackageId);
                if (userPackage != null)
                {
                    userPackage.RemainingCredits += waitlist.CreditsReserved;
                    await _userPackageRepository.UpdateAsync(userPackage);
                }

                // Mark waitlist as processed
                waitlist.IsPromoted = true;
                await _waitlistRepository.UpdateAsync(waitlist);
            }
        }
    }
}

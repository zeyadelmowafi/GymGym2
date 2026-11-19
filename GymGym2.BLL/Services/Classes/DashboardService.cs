using GymGym2.BLL.ViewModels;
using GymGym2.DAL.Repositories;

namespace GymGym2.BLL.Services.Classes;

public class DashboardService(IUnitOfWork unitOfWork) : IDashboardService
{
    public async Task<DashboardViewModel> GetAsync(CancellationToken ct = default)
    {
        var now = DateTime.Now;
        var members = await unitOfWork.Members.GetAllAsync(ct);
        var memberships = await unitOfWork.Memberships.GetAllAsync(ct);
        var trainers = await unitOfWork.Trainers.GetAllAsync(ct);
        var sessions = await unitOfWork.Sessions.GetAllWithDetailsAsync(ct);
        var activeMemberIds = memberships.Where(x => x.EndDate > now).Select(x => x.MemberId).Distinct().ToHashSet();
        return new DashboardViewModel
        {
            TotalMembers = members.Count,
            ActiveMembers = activeMemberIds.Count,
            TotalTrainers = trainers.Count,
            UpcomingSessions = sessions.Count(x => x.StartDate > now),
            OngoingSessions = sessions.Count(x => x.StartDate <= now && x.EndDate >= now),
            CompletedSessions = sessions.Count(x => x.EndDate < now)
        };
    }
}

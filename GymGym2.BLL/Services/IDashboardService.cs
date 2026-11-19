using GymGym2.BLL.ViewModels;

namespace GymGym2.BLL.Services;

public interface IDashboardService { Task<DashboardViewModel> GetAsync(CancellationToken ct = default); }

using GymGym2.BLL.Results;
using GymGym2.BLL.ViewModels;

namespace GymGym2.BLL.Services;

public interface IPlanService
{
    Task<IReadOnlyList<PlanViewModel>> GetAllAsync(CancellationToken ct = default);
    Task<Result<PlanViewModel>> GetDetailsAsync(int id, CancellationToken ct = default);
    Task<Result<UpdatePlanViewModel>> GetForUpdateAsync(int id, CancellationToken ct = default);
    Task<Result<bool>> UpdateAsync(int id, UpdatePlanViewModel model, CancellationToken ct = default);
    Task<Result<bool>> ToggleActivationAsync(int id, CancellationToken ct = default);
}

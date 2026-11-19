using GymGym2.BLL.Results;
using GymGym2.BLL.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymGym2.BLL.Services;

public interface ITrainerService
{
    Task<IReadOnlyList<TrainerViewModel>> GetAllAsync(CancellationToken ct = default);
    Task<Result<TrainerToUpdateViewModel>> GetForUpdateAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<SelectListItem>> GetSpecialtiesAsync(CancellationToken ct = default);
    Task<Result<int>> CreateAsync(CreateTrainerViewModel model, CancellationToken ct = default);
    Task<Result<bool>> UpdateAsync(TrainerToUpdateViewModel model, CancellationToken ct = default);
    Task<Result<TrainerDeleteViewModel>> GetForDeleteAsync(int id, CancellationToken ct = default);
    Task<Result<bool>> DeleteAsync(int id, CancellationToken ct = default);
}

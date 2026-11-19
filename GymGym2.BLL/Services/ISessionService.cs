using GymGym2.BLL.Results;
using GymGym2.BLL.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymGym2.BLL.Services;

public interface ISessionService
{
    Task<IReadOnlyList<SessionViewModel>> GetAllAsync(CancellationToken ct = default);
    Task<Result<SessionViewModel>> GetDetailsAsync(int id, CancellationToken ct = default);
    Task<Result<UpdateSessionViewModel>> GetForUpdateAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<SelectListItem>> GetCategoriesAsync(CancellationToken ct = default);
    Task<IReadOnlyList<SelectListItem>> GetTrainersAsync(int categoryId, CancellationToken ct = default);
    Task<Result> CreateAsync(CreateSessionViewModel model, CancellationToken ct = default);
    Task<Result> UpdateAsync(int id, UpdateSessionViewModel model, CancellationToken ct = default);
    Task<Result> DeleteAsync(int id, CancellationToken ct = default);
}

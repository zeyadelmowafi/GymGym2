using GymGym2.BLL.Results;
using GymGym2.BLL.ViewModels;

namespace GymGym2.BLL.Services;

public interface IMemberService
{
    Task<IReadOnlyList<MemberViewModel>> GetAllAsync(CancellationToken ct = default);
    Task<Result<int>> CreateAsync(CreateMemberViewModel model, CancellationToken ct = default);
}

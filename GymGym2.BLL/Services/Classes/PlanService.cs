using AutoMapper;
using GymGym2.BLL.Results;
using GymGym2.BLL.Services;
using GymGym2.BLL.ViewModels;
using GymGym2.DAL.Entities;
using GymGym2.DAL.Repositories;

namespace GymGym2.BLL.Services.Classes;

public class PlanService(IUnitOfWork unitOfWork, IMapper mapper) : IPlanService
{
    public async Task<IReadOnlyList<PlanViewModel>> GetAllAsync(CancellationToken ct = default) => mapper.Map<IReadOnlyList<PlanViewModel>>(await unitOfWork.Plans.GetAllAsync(ct));
    public async Task<Result<PlanViewModel>> GetDetailsAsync(int id, CancellationToken ct = default)
    {
        var plan = await unitOfWork.Plans.GetByIdAsync(id, ct);
        return plan is null ? Result<PlanViewModel>.NotFound("Plan not found.") : Result<PlanViewModel>.Ok(mapper.Map<PlanViewModel>(plan));
    }
    public async Task<Result<UpdatePlanViewModel>> GetForUpdateAsync(int id, CancellationToken ct = default)
    {
        var plan = await unitOfWork.Plans.GetByIdAsync(id, ct);
        if (plan is null) return Result<UpdatePlanViewModel>.NotFound("Plan not found.");
        if (!plan.IsActive) return Result<UpdatePlanViewModel>.Fail("Inactive plans cannot be edited.", ResultKind.Forbidden);
        if (await HasActiveMembershipAsync(id, ct)) return Result<UpdatePlanViewModel>.Fail("This plan has active memberships and cannot be edited.", ResultKind.Conflict);
        return Result<UpdatePlanViewModel>.Ok(mapper.Map<UpdatePlanViewModel>(plan));
    }
    public async Task<Result<bool>> UpdateAsync(int id, UpdatePlanViewModel model, CancellationToken ct = default)
    {
        var plan = await unitOfWork.Plans.GetByIdAsync(id, ct);
        if (plan is null) return Result<bool>.NotFound("Plan not found.");
        if (!plan.IsActive || await HasActiveMembershipAsync(id, ct)) return Result<bool>.Fail("The plan cannot be changed while inactive or while memberships are active.", ResultKind.Conflict);
        plan.Description = model.Description.Trim(); plan.DurationDays = model.DurationDays; plan.Price = model.Price; plan.UpdatedAt = DateTime.UtcNow;
        unitOfWork.Plans.Update(plan); await unitOfWork.SaveChangesAsync(ct); return Result<bool>.Ok(true);
    }
    public async Task<Result<bool>> ToggleActivationAsync(int id, CancellationToken ct = default)
    {
        var plan = await unitOfWork.Plans.GetByIdAsync(id, ct);
        if (plan is null) return Result<bool>.NotFound("Plan not found.");
        if (plan.IsActive && await HasActiveMembershipAsync(id, ct)) return Result<bool>.Fail("A plan with active memberships cannot be deactivated.", ResultKind.Conflict);
        plan.IsActive = !plan.IsActive; plan.UpdatedAt = DateTime.UtcNow; unitOfWork.Plans.Update(plan); await unitOfWork.SaveChangesAsync(ct);
        return Result<bool>.Ok(true);
    }
    private async Task<bool> HasActiveMembershipAsync(int planId, CancellationToken ct) =>
        (await unitOfWork.Memberships.GetAllAsync(ct)).Any(m => m.PlanId == planId && m.EndDate > DateTime.UtcNow);
}

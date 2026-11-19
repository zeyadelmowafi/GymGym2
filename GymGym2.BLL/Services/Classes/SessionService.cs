using AutoMapper;
using GymGym2.BLL.Results;
using GymGym2.BLL.ViewModels;
using GymGym2.DAL.Entities;
using GymGym2.DAL.Repositories;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymGym2.BLL.Services.Classes;

public class SessionService(IUnitOfWork unitOfWork, IMapper mapper) : ISessionService
{
    public async Task<IReadOnlyList<SessionViewModel>> GetAllAsync(CancellationToken ct = default) =>
        mapper.Map<IReadOnlyList<SessionViewModel>>(await unitOfWork.Sessions.GetAllWithDetailsAsync(ct));

    public async Task<Result<SessionViewModel>> GetDetailsAsync(int id, CancellationToken ct = default)
    {
        var session = await unitOfWork.Sessions.GetWithDetailsAsync(id, ct);
        return session is null
            ? Result<SessionViewModel>.NotFound("Session not found.")
            : Result<SessionViewModel>.Ok(mapper.Map<SessionViewModel>(session));
    }

    public async Task<Result<UpdateSessionViewModel>> GetForUpdateAsync(int id, CancellationToken ct = default)
    {
        var session = await unitOfWork.Sessions.GetWithDetailsAsync(id, ct);
        if (session is null) return Result<UpdateSessionViewModel>.NotFound("Session not found.");
        if (session.StartDate <= DateTime.Now)
            return Result<UpdateSessionViewModel>.Fail("Cannot update a session that has already started.");

        var bookingCount = await unitOfWork.Sessions.CountBookingsAsync(id, ct);
        if (bookingCount > 0)
            return Result<UpdateSessionViewModel>.Fail("Cannot update a session that already has bookings.");

        return Result<UpdateSessionViewModel>.Ok(mapper.Map<UpdateSessionViewModel>(session));
    }

    public async Task<IReadOnlyList<SelectListItem>> GetCategoriesAsync(CancellationToken ct = default) =>
        (await unitOfWork.SessionCategories.GetAllAsync(ct))
            .OrderBy(category => category.Name)
            .Select(category => new SelectListItem(category.Name, category.Id.ToString()))
            .ToList();

    public async Task<IReadOnlyList<SelectListItem>> GetTrainersAsync(int categoryId, CancellationToken ct = default)
    {
        var category = (await unitOfWork.SessionCategories.GetAllAsync(ct)).FirstOrDefault(item => item.Id == categoryId);
        var trainers = await unitOfWork.Trainers.GetAllAsync(ct);

        if (category is not null)
        {
            trainers = trainers
                .Where(trainer => trainer.Specialties.Equals(category.Name, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        return trainers
            .OrderBy(trainer => trainer.Name)
            .Select(trainer => new SelectListItem(trainer.Name, trainer.Id.ToString()))
            .ToList();
    }

    public async Task<Result> CreateAsync(CreateSessionViewModel model, CancellationToken ct = default)
    {
        var validation = await ValidateAsync(
            model.CategoryId, model.TrainerId, model.StartDate, model.EndDate, model.Capacity, null, ct);
        if (validation is not null) return validation;

        var session = mapper.Map<Session>(model);
        await unitOfWork.Sessions.AddAsync(session, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Ok();
    }

    public async Task<Result> UpdateAsync(int id, UpdateSessionViewModel model, CancellationToken ct = default)
    {
        var session = await unitOfWork.Sessions.GetWithDetailsAsync(id, ct);
        if (session is null) return Result.NotFound("Session not found.");
        if (session.StartDate <= DateTime.Now)
            return Result.Validation("Cannot update a session that has already started.");

        var bookingCount = await unitOfWork.Sessions.CountBookingsAsync(id, ct);
        if (bookingCount > 0)
            return Result.Fail("Cannot update a session that already has bookings.");

        var validation = await ValidateAsync(
            session.CategoryId, model.TrainerId, model.StartDate, model.EndDate, session.Capacity, id, ct);
        if (validation is not null) return validation;

        session.Description = model.Description.Trim();
        session.StartDate = model.StartDate;
        session.EndDate = model.EndDate;
        session.TrainerId = model.TrainerId;
        session.UpdatedAt = DateTime.Now;
        unitOfWork.Sessions.Update(session);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Ok();
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken ct = default)
    {
        var session = await unitOfWork.Sessions.GetWithDetailsAsync(id, ct);
        if (session is null) return Result.NotFound("Session not found.");

        if (session.EndDate <= DateTime.Now)
            return Result.Validation("Cannot Delete Future Session");

        var bookingCount = await unitOfWork.Sessions.CountBookingsAsync(id, ct);
        if (bookingCount > 0)
            return Result.Fail("Cannot Delete Session That Has Already Bookings");

        unitOfWork.Sessions.Remove(session);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Ok();
    }

    private async Task<Result?> ValidateAsync(
        int categoryId, int trainerId, DateTime start, DateTime end, int capacity, int? excludeId, CancellationToken ct)
    {
        if (end <= start) return Result.Validation("End date must be after the start date.");
        if (start <= DateTime.Now) return Result.Validation("Start date must be in the future.");
        if (capacity is < 1 or > 25) return Result.Validation("Capacity must be between 1 and 25.");

        var category = (await unitOfWork.SessionCategories.GetAllAsync(ct)).FirstOrDefault(item => item.Id == categoryId);
        if (category is null) return Result.NotFound("Category not found.");

        var trainer = (await unitOfWork.Trainers.GetAllAsync(ct)).FirstOrDefault(item => item.Id == trainerId);
        if (trainer is null) return Result.NotFound("Trainer not found.");
        if (!trainer.Specialties.Equals(category.Name, StringComparison.OrdinalIgnoreCase))
            return Result.Validation("Trainer specialty must match the session category.");

        if (await unitOfWork.Sessions.HasTrainerConflictAsync(trainerId, start, end, excludeId, ct))
            return Result.Fail("The trainer is already scheduled during this time slot.");

        return null;
    }
}

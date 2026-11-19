# GymGym2 — Start Here

Open `GymGym2.sln` in Visual Studio. In Solution Explorer, expand **Session Code — Open These Files** and double-click a file to show its code in the editor.

## Session feature files

- `GymGym2.BLL/Results/Result.cs` — success, validation, not-found, and conflict results.
- `GymGym2.BLL/Services/ISessionService.cs` — session service contract.
- `GymGym2.BLL/Services/Classes/SessionService.cs` — create, details, update, and delete logic.
- `GymGym2.PL/Controllers/SessionsController.cs` — MVC actions for the session pages.
- `GymGym2.PL/Views/Sessions/Index.cshtml` — session list and actions.
- `GymGym2.PL/Views/Sessions/Create.cshtml` — create page.
- `GymGym2.PL/Views/Sessions/Details.cshtml` — read-only details page.
- `GymGym2.PL/Views/Sessions/Edit.cshtml` — edit page.
- `GymGym2.PL/Views/Sessions/Delete.cshtml` — delete confirmation page.

## Project structure

- `GymGym2.PL` — MVC pages, controllers, and startup.
- `GymGym2.BLL` — session services, view models, mapping, and result types.
- `GymGym2.DAL` — database entities and repositories.

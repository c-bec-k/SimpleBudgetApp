using System.Net;
using System.Text.Json;
using SimpleBudgetApp.SqlDbServices;

namespace SimpleBudgetApp.Api;

public static class CategoryController
{
  public static void Map(WebApplication app)
  {
    app.MapPost("/category", async (HttpContext ctx, SimpleBudgetDbContext Db, UserCache cache) =>
    {
      int userId = Helpers.GetUserFromCache(ctx, cache);
      if (userId == 0) ctx.Response.Cookies.Delete("Auth");
      if (userId < 1) return Results.Unauthorized();

      using JsonDocument data = await JsonDocument.ParseAsync(ctx.Request.Body);
      JsonElement payload = data.RootElement;

      DateTime rightNow = new(DateTime.Now.Year, DateTime.Now.Month, 1);
      long offsetNow = new DateTimeOffset(rightNow).ToUnixTimeSeconds();

      Category catToSave = new()
      {
        Name = payload.GetProperty("name").GetString(),
        AmountInCents = payload.GetProperty("amountInCents").GetInt32(),
        EffectiveDateUnixTimeSeconds = offsetNow,
        UserId = userId,
        IsCurrent = true
      };

      // Check to see if a category with the same name exists
      List<Category> currentExistingVersions = Db.Categories.Where(c => c.Name == catToSave.Name && c.UserId == userId).ToList();
      var existingCat = Db.Categories.FirstOrDefault(x => x.Name == catToSave.Name && x.UserId == userId);

      // if so, mark it as not current
      if (currentExistingVersions.Count > 0)
      {
        foreach (var cat in currentExistingVersions)
        {
          cat.IsCurrent = false;
        }
      }

      var SavedCategory = await Db.Categories.AddAsync(catToSave);
      Db.SaveChanges();

      return Results.Ok(new { name = SavedCategory.Entity.Name, amountInCents = SavedCategory.Entity.AmountInCents, id = SavedCategory.Entity.Id } );
    });


    app.MapGet("/category", (HttpContext ctx, UserCache cache, SimpleBudgetDbContext Db) =>
    {
      int userId = Helpers.GetUserFromCache(ctx, cache);
      if (userId == 0) ctx.Response.Cookies.Delete("Auth");
      if (userId < 1) return Results.Unauthorized();

      List<Category> categories = Db.Categories.Where(x => x.UserId == userId && x.IsCurrent).ToList();
      List<CategoryDisplayViewModel> catsToSend = categories.Select(cat => new CategoryDisplayViewModel(Id: cat.Id, Name: cat.Name, AmountInCents: cat.AmountInCents)).ToList();

      return Results.Ok(catsToSend);
    });


    app.MapDelete("/category/{id:int}", async (int id, HttpContext ctx, UserCache cache, SimpleBudgetDbContext Db) =>
    {
      int userId = Helpers.GetUserFromCache(ctx, cache);
      if (userId == 0) ctx.Response.Cookies.Delete("Auth");
      if (userId < 1) return Results.Unauthorized();

      Category toDelete = await Db.Categories.FindAsync(id);
      if (toDelete == null) return Results.BadRequest();
      if (toDelete.UserId != userId) return Results.Forbid();

      Db.Categories.Remove(toDelete);
      Db.SaveChanges();
      return Results.Ok();
    });
  }

}
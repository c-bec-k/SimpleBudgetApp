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
      if (userId == -1) return Results.();
      if (userId == 0)
      {
        ctx.Response.Cookies.Delete("Auth");
        return Results.Unauthorized();
      }

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

      return Results.Ok(SavedCategory.Entity);
    });


    app.MapGet("/category", (HttpContext ctx, UserCache cache, SimpleBudgetDbContext Db) =>
    {
      (bool verified, int userId) = Helpers.ValidateUser(ctx.Request.Cookies["Auth"], cache);
      if (!verified) return Results.Unauthorized();
      if (userId < 1)
      {
        ctx.Response.Cookies.Delete("Auth");
        return Results.Unauthorized();
      }

      List<Category> catsToSend = Db.Categories.Where(x => x.UserId == userId && x.IsCurrent).ToList();
      
      return Results.Ok(catsToSend);
    });
  }
}
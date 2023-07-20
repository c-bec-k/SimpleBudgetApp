using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using SimpleBudgetApp.SqlDbServices;

namespace SimpleBudgetApp.Api;

public static class CategoryController
{
  public static void Map(WebApplication app)
  {
    app.MapPost("/category", async (HttpContext ctx, SimpleBudgetDbContext Db, UserCache cache) =>
    {
      string userHash = ctx.Request.Cookies["Auth"];
      using JsonDocument data = await JsonDocument.ParseAsync(ctx.Request.Body);
      JsonElement payload = data.RootElement;

      if (String.IsNullOrEmpty(userHash)) return Results.Unauthorized();

      int userId = cache.GetUser(userHash);

      if (userId < 1)
      {
        ctx.Response.Cookies.Delete("Auth");
        return Results.Unauthorized();
      }

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


    app.MapGet("/category/{id}", async (HttpContext ctx, int catId, UserCache cache, SimpleBudgetDbContext db) =>
    {

      string userHash = ctx.Request.Cookies["Auth"];
      if (String.IsNullOrEmpty(userHash)) return HttpStatusCode.Unauthorized;

      int userId = cache.GetUser(userHash);
      if (userId < 1)
      {
        ctx.Response.Cookies.Delete("Auth");
        return HttpStatusCode.Unauthorized;
      }

      Category cat = await db.Categories.FindAsync(catId);
      if (cat == null) return HttpStatusCode.NotFound;

      CategoryDisplayViewModel catToSend = new(cat.Name, cat.AmountInCents);
      await ctx.Response.WriteAsJsonAsync(catToSend);
      return HttpStatusCode.OK;
    });
  }
}
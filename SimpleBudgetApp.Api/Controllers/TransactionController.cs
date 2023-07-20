using System.Net;
using System.Text.Json;
using SimpleBudgetApp.SqlDbServices;

namespace SimpleBudgetApp.Api;

public static class TransactionController
{
  public static void Map(WebApplication app)
  {
    app.MapPost("/transaction", async (HttpContext ctx, SimpleBudgetDbContext Db, UserCache cache, Transaction txn) =>
    {
      int userId = Helpers.GetUserFromCache(ctx, cache);
      if (userId == 0) ctx.Response.Cookies.Delete("Auth");
      if (userId < 1) return Results.Unauthorized();

      using JsonDocument data = await JsonDocument.ParseAsync(ctx.Request.Body);
      JsonElement payload = data.RootElement;
      long payloadTimestamp = payload.GetProperty("DateUnixTimeSeconds").GetInt64();

      Transaction transaction = new()
      {
        AmountInCents = payload.GetProperty("AmountInCents").GetInt32(),
        CategoryId = payload.GetProperty("Category").GetInt32(),
        Note = payload.GetProperty("Note").GetString() ?? "",
        DateUnixTimeSeconds = payloadTimestamp != 0 ? payloadTimestamp : DateTimeOffset.Now.ToUnixTimeSeconds(),
        UserId = userId,
        Vendor = payload.GetProperty("Vendor").GetString() ?? ""
      };
      var SavedTxn = await Db.Transactions.AddAsync(transaction);
      Db.SaveChanges();
      Transaction middleGround = SavedTxn.Entity;
      TransactionDisplayViewModel txnToSend = new(Id: middleGround.Id, Note: middleGround.Note, AmountInCents: middleGround.AmountInCents, Vendor: middleGround.Vendor, Category: middleGround.CategoryId);

      return Results.Ok(txnToSend);
    });


    app.MapGet("/transaction/{categoryId:int}", (int categoryId, HttpContext ctx, UserCache cache, SimpleBudgetDbContext Db) =>
    {
      int userId = Helpers.GetUserFromCache(ctx, cache);
      if (userId == 0) ctx.Response.Cookies.Delete("Auth");
      if (userId < 1) return Results.Unauthorized();

      List<Transaction> txns = Db.Transactions.Where(x => x.UserId == userId && x.CategoryId == categoryId).ToList();
      List<TransactionDisplayViewModel> txnsToSend = new();

      foreach (var txn in txns)
      {
        txnsToSend.Add(new TransactionDisplayViewModel(Id: txn.Id, AmountInCents: txn.AmountInCents, Category: txn.CategoryId, Vendor: txn.Vendor, Note: txn.Note));
      }

      return Results.Ok(txnsToSend);
    });


    app.MapDelete("/category/{int:id}", async (int id, HttpContext ctx, UserCache cache, SimpleBudgetDbContext Db) =>
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
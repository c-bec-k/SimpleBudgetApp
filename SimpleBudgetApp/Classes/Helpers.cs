using System;
namespace SimpleBudgetApp;

public static class Helpers
{
	// Returns -1 if there is no auth cookie
	// Returns 0 if there is no cache hit
	// Returns the User ID otherwise
	public static int GetUserFromCache(HttpContext ctx, UserCache cache)
	{
		string userHash = ctx.Request.Cookies["Auth"];
		if (String.IsNullOrEmpty(userHash)) return -1;
		int userId = cache.GetUser(userHash);
		return userId;
	}
}


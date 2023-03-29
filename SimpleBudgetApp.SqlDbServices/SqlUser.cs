using System;

namespace SimpleBudgetApp.SqlDbServices;

public class SqlUser : IUser
{
  private readonly SimpleBudgetDbContext _context;
	public SqlUser(SimpleBudgetDbContext ctx)
	{
    _context = ctx;
	}

  public void Add(User user)
  {
    _context.Users.Add(user);
  }

  public void Commit()
  {
    _context.SaveChanges();
  }

  public void Delete(int id)
  {
    User? user = _context.Users.Find(id);
    if (user == null) return;
    _context.Users.Remove(user);
  }


  public User? Get(int id)
  {
    return _context.Users.Find(id);
  }

  public void Update(User user)
  {
    _context.Users.Update(user);
  }
}


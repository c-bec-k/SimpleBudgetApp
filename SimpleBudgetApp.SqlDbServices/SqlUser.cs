using System;

namespace SimpleBudgetApp.SqlDbServices;

public class SqlUser : IUserData
{
    private readonly SimpleBudgetDbContext _context;
    public SqlUser(SimpleBudgetDbContext ctx)
    {
        _context = ctx;
    }

    public User Add(User user)
    {
        return _context.Users.Add(user).Entity;
    }

    public void Commit()
    {
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        User user = _context.Users.Find(id);
        if (user == null) return;
        _context.Users.Remove(user);
    }


    public User Get(int id)
    {
        return _context.Users.Find(id);
    }

    public void Update(User user)
    {
        _context.Users.Update(user);
    }
}


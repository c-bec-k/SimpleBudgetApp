using System;
using SimpleBudgetApp;

namespace SimpleBudgetApp.SqlDbServices;

public class SqlCategories : ICategoryData
{
    private readonly SimpleBudgetDbContext _context;
    public SqlCategories(SimpleBudgetDbContext ctx)
    {
        _context = ctx;
    }

    public void Add(Category cat)
    {
        _context.Categories.Add(cat);
    }

    public void Commit()
    {
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        Category cat = _context.Categories.Find(id);
        if (cat == null) return;
        _context.Categories.Remove(cat);
    }

    public Category Get(int id)
    {
        return _context.Categories.Find(id);
    }

    public void Update(Category cat)
    {
        _context.Categories.Update(cat);
    }
}
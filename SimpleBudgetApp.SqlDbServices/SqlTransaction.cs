using System;
namespace SimpleBudgetApp.SqlDbServices;

public class SqlTransaction : ITransaction
{
  private readonly SimpleBudgetDbContext _context;
  public SqlTransaction(SimpleBudgetDbContext ctx)
  {
    _context = ctx;
  }

  public void Add(Transaction txn)
  {
    _context.Transactions.Add(txn);
  }

  public void Commit()
  {
    _context.SaveChanges();
  }

  public void Delete(int id)
  {
    Transaction? txn = _context.Transactions.Find(id);
    if (txn == null) return;
    _context.Transactions.Remove(txn);
  }

  public Transaction? Get(int id)
  {
    return _context.Transactions.Find(id);
  }

  public void Update(Transaction txn)
  {
    _context.Transactions.Update(txn);
  }
}
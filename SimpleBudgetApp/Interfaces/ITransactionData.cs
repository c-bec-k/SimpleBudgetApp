using System;

namespace SimpleBudgetApp;

public interface ITransactionData
{
    void Add(Transaction txn);    // Create
    Transaction Get(int id);      // Read
    void Update(Transaction txn); // Update
    void Delete(int id);          // Delete
    void Commit();                // Needed for DbContext
}


using System;
namespace SimpleBudgetApp;

public interface IUser
{
  void Add(User user);    // Create
  User? Get(int id);      // Read
  void Update(User user); // Update
  void Delete(int id);    // Delete
  void Commit();          // Needed for DbContext
}


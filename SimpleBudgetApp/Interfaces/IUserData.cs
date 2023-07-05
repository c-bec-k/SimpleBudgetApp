using System;

namespace SimpleBudgetApp;

public interface IUserData
{
    User Add(User user);    // Create
    User Get(int id);       // Read
    void Update(User user); // Update
    void Delete(int id);    // Delete
    void Commit();          // Needed for DbContext
}


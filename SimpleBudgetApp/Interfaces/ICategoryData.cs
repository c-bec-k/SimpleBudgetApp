﻿namespace SimpleBudgetApp;

public interface ICategoryData
{
    void Add(Category cat);    // Create
    Category Get(int id);      // Read
    void Update(Category cat); // Update
    void Delete(int id);       // Delete
    void Commit();             // Needed for DbContext
}
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleBudgetApp;

//public class User
//{
//  [DatabaseGenerated(DatabaseGeneratedOption.Identity), Key]
//  public int Id { get; set; }
//  public string MSId { get; set; }
//  public string Email { get; set; }
//  public string DisplayName { get; set; }
//}

public record User(
  [property: DatabaseGenerated(DatabaseGeneratedOption.Identity), Key]
  int Id,
  string MSId,
  string Email,
  string DisplayName
);

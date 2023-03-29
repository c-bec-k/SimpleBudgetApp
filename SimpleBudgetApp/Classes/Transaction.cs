using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleBudgetApp;

public record Transaction(
  [property: DatabaseGenerated(DatabaseGeneratedOption.Identity), Key]
  int Id,
  int AmountInCents,
  int Category,
  int User,
  int DateInUnix,
  string Vendor,
  string Note
);

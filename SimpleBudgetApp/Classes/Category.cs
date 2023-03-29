using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleBudgetApp;

public record Category(
  [property: DatabaseGenerated(DatabaseGeneratedOption.Identity)]
  int Id,
  string Name,
  int AmountInCents,
  int EffectiveDateInUnix,
  int User,
  bool IsCurrent
);

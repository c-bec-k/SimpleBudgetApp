using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleBudgetApp;

[Table(name: "transactions")]
public class Transaction
{
  [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column(name: "id")]
  public int Id { get; set; }

  [Column(name: "amount_in_cents")]
  public int AmountInCents { get; set; }

  [Column(name: "category_id")]
  public int CategoryId { get; set; }

  [Column(name: "user_id")]
  public int UserId { get; set; }

  [Column(name: "date_unix_time_seconds")]
  public long DateUnixTimeSeconds { get; set; }

  [Column(name: "vendor")]
  public string Vendor { get; set; }

  [Column(name: "notes")]
  public string Note { get; set; }
}

public record TransactionDisplayViewModel(
  int Id,
  int AmountInCents,
  int Category,
  string Vendor,
  string Note
);
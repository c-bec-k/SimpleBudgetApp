using System;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleBudgetApp;

[Table(name: "categories")]
public class Category
{

    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column(name: "id")]
    public int Id { get; set; }

    [Column(name: "name")]
    public string Name { get; set; }

    [Column(name: "amount_in_cents")]
    public long AmountInCents { get; set; }

    [Column(name: "effective_date_unix_time_seconds")]
    public long EffectiveDateUnixTimeSeconds { get; set; }

    [Column(name: "user_id")]
    public int UserId { get; set; }

    [Column(name: "is_current")]
    public bool IsCurrent { get; set; }
}

public record CategoryDisplayViewModel(
string Name,
long AmountInCents,
int Id
);

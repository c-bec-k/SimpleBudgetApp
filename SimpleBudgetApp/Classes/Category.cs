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
    public int AmountInCents { get; set; }

    [Column(name: "effective_date_in_unix")]
    public int EffectiveDateInUnix { get; set; }

    [Column(name: "user_id")]
    public int UserId { get; set; }

    [Column(name: "is_current")]
    public bool IsCurrent { get; set; }
}

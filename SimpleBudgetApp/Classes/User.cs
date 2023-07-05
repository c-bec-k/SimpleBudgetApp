using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Security.Cryptography;
using System.ComponentModel;

namespace SimpleBudgetApp;

[Table(name: "users")]

public class User
{
    [Column(name: "id"), Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column(name: "display_name")]
    public string DisplayName { get; set; }

    [Column(name: "email_address")]
    public string EmailAddress { get; set; }

    // OAuth login Id string
    [Column(name: "oauth_id")]
    public string OAuth_Id { get; set; }

    public static string GetCode(User user)
    {
        UTF8Encoding encoding = new();
        long now = new DateTimeOffset(new DateTime()).ToUnixTimeSeconds();
        string toEncode = $"{now}{user.EmailAddress}{user.DisplayName}";
        byte[] bytesToEncode = encoding.GetBytes(toEncode);
        byte[] hashed = SHA256.HashData(bytesToEncode);
        return Convert.ToHexString(hashed);
    }
}
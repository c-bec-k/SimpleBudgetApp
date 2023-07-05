using System;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using SimpleBudgetApp;
using SimpleBudgetApp.SqlDbServices;

var builder = WebApplication.CreateBuilder(args);
string connectionString = builder.Configuration.GetConnectionString("SimpleBudgetAppConnection");

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSqlite<SimpleBudgetDbContext>(connectionString ?? "");
builder.Services.AddSingleton<UserCache>();
var app = builder.Build();

app.UseFileServer();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.MapGet("/signin-microsoft", async (HttpRequest req, IConfiguration config, SimpleBudgetDbContext Db, UserCache cache) =>
{
    string token = req.Query["code"];
    string host = "http://localhost:5288";

    using HttpClient client = new();
    Dictionary<string, string> tokenParams = new(){
        { "client_id", config.GetValue<string>("MicrosoftLogin:ClientId") },
        { "code", token },
        { "redirect_uri", host + "/signin-microsoft" },
        { "grant_type", "authorization_code" },
        { "scope" , "email openid profile User.Read" },
        { "client_secret", config.GetValue<string>("MicrosoftLogin:ClientSecret")},
    };

    FormUrlEncodedContent reqContent = new(tokenParams);

    using HttpRequestMessage requestMessage = new(HttpMethod.Post, "https://login.microsoftonline.com/common/oauth2/v2.0/token");
    requestMessage.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
    requestMessage.Content = reqContent;

    HttpResponseMessage response = await client.SendAsync(requestMessage);
    TokenReply payload = await response.Content.ReadFromJsonAsync<TokenReply>();

    using HttpRequestMessage graphReq = new(HttpMethod.Get, "https://graph.microsoft.com/v1.0/me");
    graphReq.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
    graphReq.Headers.Add("Authorization", $"Bearer {payload.AccessToken}");
    using HttpResponseMessage graphRes = await client.SendAsync(graphReq);

    GraphReply graphReply = await graphRes.Content.ReadFromJsonAsync<GraphReply>();
    
    User user;
    User checkUser = Db.Users.First(x =>  x.OAuth_Id == graphReply.MsId);
    if (checkUser != null)
    {
        user = checkUser;
    } else {
        User _user = new()
        {
            DisplayName = graphReply.DisplayName,
            EmailAddress = graphReply.Email ?? graphReply.UserPrincipalName,
            OAuth_Id = graphReply.MsId
        };
        var newUser = Db.Users.Add(_user);
        Db.SaveChanges();
        user = Db.Users.First(x => x.OAuth_Id == graphReply.MsId);
    }

    
    string userCode = User.GetCode(user);
    // cache.Add(userCode, user);
    // foreach (KeyValuePair<string, User> pair in cache) 
    // {
    //     Console.Writeline($"Key {pair.Key} and value {pair.value} added!");
    // }
    return userCode;

});

app.Run();

record TokenReply(
    [property:JsonPropertyName("token_type")]
    string TokenType,
    [property:JsonPropertyName("scope")]
    string Scope,
    [property:JsonPropertyName("expires_in")]
    int ExpiresIn,
    [property:JsonPropertyName("ext_expires_in")]
    int ExtExipresIn,
    [property:JsonPropertyName("access_token")]
    string AccessToken,
    [property:JsonPropertyName("id_token")]
    string IdToken
);

record GraphReply(
    [property:JsonPropertyName("@odata.context")]
    string DataContext,
    [property:JsonPropertyName("userPrincipalName")]
    string UserPrincipalName,
    [property:JsonPropertyName("id")]
    string MsId,
    [property:JsonPropertyName("displayName")]
    string DisplayName,
    [property:JsonPropertyName("surname")]
    string Surname,
    [property:JsonPropertyName("givenName")]
    string GivenName,
    [property:JsonPropertyName("mail")]
    string Email
);
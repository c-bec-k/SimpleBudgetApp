using System;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using SimpleBudgetApp;
using SimpleBudgetApp.Api;
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

MicrosoftOAuth.Map(app);
CategoryController.Map(app);

app.Run();


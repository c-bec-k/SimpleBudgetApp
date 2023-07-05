using System;
using Microsoft.AspNetCore.Mvc;
using SimpleBudgetApp;

namespace SimpleBudgetApp.Api.Controllers;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly IUserData _user;
    public UserController(IUserData user)
    {
        _user = user;
    }

    [HttpPost("register")]
    public User Register([FromBody] User user)
    {
        User newUser = _user.Add(user);
        _user.Commit();
        return newUser;
    }
}


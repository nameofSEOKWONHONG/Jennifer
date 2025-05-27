using System.Security.Claims;
using eXtensionSharp;
using Jennifer.Account.Models;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Hybrid;

namespace Jennifer.Account.Application.Auth.Commands.SignOut;

public sealed record SignOutCommand(bool dummy):ICommand<Result>;


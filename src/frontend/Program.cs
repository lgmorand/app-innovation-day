using System.Configuration;
using System.Security.Claims;
using CafeReadConf;
using CafeReadConf.Frontend.Models;
using CafeReadConf.Frontend.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web.UI;
using CafeReadConf.Configure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Validate configuration contains the mandatory settings
var config = builder.Configuration;

//Make sure all the critical configs are set before proceeding
config.ValidateCriticalConfig();

// Add services to the container based on the Service configuration custom class 
builder.Services.ConfigureServices(config);

//Build the app with the expected Use directives
var app = builder.Build()
    .ConfigureAppPipeline(builder.Environment);

app.Run();

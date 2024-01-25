using CafeReadConf;
using CafeReadConf.Frontend.Models;
using CafeReadConf.Frontend.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Validate configuration contains the mandatory settings
var config = builder.Configuration;

builder.Services.AddSingleton<UserEntityFactory>();

// UserService Conditional Dependency Injection based on Backend API URL configuration 
if (string.IsNullOrEmpty(config["BACKEND_API_URL"]))
{
    // If no backend API URL is provided, we assume we are connecting to TableStorage directly from the frontend
    builder.Services.AddSingleton<IUserService, UserServiceTableStorage>();
}
else
{
    builder.Services.AddHttpClient("ApiBaseAddress", client =>
    {

        client.BaseAddress = new Uri(config["BACKEND_API_URL"]);
    });
    // If backend API URL is provided, we assume we are connecting to the Azure Function backend API
    builder.Services.AddSingleton<IUserService, UserServiceAPI>();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();

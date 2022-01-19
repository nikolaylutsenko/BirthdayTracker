using BirthdayTracker.Backend.Data;
using BirthdayTracker.Shared;
using BirthdayTracker.Shared.Models.Request;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder()
    // Read from your appsettings.json.
    .AddJsonFile("appsettings.json")
    // Read from your secrets.
    .AddUserSecrets<Program>(optional: true)
    .AddEnvironmentVariables()
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("ConnectionName"));
});

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(o =>
{
    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Authentication:Issuer"],
        ValidAudience = builder.Configuration["Authentication:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Authentication:Key"]))
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var employees = new List<Employee>
{
    new Employee { Id = 1, FirstName = "Basia", LastName = "Purpur", Position = "Nasty Kitten", DateOfBirth = DateTime.Parse("12/10/2020 12:00:00 AM")},
    new Employee { Id = 2, FirstName = "Nikolay", LastName = "Lutsenko", Position = ".NET Developer", DateOfBirth = DateTime.Parse("17/11/1987 12:00:00 AM")},
    new Employee { Id = 3, FirstName = "Daria", LastName = "Lutsenko", Position = "Accountant", DateOfBirth = DateTime.Parse("15/10/1992 12:00:00 AM")}
};

app.MapGet("/api/employees", [Authorize] async () =>
 {
     var task = Task.Run(() => employees);

     return Results.Ok(await task);
 })
.WithName("GetEmployees");

app.MapGet("/api/employees/{id:int}", [Authorize] async (int id) =>
{
    var task = Task.Run(() => employees.FirstOrDefault(x => x.Id == id));

    if (task.Result == null)
        return Results.NotFound();

    return Results.Ok(task.Result);
})
.WithName("GetEmployee");

app.MapPost("/api/employees", [Authorize] async (EmployeeRequest request) =>
{
    Employee employee = new();
    employee.Id = employees.LastOrDefault().Id + 1;
    employee.FirstName = request.FirstName;
    employee.LastName = request.LastName;
    employee.Position = request.Position;
    employee.DateOfBirth = request.DateOfBirth;

    var task = Task.Run(() => employees.Add(employee));

    await task;

    return Results.Created("/api/employees", employee);
});

app.MapPut("/api/employees/{id:int}", [Authorize] async (int id, EmployeeRequest request) =>
{
    var employee = employees.FirstOrDefault(employees => employees.Id == id);

    if (employee is null)
        return Results.NotFound();

    employee.FirstName = request.FirstName;
    employee.LastName = request.LastName;
    employee.Position = request.Position;
    employee.DateOfBirth = request.DateOfBirth;

    return Results.NoContent();
});

app.MapDelete("/api/employees/{id:int}", [Authorize] async (int id) =>
{
    var employee = employees.FirstOrDefault(employee => employee.Id == id);

    employees.Remove(employee);

    return Results.NoContent();
});

app.MapPost("/minimalapi/security/createUser",
    [AllowAnonymous]async (UserManager<IdentityUser> userMgr, User user) =>
 {
     var identityUser = new IdentityUser()
     {
         UserName = user.UserName,
         Email = user.UserName + "@example.com"
     };

     var result = await userMgr.CreateAsync(identityUser, user.Password);

     if (result.Succeeded)
     {
         return Results.Ok();
     }
     else
     {
         return Results.BadRequest();
     }
 });

app.MapPost("/minimalapi/security/getToken",
    [AllowAnonymous]async (UserManager<IdentityUser> userMgr, User user) =>
{
    var identityUsr = await userMgr.FindByNameAsync(user.UserName);

    if (await userMgr.CheckPasswordAsync(identityUsr, user.Password))
    {
        var issuer = builder.Configuration["Authentication:Issuer"];
        var audience = builder.Configuration["Authentication:Audience"];
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Authentication:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(issuer: issuer, audience: audience, signingCredentials: credentials);
        var tokenHandler = new JwtSecurityTokenHandler();
        var stringToken = tokenHandler.WriteToken(token);
        return Results.Ok(stringToken);
    }
    else
    {
        return Results.Unauthorized();
    }
});

app.Run();
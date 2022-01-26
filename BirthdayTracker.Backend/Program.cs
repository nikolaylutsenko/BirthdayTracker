using AutoMapper;
using BirthdayTracker.Backend.Data;
using BirthdayTracker.Backend.Services;
using BirthdayTracker.Shared.Constants;
using BirthdayTracker.Shared.Entities;
using BirthdayTracker.Shared.Models.Request;
using BirthdayTracker.Shared.Models.Response;
using BirthdayTracker.Shared.Requests;
using BirthdayTracker.Shared.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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

//builder.Services.AddDatabaseDeveloperPageExceptionFilter(); // wtf is this for?

builder.Services.AddIdentity<AppUser, AppRole>()
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

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddScoped<ICompanyService, CompanyService>();

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

// this endpoint is for register company owner with role CompanyOwner
app.MapPost("/api/register",
    [AllowAnonymous]async (IMapper mapper, ICompanyService companyService, UserManager<AppUser> userMgr, CompanyOwnerRequest companyOwnerRequest) =>
 {
     if(companyService.GetAllAsync().Result.FirstOrDefault(x => x.Name == companyOwnerRequest.CompanyName) != null)
     {
         return Results.BadRequest($"Company with name {companyOwnerRequest.CompanyName} is already exists");
     }

     var company = new Company { Id = Guid.NewGuid().ToString(), Name = companyOwnerRequest.CompanyName};

     var companyOwner = mapper.Map<AppUser>(companyOwnerRequest);
     companyOwner.CompanyId = company.Id;

     var addUserResult = await userMgr.CreateAsync(companyOwner, companyOwnerRequest.Password);

     if (addUserResult.Succeeded)
     {
         var setRoleResult = await userMgr.AddToRoleAsync(companyOwner, "Owner");

         if (setRoleResult.Succeeded)
         {
             company.CompanyOwnerId = companyOwner.Id;
             await companyService.AddAsync(company);

             return Results.Ok();
         }

         return Results.BadRequest();
     }
     else
     {
         // todo: rewrite this return, because it is not correct
         return Results.BadRequest();
     }
 });

app.MapGet("/api/employees", [Authorize]async (UserManager<AppUser> userManager, AppDbContext context, HttpContext httpContext, IMapper mapper) =>
{
    var ownerId = httpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Contains("name"))?.Value;

    if (ownerId is null)
        return Results.StatusCode(500);

    var owner = await userManager.FindByIdAsync(ownerId);

    var users = await context.Users.Where(x => x.CompanyId == owner.CompanyId).ToListAsync();

    return Results.Ok(mapper.Map<IEnumerable<EmployeeResponse>>(users));
})
.WithName("GetEmployees");

app.MapGet("/api/employees/{id:int}", [Authorize] async (UserManager < AppUser > userManager, AppDbContext context, HttpContext httpContext, string id) =>
{
    var ownerId = httpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Contains("name"))?.Value;

    if (ownerId is null)
        return Results.StatusCode(500);

    var owner = await userManager.FindByIdAsync(ownerId);

    var user = await context.Users.Where(x => x.CompanyId == owner.CompanyId).FirstOrDefaultAsync(x => x.Id == id);

    if (user == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(user);
})
.WithName("GetEmployee");

// this endpoint is for add employee for company with role User
app.MapPost("/api/employee",
    [Authorize] async (UserManager<AppUser> userMgr, HttpContext httpContext, ICompanyService companyService, IMapper mapper, AddEmployeeRequest addEmployeeRequest) =>
    {
        var ownerId = httpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Contains("name"))?.Value;

        if (ownerId is null)
            return Results.StatusCode(500);

        var company = await companyService.GetByOwnerIdAsync(ownerId);

        var employee = mapper.Map<AppUser>(addEmployeeRequest);
        employee.CompanyId = company.Id;

        var addEmployeeResult = await userMgr.CreateAsync(employee, addEmployeeRequest.Password);

        if (addEmployeeResult.Succeeded)
        {
            var setUserRole = await userMgr.AddToRoleAsync(employee, AppConstants.UserRoleName);

            if (setUserRole.Succeeded)
            {
                return Results.Created("/api/employees", mapper.Map<EmployeeResponse>(employee));
            }

            return Results.StatusCode(500);
        }
        else
        {
            // todo: add single error message and set it up at AutomapperProfile
            return Results.BadRequest(addEmployeeResult.Errors.Select(x => x.Description));
        }
    }).WithName("AddEmployee");

app.MapPut("/api/employees/{id:int}", [Authorize] async (IMapper mapper, UserManager<AppUser> userManager, string id, UpdateEmployeeRequest updateEmployeeRequest) =>
{
    var oldEmployee = await userManager.FindByIdAsync(id);

    if (oldEmployee is null)
        return Results.NotFound($"Employee with privided id {id} not found");

    var updatedEmployee = mapper.Map(updateEmployeeRequest, oldEmployee);

    var updateResult = await userManager.UpdateAsync(updatedEmployee);

    if (!updateResult.Succeeded)
    {
        return Results.StatusCode(500);
    }

    return Results.NoContent();
}).WithName("UpdateEmployee");

app.MapDelete("/api/employees/{id:int}", [Authorize] async (UserManager<AppUser> userManager, HttpContext httpContext, string id) =>
{
    // only Admin and Owner may delete all users
    // but user may delete himself
    if (httpContext.User.IsInRole("Admin") || httpContext.User.IsInRole("Owner") || httpContext.User.Claims.First(x => x.Type == "Name").Value == id.ToString())
    {
        var employee = await userManager.FindByIdAsync(id);

        if (employee is null)
        {
            return Results.NotFound($"User with provided id {id} not found");
        }

        var deleteResult = await userManager.DeleteAsync(employee);

        if (!deleteResult.Succeeded)
        {
            return Results.StatusCode(500);
        }

        return Results.NoContent();
    }

    return Results.Unauthorized();
}).WithName("DeleteEmployee");

app.MapPost("/api/security/getToken",
    [AllowAnonymous]async (UserManager<AppUser> userMgr, LoginRequest request) =>
{
    var identityUsr = await userMgr.FindByNameAsync(request.UserName);
    var userRole = await userMgr.GetRolesAsync(identityUsr);

    if (await userMgr.CheckPasswordAsync(identityUsr, request.Password))
    {
        var issuer = builder.Configuration["Authentication:Issuer"];
        var audience = builder.Configuration["Authentication:Audience"];
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Authentication:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(issuer: issuer, audience: audience, new List<Claim>{new Claim("Name", identityUsr.Id.ToString()), new Claim(ClaimTypes.Role, userRole.First().ToString())}, signingCredentials: credentials);
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
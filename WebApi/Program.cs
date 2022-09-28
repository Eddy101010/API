using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Interfaces;
using WebApi.Helpers;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using WebApi.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();
builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("shhh.. this is my top secret"));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) .AddJwtBearer(opt =>
{
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = false,
        ValidateAudience = false,
        IssuerSigningKey = key
    };
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("corsapp");

}
//else
//{
//    app.UseExceptionHandler(
//            options => {
//                options.Run(
//                      async context =>
//                       {
//                          context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
//                          var ex = context.Features.Get<IExceptionHandlerFeature>();
//                          if (ex != null)
//                          {
//                              await context.Response.WriteAsync(ex.Error.Message);
//                          }
//                       }
//                    );     
//            }
//        );
//}

app.UseMiddleware<ExceptionMiddleware>();

app.UseCors(m => m.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

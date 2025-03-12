using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UKParliament.CodeTest.Data;
using UKParliament.CodeTest.Services;
using UKParliament.CodeTest.Services.Mappers;
using UKParliament.CodeTest.Services.Models;
using UKParliament.CodeTest.Services.Validators;

namespace UKParliament.CodeTest.Web;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllersWithViews();

        builder.Services.AddDbContext<PersonManagerContext>(op => op.UseInMemoryDatabase("PersonManager"));

        builder.Services.AddScoped<IPersonService, PersonService>();
        builder.Services.AddScoped<IDepartmentService, DepartmentService>();
        builder.Services.AddScoped<IPersonRepository, PersonRepository>();
        builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();

        //Validators:
        builder.Services.AddScoped<PersonValidator>(); 

        builder.Services.AddScoped<IValidator<PersonDto>, PersonValidator>();

        builder.Services.AddScoped<IValidator<PersonDetailsDto>, PersonDetailsValidator>();

        //Mappers:
        var mapperConfig = new MapperConfiguration(conf =>
        {
            conf.AddProfile<PersonMappingProfile>(); 
        });
        var mapper = mapperConfig.CreateMapper();
        builder.Services.AddSingleton(mapper);


        var app = builder.Build();

        // Create database so the data seeds
        using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            using var context = serviceScope.ServiceProvider.GetRequiredService<PersonManagerContext>();
            context.Database.EnsureCreated();
        }

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller}/{action=Index}/{id?}");

        app.MapFallbackToFile("index.html");

        app.Run();
    }
}
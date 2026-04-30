using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rent_for_students.Application.Commands;
using rent_for_students.Application.Notifications;
using rent_for_students.Application.UseCases;
using rent_for_students.Domain.Contracts;
using rent_for_students.Domain.Flyweight;
using rent_for_students.Domain.Services;
using rent_for_students.Infrastructure.Data;
using rent_for_students.Infrastructure.Notifications;
using rent_for_students.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    // PROMPT v1.6: Database migration SQLite -> SQL Server
    var cs = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(cs);
});

// PROMPT v1.6: SP-based repositories (integrated stored procedures)
builder.Services.AddScoped<IHousingRepository, SpHousingRepository>();
builder.Services.AddScoped<IRentalApplicationRepository, SpRentalApplicationRepository>();
builder.Services.AddScoped<IRentalApplicationProfileRepository, SpRentalApplicationProfileRepository>();
// PROMPT v1.7: Report repository — View + cursor SP
builder.Services.AddScoped<IListingReportRepository, SpListingReportRepository>();
builder.Services.AddScoped<HousingService>();

builder.Services.AddScoped<INotificationService, LogNotificationService>();
builder.Services.AddScoped<IListingUseCaseMediator, ListingUseCaseMediator>();
builder.Services.AddScoped<IApplicationUseCaseMediator, ApplicationUseCaseMediator>();

builder.Services.AddScoped<CommandDispatcher>();

// PROMPT v1.5: Flyweight Factory registered as Singleton (shared cache)
builder.Services.AddSingleton<RoomTypeFlyweightFactory>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

app.Run();

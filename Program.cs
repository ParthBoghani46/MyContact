using FluentValidation;
using MyContact.Repositories;
using MyContact.Repositories.Implementations;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddViewOptions(options =>
    {
        options.HtmlHelperOptions.ClientValidationEnabled = true;
    });

builder.Services.AddScoped<IContactInterface, ContactRepository>();
builder.Services.AddScoped<IUserInterface, UserRepository>();
builder.Services.AddScoped<NpgsqlConnection>((UserRepository) =>
{
    var connectionString = UserRepository.GetRequiredService<IConfiguration>().GetConnectionString("pgconn");
    return new NpgsqlConnection(connectionString);
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=User}/{action=Login}/{id?}");

app.Run();

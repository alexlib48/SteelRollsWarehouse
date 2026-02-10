using Microsoft.EntityFrameworkCore;
using SteelRollsWarehouse.Application.Services;
using SteelRollsWarehouse.Infrastructure.Data;
using SteelRollsWarehouse.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite(connectionString));
}
else
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(connectionString));
}

builder.Services.AddScoped<IRollRepository, RollRepository>();
builder.Services.AddScoped<IRollService, RollService>();

builder.Services.AddLogging();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.UseExceptionHandler("/error");

app.MapGet("/error", () => 
    Results.Problem("An unexpected error occurred.", statusCode: 500));

app.Run();
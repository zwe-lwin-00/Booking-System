using BookingSystem.API.Extensions;
using BookingSystem.API.Filters;
using BookingSystem.API.Middleware;
using BookingSystem.Application;
using BookingSystem.Infrastructure;
using BookingSystem.Infrastructure.Jobs;
using Hangfire;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerDocumentation();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add Application and Infrastructure layers
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    // Note: Database setup should be done using SQL scripts in database/ folder
    // Run: 01_CreateDatabase.sql, 02_CreateTables.sql, 03_SeedReferenceData.sql
}

// Configure Hangfire
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireAuthorizationFilter() }
});

// Schedule waitlist refund job to run every hour
RecurringJob.AddOrUpdate<WaitlistRefundJob>(
    "waitlist-refund-job",
    job => job.ProcessWaitlistRefunds(),
    Cron.Hourly);

// Add exception handling middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Enable CORS
app.UseCors("AllowAll");

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

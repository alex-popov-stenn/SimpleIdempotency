using Microsoft.EntityFrameworkCore;
using SimpleIdempotency;
using SimpleIdempotency.Persistence;
using SimpleIdempotency.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.OperationFilter<AddIdempotencyKeyHeaderParameter>();
});

builder.Services.AddInvoices();
builder.Services.AddPersistence(builder.Configuration["DbConnectionString"] ?? throw new ArgumentException("The database configuration string is missing"));
builder.Services.AddIdempotency();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
await dbContext.Database.MigrateAsync(CancellationToken.None);

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
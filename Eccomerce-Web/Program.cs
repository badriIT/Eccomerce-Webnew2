using Eccomerce_Web.Data;
using Eccomerce_Web.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddAppControllers();

builder.Services.AddCommonServices(builder.Configuration);

builder.Services.AddAppCors();

builder.Services.AddAppAuthentication(builder.Configuration);

builder.Services.AddAppSwagger();
builder.Services.AddModules();
builder.Services.AddDbContext<DataContext>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
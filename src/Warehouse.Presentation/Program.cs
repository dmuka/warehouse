using Warehouse.Application;
using Warehouse.Infrastructure;
using Warehouse.Presentation;
using Warehouse.Presentation.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddPresentation()
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage()
        .UseMigrationsEndPoint();
    
    app.UseSwaggerWithUi();

    app.ApplyMigrations();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseCors("AllowWarehouseClientApp");

app.UseHttpsRedirection();

app.UseRouting();

app.MapControllers();

app.Run();
using AgendaManager.Infrastructure;
using AgendaManager.Models.Indexes;
using Raven.Client.Documents;
using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDataProtection();

builder.Services.Configure<RouteOptions>(opt =>
{
    opt.ConstraintMap.Add("encrypt", typeof(EncryptedParameter));
});

var databaseName = "Productivity";

builder.Services.AddSingleton<IDocumentStore>(_ =>
    {
        var store = new DocumentStore
        {
            Database = databaseName,
            Urls = [config.GetConnectionString("server")],
        };

        store.Initialize();
        store.ExecuteIndex(new Agendas());
        return store;
    });

var app = builder.Build();

using var store = app.Services.GetRequiredService<IDocumentStore>();
if (!store.DatabaseExists(databaseName)) {
    await store.Maintenance.Server.SendAsync(new CreateDatabaseOperation(new DatabaseRecord(databaseName)));
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
using AgendaManager.Infrastructure;
using Marten;
using Weasel.Core;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDataProtection();

builder.Services.Configure<RouteOptions>(opt =>  {
    opt.ConstraintMap.Add("encrypt", typeof(EncryptedParameter));
});

builder.Services.AddMarten(options => {
        options.Connection(config.GetConnectionString("server")!);
        options.UseSystemTextJsonForSerialization();
        options.AutoCreateSchemaObjects = AutoCreate.All;
    }
);

var app = builder.Build();

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
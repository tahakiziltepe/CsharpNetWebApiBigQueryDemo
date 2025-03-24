using CsharpNetWebApiBigQueryDemo.BlazorApp.Components;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddMemoryCache();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register HttpClient
builder.Services.AddScoped<HttpClient>(sp =>
{
    var baseAddress = "https://localhost:7171";
    return new HttpClient { BaseAddress = new Uri(baseAddress) };
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

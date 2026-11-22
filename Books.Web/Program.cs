var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseHttpsRedirection();

// Serves wwwroot/index.html for the root URL and the static files under wwwroot.
app.UseDefaultFiles();
app.UseStaticFiles();

app.Run();

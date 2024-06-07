using Maple.API;
using Maple.Application;
using Maple.Infrastructure;
using Maple.Infrastructure.DbContext;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();

builder.Services.AddApplication().AddInfrastructure().AddPresentation();

var app = builder.Build();

// Initialize the Supabase client
var supabaseClient = app.Services.GetRequiredService<SupabaseClient>();
await supabaseClient.InitializeAsync();

app.UseExceptionHandler("/error");


app.UseAuthentication();
app.UseAuthorization();

app.UseCors(builder => builder
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowAnyOrigin()
);

app.UseHttpsRedirection();

app.MapControllers();

app.MapGet("/", () => "Hello World!");

app.Run();
using Maple.API;
using Maple.Application;
using Maple.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();

builder.Services.AddApplication().AddInfrastructure().AddPresentation();

const string supabaseUrl = "https://timbkbpfurjfwtqnfrov.supabase.co";
const string supabaseKey =
    "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InRpbWJrYnBmdXJqZnd0cW5mcm92Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3MTU1MzQ5MzEsImV4cCI6MjAzMTExMDkzMX0.P_Nnd3zfLhwCdkYxNkMpMgdkRpAtjeTHp_berx1oBLI";

// Register the SupabaseClient as a singleton
builder.Services.AddSingleton(new SupabaseClient(supabaseUrl, supabaseKey));


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
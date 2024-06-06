using System.Text;
using Maple.API;
using Maple.Application;
using Maple.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();

const string supabaseJwtSecret =
    "dUk0UwQ+xdCeUBHkI9qP2sjsltqIJ+W8MVMJ/Vn7QC1THC/HX/p+9tOqaxMmSbBNgidGfPQ1H8JdzuhppIQ8zg==";
// Configure JWT authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(supabaseJwtSecret))
        };
    });


builder.Services.AddApplication().AddInfrastructure().AddPresentation();

const string supabaseUrl = "https://timbkbpfurjfwtqnfrov.supabase.co";
const string supabaseKey =
    "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InRpbWJrYnBmdXJqZnd0cW5mcm92Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3MTU1MzQ5MzEsImV4cCI6MjAzMTExMDkzMX0.P_Nnd3zfLhwCdkYxNkMpMgdkRpAtjeTHp_berx1oBLI";

// Register the SupabaseClient as a singleton
builder.Services.AddSingleton(new SupabaseClient(supabaseUrl, supabaseKey));

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling =
        ReferenceLoopHandling.Ignore; // This may be needed for your relationships.
});

builder.Logging.ClearProviders().AddConsole();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
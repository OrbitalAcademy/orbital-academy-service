using Microsoft.OpenApi;
using OrbitalAcademy.Application;
using OrbitalAcademy.Application.Security;
using OrbitalAcademy.Api.Security;
using OrbitalAcademy.Infrastructure;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Orbital Academy Service",
        Version = "v1",
        Description = "Base dos endpoints minimos do MVP do Orbital Academy."
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Cole somente o token JWT. O Swagger envia o prefixo Bearer automaticamente."
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document, null)] = []
    });
});
builder.Services.AddProblemDetails();
builder.Services.AddHealthChecks();
builder.Services.AddDataProtection();
builder.Services.AddApplication();
builder.Services.AddConfiguredSecurity(builder.Configuration);
builder.Services.AddJwtTokenGeneration();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<ISensitiveDataEncryptionService, DataProtectionSensitiveDataEncryptionService>();

WebApplication app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Orbital Academy Service v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseCors(CorsPolicyNames.ConfiguredOrigins);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health").AllowAnonymous();

app.Run();

public partial class Program;

using Microsoft.EntityFrameworkCore;
using SocialCare.Extensions;
using SocialCare.Infrastructure.Bootstrap;
using SocialCare.Infrastructure.Data;
using SocialCare.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMemoryCache();
builder.Services.AddSocialCareAuthentication(builder.Configuration);
builder.Services.AddSocialCareExternalApis(builder.Configuration);
builder.Services.AddSocialCarePersistence();
builder.Services.AddSocialCareApplication();
builder.Services.AddSocialCareSwagger();
builder.Services.AddControllers();

// Validação fica a cargo da FluentValidation (mensagens padronizadas em PT-BR via middleware).
builder.Services.Configure<Microsoft.AspNetCore.Mvc.ApiBehaviorOptions>(options =>
    options.SuppressModelStateInvalidFilter = true);

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

// Trilha de auditoria das requisições mutantes (depende do usuário autenticado).
app.UseMiddleware<AuditLogMiddleware>();

app.MapGet("/", () => Results.Redirect("/swagger"));
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    await AdminBootstrap.GarantirAdminAsync(app.Services, app.Configuration, logger);

    if (app.Configuration.GetValue("Ibge:BootstrapNaInicializacao", false))
    {
        await IbgeBootstrap.PopularEstadosEMunicipiosAsync(app.Services, logger);
    }
}

app.Run();

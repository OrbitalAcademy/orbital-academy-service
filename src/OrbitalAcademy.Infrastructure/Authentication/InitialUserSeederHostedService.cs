using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OrbitalAcademy.Domain.Usuarios;
using OrbitalAcademy.Infrastructure.Persistence;

namespace OrbitalAcademy.Infrastructure.Authentication;

public sealed class InitialUserSeederHostedService : IHostedService
{
    private const string PendingHashPlaceholder = "pending-password-hash";

    private readonly IServiceProvider serviceProvider;
    private readonly InitialUserOptions options;

    public InitialUserSeederHostedService(
        IServiceProvider serviceProvider,
        IOptions<InitialUserOptions> options)
    {
        this.serviceProvider = serviceProvider;
        this.options = options.Value;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!options.Enabled)
        {
            return;
        }

        await using AsyncServiceScope scope = serviceProvider.CreateAsyncScope();

        OrbitalAcademyDbContext dbContext = scope.ServiceProvider.GetRequiredService<OrbitalAcademyDbContext>();
        IPasswordHasher<Usuario> passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<Usuario>>();

        await dbContext.Database.MigrateAsync(cancellationToken);

        string emailNormalizado = Usuario.NormalizarEmail(options.Email!);
        bool usuarioJaExiste = await dbContext.Usuarios
            .AnyAsync(usuario => usuario.EmailNormalizado == emailNormalizado, cancellationToken);

        if (usuarioJaExiste)
        {
            return;
        }

        Usuario usuario = new(
            Guid.NewGuid(),
            options.Nome!,
            options.Email!,
            PendingHashPlaceholder,
            options.Papel!,
            options.Unidade!);

        string senhaHash = passwordHasher.HashPassword(usuario, options.Password!);
        usuario.DefinirSenhaHash(senhaHash);

        dbContext.Usuarios.Add(usuario);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

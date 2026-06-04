using Microsoft.EntityFrameworkCore;
using OrbitalAcademy.Application.Usuarios;
using OrbitalAcademy.Domain.Usuarios;
using OrbitalAcademy.Infrastructure.Persistence;

namespace OrbitalAcademy.Infrastructure.Usuarios;

public sealed class EfUsuarioRepository : IUsuarioRepository
{
    private readonly OrbitalAcademyDbContext dbContext;

    public EfUsuarioRepository(OrbitalAcademyDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public Task<Usuario?> BuscarPorEmailNormalizadoAsync(
        string emailNormalizado,
        CancellationToken cancellationToken)
    {
        return dbContext.Usuarios
            .AsNoTracking()
            .SingleOrDefaultAsync(
                usuario => usuario.EmailNormalizado == emailNormalizado,
                cancellationToken);
    }
}

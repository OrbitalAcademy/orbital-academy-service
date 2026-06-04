namespace OrbitalAcademy.Api.Security;

public sealed record JwtTokenResult(
    string Token,
    DateTimeOffset ExpiresAt);

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrbitalAcademy.Api.Contracts.Security;
using OrbitalAcademy.Application.Security;

namespace OrbitalAcademy.Api.Controllers;

[ApiController]
[Authorize(Roles = "admin")]
[Route("api/security")]
public sealed class SecurityController : ControllerBase
{
    private readonly IDatabaseBackupService databaseBackupService;
    private readonly ISensitiveDataEncryptionService sensitiveDataEncryptionService;

    public SecurityController(
        IDatabaseBackupService databaseBackupService,
        ISensitiveDataEncryptionService sensitiveDataEncryptionService)
    {
        this.databaseBackupService = databaseBackupService;
        this.sensitiveDataEncryptionService = sensitiveDataEncryptionService;
    }

    [HttpPost("backup/run")]
    [ProducesResponseType(typeof(BackupRunResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BackupRunResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(BackupRunResponse), StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<BackupRunResponse>> RunBackup(CancellationToken cancellationToken)
    {
        BackupExecutionResult result = await databaseBackupService.RunAsync(cancellationToken);
        BackupRunResponse response = MapBackupRun(result);

        if (result.AlreadyRunning)
        {
            return Conflict(response);
        }

        if (!result.Succeeded)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, response);
        }

        return Ok(response);
    }

    [HttpGet("backup/status")]
    [ProducesResponseType(typeof(BackupStatusResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<BackupStatusResponse>> GetBackupStatus(CancellationToken cancellationToken)
    {
        BackupStatusResult status = await databaseBackupService.GetStatusAsync(cancellationToken);

        return Ok(new BackupStatusResponse(
            status.ExecutedAt,
            status.Status,
            status.FileName,
            status.ExitCode,
            status.LogFilePath,
            status.Message));
    }

    [HttpGet("logs")]
    [ProducesResponseType(typeof(BackupLogResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<BackupLogResponse>> GetLogs(
        [FromQuery]
        [Range(1, 100)]
        int lines = 20,
        CancellationToken cancellationToken = default)
    {
        BackupLogResult log = await databaseBackupService.ReadLogAsync(lines, cancellationToken);

        return Ok(new BackupLogResponse(
            log.LogFilePath,
            log.RequestedLines,
            log.Entries.Count,
            log.Entries));
    }

    [HttpPost("encrypt-test")]
    [ProducesResponseType(typeof(EncryptTestResponse), StatusCodes.Status200OK)]
    public ActionResult<EncryptTestResponse> EncryptTest(EncryptTestRequest request)
    {
        SensitiveDataEncryptionResult result = sensitiveDataEncryptionService.Encrypt(request.Valor);

        return Ok(new EncryptTestResponse(
            result.ProtectedValue,
            result.Purpose));
    }

    private static BackupRunResponse MapBackupRun(BackupExecutionResult result)
    {
        return new BackupRunResponse(
            result.ExecutedAt,
            result.Status,
            result.FileName,
            result.ExitCode,
            result.LogFilePath,
            result.Message);
    }
}

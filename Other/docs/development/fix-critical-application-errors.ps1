#!/usr/bin/env pwsh

Write-Host "Fixing critical Application layer compilation errors..." -ForegroundColor Green

# List of files that need MediatR using statements
$mediatRFiles = @(
    "Core/Wekeza.Core.Application/Common/Behaviors/ValidationBehavior.cs",
    "Core/Wekeza.Core.Application/Common/Behaviors/PerformanceBehavior.cs",
    "Core/Wekeza.Core.Application/Common/Behaviors/AuditingBehavior.cs"
)

# Add MediatR using statements to behavior files
foreach ($file in $mediatRFiles) {
    if (Test-Path $file) {
        $content = Get-Content $file -Raw
        if ($content -notmatch "using MediatR;") {
            $newContent = "using MediatR;`n" + $content
            Set-Content -Path $file -Value $newContent -Encoding UTF8
            Write-Host "Added MediatR using to: $file" -ForegroundColor Cyan
        }
    }
}

# Create missing common interfaces
$interfaces = @{
    "Core/Wekeza.Core.Application/Common/Interfaces/ICurrentUserService.cs" = @"
namespace Wekeza.Core.Application.Common.Interfaces;

public interface ICurrentUserService
{
    string? UserId { get; }
    string? UserName { get; }
    bool IsAuthenticated { get; }
    bool IsInRole(string role);
    IEnumerable<string> GetRoles();
}
"@
    
    "Core/Wekeza.Core.Application/Common/Interfaces/IEmailService.cs" = @"
namespace Wekeza.Core.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default);
    Task SendTemplateEmailAsync(string to, string templateId, object data, CancellationToken cancellationToken = default);
}
"@

    "Core/Wekeza.Core.Application/Common/Interfaces/ISmsService.cs" = @"
namespace Wekeza.Core.Application.Common.Interfaces;

public interface ISmsService
{
    Task SendSmsAsync(string phoneNumber, string message, CancellationToken cancellationToken = default);
    Task SendOtpAsync(string phoneNumber, string otp, CancellationToken cancellationToken = default);
}
"@
}

foreach ($interface in $interfaces.GetEnumerator()) {
    if (-not (Test-Path $interface.Key)) {
        New-Item -Path $interface.Key -ItemType File -Force | Out-Null
        Set-Content -Path $interface.Key -Value $interface.Value -Encoding UTF8
        Write-Host "Created interface: $($interface.Key)" -ForegroundColor Green
    }
}

# Create missing exception classes
$exceptions = @{
    "Core/Wekeza.Core.Application/Common/Exceptions/NotFoundException.cs" = @"
namespace Wekeza.Core.Application.Common.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string name, object key)
        : base(`$"Entity '{name}' ({key}) was not found.")
    {
    }

    public NotFoundException(string message)
        : base(message)
    {
    }

    public NotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
"@

    "Core/Wekeza.Core.Application/Common/Exceptions/ValidationException.cs" = @"
using FluentValidation.Results;

namespace Wekeza.Core.Application.Common.Exceptions;

public class ValidationException : Exception
{
    public ValidationException()
        : base("One or more validation failures have occurred.")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IEnumerable<ValidationFailure> failures)
        : this()
    {
        Errors = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
    }

    public IDictionary<string, string[]> Errors { get; }
}
"@
}

foreach ($exception in $exceptions.GetEnumerator()) {
    if (-not (Test-Path $exception.Key)) {
        New-Item -Path $exception.Key -ItemType File -Force | Out-Null
        Set-Content -Path $exception.Key -Value $exception.Value -Encoding UTF8
        Write-Host "Created exception: $($exception.Key)" -ForegroundColor Green
    }
}

Write-Host ""
Write-Host "Critical interfaces and exceptions created!" -ForegroundColor Green
Write-Host "Building Application project to check progress..." -ForegroundColor Yellow

dotnet build "Core/Wekeza.Core.Application/Wekeza.Core.Application.csproj" --verbosity minimal | Select-String "error" | Measure-Object | ForEach-Object { 
    Write-Host "Remaining errors: $($_.Count)" -ForegroundColor $(if ($_.Count -lt 100) { "Green" } else { "Yellow" })
}

Write-Host ""
Write-Host "Next: Continue fixing remaining compilation errors systematically" -ForegroundColor Cyan
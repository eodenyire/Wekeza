using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.ValueObjects;

public class APIRoute : ValueObject
{
    public string Path { get; private set; }
    public string Method { get; private set; }
    public string Description { get; private set; }
    public string UpstreamUrl { get; private set; }
    public bool RequiresAuth { get; private set; }
    public SecurityLevel SecurityLevel { get; private set; }

    private APIRoute() { } // EF Core

    public APIRoute(string path, string method, string description, string upstreamUrl, bool requiresAuth = true, SecurityLevel securityLevel = SecurityLevel.Standard)
    {
        Path = path ?? throw new ArgumentNullException(nameof(path));
        Method = method ?? throw new ArgumentNullException(nameof(method));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        UpstreamUrl = upstreamUrl ?? throw new ArgumentNullException(nameof(upstreamUrl));
        RequiresAuth = requiresAuth;
        SecurityLevel = securityLevel;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Path;
        yield return Method;
        yield return UpstreamUrl;
        yield return RequiresAuth;
        yield return SecurityLevel;
    }
}

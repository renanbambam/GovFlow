namespace GovFlow.API.Authentication;

/// <summary>
/// Names of the role-based authorization policies. Endpoints are currently open; apply
/// <c>[Authorize(Policy = GovFlowPolicies.X)]</c> once the Identity flows (login/JWT
/// issuance) are implemented.
/// </summary>
public static class GovFlowPolicies
{
    public const string RequireAdmin = "RequireAdmin";
    public const string RequireManager = "RequireManager";
    public const string RequireAnalyst = "RequireAnalyst";
}

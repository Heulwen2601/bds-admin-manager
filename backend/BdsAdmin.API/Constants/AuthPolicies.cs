namespace BdsAdmin.API.Constants;

public static class AuthPolicies
{
    public const string AdminOnly = "AdminOnly";
    public const string SellerOnly = "SellerOnly";
    public const string ConsultantOnly = "ConsultantOnly";
    public const string UserOrSeller = "UserOrSeller";
    public const string AuthenticatedUser = "AuthenticatedUser";
}

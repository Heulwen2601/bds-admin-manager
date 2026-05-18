namespace BdsAdmin.API.Constants;

public static class SellerTypes
{
    public const string Broker = "Broker";
    public const string CompanyRepresentative = "CompanyRepresentative";
    public const string Owner = "Owner";

    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        Broker,
        CompanyRepresentative,
        Owner
    };

    public static bool IsValid(string? sellerType) =>
        !string.IsNullOrWhiteSpace(sellerType) && All.Contains(sellerType.Trim());

    public static string Normalize(string? sellerType)
    {
        if (string.IsNullOrWhiteSpace(sellerType)) return Broker;

        return sellerType.Trim().ToLowerInvariant() switch
        {
            "broker" => Broker,
            "companyrepresentative" or "company-representative" or "company_representative" =>
                CompanyRepresentative,
            "owner" or "individual" => Owner,
            _ => sellerType.Trim()
        };
    }

    public static bool RequiresCompany(string? sellerType) =>
        Normalize(sellerType) == CompanyRepresentative;

    public static string GetDisplayName(string? sellerType) => Normalize(sellerType) switch
    {
        Broker => "Môi giới",
        CompanyRepresentative => "Đại diện công ty",
        Owner => "Chủ nhà",
        _ => "Người bán"
    };
}

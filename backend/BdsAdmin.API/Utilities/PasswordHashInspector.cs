namespace BdsAdmin.API.Utilities;

public static class PasswordHashInspector
{
    public static bool LooksLikeIdentityV3Hash(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        try
        {
            var bytes = Convert.FromBase64String(value);
            return bytes.Length > 0 && bytes[0] == 0x01;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}

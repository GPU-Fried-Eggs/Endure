namespace Endure;

public static class Constants
{
    public static bool Desktop => DeviceInfo.Platform == DevicePlatform.WinUI || DeviceInfo.Platform == DevicePlatform.MacCatalyst;

    public static readonly string Port = "8989";

    public static readonly string Localhost = DeviceInfo.Platform == DevicePlatform.Android ? "10.0.2.2" : "localhost";

    public static readonly string LocalhostUrl = $"https://{Localhost}:{Port}";

    public static readonly string RestUrl = "https://endure-zyod2ddhw2szk-service.azurewebsites.net";

    public static readonly string ApiUrl = $"{RestUrl}/api";

    // B2C setting
    private static readonly string TenantName = "noendure";

    private static readonly string Tenant = $"{TenantName}.onmicrosoft.com";

    private static readonly string AzureAdB2CHostname = $"{TenantName}.b2clogin.com";

    private static readonly string ClientId = "73d8319f-92d8-43a1-93aa-92993d3e7298";

    private static readonly string RedirectUri = $"https://{TenantName}.b2clogin.com/oauth2/nativeclient";

    // B2C user flows
    public static string PolicySignUpSignIn = "b2c_1_susi";
    
    public static string[] ApiScopes = { $"https://{Tenant}/emotion/read" };
}
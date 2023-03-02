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
    public static readonly string TenantName = "noendure";

    public static readonly string Host = $"{TenantName}.b2clogin.com";

    public static readonly string Domain = $"{TenantName}.onmicrosoft.com";

    public static readonly string TenantId = "59a5b817-8ff1-47cb-a17e-2b29730b548a";

    public static readonly string ClientId = "73d8319f-92d8-43a1-93aa-92993d3e7298";

    // B2C user flows
    public static readonly string SignUpSignInPolicyId = "b2c_1_susi";

    // DownstreamApi
    public static string[] ApiScopes =
    {
        $"https://{Domain}/emotion/read",
        $"https://{Domain}/emotion/write"
    };
}
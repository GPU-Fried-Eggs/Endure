﻿namespace Endure;

public static class Constants
{
    public static bool Desktop =>
        DeviceInfo.Platform == DevicePlatform.WinUI || DeviceInfo.Platform == DevicePlatform.MacCatalyst;

    // URL of REST service (Android does not use localhost)
    // Use http cleartext for local deployment. Change to https for production
    public static readonly string LocalhostUrl =
        DeviceInfo.Platform == DevicePlatform.Android ? "10.0.2.2" : "localhost";

    public static readonly string Scheme = "http"; // or http

    public static readonly string Port = "8989";

    public static readonly string ApiUrl = $"{Scheme}://{LocalhostUrl}:{Port}/api/";
    
    // B2c
    private static readonly string TenantName = "";

    private static readonly string Tenant = $"{TenantName}.onmicrosoft.com";

    private static readonly string AzureAdB2CHostname = $"{TenantName}.b2clogin.com";

    private static readonly string ClientId = "";

    // B2C user flows
    public static string PolicySignUpSignIn = "b2c_1_susi";
}
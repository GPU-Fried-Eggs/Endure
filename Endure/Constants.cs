namespace Endure;

public static class Constants
{
    // URL of REST service (Android does not use localhost)
    // Use http cleartext for local deployment. Change to https for production
    public static readonly string LocalhostUrl = DeviceInfo.Platform == DevicePlatform.Android ? "10.0.2.2" : "localhost";

    public static readonly string Scheme = "http"; // or http
    
    public static readonly string Port = "8989";
    
    public static readonly string ApiUrl = $"{Scheme}://{LocalhostUrl}:{Port}/api/";
}
namespace Endure.Models;

public class AzureADB2C
{
    /// <summary>
    /// Gets or sets the login instance for your B2C tenant. In B2C usually of the form https://{YOUR_DOMAIN}.b2clogin.com
    /// </summary>
    /// <value>
    /// The login instance for your B2C tenant
    /// </value>
    public string Instance { get; set; }

    /// <summary>
    /// Gets or sets the tenant identifier (tenant Id/directory id) of the Azure AD tenant where the app registration exists.
    /// </summary>
    /// <value>
    /// The tenant identifier.
    /// </value>
    public string TenantId { get; set; }

    /// <summary>
    /// Gets or sets the client Id (App Id) from the app registration in the Azure AD portal.
    /// </summary>
    /// <value>
    /// The client identifier.
    /// </value>
    public string ClientId { get; set; }

    /// <summary>
    /// Gets or sets the domain of your B2C tenant
    /// </summary>
    /// <value>
    /// The login instance for your B2C tenant
    /// </value>
    public string Domain { get; set; }

    /// <summary>
    /// Gets or sets the sign-in/sign-up policy you wish to use for your application
    /// </summary>
    /// <value>
    /// The sign-in/sign-up policy to use with this application
    public string SignUpSignInPolicyid { get; set; }

    /// <summary>
    /// Gets or sets the reset password policy you wish to use for your application
    /// </summary>
    /// <value>
    /// The reset password policy to use with this application
    public string ResetPasswordPolicyId { get; set; }

    /// <summary>
    /// Gets or sets the edit user profile policy you wish to use for your application
    /// </summary>
    /// <value>
    /// The edit user profile policy to use with this application
    public string EditProfilePolicyId { get; set; }

    /// <summary>
    /// Gets or sets the file name of the token cache file.
    /// </summary>
    /// <value>
    /// The name of the cache file.
    /// </value>
    public string CacheFileName { get; set; }
}
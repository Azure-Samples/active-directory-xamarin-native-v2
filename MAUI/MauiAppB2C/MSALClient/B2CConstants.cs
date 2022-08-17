namespace MauiB2C.MSALClient
{
    public static class B2CConstants
    {
        // Azure AD B2C Coordinates
        public const string Tenant = "fabrikamb2c.onmicrosoft.com";
        public const string AzureADB2CHostname = "fabrikamb2c.b2clogin.com";
        public const string ClientID = "e5737214-6372-472c-a85a-68e8fbe6cf3c";
        public static readonly string RedirectUri = $"msal{ClientID}://auth";
        public const string PolicySignUpSignIn = "b2c_1_susi";

        public static readonly string[] Scopes = { "https://fabrikamb2c.onmicrosoft.com/helloapi/demo.read" };

        public static readonly string AuthorityBase = $"https://{AzureADB2CHostname}/tfp/{Tenant}/";
        public static readonly string AuthoritySignInSignUp = $"{AuthorityBase}{PolicySignUpSignIn}";

        public const string IOSKeyChainGroup = "com.microsoft.adalcache";
    }
}

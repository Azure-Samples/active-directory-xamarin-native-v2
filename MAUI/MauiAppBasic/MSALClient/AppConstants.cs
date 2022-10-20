using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiAppBasic.MSALClient
{
    /// <summary>
    /// Defines constants for the app.
    /// Please change them for your app
    /// </summary>
    internal class AppConstants
    {
        // ClientID of the application
        internal const string ClientId = "[REPLACE THIS WITH THE CLIENT ID OF YOUR APP]"; // TODO - Replace with your client Id. And also replace in the AndroidManifest.xml


        // TenantID of the organization
        internal const string TenantId = "[REPLACE THIS WITH YOUR TENANT ID]"; // TODO - Replace with your TenantID.

        /// <summary>
        /// Scopes defining what app can access in the graph
        /// </summary>
        internal static string[] Scopes = { "User.Read" };


    }
}

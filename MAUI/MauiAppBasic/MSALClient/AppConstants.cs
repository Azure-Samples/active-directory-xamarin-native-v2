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
        // ClientID of the application in (sample-testing.com)
        internal const string ClientId = "4b706872-7c33-43f0-9325-55bf81d39b93"; // TODO - Replace with your client Id. And also replace in the AndroidManifest.xml

        /// <summary>
        /// Scopes defining what app can access in the graph
        /// </summary>
        internal static string[] Scopes = { "User.Read" };


    }
}

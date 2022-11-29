using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIB2C.MSALClient
{
    public class DownstreamApiHelper
    {
        private string[] DownstreamApiScopes;
        public DownStreamApiConfig DownstreamApiConfig;
        private MSALClientHelper MSALClient;

        public DownstreamApiHelper(DownStreamApiConfig downstreamApiConfig, MSALClientHelper msalClientHelper)
        {
            if (msalClientHelper == null)
            {
                throw new ArgumentNullException(nameof(msalClientHelper));
            }

            this.DownstreamApiConfig = downstreamApiConfig;
            this.MSALClient = msalClientHelper;
            this.DownstreamApiScopes = this.DownstreamApiConfig.ScopesArray;
        }
    }
}

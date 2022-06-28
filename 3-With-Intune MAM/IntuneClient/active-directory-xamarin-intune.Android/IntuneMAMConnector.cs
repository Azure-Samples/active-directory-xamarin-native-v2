// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using activedirectoryxamarinintune;
using Microsoft.Identity.Client;
using Microsoft.Intune.Mam.Client.App;
using Microsoft.Intune.Mam.Policy;

namespace active_directory_xamarin_intune.Droid
{
    public class IntuneMAMConnector : IIntuneMAMConnector
    {
        /// <summary>
        /// Perform registration with MAM
        /// </summary>
        /// <param name="exProtection"></param>
        /// <returns></returns>
        public async Task DoMAMRegisterAsync(IntuneAppProtectionPolicyRequiredException exProtection)
        {
            // reset the registered event
            PCAWrapper.MAMRegsiteredEvent.Reset();

            // Invoke compliance API on a different thread
            await Task.Run(() =>
                                {
                                    IMAMComplianceManager mgr = MAMComponents.Get<IMAMComplianceManager>();
                                    mgr.RemediateCompliance(exProtection.Upn, exProtection.AccountUserId, exProtection.TenantId, exProtection.AuthorityUrl, false);
                                }).ConfigureAwait(false);


            // wait till the registration completes
            // Note: This is a sample app for MSAL.NET. Scenarios such as what if enrollment fails or user chooses not to enroll will be as
            // per the business requirements of the app and not considered in the sample app.
            PCAWrapper.MAMRegsiteredEvent.WaitOne();
        }

        public void Unenroll()
        {
            // Not implpemented on Android.
        }
    }
}

﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using activedirectoryxamarinintune;
using Android.Util;
using Microsoft.Identity.Client;
using Microsoft.Intune.Mam.Client.App;
using Microsoft.Intune.Mam.Policy;

[assembly: Xamarin.Forms.Dependency(typeof(active_directory_xamarin_intune.Droid.IntuneMAMConnector))]
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
            Log.Info(GetType().Name, "***Begin DoMAMRegisterAsync");
            Debug.WriteLine("***Begin DoMAMRegisterAsync");
            // reset the registered event
            PCAWrapper.MAMRegsiteredEvent.Reset();

            // Invoke compliance API on a different thread
            await Task.Run(() =>
            {
                Log.Info(GetType().Name, "Attempting to remediateCompliance.");
                Debug.WriteLine("***DoMAMRegisterAsync->RemediateCompliance");
                IMAMComplianceManager mgr = MAMComponents.Get<IMAMComplianceManager>();
                mgr.RemediateCompliance(exProtection.Upn, exProtection.AccountUserId, exProtection.TenantId, exProtection.AuthorityUrl, true);
            }).ConfigureAwait(false);


            // wait till the registration completes
            // Note: This is a sample app for MSAL.NET. Scenarios such as what if enrollment fails or user chooses not to enroll will be as
            // per the business requirements of the app and not considered in the sample app.
            Debug.WriteLine("***DoMAMRegisterAsync->Pre-Wait");
            Log.Info(GetType().Name, "***DoMAMRegisterAsync->Pre-Wait");
            PCAWrapper.MAMRegsiteredEvent.WaitOne();
            Debug.WriteLine("***DoMAMRegisterAsync->Post-Wait");
            Log.Info(GetType().Name, "***DoMAMRegisterAsync->Post-Wait");
        }

        public void Unenroll()
        {
            // Not implpemented on Android by Broker. Known issue.
        }
    }
}

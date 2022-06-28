// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using activedirectoryxamarinintune;
using Microsoft.Identity.Client;
using Microsoft.Intune.MAM;

namespace active_directory_xamarin_intune.iOS
{
    public class IntuneMAMConnector : IIntuneMAMConnector
    {
        public async Task DoMAMRegisterAsync(IntuneAppProtectionPolicyRequiredException exProtection)
        {
            // Reset the registration event. So the app will wait till it is complete.
            PCAWrapper.MAMRegsiteredEvent.Reset();
            // Using IntuneMAMComplianceManager, ensure that the device is compliant.
            // This will raise UI for compliance. After user satisfies the compliance requirements, MainIntuneMAMComplianceDelegate method will be called.
            // the delegate will set the semaphore
            IntuneMAMComplianceManager.Instance.RemediateComplianceForIdentity(exProtection.Upn, false);

            // Wait for the delegate to signal that it is compliant
            PCAWrapper.MAMRegsiteredEvent.WaitOne();
        }

        public void Unenroll()
        {
            IntuneMAMEnrollmentManager.Instance.DeRegisterAndUnenrollAccount(IntuneMAMEnrollmentManager.Instance.EnrolledAccount, true);
        }
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using activedirectoryxamarinintune;
using Microsoft.Intune.MAM;

namespace active_directory_xamarin_intune.iOS
{
    /// <summary>
    /// When device becomes Intune MAM compliant, IdentityHasComplianceStatus method in this class will be called.
    /// It will set the event that will let the calling app know that the device is now compliant.
    /// And app can take the further actions such as calling silent token acquisition.
    /// </summary>
    public class MainIntuneMAMComplianceDelegate : IntuneMAMComplianceDelegate
    {
        public override void IdentityHasComplianceStatus(string identity, IntuneMAMComplianceStatus status, string errorMessage, string errorTitle)
        {
            if (status == IntuneMAMComplianceStatus.Compliant)
            {
                try
                {
                    // Now the app is compliant, set the event. It will notify the App to take the next steps.
                    PCAWrapper.MAMRegsiteredEvent.Set();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Ex = {ex.Message}");
                }

            }
        }
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Android.App;
using Microsoft.Intune.Mam.Client.App;
using Microsoft.Intune.Mam.Client.Notification;
using Microsoft.Intune.Mam.Policy;
using Microsoft.Intune.Mam.Policy.Notification;

namespace active_directory_xamarin_intune.Droid
{
#if DEBUG
    /// <remarks>
    /// Due to an issue with debugging the Xamarin bound MAM SDK the Debuggable = false attribute must be added to the Application in order to enable debugging.
    /// Without this attribute the application will crash when launched in Debug mode. Additional investigation is being performed to identify the root cause.
    /// </remarks>
    [Application(Debuggable = false)]
#else
    [Application]
#endif
    class IntuneSampleApp : MAMApplication
    {
        public IntuneSampleApp(IntPtr handle, Android.Runtime.JniHandleOwnership transfer)
            : base(handle, transfer) { }

        public override void OnMAMCreate()
        {
            // as per Intune SDK doc, callback registration must be done here.
            // https://docs.microsoft.com/en-us/mem/intune/developer/app-sdk-android
            IMAMEnrollmentManager mgr = MAMComponents.Get<IMAMEnrollmentManager>();
            mgr.RegisterAuthenticationCallback(new MAMWEAuthCallback());

            // Register the notification receivers to receive MAM notifications.
            // Along with other, this will receive notification that the device has been enrolled.
            var notificationRcvr = new EnrollmentNotificationReceiver();
            IMAMNotificationReceiverRegistry registry = MAMComponents.Get<IMAMNotificationReceiverRegistry>();
            registry.RegisterReceiver(notificationRcvr, MAMNotificationType.MamEnrollmentResult);
            registry.RegisterReceiver(notificationRcvr, MAMNotificationType.ComplianceStatus);

            base.OnMAMCreate();
        }
    }
}

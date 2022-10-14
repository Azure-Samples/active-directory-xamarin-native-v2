// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Diagnostics;
using activedirectoryxamarinintune;
using Android.Runtime;
using Android.Util;
using Microsoft.Intune.Mam.Client.Notification;
using Microsoft.Intune.Mam.Policy;
using Microsoft.Intune.Mam.Policy.Notification;

namespace active_directory_xamarin_intune.Droid
{
    /// <summary>
    /// Receives enrollment notifications from the Intune service and performs the corresponding action for the enrollment result.
    /// See: https://docs.microsoft.com/en-us/intune/app-sdk-android#mamnotificationreceiver
    /// </summary>
    class EnrollmentNotificationReceiver : Java.Lang.Object, IMAMNotificationReceiver
    {
        /// <summary>
        /// When using the MAM-WE APIs found in IMAMEnrollManager, your app wil receive 
        /// IMAMEnrollmentNotifications back to signal the result of your calls.
        /// When enrollment is successful, this will signal that app has been registered and it can proceed ahead.
        /// </summary>
        /// <param name="notification">The notification that was received.</param>
        /// <returns>
        /// The receiver should return true if it handled the notification without error(or if it decided to ignore the notification). 
        /// If the receiver tried to take some action in response to the notification but failed to complete that action it should return false.
        /// </returns>
        public bool OnReceive(IMAMNotification notification)
        {
            Debug.WriteLine("***Begin EnrollmentNotificationReceiver -> OnReceive");
            if (notification.Type == MAMNotificationType.MamEnrollmentResult)
            {
                Debug.WriteLine("***Begin EnrollmentNotificationReceiver -> OnReceive (notification.Type == MAMNotificationType.MamEnrollmentResult)");
                IMAMEnrollmentNotification enrollmentNotification = notification.JavaCast<IMAMEnrollmentNotification>();
                MAMEnrollmentManagerResult result = enrollmentNotification.EnrollmentResult;
                Debug.WriteLine($"***Begin OnReceive-> Enrollment = {result} Code = {result.Code}");
            }
            else if (notification.Type == MAMNotificationType.ComplianceStatus)
            {
                Debug.WriteLine("***Begin EnrollmentNotificationReceiver -> OnReceive (notification.Type == MAMNotificationType.ComplianceStatus)");
                IMAMComplianceNotification complianceNotification = notification.JavaCast<IMAMComplianceNotification>();
                MAMCAComplianceStatus result = complianceNotification.ComplianceStatus;
                Debug.WriteLine($"***Begin OnReceive-> Compliance = {result} Code = {result.Code}");
                if (result.Equals(MAMCAComplianceStatus.Compliant))
                {
                    Log.Info(GetType().Name, "remediateCompliance succeeded; status = " + result);
                    Debug.WriteLine("***Begin OnReceive-> Compliant");
                    // this signals that MAM registration is complete and the app can proceed
                    PCAWrapper.MAMRegsiteredEvent.Set();
                }
                else
                {
                    Log.Info(GetType().Name, "remediateCompliance failed; status = " + result);
                    Log.Info(GetType().Name, complianceNotification.ComplianceErrorTitle);
                    Log.Info(GetType().Name, complianceNotification.ComplianceErrorMessage);
                }
            }

            return true;
        }
    }
}

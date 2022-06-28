using System;
using activedirectoryxamarinintune;
using Microsoft.Intune.MAM;

namespace active_directory_xamarin_intune.iOS
{
    public class MainIntuneMAMEnrollmentDelegate : IntuneMAMEnrollmentDelegate
    {
        public override async void UnenrollRequestWithStatus(IntuneMAMEnrollmentStatus status)
        {
            if(!status.DidSucceed)
            {
                Console.WriteLine("Intune Unenrollment failed with status code: " + status.StatusCode + ". Error message: " + status.ErrorString);
            }
            await PCAWrapper.Instance.SignOut().ConfigureAwait(false);
        }
    }
}

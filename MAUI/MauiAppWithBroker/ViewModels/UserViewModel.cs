using System.ComponentModel;

namespace MauiAppWithBroker.ViewModels
{
    internal class UserViewModel : INotifyPropertyChanged
    {
        private bool _isSignedIn = false;
        public bool IsSignedIn
        {
            get => _isSignedIn;
            set
            {
                _isSignedIn = value;
                OnPropertyChanged(nameof(IsSignedIn));
            }
        }

        string _displayName = "";
        public string DisplayName
        {
            get => _displayName;
            set
            {
                _displayName = value;
                OnPropertyChanged(nameof(DisplayName));
            }
        }

        string _firstName = "";
        public string FirstName
        {
            get => _firstName;
            set
            {
                _firstName = value;
                OnPropertyChanged(nameof(FirstName));
            }
        }

        string _email = "";
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        private ImageSource _userImage;
        public ImageSource UserImage
        {
            get => _userImage;
            set
            {
                _userImage = value;
                OnPropertyChanged(nameof(UserImage));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string value) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(value));
    }
}

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Security.Claims;

namespace MauiB2C.ViewModels
{
    internal class ScopeViewModel : INotifyPropertyChanged
    {
        string _name = "";
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        string _trustFrameworkPolicy = "";
        public string TrustFrameworkPolicy
        {
            get => _trustFrameworkPolicy;
            set
            {
                _trustFrameworkPolicy = value;
                OnPropertyChanged(nameof(TrustFrameworkPolicy));
            }
        }

        DateTime _issuedAt;
        public DateTime IssuedAt
        {
            get => _issuedAt;
            set
            {
                _issuedAt = value;
                OnPropertyChanged(nameof(IssuedAt));
            }
        }

        private DateTime _expiresAt;
        public DateTime ExpiresAt
        {
            get => _expiresAt;
            set
            {
                _expiresAt = value;
                OnPropertyChanged(nameof(ExpiresAt));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string value) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(value));
    }
}

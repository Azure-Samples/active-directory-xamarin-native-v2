using Microsoft.Graph.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiAppBasic.ViewModels
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        private string _displayName = "NO NAME";
        public string DisplayName
        {
            get => _displayName;
            set
            {
                _displayName = value;
                OnPropertyChanged(nameof(DisplayName));
            }
        }

        private bool _isSignedIn = false;
        public bool IsSignedIn
        {
            get => _isSignedIn;
            set
            {
                _isSignedIn = value;
                OnPropertyChanged(nameof(IsSignedIn));
                OnPropertyChanged(nameof(IsSignedOut));
            }
        }

        public bool IsSignedOut
        {
            get => !_isSignedIn;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string value) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(value));
    }
}

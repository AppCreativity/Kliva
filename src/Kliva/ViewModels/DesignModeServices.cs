using Cimbalino.Toolkit.Services;
using System;
using System.Collections.Generic;

namespace Kliva.ViewModels
{
    internal class DesignModeNavigationService : INavigationService
    {
        public bool CanGoBack
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool CanGoForward
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public object CurrentParameter
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Uri CurrentSource
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IEnumerable<KeyValuePair<string, string>> QueryString
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public event EventHandler<NavigationServiceBackKeyPressedEventArgs> BackKeyPressed;
        public event EventHandler Navigated;

        public void GoBack()
        {
            throw new NotImplementedException();
        }

        public void GoForward()
        {
            throw new NotImplementedException();
        }

        public bool Navigate(Uri source)
        {
            throw new NotImplementedException();
        }

        public bool Navigate(Type type)
        {
            throw new NotImplementedException();
        }

        public bool Navigate(string source)
        {
            throw new NotImplementedException();
        }

        public bool Navigate(Type type, object parameter)
        {
            throw new NotImplementedException();
        }

        public bool Navigate<T>()
        {
            throw new NotImplementedException();
        }

        public bool Navigate<T>(object parameter)
        {
            throw new NotImplementedException();
        }

        public bool RemoveBackEntry()
        {
            throw new NotImplementedException();
        }
    }
}

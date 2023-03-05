using CryptoMonWidgets.Services.Interface;
using Microsoft.UI.Xaml.Controls;
using System;

namespace CryptoMonWidgets.Services
{
    public class MainNavigationService : INavigationService
    {
        public Frame ViewFrame { get; set; }
        private object _lastContent;

        public void NavigateTo(Type pageType, object param = null)
        {
            if (ViewFrame == null)
                throw new InvalidOperationException("View frame is not set!");

            _lastContent = ViewFrame.Content;

            ViewFrame.Navigate(pageType, param);
        }

        public void NavigateFrom(bool restoreContent)
        {
            if (ViewFrame == null)
                throw new InvalidOperationException("View frame is not set!");

            ViewFrame.Content = restoreContent ? _lastContent : null;
        }
    }
}
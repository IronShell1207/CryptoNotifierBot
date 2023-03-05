using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;

namespace CryptoMonWidgets.Services.Interface
{
    public interface INavigationService
    {
        Frame ViewFrame { get; set; }
        void NavigateTo(Type pageType, object param = null);
        void NavigateFrom(bool restoreContent);
    }
}

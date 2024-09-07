using CommunityToolkit.Mvvm.ComponentModel;
using ServiceHelper.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SellerInformationApps.UpdatesViewModel
{
    public partial class UpdateProfilePassword : Authentication
    {
        [ObservableProperty] private string oldPassword;
        [ObservableProperty] private string newPassword;
        [ObservableProperty] private string verifyNewPassword;

    }
}

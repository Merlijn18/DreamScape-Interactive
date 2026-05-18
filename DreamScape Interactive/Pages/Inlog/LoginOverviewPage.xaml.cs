using DreamScape_Interactive.Data;
using DreamScape_Interactive.Pages.Player;
using DreamScape_Interactive.Data.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DreamScape_Interactive.Pages.Inlog
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginOverviewPage : Page
    {
        public LoginOverviewPage()
        {
            InitializeComponent();
        }
        
        private void DevLogin_Click(object sender, RoutedEventArgs e)
        {
            var username = "merlijn@dreamscape.com";
            var password = "123456";

            using var db = new AppDbContext();

            var user = db.Users.FirstOrDefault(u =>
            u.Email.ToLower() == username.ToLower());

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                ShowError("⚠ ACCESS DENIED: Incorrect Wachtwoord!");

            }
            else
            {
                User.LoggedInUser = user;
                Frame.Navigate(typeof(BeheerOverviewPage));
            }
        }
        private void DevLoginPlayer_Click(object sender, RoutedEventArgs e)
        {
            var username = "dragon@player.com";
            var password = "123456";

            using var db = new AppDbContext();

            var user = db.Users.FirstOrDefault(u =>
            u.Email.ToLower() == username.ToLower());

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                ShowError("⚠ ACCESS DENIED: Incorrect Wachtwoord!");

            }
            else
            {
                User.LoggedInUser = user;
                Frame.Navigate(typeof(UserDashboard));
            }
        }
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            AttemtLogin();
        }

        private void AttemtLogin()
        {
            string enterdEmail = EmailBox.Text.Trim();
            string enterdPassword = PasswordBox.Password;

            if (string.IsNullOrEmpty(enterdEmail) || string.IsNullOrEmpty(enterdPassword))
            {
                ShowError("Een van de gegevens zijn niet ingevuld!");
                return;
            }

            using var db = new AppDbContext();

            var user = db.Users.FirstOrDefault(u => u.Email == enterdEmail);
            if (user == null || !BCrypt.Net.BCrypt.Verify(enterdPassword, user.Password))
            {
                ShowError("Wachtwoord Onjuist!");

                PasswordBox.Password = string.Empty;
                PasswordBox.Focus(FocusState.Programmatic);
            }
            else
            {
                User.LoggedInUser = user;

                if(user.Role == "Beheer")
                {
                    Frame.Navigate(typeof(MainPage));
                }
                if (user.Role == "Player")
                {
                    Frame.Navigate(typeof(UserDashboard));
                }
            }

        }

        private void ShowError(string message)
        {
            ErrorMessage.Text = message;
        }

        private void CreateAccount_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(RegisterPage));
        }
    }
}

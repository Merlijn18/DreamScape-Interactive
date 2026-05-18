using Microsoft.UI.Xaml;
using DreamScape_Interactive.Pages.Inlog;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Linq;
using DreamScape_Interactive.Data;
using DreamScape_Interactive.Data.Models;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DreamScape_Interactive.Pages.Beheer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BeheerUserOverview : Page
    {
        public BeheerUserOverview()
        {
            InitializeComponent();
            LoadUsers();
        }

        private void LoadUsers(string searchQuery = "")
        {
            using var db = new AppDbContext();

            var users = db.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                users = users.Where(u => u.Username.Contains(searchQuery) || u.Email.Contains(searchQuery));
            }

            BeheerUserList.ItemsSource = users
                .OrderByDescending(u => u.CreatedAt)
                .ToList();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            User.LoggedInUser = null;
            Frame.Navigate(typeof(LoginOverviewPage));
        }

        private void UserButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(BeheerUserOverview));
        }
        private void BeheerOverviewButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(BeheerOverviewPage));
        }

        private void ItemsButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ItemsManagementPage));
        }

        private void TradesButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(TradesManagementPage));
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SystemSettingsPage));
        }

        private void userSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadUsers(userSearchTextBox.Text);
        }

        private async void ViewInventoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is User user)
            {
                using var db = new AppDbContext();

                var inventory = db.PlayerItems
                    .Where(pi => pi.UserId == user.Id)
                    .Join(db.Items,
                        pi => pi.ItemId,
                        item => item.Id,
                        (pi, item) => new { Item = item, pi.Quantity })
                    .ToList();

                var inventoryText = inventory.Any()
                    ? string.Join("\n", inventory.Select(i => $"• {i.Item.Name} (x{i.Quantity})"))
                    : "No items in inventory";

                ContentDialog dialog = new ContentDialog
                {
                    Title = $"?? {user.Username}'s Inventory",
                    Content = inventoryText,
                    CloseButtonText = "Close",
                    XamlRoot = this.XamlRoot
                };

                await dialog.ShowAsync();
            }
        }

        private async void DeleteUserButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is User user)
            {
                ContentDialog confirmDialog = new ContentDialog
                {
                    Title = "Confirm Delete",
                    Content = $"Are you sure you want to delete user '{user.Username}'? This action cannot be undone.",
                    PrimaryButtonText = "Delete",
                    CloseButtonText = "Cancel",
                    XamlRoot = this.XamlRoot
                };

                var result = await confirmDialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    using var db = new AppDbContext();
                    var userToDelete = db.Users.FirstOrDefault(u => u.Id == user.Id);

                    if (userToDelete != null)
                    {
                        db.Users.Remove(userToDelete);
                        db.SaveChanges();
                        LoadUsers();
                    }
                }
            }
        }
    }
}

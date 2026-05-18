using DreamScape_Interactive.Pages.Inlog;
using DreamScape_Interactive.Data.Models;
using DreamScape_Interactive.Data;
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

namespace DreamScape_Interactive.Pages.Player
{
    public sealed partial class UserDashboard : Page
    {
        public UserDashboard()
        {
            InitializeComponent();
            LoadUserStats();
        }

        private void LoadUserStats()
        {
            if (User.LoggedInUser != null)
            {
                var user = User.LoggedInUser;

                LevelText.Text = user.Level.ToString();

                XPText.Text = $"{user.CurrentXP} / {user.XPToNextLevel}";
                XPProgressBar.Maximum = user.XPToNextLevel;
                XPProgressBar.Value = user.CurrentXP;

                using var db = new AppDbContext();
                var tradesCount = db.Trades
                    .Where(t => t.Player1Id == user.Id || t.Player2Id == user.Id)
                    .Count();
                TradesText.Text = tradesCount.ToString();
            }
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(LoginOverviewPage));
        }

        private void TradingButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(TradingPage), User.LoggedInUser);
        }

        private void InventoryButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PlayerInventoryPage), User.LoggedInUser);
        }

        private void LeaderboardButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Pages.Player.LeaderboardPage), User.LoggedInUser);
        }

        private async void DailyLootButton_Click(object sender, RoutedEventArgs e)
        {
            await ClaimDailyLoot();
        }

        private async System.Threading.Tasks.Task ClaimDailyLoot()
        {
            if (User.LoggedInUser == null)
            {
                var dlg = new ContentDialog { Title = "Error", Content = "No user logged in.", CloseButtonText = "OK", XamlRoot = this.XamlRoot };
                await dlg.ShowAsync();
                return;
            }

            using var db = new AppDbContext();
            var user = db.Users.FirstOrDefault(u => u.Id == User.LoggedInUser.Id);
            if (user == null)
            {
                var dlg = new ContentDialog { Title = "Error", Content = "User not found.", CloseButtonText = "OK", XamlRoot = this.XamlRoot };
                await dlg.ShowAsync();
                return;
            }

            var now = DateTime.UtcNow;
            if (user.LastDailyClaim.HasValue && (now - user.LastDailyClaim.Value).TotalHours < 24)
            {
                var next = user.LastDailyClaim.Value.AddHours(24);
                var dlg = new ContentDialog { Title = "Already claimed", Content = $"You can claim your next daily loot at {next.ToLocalTime():g}.", CloseButtonText = "OK", XamlRoot = this.XamlRoot };
                await dlg.ShowAsync();
                return;
            }

            // Award daily loot: 100 XP and a random common item if available
            int xpAwarded = 100;
            user.CurrentXP += xpAwarded;

            // Update User.LoggedInUser so stats reflect it everywhere that references the static instance
            User.LoggedInUser.CurrentXP += xpAwarded;

            // simple level up check
            while (user.CurrentXP >= user.XPToNextLevel)
            {
                user.CurrentXP -= user.XPToNextLevel;
                user.Level++;
                user.XPToNextLevel = (int)(user.XPToNextLevel * 1.5);

                User.LoggedInUser.CurrentXP = user.CurrentXP;
                User.LoggedInUser.Level = user.Level;
                User.LoggedInUser.XPToNextLevel = user.XPToNextLevel;
            }

            // give starter/common item if exists
            var commonItem = db.Items.FirstOrDefault(i => i.Rarity == "Common");
            if (commonItem != null)
            {
                var pi = db.PlayerItems.FirstOrDefault(p => p.UserId == user.Id && p.ItemId == commonItem.Id);
                if (pi != null) pi.Quantity += 1;
                else db.PlayerItems.Add(new PlayerItem { UserId = user.Id, ItemId = commonItem.Id, Quantity = 1 });
            }

            user.LastDailyClaim = now;
            User.LoggedInUser.LastDailyClaim = now;
            db.SaveChanges();

            // refresh logged in user and UI
            User.LoggedInUser = user;
            LoadUserStats();

            var dialog = new ContentDialog { Title = "Daily Loot", Content = $"You received {xpAwarded} XP and a reward item (if available).", CloseButtonText = "OK", XamlRoot = this.XamlRoot };
            await dialog.ShowAsync();
        }
    }
}

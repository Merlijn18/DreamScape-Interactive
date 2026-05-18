using DreamScape_Interactive.Pages.Beheer;
using DreamScape_Interactive.Pages.Inlog;
using DreamScape_Interactive.Data;
using DreamScape_Interactive.Data.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Linq;

namespace DreamScape_Interactive.Pages
{
  public sealed partial class BeheerOverviewPage : Page
    {
        public BeheerOverviewPage()
   {
            InitializeComponent();
            LoadDashboardStats();
     }

    private void LoadDashboardStats()
 {
      using var db = new AppDbContext();

           var totalUsers = db.Users.Count();
           var totalPlayers = db.Users.Count(u => u.Role == "Player");
           var totalItems = db.Items.Count();
           var totalTrades = db.Trades.Count();
           var pendingTrades = db.TradeRequests.Count(tr => tr.Status == "Pending");
           var completedTradesToday = db.Trades.Count(t => t.TradeDate.Date == DateTime.Now.Date);

            TotalUsersText.Text = totalUsers.ToString();
            TotalPlayersText.Text = totalPlayers.ToString();
            TotalItemsText.Text = totalItems.ToString();
            TotalTradesText.Text = totalTrades.ToString();
            PendingTradesText.Text = pendingTrades.ToString();
            TodayTradesText.Text = completedTradesToday.ToString();
   }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
           User.LoggedInUser = null;
                Frame.Navigate(typeof(LoginOverviewPage));
        }

        public void UserButton_Click(object sender, RoutedEventArgs e)
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
  }
}
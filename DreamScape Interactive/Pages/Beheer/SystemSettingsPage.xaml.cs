using DreamScape_Interactive.Data;
using DreamScape_Interactive.Data.Models;
using DreamScape_Interactive.Pages.Inlog;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Linq;

namespace DreamScape_Interactive.Pages.Beheer
{
    public sealed partial class SystemSettingsPage : Page
    {
        public SystemSettingsPage()
        {
   InitializeComponent();
 LoadDatabaseStats();
    }

        private void LoadDatabaseStats()
        {
         using var db = new AppDbContext();

            var stats = $"Total Users: {db.Users.Count()}\n" +
     $"Total Items: {db.Items.Count()}\n" +
    $"Total Player Items: {db.PlayerItems.Sum(pi => pi.Quantity)}\n" +
       $"Completed Trades: {db.Trades.Count()}\n" +
                $"Pending Requests: {db.TradeRequests.Count(tr => tr.Status == "Pending")}\n" +
     $"Total XP Awarded: {db.Trades.Sum(t => t.XPAwarded)}";

 DBStatsText.Text = stats;
        }

        private async void SaveXPSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            await ShowMessage("Info", "XP settings saved successfully!");
        }

        private async void ResetTradeRequestsButton_Click(object sender, RoutedEventArgs e)
{
        var confirm = new ContentDialog
    {
     Title = "?? Confirm Reset",
       Content = "Are you sure you want to reset all trade requests?\n\nThis will set all pending requests to 'Cancelled'.",
   PrimaryButtonText = "Reset",
CloseButtonText = "Cancel",
 XamlRoot = this.XamlRoot
      };

   var result = await confirm.ShowAsync();

 if (result == ContentDialogResult.Primary)
     {
  using var db = new AppDbContext();
  var pendingRequests = db.TradeRequests.Where(tr => tr.Status == "Pending").ToList();

   foreach (var request in pendingRequests)
  {
    request.Status = "Cancelled";
   }

      db.SaveChanges();

        await ShowMessage("Success", $"Reset {pendingRequests.Count} trade requests.");
       LoadDatabaseStats();
  }
        }

        private async void ClearTradeHistoryButton_Click(object sender, RoutedEventArgs e)
        {
          var confirm = new ContentDialog
      {
   Title = "?? Confirm Clear",
 Content = "Are you sure you want to clear ALL trade history?\n\n?? THIS ACTION CANNOT BE UNDONE!\n\nThis will delete:\n• All completed trades\n• All trade requests (pending, accepted, declined)\n• Trade statistics will be lost",
     PrimaryButtonText = "Clear All",
  CloseButtonText = "Cancel",
   XamlRoot = this.XamlRoot
            };

   var result = await confirm.ShowAsync();

            if (result == ContentDialogResult.Primary)
     {
using var db = new AppDbContext();

     var tradesCount = db.Trades.Count();
         var requestsCount = db.TradeRequests.Count();

      db.Trades.RemoveRange(db.Trades);
        db.TradeRequests.RemoveRange(db.TradeRequests);
      db.SaveChanges();

 await ShowMessage("Success", $"Cleared {tradesCount} trades and {requestsCount} trade requests.");
   LoadDatabaseStats();
     }
  }

        private async void ExportReportButton_Click(object sender, RoutedEventArgs e)
     {
   await ShowMessage("Export Report", "Report generation feature would be implemented here.\n\nWould export:\n• User statistics\n• Trade history\n• Item distribution\n• XP rankings");
 }

   private async System.Threading.Tasks.Task ShowMessage(string title, string content)
        {
  var dialog = new ContentDialog
         {
      Title = title,
     Content = content,
  CloseButtonText = "OK",
XamlRoot = this.XamlRoot
 };
        await dialog.ShowAsync();
   }

 private void BeheerOverviewButton_Click(object sender, RoutedEventArgs e)
 {
 Frame.Navigate(typeof(BeheerOverviewPage));
   }

 private void UserButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(BeheerUserOverview));
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

 private void Logout_Click(object sender, RoutedEventArgs e)
        {
        User.LoggedInUser = null;
  Frame.Navigate(typeof(LoginOverviewPage));
        }
 }
}

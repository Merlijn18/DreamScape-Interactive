using DreamScape_Interactive.Data;
using DreamScape_Interactive.Data.Models;
using DreamScape_Interactive.Pages.Inlog;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Linq;
using Windows.UI;

namespace DreamScape_Interactive.Pages.Beheer
{
    public sealed partial class TradesManagementPage : Page
    {
        public TradesManagementPage()
        {
            InitializeComponent();
            LoadTrades();
            LoadStats();
        }

        private void LoadStats()
        {
            try
            {
                using var db = new AppDbContext();

                var totalTrades = db.Trades?.Count() ?? 0;
                var pending = db.TradeRequests?.Count(tr => tr.Status == "Pending") ?? 0;
                var accepted = db.TradeRequests?.Count(tr => tr.Status == "Accepted") ?? 0;
                var declined = db.TradeRequests?.Count(tr => tr.Status == "Declined") ?? 0;

                TotalTradesText.Text = totalTrades.ToString();
                PendingTradesText.Text = pending.ToString();
                AcceptedTradesText.Text = accepted.ToString();
                DeclinedTradesText.Text = declined.ToString();
            }
            catch (Exception ex)
            {
                // Show a non-blocking error and set defaults
                _ = ShowMessageAsync("Error", $"Failed to load trade stats: {ex.Message}");
                TotalTradesText.Text = "0";
                PendingTradesText.Text = "0";
                AcceptedTradesText.Text = "0";
                DeclinedTradesText.Text = "0";
            }
        }

        private void LoadTrades()
        {
            try
            {
                using var db = new AppDbContext();

                // First get a lightweight projection from EF (no complex operators)
                var raw = db.TradeRequests
                    .OrderByDescending(tr => tr.CreatedDate)
                    .Join(db.Users, tr => tr.SenderId, u => u.Id, (tr, sender) => new { TradeRequest = tr, SenderName = sender.Username })
                    .Join(db.Users, x => x.TradeRequest.ReceiverId, u => u.Id, (x, receiver) => new { x.TradeRequest, x.SenderName, ReceiverName = receiver.Username })
                    .Select(x => new
                    {
                        Id = x.TradeRequest.Id,
                        Status = x.TradeRequest.Status,
                        SenderName = x.SenderName,
                        ReceiverName = x.ReceiverName,
                        SenderItemId = x.TradeRequest.SenderItemId,
                        SenderQuantity = x.TradeRequest.SenderQuantity,
                        ReceiverItemId = x.TradeRequest.ReceiverItemId,
                        ReceiverQuantity = x.TradeRequest.ReceiverQuantity,
                        CreatedDate = x.TradeRequest.CreatedDate
                    })
                    .ToList();

                // Load items into memory for name lookup
                var items = db.Items.ToDictionary(i => i.Id, i => i.Name);

                var requests = raw.Select(x => new
                {
                    RequestId = x.Id,
                    Status = x.Status,
                    StatusIcon = GetStatusIcon(x.Status),
                    StatusColor = GetStatusColor(x.Status),
                    Player1Name = x.SenderName,
                    Player2Name = x.ReceiverName,
                    Player1ItemName = items.TryGetValue(x.SenderItemId, out var sName) ? sName : "Unknown",
                    Player1Quantity = x.SenderQuantity,
                    Player2ItemName = items.TryGetValue(x.ReceiverItemId, out var rName) ? rName : "Unknown",
                    Player2Quantity = x.ReceiverQuantity,
                    XPAwarded = x.Status == "Accepted" ? 50 : 0,
                    Date = x.CreatedDate.ToString("MMM dd, yyyy - hh:mm tt")
                }).ToList();

                TradesListView.ItemsSource = requests;
            }
            catch (Exception ex)
            {
                _ = ShowMessageAsync("Error", $"Failed to load trades: {ex.Message}");
                TradesListView.ItemsSource = Array.Empty<object>();
            }
        }

        private string GetStatusIcon(string status)
        {
            return status switch
            {
                "Pending" => "?",
                "Accepted" => "?",
                "Declined" => "?",
                "Cancelled" => "??",
                _ => "?",
            };
        }

        private SolidColorBrush GetStatusColor(string status)
        {
            return status switch
            {
                "Pending" => new SolidColorBrush(Color.FromArgb(255, 245, 158, 11)), // #F59E0B
                "Accepted" => new SolidColorBrush(Color.FromArgb(255, 16, 185, 129)), // #10B981
                "Declined" => new SolidColorBrush(Color.FromArgb(255, 239, 68, 68)), // #EF4444
                "Cancelled" => new SolidColorBrush(Color.FromArgb(255, 156, 163, 175)), // #9CA3AF
                _ => new SolidColorBrush(Color.FromArgb(255, 107, 114, 128)), // #6B7280
            };
        }

        private async System.Threading.Tasks.Task ShowMessageAsync(string title, string content)
        {
            var dialog = new ContentDialog
            {
                Title = title,
                Content = content,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot,
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

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            User.LoggedInUser = null;
            Frame.Navigate(typeof(LoginOverviewPage));
        }
    }
}

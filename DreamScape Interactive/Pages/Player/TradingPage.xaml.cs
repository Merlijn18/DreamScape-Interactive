using DreamScape_Interactive.Data;
using DreamScape_Interactive.Data.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Linq;

namespace DreamScape_Interactive.Pages.Player
{
    public sealed partial class TradingPage : Page
    {
  private User _currentUser;

        public TradingPage()
        {
        InitializeComponent();
  }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
      base.OnNavigatedTo(e);

     if (e.Parameter is User user)
         {
        _currentUser = user;
 LoadPlayers();
     LoadTradeRequests();
   }
        }

        private void LoadPlayers(string searchQuery = "")
 {
            if (_currentUser == null) return;

       using var db = new AppDbContext();

      var players = db.Users
      .Where(u => u.Id != _currentUser.Id && u.Role == "Player")
      .Where(u => string.IsNullOrEmpty(searchQuery) || u.Username.Contains(searchQuery) || u.Email.Contains(searchQuery))
       .Select(u => new
       {
                u.Id,
   u.Username,
           u.Email,
        u.Level
 })
       .ToList();

            PlayersListView.ItemsSource = players;
        }

        private void LoadTradeRequests()
        {
      if (_currentUser == null) return;

            using var db = new AppDbContext();

  var requests = db.TradeRequests
                .Where(tr => tr.ReceiverId == _currentUser.Id && tr.Status == "Pending")
    .Join(db.Users,
tr => tr.SenderId,
           u => u.Id,
        (tr, u) => new { TradeRequest = tr, SenderName = u.Username })
        .Join(db.Items,
           x => x.TradeRequest.SenderItemId,
   item => item.Id,
      (x, item) => new { x.TradeRequest, x.SenderName, SenderItem = item })
       .Join(db.Items,
 x => x.TradeRequest.ReceiverItemId,
      item => item.Id,
    (x, item) => new
          {
          RequestId = x.TradeRequest.Id,
SenderId = x.TradeRequest.SenderId,
     x.SenderName,
      SenderItemId = x.TradeRequest.SenderItemId,
         SenderItemName = x.SenderItem.Name,
    SenderQuantity = x.TradeRequest.SenderQuantity,
   ReceiverItemId = x.TradeRequest.ReceiverItemId,
         ReceiverItemName = item.Name,
              ReceiverQuantity = x.TradeRequest.ReceiverQuantity,
        CreatedDate = x.TradeRequest.CreatedDate.ToString("MMM dd, hh:mm tt")
    })
         .ToList();

            TradeRequestsListView.ItemsSource = requests;

 // Update notification badge
          if (requests.Count > 0)
       {
                NotificationBadge.Visibility = Visibility.Visible;
         NotificationCount.Text = requests.Count.ToString();
      }
            else
            {
       NotificationBadge.Visibility = Visibility.Collapsed;
        }
        }

        private void PlayerSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadPlayers(PlayerSearchBox.Text);
        }

        private async void TradeWithPlayerButton_Click(object sender, RoutedEventArgs e)
   {
   if (sender is Button button && button.Tag != null)
            {
            dynamic player = button.Tag;
 Frame.Navigate(typeof(CreateTradeRequestPage), new object[] { _currentUser, (int)player.Id, (string)player.Username });
  }
     }

  private async void AcceptTradeButton_Click(object sender, RoutedEventArgs e)
        {
         if (sender is Button button && button.Tag != null)
            {
     dynamic request = button.Tag;
        int requestId = request.RequestId;
           int senderId = request.SenderId;
                int senderItemId = request.SenderItemId;
     int senderQuantity = request.SenderQuantity;
     int receiverItemId = request.ReceiverItemId;
        int receiverQuantity = request.ReceiverQuantity;

                using var db = new AppDbContext();

  try
       {
          // Find the trade request
        var tradeRequest = db.TradeRequests.FirstOrDefault(tr => tr.Id == requestId);
           if (tradeRequest == null || tradeRequest.Status != "Pending")
        {
        await ShowMessageDialog("Error", "This trade request is no longer valid.");
      LoadTradeRequests();
         return;
     }

     // Verify both players have the items
        var senderItem = db.PlayerItems
  .FirstOrDefault(pi => pi.UserId == senderId && pi.ItemId == senderItemId && pi.Quantity >= senderQuantity);
             var receiverItem = db.PlayerItems
     .FirstOrDefault(pi => pi.UserId == _currentUser.Id && pi.ItemId == receiverItemId && pi.Quantity >= receiverQuantity);

           if (senderItem == null)
         {
        await ShowMessageDialog("Error", "The other player no longer has the items they offered.");
        tradeRequest.Status = "Cancelled";
    db.SaveChanges();
  LoadTradeRequests();
  return;
          }

if (receiverItem == null)
     {
               await ShowMessageDialog("Error", "You don't have enough of the requested items.");
              LoadTradeRequests();
          return;
            }

                // Perform the trade
        // Remove items from sender
  senderItem.Quantity -= senderQuantity;
if (senderItem.Quantity <= 0)
          db.PlayerItems.Remove(senderItem);

      // Remove items from receiver
             receiverItem.Quantity -= receiverQuantity;
          if (receiverItem.Quantity <= 0)
        db.PlayerItems.Remove(receiverItem);

      // Add items to sender (what receiver gave)
   var senderNewItem = db.PlayerItems
  .FirstOrDefault(pi => pi.UserId == senderId && pi.ItemId == receiverItemId);
    if (senderNewItem != null)
 senderNewItem.Quantity += receiverQuantity;
           else
            db.PlayerItems.Add(new PlayerItem { UserId = senderId, ItemId = receiverItemId, Quantity = receiverQuantity });

        // Add items to receiver (what sender gave)
        var receiverNewItem = db.PlayerItems
     .FirstOrDefault(pi => pi.UserId == _currentUser.Id && pi.ItemId == senderItemId);
     if (receiverNewItem != null)
          receiverNewItem.Quantity += senderQuantity;
else
          db.PlayerItems.Add(new PlayerItem { UserId = _currentUser.Id, ItemId = senderItemId, Quantity = senderQuantity });

   // Award XP to both players
      int xpAwarded = 50;
       var senderUser = db.Users.FirstOrDefault(u => u.Id == senderId);
   var receiverUser = db.Users.FirstOrDefault(u => u.Id == _currentUser.Id);

    if (senderUser != null)
    {
   senderUser.CurrentXP += xpAwarded;
   CheckLevelUp(senderUser);
}

          if (receiverUser != null)
      {
     receiverUser.CurrentXP += xpAwarded;
    CheckLevelUp(receiverUser);
  
     // Update logged in user
      User.LoggedInUser = receiverUser;
          }

        // Record the trade
    db.Trades.Add(new Trade
      {
    Player1Id = senderId,
            Player2Id = _currentUser.Id,
         Player1ItemId = senderItemId,
          Player1Quantity = senderQuantity,
       Player2ItemId = receiverItemId,
  Player2Quantity = receiverQuantity,
 XPAwarded = xpAwarded,
         TradeDate = DateTime.Now
      });

  // Mark trade request as accepted
      tradeRequest.Status = "Accepted";
         tradeRequest.ResponseDate = DateTime.Now;

         db.SaveChanges();

   await ShowMessageDialog("Success", $"Trade completed! You earned {xpAwarded} XP! ??");
           LoadTradeRequests();
       }
            catch (Exception ex)
{
        await ShowMessageDialog("Error", $"Failed to complete trade: {ex.Message}");
          }
         }
        }

        private void CheckLevelUp(User user)
        {
            while (user.CurrentXP >= user.XPToNextLevel)
            {
       user.CurrentXP -= user.XPToNextLevel;
           user.Level++;
 user.XPToNextLevel = (int)(user.XPToNextLevel * 1.5); // Increase XP needed for next level by 50%
     }
        }

        private async void DeclineTradeButton_Click(object sender, RoutedEventArgs e)
        {
   if (sender is Button button && button.Tag != null)
            {
    dynamic request = button.Tag;
        int requestId = request.RequestId;

     using var db = new AppDbContext();

     var tradeRequest = db.TradeRequests.FirstOrDefault(tr => tr.Id == requestId);
        if (tradeRequest != null)
     {
    tradeRequest.Status = "Declined";
  tradeRequest.ResponseDate = DateTime.Now;
          db.SaveChanges();

                    await ShowMessageDialog("Trade Declined", "You have declined this trade request.");
        LoadTradeRequests();
    }
            }
        }

        private async System.Threading.Tasks.Task ShowMessageDialog(string title, string content)
        {
   ContentDialog dialog = new ContentDialog
      {
      Title = title,
           Content = content,
          CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };

         await dialog.ShowAsync();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
  }
}

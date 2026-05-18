using DreamScape_Interactive.Data;
using DreamScape_Interactive.Data.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Linq;

namespace DreamScape_Interactive.Pages.Player
{
    public sealed partial class CreateTradeRequestPage : Page
    {
        private User _currentUser;
        private int _targetPlayerId;
 private string _targetPlayerName;
        private dynamic _selectedYourItem;
private dynamic _selectedTheirItem;

        public CreateTradeRequestPage()
  {
          InitializeComponent();
}

        protected override void OnNavigatedTo(NavigationEventArgs e)
 {
      base.OnNavigatedTo(e);

  if (e.Parameter is object[] parameters && parameters.Length >= 3)
            {
    _currentUser = (User)parameters[0];
      _targetPlayerId = (int)parameters[1];
   _targetPlayerName = (string)parameters[2];

        TargetPlayerText.Text = $"Trading with: {_targetPlayerName}";

    LoadYourInventory();
   LoadTheirInventory();
   }
        }

 private void LoadYourInventory()
  {
      if (_currentUser == null) return;

      using var db = new AppDbContext();

         var inventory = db.PlayerItems
   .Where(pi => pi.UserId == _currentUser.Id && pi.Quantity > 0)
      .Join(db.Items,
     playerItem => playerItem.ItemId,
      item => item.Id,
      (playerItem, item) => new
         {
     PlayerItemId = playerItem.Id,
    Item = item,
 Quantity = playerItem.Quantity
   })
       .ToList();

       YourItemComboBox.ItemsSource = inventory;
 }

   private void LoadTheirInventory()
        {
      if (_targetPlayerId == 0) return;

     using var db = new AppDbContext();

       var inventory = db.PlayerItems
      .Where(pi => pi.UserId == _targetPlayerId && pi.Quantity > 0)
        .Join(db.Items,
  playerItem => playerItem.ItemId,
        item => item.Id,
  (playerItem, item) => new
     {
  PlayerItemId = playerItem.Id,
  Item = item,
      Quantity = playerItem.Quantity
          })
  .ToList();

            TheirItemComboBox.ItemsSource = inventory;
        }

      private void YourItemComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
   if (YourItemComboBox.SelectedItem != null)
            {
    _selectedYourItem = YourItemComboBox.SelectedItem;

   YourItemPreview.Visibility = Visibility.Visible;
  YourItemNameText.Text = _selectedYourItem.Item.Name;
YourItemRarityText.Text = $"Rarity: {_selectedYourItem.Item.Rarity}";

   YourQuantityNumberBox.Maximum = _selectedYourItem.Quantity;
            }
       else
            {
  YourItemPreview.Visibility = Visibility.Collapsed;
   }
        }

      private void TheirItemComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
   {
            if (TheirItemComboBox.SelectedItem != null)
       {
   _selectedTheirItem = TheirItemComboBox.SelectedItem;

TheirItemPreview.Visibility = Visibility.Visible;
       TheirItemNameText.Text = _selectedTheirItem.Item.Name;
       TheirItemRarityText.Text = $"Rarity: {_selectedTheirItem.Item.Rarity}";

 // Fix TheirQuantityNumberBox max since they shouldn't be limited to 1
 TheirQuantityNumberBox.Maximum = _selectedTheirItem.Quantity;
       }
   else
   {
         TheirItemPreview.Visibility = Visibility.Collapsed;
            }
    }

        private async void SendTradeRequestButton_Click(object sender, RoutedEventArgs e)
  {
      if (_selectedYourItem == null)
         {
            await ShowMessageDialog("Error", "Please select an item you want to offer.");
  return;
 }

      if (_selectedTheirItem == null)
    {
           await ShowMessageDialog("Error", "Please select an item you want to receive.");
        return;
       }

       var yourQuantity = (int)YourQuantityNumberBox.Value;
            var theirQuantity = (int)TheirQuantityNumberBox.Value;

       if (yourQuantity <= 0 || theirQuantity <= 0)
   {
   await ShowMessageDialog("Error", "Quantities must be at least 1.");
         return;
   }

            using var db = new AppDbContext();

         try
      {
   db.TradeRequests.Add(new TradeRequest
      {
   SenderId = _currentUser.Id,
  ReceiverId = _targetPlayerId,
        SenderItemId = _selectedYourItem.Item.Id,
              SenderQuantity = yourQuantity,
      ReceiverItemId = _selectedTheirItem.Item.Id,
  ReceiverQuantity = theirQuantity,
      Status = "Pending",
       CreatedDate = DateTime.Now
     });

 db.SaveChanges();

           await ShowMessageDialog("Success", $"Trade request sent to {_targetPlayerName}!");
      Frame.GoBack();
   }
     catch (Exception ex)
  {
       await ShowMessageDialog("Error", $"Failed to send trade request: {ex.Message}");
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

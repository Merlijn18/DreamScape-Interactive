using DreamScape_Interactive.Data;
using DreamScape_Interactive.Data.Models;
using DreamScape_Interactive.Pages.Inlog;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Linq;
using Windows.UI;

namespace DreamScape_Interactive.Pages.Beheer
{
    public sealed partial class ItemsManagementPage : Page
    {
        private string _currentFilter = "All";

 public ItemsManagementPage()
        {
            InitializeComponent();
     LoadItems();
        }

        private void LoadItems(string searchQuery = "")
        {
  using var db = new AppDbContext();

      var query = db.Items.AsQueryable();

   // Apply type filter
  if (_currentFilter == "Weapon")
           query = query.Where(i => i.Type == "Weapon");
     else if (_currentFilter == "Armor")
           query = query.Where(i => i.Type == "Armor");

        // Apply search filter
       if (!string.IsNullOrWhiteSpace(searchQuery))
            {
    query = query.Where(i => i.Name.Contains(searchQuery) || i.Description.Contains(searchQuery));
}

    ItemsListView.ItemsSource = query.OrderBy(i => i.Name).ToList();
        }

        private void AllItemsTab_Click(object sender, RoutedEventArgs e)
      {
            _currentFilter = "All";
    AllItemsTab.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Color.FromArgb(255, 16, 185, 129));
    WeaponsTab.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Color.FromArgb(255, 55, 65, 81));
          ArmorTab.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Color.FromArgb(255, 55, 65, 81));
            LoadItems(ItemSearchBox.Text);
        }

        private void WeaponsTab_Click(object sender, RoutedEventArgs e)
        {
            _currentFilter = "Weapon";
    AllItemsTab.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Color.FromArgb(255, 55, 65, 81));
            WeaponsTab.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Color.FromArgb(255, 16, 185, 129));
            ArmorTab.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Color.FromArgb(255, 55, 65, 81));
            LoadItems(ItemSearchBox.Text);
   }

  private void ArmorTab_Click(object sender, RoutedEventArgs e)
        {
      _currentFilter = "Armor";
AllItemsTab.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Color.FromArgb(255, 55, 65, 81));
    WeaponsTab.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Color.FromArgb(255, 55, 65, 81));
            ArmorTab.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Color.FromArgb(255, 16, 185, 129));
            LoadItems(ItemSearchBox.Text);
      }

        private void ItemSearchBox_TextChanged(object sender, TextChangedEventArgs e)
{
     LoadItems(ItemSearchBox.Text);
}

        private async void AddItemButton_Click(object sender, RoutedEventArgs e)
        {
            // Create dialog for adding new item
         var dialog = new ContentDialog
          {
    Title = "? Add New Item",
     PrimaryButtonText = "Create",
        CloseButtonText = "Cancel",
         XamlRoot = this.XamlRoot
            };

  var panel = new StackPanel { Spacing = 15 };

 var nameBox = new TextBox { PlaceholderText = "Item Name", Header = "Name" };
            var descBox = new TextBox { PlaceholderText = "Description", Header = "Description", TextWrapping = TextWrapping.Wrap, Height = 80 };
       var typeCombo = new ComboBox { Header = "Type", PlaceholderText = "Select Type", ItemsSource = new[] { "Weapon", "Armor" } };
            var rarityCombo = new ComboBox { Header = "Rarity", PlaceholderText = "Select Rarity", ItemsSource = new[] { "Common", "Rare", "Epic", "Legendary" } };
            var strengthBox = new NumberBox { Header = "Strength", Value = 0, Minimum = 0 };
            var speedBox = new NumberBox { Header = "Speed", Value = 0 };
            var durabilityBox = new NumberBox { Header = "Durability", Value = 0, Minimum = 0 };
     var magicBox = new TextBox { PlaceholderText = "Magic Effect", Header = "Magic Effect" };

    panel.Children.Add(nameBox);
     panel.Children.Add(descBox);
    panel.Children.Add(typeCombo);
            panel.Children.Add(rarityCombo);
 panel.Children.Add(strengthBox);
            panel.Children.Add(speedBox);
            panel.Children.Add(durabilityBox);
    panel.Children.Add(magicBox);

            dialog.Content = panel;

    var result = await dialog.ShowAsync();

       if (result == ContentDialogResult.Primary)
     {
   if (string.IsNullOrWhiteSpace(nameBox.Text) || typeCombo.SelectedItem == null || rarityCombo.SelectedItem == null)
    {
     await ShowMessage("Error", "Please fill in all required fields (Name, Type, Rarity).");
        return;
      }

     using var db = new AppDbContext();
 db.Items.Add(new Item
             {
       Name = nameBox.Text,
    Description = descBox.Text,
             Type = typeCombo.SelectedItem.ToString()!,
         Rarity = rarityCombo.SelectedItem.ToString()!,
            strength = (int)strengthBox.Value,
      Speed = (int)speedBox.Value,
          Durability = (int)durabilityBox.Value,
        Magic_Effect = magicBox.Text
     });

      db.SaveChanges();
      await ShowMessage("Success", $"Item '{nameBox.Text}' created successfully!");
       LoadItems();
     }
        }

        private async void EditItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Item item)
    {
var dialog = new ContentDialog
           {
   Title = $"?? Edit Item: {item.Name}",
      PrimaryButtonText = "Save",
      CloseButtonText = "Cancel",
     XamlRoot = this.XamlRoot
                };

        var panel = new StackPanel { Spacing = 15 };

      var nameBox = new TextBox { Text = item.Name, Header = "Name" };
     var descBox = new TextBox { Text = item.Description, Header = "Description", TextWrapping = TextWrapping.Wrap, Height = 80 };
            var strengthBox = new NumberBox { Header = "Strength", Value = item.strength, Minimum = 0 };
       var speedBox = new NumberBox { Header = "Speed", Value = item.Speed };
       var durabilityBox = new NumberBox { Header = "Durability", Value = item.Durability, Minimum = 0 };
          var magicBox = new TextBox { Text = item.Magic_Effect, Header = "Magic Effect" };

 panel.Children.Add(nameBox);
     panel.Children.Add(descBox);
   panel.Children.Add(strengthBox);
           panel.Children.Add(speedBox);
        panel.Children.Add(durabilityBox);
             panel.Children.Add(magicBox);

  dialog.Content = panel;

     var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
  using var db = new AppDbContext();
           var itemToUpdate = db.Items.FirstOrDefault(i => i.Id == item.Id);

   if (itemToUpdate != null)
       {
   itemToUpdate.Name = nameBox.Text;
   itemToUpdate.Description = descBox.Text;
      itemToUpdate.strength = (int)strengthBox.Value;
          itemToUpdate.Speed = (int)speedBox.Value;
             itemToUpdate.Durability = (int)durabilityBox.Value;
          itemToUpdate.Magic_Effect = magicBox.Text;

      db.SaveChanges();
                await ShowMessage("Success", $"Item '{item.Name}' updated successfully!");
    LoadItems();
      }
   }
            }
        }

        private async void DeleteItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Item item)
    {
            var confirm = new ContentDialog
      {
 Title = "Confirm Delete",
 Content = $"Are you sure you want to delete '{item.Name}'?\n\nThis will remove it from all player inventories!",
      PrimaryButtonText = "Delete",
      CloseButtonText = "Cancel",
               XamlRoot = this.XamlRoot
     };

   var result = await confirm.ShowAsync();

       if (result == ContentDialogResult.Primary)
                {
      using var db = new AppDbContext();
          var itemToDelete = db.Items.FirstOrDefault(i => i.Id == item.Id);

        if (itemToDelete != null)
     {
             // Remove from player inventories first
        var playerItems = db.PlayerItems.Where(pi => pi.ItemId == item.Id).ToList();
     db.PlayerItems.RemoveRange(playerItems);

           // Remove the item
        db.Items.Remove(itemToDelete);
        db.SaveChanges();

    await ShowMessage("Success", $"Item '{item.Name}' deleted successfully!");
    LoadItems();
     }
            }
   }
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

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
User.LoggedInUser = null;
  Frame.Navigate(typeof(LoginOverviewPage));
        }
    }
}

using DreamScape_Interactive.Data;
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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DreamScape_Interactive.Pages.Player
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PlayerInventoryPage : Page
    {
        public List<PlayerItem> InventoryItems { get; set; } = new();
        private User _currentUser;
        
        public PlayerInventoryPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            
            if (e.Parameter is User user)
            {
                _currentUser = user;
                LoadItems();
            }
        }

        private void LoadItems()
        {
            if (_currentUser == null) return;

            using var db = new AppDbContext();

            // Load items that belong to the current user through PlayerItems
            var userInventory = db.PlayerItems
                .Where(pi => pi.UserId == _currentUser.Id)
                .Join(db.Items,
                    playerItem => playerItem.ItemId,
                    item => item.Id,
                    (playerItem, item) => new 
                    { 
                        Item = item, 
                        Quantity = playerItem.Quantity 
                    })
                .ToList();

            ItemListView.ItemsSource = userInventory;
        }

        private void ItemSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_currentUser == null) return;

            var searchQuery = ItemSearchBox.Text;

            using var db = new AppDbContext();

            // Search only in the user's inventory
            var userInventory = db.PlayerItems
                .Where(pi => pi.UserId == _currentUser.Id)
                .Join(db.Items,
                    playerItem => playerItem.ItemId,
                    item => item.Id,
                    (playerItem, item) => new 
                    { 
                        Item = item, 
                        Quantity = playerItem.Quantity 
                    })
                .Where(x => x.Item.Name.Contains(searchQuery))
                .ToList();

            ItemListView.ItemsSource = userInventory;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}

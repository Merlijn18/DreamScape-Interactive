using DreamScape_Interactive.Data;
using DreamScape_Interactive.Data.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Linq;

namespace DreamScape_Interactive.Pages.Inlog
{
 public sealed partial class RegisterPage : Page
 {
 public RegisterPage()
 {
 InitializeComponent();
 LoadStarterItems();
 }

 private void LoadStarterItems()
 {
 using var db = new AppDbContext();
 var items = db.Items.Take(10).ToList();
 StarterItemCombo.ItemsSource = items;
 StarterItemCombo.DisplayMemberPath = "Name";
 }

 private void RegisterButton_Click(object sender, RoutedEventArgs e)
 {
 var username = UsernameBox.Text.Trim();
 var email = EmailRegisterBox.Text.Trim();
 var password = PasswordRegisterBox.Password;
 var selected = StarterItemCombo.SelectedItem as Item;

 if(string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
 {
 RegisterError.Text = "Please fill all required fields.";
 return;
 }

 using var db = new AppDbContext();
 if(db.Users.Any(u => u.Email == email))
 {
 RegisterError.Text = "Email already registered.";
 return;
 }

 var hashed = BCrypt.Net.BCrypt.HashPassword(password);
 var user = new User
 {
 Username = username,
 Email = email,
 Password = hashed,
 Role = "Player",
 CreatedAt = System.DateTime.Now,
 Level =1,
 CurrentXP =0,
 XPToNextLevel =100
 };
 db.Users.Add(user);
 db.SaveChanges();

 // add starter item
 if(selected != null)
 {
 var playerItem = new PlayerItem { UserId = user.Id, ItemId = selected.Id, Quantity =1 };
 db.PlayerItems.Add(playerItem);
 db.SaveChanges();
 }

 // navigate to dashboard
 User.LoggedInUser = user;
 Frame.Navigate(typeof(Pages.Player.UserDashboard));
 }
 }
}

using DreamScape_Interactive.Data;
using DreamScape_Interactive.Data.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Linq;

namespace DreamScape_Interactive.Pages.Player
{
 public sealed partial class LeaderboardPage : Page
 {
 private User _currentUser;
 private bool _sortByLevel = true;

 public LeaderboardPage()
 {
 InitializeComponent();
 }

 protected override void OnNavigatedTo(NavigationEventArgs e)
 {
 base.OnNavigatedTo(e);
 if (e.Parameter is User user)
 {
 _currentUser = user;
 LoadLeaderboard();
 }
 }

 private void LoadLeaderboard(string searchQuery = "")
 {
 if (_currentUser == null) return;

 using var db = new AppDbContext();

 var query = db.Users.Where(u => u.Role == "Player")
 .Select(u => new
 {
 u.Id,
 u.Username,
 u.Email,
 u.Level,
 u.CurrentXP,
 });

 if (!string.IsNullOrWhiteSpace(searchQuery))
 query = query.Where(u => u.Username.Contains(searchQuery) || u.Email.Contains(searchQuery));

 var leaderboard = _sortByLevel
 ? query.OrderByDescending(u => u.Level).ThenByDescending(u => u.CurrentXP).ToList()
 : query.OrderByDescending(u => u.CurrentXP).ThenByDescending(u => u.Level).ToList();

 var ranked = leaderboard.Select((u, i) => new
 {
 Rank = i +1,
 u.Id,
 u.Username,
 u.Email,
 u.Level,
 TotalXP = CalculateTotalXP(u.Level, u.CurrentXP)
 }).ToList();

 LeaderboardListView.ItemsSource = ranked;
 }

 private int CalculateTotalXP(int level, int currentXP)
 {
 int total = currentXP;
 int xpForLevel =100;
 for (int i =1; i < level; i++)
 {
 total += xpForLevel;
 xpForLevel = (int)(xpForLevel *1.5);
 }
 return total;
 }

 private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
 {
 LoadLeaderboard(SearchBox.Text);
 }

 private void BackButton_Click(object sender, RoutedEventArgs e)
 {
 if (Frame.CanGoBack)
 {
 Frame.GoBack();
 }
 }
 }
}

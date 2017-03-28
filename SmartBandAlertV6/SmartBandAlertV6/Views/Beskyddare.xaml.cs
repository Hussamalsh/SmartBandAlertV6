using SmartBandAlertV6.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmartBandAlertV6.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    class Friends
    {
        public string Name { get; set; }
        public String profilePic { get; set; }
        public String fbid { get; set; }
    }

    public partial class Beskyddare : ContentPage
    {
        List<Friends> friends;
        List<Friends> friendsExisting;
        public Beskyddare()
        {
            InitializeComponent();

            friends = new List<Friends> {
                new Friends {Name="Xander", profilePic = "https://s-media-cache-ak0.pinimg.com/originals/2b/05/14/2b05140a776f25a8047c88fbe2bcbdb9.jpg", fbid="123123123123"},
                new Friends {Name="Xander", profilePic = "installningar.png", fbid="123123123123"},
                new Friends {Name="Rupert",  profilePic = "hem.png", fbid="123123123123"}
            };
            friendsExisting = new List<Friends> {
                new Friends {Name="Hussi", profilePic = "om.png", fbid="123123123123"},
                new Friends {Name="charlie", profilePic = "om.png", fbid="123123123123"},
                new Friends {Name="David",  profilePic = "om.png", fbid="123123123123"}
            };

            topRightButton.BindingContext = new { text2 = "Sök beskyddare", w1 = App.ScreenWidth * 160 / (App.ScreenDPI * 2), bgc2 = Color.White };
            topButton.BindingContext = new { text = "Existerande beskyddare", w0 = App.ScreenWidth * 160 / (App.ScreenDPI * 2), bgc1 = Color.FromHex("#ededed") };

            friendEXISTINGView.ItemsSource = friendsExisting;


            friendSEARCHView.ItemsSource = friends;


            friendEXISTINGView.IsVisible = true;
            friendSEARCHView.IsVisible = false;
            searchFriends.IsVisible = false;
            friendEXISTINGView.ItemSelected += (sender, e) => friendEXISTINGView.SelectedItem = null;
            friendSEARCHView.ItemSelected += (sender, e) => friendSEARCHView.SelectedItem = null;

        }


        protected async override void OnAppearing()
        {

            base.OnAppearing();
            //var list = await App.UserManager.GetTasksAsync();
            var list = await App.FriendsManager.GetTasksAsync();
            friendEXISTINGView.ItemsSource = list;

        }


        void topButtonClicked(object sender, EventArgs e)
        {
            topButton.BindingContext = new { text = "Existerande beskyddare", w0 = App.ScreenWidth * 160 / (App.ScreenDPI * 2), bgc1 = Color.FromHex("#ededed") };
            topRightButton.BindingContext = new { text2 = "Sök beskyddare", w1 = App.ScreenWidth * 160 / (App.ScreenDPI * 2), bgc2 = Color.White };
            friendEXISTINGView.IsVisible = true;
            friendSEARCHView.IsVisible = false;
            searchFriends.IsVisible = false;
        }

        async void topRightButtonClicked(object sender, EventArgs e)
        {
            friendSEARCHView.ItemsSource = await App.UserManager.GetTasksAsync();
            topButton.BindingContext = new { text = "Existerande beskyddare", w0 = App.ScreenWidth * 160 / (App.ScreenDPI * 2), bgc1 = Color.White };
            topRightButton.BindingContext = new { text2 = "Sök beskyddare", w1 = App.ScreenWidth * 160 / (App.ScreenDPI * 2), bgc2 = Color.FromHex("#ededed") };
            friendEXISTINGView.IsVisible = false;
            friendSEARCHView.IsVisible = true;
            searchFriends.IsVisible = true;
        }

        async void checkTapped(object sender, EventArgs args)
        {
            
            var answer = await DisplayAlert("Beskyddare", "Vill du lägga till den här beskyddaren?", "Nej", "Ja");
            Debug.WriteLine("Answer: " + answer);


            var button = sender as Image;
            User todo = button.BindingContext as User;

            FriendsList f = new FriendsList() { FriendFBID = todo.FBID, UserName = todo.UserName, ImgLink = todo.ImgLink, UserFBID = App.FacebookId };
            await CompleteAdd(f);
        }

        async Task CompleteAdd(FriendsList item)
        {
            await App.FriendsManager.SaveTaskAsync(item, true);
            var list = await App.FriendsManager.GetTasksAsync();
            friendEXISTINGView.ItemsSource = list;
        }

        async void trashTapped(object sender, EventArgs args)
        {
            
            //((Image)sender).Opacity = 0.5;
            var answer = await DisplayAlert("Beskyddare", "Vill du ta bort den här beskyddaren?", "Ja", "Nej");
            Debug.WriteLine("Answer: " + answer);
            //Debug.WriteLine(((Friends)((Image)sender).BindingContext).Name);

            //((Image)sender).Opacity = 1;


            var button = sender as Image;
            FriendsList item = button.BindingContext as FriendsList;

            await CompleteItem(item);

        }

        async Task CompleteItem(FriendsList item)
        {

            //await manager.SaveTaskAsync(item);
            App.FriendsManager.DeleteTaskAsync(item);
            var list = await App.FriendsManager.GetTasksAsync();
            friendEXISTINGView.ItemsSource = list;
        }

    }
}

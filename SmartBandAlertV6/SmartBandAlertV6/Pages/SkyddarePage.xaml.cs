using SmartBandAlertV6.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmartBandAlertV6.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SkyddarePage : ContentPage
    {
        public SkyddarePage()
        {
            InitializeComponent();
        }

        protected async override void OnAppearing()
        {

            base.OnAppearing();
            //var list = await App.UserManager.GetTasksAsync();
            var list = await App.FriendsManager.GetTasksAsync();
            listView.ItemsSource = list;

        }


        private async void MainSearchBar_OnSearchButtonPressed(object sender, EventArgs e)
        {
            string keyword = MainSearchBar.Text;

            var list = await App.UserManager.SearchUsersAsync(keyword);
            listView.ItemsSource = list;

        }


        // http://developer.xamarin.com/guides/cross-platform/xamarin-forms/working-with/listview/#context
        public async void OnComplete(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var todo = mi.CommandParameter as FriendsList;
            await CompleteItem(todo);
        }


        async Task CompleteItem(FriendsList item)
        {

            //await manager.SaveTaskAsync(item);
            App.FriendsManager.DeleteTaskAsync(item);
            var list = await App.FriendsManager.GetTasksAsync();
            listView.ItemsSource = list;
        }

        //OnCompleteAdd
        public async void OnCompleteAdd(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var todo = mi.CommandParameter as User;
            FriendsList f = new FriendsList() { FriendFBID = todo.FBID, UserName = todo.UserName, ImgLink = todo.ImgLink, UserFBID = App.FacebookId };
            await CompleteAdd(f);
        }


        async Task CompleteAdd(FriendsList item)
        {
            await App.FriendsManager.SaveTaskAsync(item,true);
            var list = await App.FriendsManager.GetTasksAsync();
            listView.ItemsSource = list;
        }
        void OnItemSelected(object sender, SelectedItemChangedEventArgs e)

        {

            var todoItem = e.SelectedItem as User;

            //var todoPage = new TodoItemPage();

            //todoPage.BindingContext = todoItem;

            //Navigation.PushAsync(todoPage);

        }


    }
}

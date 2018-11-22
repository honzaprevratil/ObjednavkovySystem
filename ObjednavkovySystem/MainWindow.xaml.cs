using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ObjednavkovySystem
{
    /// <summary>
    /// Interakční logika pro MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ApiProvider ApiProvider = new ApiProvider();

        Item loadedItem;

        public MainWindow()
        {
            InitializeComponent();
            this.KeyDown += EnterDown;
            //ApiProvider.GetUsers();

            logOutButton.Visibility = Visibility.Hidden;
            TabOrders.Visibility = Visibility.Hidden;
            TabItems.Visibility = Visibility.Hidden;
        }

        private void EnterDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TryToLogin();
            }
        }

        private void Button_LogIn(object sender, RoutedEventArgs e)
        {
            TryToLogin();
        }

        private void Button_LogOut(object sender, RoutedEventArgs e)
        {
            ordersListview.ItemsSource = new List<Order>();
            itemsListview.ItemsSource = new List<Item>();
            ApiProvider.LoggedUser = new User();
            loggedUserLabel.Content = "";
            messageLabel.Content = "Succesfully logged out!";
            messageLabel.BorderBrush = Brushes.Blue;

            logOutButton.Visibility = Visibility.Hidden;
            TabOrders.Visibility = Visibility.Hidden;
            TabItems.Visibility = Visibility.Hidden;
        }

        private async void Button_OrderItem(object sender, RoutedEventArgs e)
        {
            if (itemsListview.SelectedItem != null)
            {
                await ApiProvider.PostData("order", (Item)itemsListview.SelectedItem);
                ordersListview.ItemsSource = await ApiProvider.GetData<Order>();
                messageLabel.Content = "Order placed.";
                messageLabel.BorderBrush = Brushes.Blue;
            }
            else
            {
                messageLabel.Content = "Select item to order.";
                messageLabel.BorderBrush = Brushes.Red;
            }
        }

        private async void Button_DeleteOrder(object sender, RoutedEventArgs e)
        { 
            if (ordersListview.SelectedItem != null)
            {
                await ApiProvider.PostData("hide", (Order)ordersListview.SelectedItem);
                ordersListview.ItemsSource = await ApiProvider.GetData<Order>();
                messageLabel.Content = "Order hidden.";
                messageLabel.BorderBrush = Brushes.Blue;
            }
            else
            {
                messageLabel.Content = "Select order to hide.";
                messageLabel.BorderBrush = Brushes.Red;
            }
        }

        private async void TryToLogin()
        {
            if (passInput.Password != "" && nickInput.Text != "")
            {
                User postUser = new User()
                {
                    nick = nickInput.Text,
                    password = Hash.sha256_hash(passInput.Password)
                };

                if (await ApiProvider.PostData("login", postUser))
                {
                    messageLabel.Content = "Logged in succesfully!";
                    messageLabel.BorderBrush = Brushes.Blue;
                    loggedUserLabel.Content = ApiProvider.LoggedUser.name + " " + ApiProvider.LoggedUser.surname;

                    // get user's orders
                    ordersListview.ItemsSource = await ApiProvider.GetData<Order>();

                    // navigate to next page
                    MineTabControl.SelectedIndex = 1;

                    // get items
                    itemsListview.ItemsSource = await ApiProvider.GetData<Item>();

                    // set visiblity
                    logOutButton.Visibility = Visibility.Visible;
                    TabOrders.Visibility = Visibility.Visible;
                    TabItems.Visibility = Visibility.Visible;
                }
                else
                {
                    loggedUserLabel.Content = "";
                    messageLabel.Content = "Invalid nick or password!";
                    messageLabel.BorderBrush = Brushes.Red;
                    ordersListview.ItemsSource = new List<Order>();
                    itemsListview.ItemsSource = new List<Item>();
                }
                passInput.Password = "";
                nickInput.Text = "";
            }
            else
            {
                messageLabel.Content = "Missing nick or password!";
                messageLabel.BorderBrush = Brushes.Red;
            }
        }

        private void ItemsListview_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ListView).SelectedItem != null )
                labelDescription.Text = ((sender as ListView).SelectedItem as Item).description;
        }

        private async void Button_SaveItem(object sender, RoutedEventArgs e)
        {
            if (loadedItem != null)
            {
                loadedItem.name = itemName.Text;
                loadedItem.description = itemDescription.Text;
                loadedItem.price = int.Parse(itemPrice.Text);

                await ApiProvider.PostData("updateItem", loadedItem);
                
                // clear inputs
                loadedItem = null;
                itemName.Text = "";
                itemPrice.Text = "";
                itemDescription.Text = "";
            }
            else
            {
                await ApiProvider.PostData("addItem", new Item() {
                    name = itemName.Text,
                    price = int.Parse(itemPrice.Text),
                    description = itemDescription.Text
                });

                // clear inputs
                itemName.Text = "";
                itemPrice.Text = "";
                itemDescription.Text = "";
            }

            // refresh list
            itemsListview.ItemsSource = await ApiProvider.GetData<Item>();
            labelDescription.Text = "";
        }

        private void Button_EditItem(object sender, RoutedEventArgs e)
        {
            if (itemsListview.SelectedItem != null)
            {
                loadedItem = itemsListview.SelectedItem as Item;
                itemName.Text = loadedItem.name;
                itemPrice.Text = loadedItem.price.ToString();
                itemDescription.Text = loadedItem.description;
            }
        }

        private void Button_CancelItem(object sender, RoutedEventArgs e)
        {
            loadedItem = null;
            itemName.Text = "";
            itemPrice.Text = "";
            itemDescription.Text = "";
        }

        private async void Button_DeleteItem(object sender, RoutedEventArgs e)
        {
            if (itemsListview.SelectedItem != null)
                await ApiProvider.PostData("deleteItem", (itemsListview.SelectedItem as Item));

            // refresh list
            itemsListview.ItemsSource = await ApiProvider.GetData<Item>();
            labelDescription.Text = "";
        }
    }
}

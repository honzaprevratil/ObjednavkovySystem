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
            TabCart.Visibility = Visibility.Hidden;
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

        private void RefreshCart()
        {
            // bind cart items
            cartItems.ItemsSource = null;
            cartItems.ItemsSource = ApiProvider.ItemsInCart;

            int totalPrice = 0;
            ApiProvider.ItemsInCart.ForEach(x => totalPrice = totalPrice + x.amount * x.price);

            TotalPrice.Content = totalPrice.ToString() + " CZK";
        }

        private void Button_AddItemToCart(object sender, RoutedEventArgs e)
        {
            if (itemsListview.SelectedItem != null)
            {
                if (ApiProvider.ItemsInCart.Where(x => x.id == (itemsListview.SelectedItem as Item).id).ToArray().Length == 1)
                {
                    foreach (Item item in ApiProvider.ItemsInCart)
                    {
                        if (item.id == (itemsListview.SelectedItem as Item).id)
                            item.amount = item.amount + 1;
                    }
                }
                else
                {
                    Item item = (Item)itemsListview.SelectedItem;
                    item.amount = 1;
                    ApiProvider.ItemsInCart.Add(item);
                }

                messageLabel2.Content = "Added to cart.";
                messageLabel2.BorderBrush = Brushes.Blue;
            }
            else
            {
                messageLabel2.Content = "Select item to add to cart.";
                messageLabel2.BorderBrush = Brushes.Red;
            }
            RefreshCart();
        }

        private void Button_RemoveItemFromCart(object sender, RoutedEventArgs e)
        {
            if (cartItems.SelectedItem != null)
            {
                ApiProvider.ItemsInCart.Remove((cartItems.SelectedItem as Item));
                messageLabel2.Content = "Removed from cart.";
                messageLabel2.BorderBrush = Brushes.Blue;
            }
            else
            {
                messageLabel2.Content = "Select item to remove it from cart.";
                messageLabel2.BorderBrush = Brushes.Red;
            }
            RefreshCart();
        }

        private void Button_Remove1(object sender, RoutedEventArgs e)
        {
            if (cartItems.SelectedItem != null)
            {
                if ((cartItems.SelectedItem as Item).amount == 1)
                    ApiProvider.ItemsInCart.Remove((cartItems.SelectedItem as Item));
                else
                    (cartItems.SelectedItem as Item).amount = (cartItems.SelectedItem as Item).amount - 1;
                messageLabel2.Content = "Removed 1 from cart.";
                messageLabel2.BorderBrush = Brushes.Blue;
            }
            else
            {
                messageLabel2.Content = "Select item to remove 1 it from cart.";
                messageLabel2.BorderBrush = Brushes.Red;
            }
            RefreshCart();
        }

        private void Button_Add1(object sender, RoutedEventArgs e)
        {
            if (cartItems.SelectedItem != null)
            {
                (cartItems.SelectedItem as Item).amount = (cartItems.SelectedItem as Item).amount + 1;
                messageLabel2.Content = "Added 1 to cart.";
                messageLabel2.BorderBrush = Brushes.Blue;
            }
            else
            {
                messageLabel2.Content = "Select item to add 1 to cart.";
                messageLabel2.BorderBrush = Brushes.Red;
            }
            RefreshCart();
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
                    TabCart.Visibility = Visibility.Visible;
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
            if (!string.IsNullOrEmpty(itemName.Text) && !string.IsNullOrEmpty(itemDescription.Text) && !string.IsNullOrEmpty(itemPrice.Text) && int.TryParse(itemPrice.Text, out int priceInt))
            {
                if (loadedItem != null)
                {
                    loadedItem.name = itemName.Text;
                    loadedItem.description = itemDescription.Text;
                    loadedItem.price = priceInt;

                    await ApiProvider.PostData("updateItem", loadedItem);

                    // clear loaded item
                    loadedItem = null;
                }
                else
                {
                    await ApiProvider.PostData("addItem", new Item()
                    {
                        name = itemName.Text,
                        price = priceInt,
                        description = itemDescription.Text
                    });
                }

                // clear inputs
                itemName.Text = "";
                itemPrice.Text = "";
                itemDescription.Text = "";

                // refresh list
                itemsListview.ItemsSource = await ApiProvider.GetData<Item>();
                labelDescription.Text = "";
            }
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

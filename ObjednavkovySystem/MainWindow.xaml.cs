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
            TabProfile.Visibility = Visibility.Hidden;
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

                    newnameInput.Text = ApiProvider.LoggedUser.name;
                    newsurnameInput.Text = ApiProvider.LoggedUser.surname;


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
                    TabProfile.Visibility = Visibility.Visible;
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

        private void Button_Register(object sender, RoutedEventArgs e)
        {
            TryToRegister();
        }

        private async void TryToRegister()
        {
            if (regnickInput.Text != "" && regpassInput.Password != "" && regpassInput2.Password != "" && regnameInput.Text != "" && regsurnameInput.Text != "")
            {
                if (regpassInput.Password == regpassInput2.Password)
                {

                    User postUser = new User()
                    {
                        nick = regnickInput.Text,
                        password = Hash.sha256_hash(regpassInput.Password),
                        name = regnameInput.Text,
                        surname = regsurnameInput.Text
                    };

                    if (await ApiProvider.PostData("register", postUser))
                    {
                        messageLabel.Content = "Registered in succesfully!";
                        messageLabel.BorderBrush = Brushes.Blue;
                        loggedUserLabel.Content = ApiProvider.LoggedUser.name + " " + ApiProvider.LoggedUser.surname;

                        newnameInput.Text = ApiProvider.LoggedUser.name;
                        newsurnameInput.Text = ApiProvider.LoggedUser.surname;

                        // navigate to next page
                        MineTabControl.SelectedIndex = 1;

                        // get items
                        itemsListview.ItemsSource = await ApiProvider.GetData<Item>();

                        // set visiblity
                        logOutButton.Visibility = Visibility.Visible;
                        TabOrders.Visibility = Visibility.Visible;
                        TabItems.Visibility = Visibility.Visible;
                        TabCart.Visibility = Visibility.Visible;
                        TabProfile.Visibility = Visibility.Visible;

                        // clear data
                        regnickInput.Text = "";
                        regpassInput.Password = "";
                        regpassInput2.Password = "";
                        regnameInput.Text = "";
                        regsurnameInput.Text = "";
                    }
                    else
                    {
                        loggedUserLabel.Content = "";
                        messageLabel.Content = "This nick is used";
                        messageLabel.BorderBrush = Brushes.Red;
                        ordersListview.ItemsSource = new List<Order>();
                        itemsListview.ItemsSource = new List<Item>();
                        regnickInput.Text = "";
                    }
                }
                else
                {
                    messageLabel.Content = "Passwords are not equal";
                    regpassInput.Password = "";
                    regpassInput2.Password = "";
                    messageLabel.BorderBrush = Brushes.Red;
                }
            }
            else
            {
                messageLabel.Content = "Fill the whole form!";
                messageLabel.BorderBrush = Brushes.Red;
            }
        }

        private async void Button_UpdateData(object sender, RoutedEventArgs e)
        {
            if (newnameInput.Text != "" && newsurnameInput.Text != "" && newpassInputConf.Password != "" )
            {
                if (Hash.sha256_hash(newpassInputConf.Password) == ApiProvider.LoggedUser.password)
                {
                    User postUser = new User()
                    {
                        id = ApiProvider.LoggedUser.id,
                        password = ApiProvider.LoggedUser.password,
                        nick = ApiProvider.LoggedUser.nick,
                        name = newnameInput.Text,
                        surname = newsurnameInput.Text
                    };

                    if (await ApiProvider.PostData("updateData", postUser))
                    {
                        messageLabe5.Content = "Data updated succesfully!";
                        messageLabe5.BorderBrush = Brushes.Blue;
                        loggedUserLabel.Content = ApiProvider.LoggedUser.name + " " + ApiProvider.LoggedUser.surname;

                        newnameInput.Text = ApiProvider.LoggedUser.name;
                        newsurnameInput.Text = ApiProvider.LoggedUser.surname;

                        // clear data
                        newpassInputConf.Password = "";

                        newpassInput.Password = "";
                        newpassInput2.Password = "";
                        newpassInputConf2.Password = "";
                    }
                    else
                    {
                        messageLabe5.Content = "Error...";
                        messageLabe5.BorderBrush = Brushes.Red;
                    }
                }
                else
                {
                    messageLabe5.Content = "Wrong password!";
                    messageLabe5.BorderBrush = Brushes.Red;
                    newpassInputConf.Password = "";
                }
            }
            else
            {
                messageLabe5.Content = "Fill the whole form!";
                messageLabe5.BorderBrush = Brushes.Red;
            }
        }

        private async void Button_UpdatePassword(object sender, RoutedEventArgs e)
        {
            if (newpassInput.Password != "" && newpassInput2.Password != "" && newpassInputConf2.Password != "")
            {
                if (Hash.sha256_hash(newpassInputConf2.Password) == ApiProvider.LoggedUser.password)
                {
                    if (newpassInput.Password == newpassInput2.Password)
                    {
                        User postUser = new User()
                        {
                            id = ApiProvider.LoggedUser.id,
                            password = ApiProvider.LoggedUser.password,
                            nick = ApiProvider.LoggedUser.nick
                        };

                        if (await ApiProvider.PostData("updatePassword", postUser, Hash.sha256_hash(newpassInput.Password)))
                        {
                            messageLabe5.Content = "Password updated succesfully!";
                            messageLabe5.BorderBrush = Brushes.Blue;

                            // clear data
                            newpassInput.Password = "";
                            newpassInput2.Password = "";
                            newpassInputConf2.Password = "";
                        }
                        else
                        {
                            messageLabe5.Content = "Error...";
                            messageLabe5.BorderBrush = Brushes.Red;
                        }
                    }
                    else
                    {
                        messageLabe5.Content = "Passwords are not equal";
                        newpassInput.Password = "";
                        newpassInput2.Password = "";
                        messageLabe5.BorderBrush = Brushes.Red;
                    }
                }
                else
                {
                    messageLabe5.Content = "Wrong password!";
                    messageLabe5.BorderBrush = Brushes.Red;
                    newpassInputConf2.Password = "";
                }
            }
            else
            {
                messageLabe5.Content = "Fill the whole form!";
                messageLabe5.BorderBrush = Brushes.Red;
            }

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
            TabCart.Visibility = Visibility.Hidden;
            TabProfile.Visibility = Visibility.Hidden;
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
                messageLabel3.Content = "Removed from cart.";
                messageLabel3.BorderBrush = Brushes.Blue;
            }
            else
            {
                messageLabel3.Content = "Select item to remove it from cart.";
                messageLabel3.BorderBrush = Brushes.Red;
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
                messageLabel3.Content = "Removed 1 from cart.";
                messageLabel3.BorderBrush = Brushes.Blue;
            }
            else
            {
                messageLabel3.Content = "Select item to remove 1 it from cart.";
                messageLabel3.BorderBrush = Brushes.Red;
            }
            RefreshCart();
        }

        private void Button_Add1(object sender, RoutedEventArgs e)
        {
            if (cartItems.SelectedItem != null)
            {
                (cartItems.SelectedItem as Item).amount = (cartItems.SelectedItem as Item).amount + 1;
                messageLabel3.Content = "Added 1 to cart.";
                messageLabel3.BorderBrush = Brushes.Blue;
            }
            else
            {
                messageLabel3.Content = "Select item to add 1 to cart.";
                messageLabel3.BorderBrush = Brushes.Red;
            }
            RefreshCart();
        }

        private async void Button_PlaceOrder(object sender, RoutedEventArgs e)
        {
            if (ApiProvider.ItemsInCart.Count > 0)
            {
                Order order = new Order() { items = ApiProvider.ItemsInCart.ToList() };
                await ApiProvider.PostData("addOrder", order);
                ordersListview.ItemsSource = await ApiProvider.GetData<Order>();
                messageLabel3.Content = "Order placed.";
                messageLabel3.BorderBrush = Brushes.Blue;

                ApiProvider.ItemsInCart = new List<Item>();
                RefreshCart();
            }
            else
            {
                messageLabel3.Content = "Add at least 1 item to cart.";
                messageLabel3.BorderBrush = Brushes.Red;
            }
        }

        private async void Button_DeleteOrder(object sender, RoutedEventArgs e)
        { 
            if (ordersListview.SelectedItem != null)
            {
                await ApiProvider.PostData("hideOrder", (Order)ordersListview.SelectedItem);
                ordersListview.ItemsSource = await ApiProvider.GetData<Order>();
                messageLabel4.Content = "Order hidden.";
                messageLabel4.BorderBrush = Brushes.Blue;
                TotalPrice2.Content = "0 CZK";
                itemsInOrder.ItemsSource = null;
            }
            else
            {
                messageLabel4.Content = "Select order to hide.";
                messageLabel4.BorderBrush = Brushes.Red;
            }
        }

        private void ItemsListview_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ListView).SelectedItem != null )
                labelDescription.Text = ((sender as ListView).SelectedItem as Item).description;
        }
        private void ordersListview_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ListView).SelectedItem != null)
            {
                itemsInOrder.ItemsSource = ((sender as ListView).SelectedItem as Order).items;
                labelDescription2.Text = "Select item to see it's description...";

                int totalPrice = 0;
                ((sender as ListView).SelectedItem as Order).items.ForEach(x => totalPrice = totalPrice + x.amount * x.price);
                TotalPrice2.Content = totalPrice.ToString() + " CZK";
            }
        }

        private void itemsInOrder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ListView).SelectedItem != null)
                labelDescription2.Text = ((sender as ListView).SelectedItem as Item).description;
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
                    messageLabel2.Content = "Item saved.";
                    messageLabel2.BorderBrush = Brushes.Blue;
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
            else
            {
                messageLabel2.Content = "Fill the form to add new item.";
                messageLabel2.BorderBrush = Brushes.Red;
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
            else
            {
                messageLabel2.Content = "Select item to edit it.";
                messageLabel2.BorderBrush = Brushes.Red;
            }
        }

        private void Button_CancelItem(object sender, RoutedEventArgs e)
        {
            itemName.Text = "";
            itemPrice.Text = "";
            itemDescription.Text = "";
            if (loadedItem != null)
            {
                messageLabel2.Content = "Edit canceled.";
                messageLabel2.BorderBrush = Brushes.Blue;
            }
            else
            {
                messageLabel2.Content = "First try to edit item.";
                messageLabel2.BorderBrush = Brushes.Red;
            }
            loadedItem = null;
        }

        private async void Button_DeleteItem(object sender, RoutedEventArgs e)
        {
            if (itemsListview.SelectedItem != null)
            {
                await ApiProvider.PostData("deleteItem", (itemsListview.SelectedItem as Item));
                // refresh list
                itemsListview.ItemsSource = await ApiProvider.GetData<Item>();
                labelDescription.Text = "";
            }
            else
            {
                messageLabel2.Content = "Select item to delete it.";
                messageLabel2.BorderBrush = Brushes.Red;
            }
        }
    }
}

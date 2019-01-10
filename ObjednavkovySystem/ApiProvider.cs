using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
namespace ObjednavkovySystem
{
    class ApiProvider
    {
        HttpClient client = new HttpClient();
        string rootPath = @"https://student.sps-prosek.cz/~prevrja15/ObjednavkovySystem/API.php";

        public User LoggedUser { get; set; } = new User();

        public List<Order> Orders { get; set; } = new List<Order>();
        public List<Item> Items { get; set; } = new List<Item>();
        public List<Item> ItemsInCart { get; set; } = new List<Item>();

        public async void GetUsers()
        {
            var response = await client.GetAsync(rootPath);

            string json = await response.Content.ReadAsStringAsync();
            List<User> userList = JsonConvert.DeserializeObject<List<User>>(json);
            string name = userList[0].name;
        }

        public async Task<bool> PostData<T>(string action, T data) where T : DBitem
        {
            var request = new HttpRequestMessage(HttpMethod.Post, rootPath);
            var keyValues = new List<KeyValuePair<string, string>>();

            // set action
            keyValues.Add(new KeyValuePair<string, string>("action", action));

            // pass action parameters
            if (typeof(T) == typeof(User))
            {
                keyValues.Add(new KeyValuePair<string, string>("nick", (data as User).nick));
                keyValues.Add(new KeyValuePair<string, string>("password", (data as User).password));
            }
            else if (typeof(T) == typeof(Item))
            {
                if (action == "updateItem")
                {
                    keyValues.Add(new KeyValuePair<string, string>("id", (data as Item).id.ToString()));
                    keyValues.Add(new KeyValuePair<string, string>("name", (data as Item).name));
                    keyValues.Add(new KeyValuePair<string, string>("description", (data as Item).description));
                    keyValues.Add(new KeyValuePair<string, string>("price", (data as Item).price.ToString()));
                }
                else if (action == "addItem")
                {
                    keyValues.Add(new KeyValuePair<string, string>("name", (data as Item).name));
                    keyValues.Add(new KeyValuePair<string, string>("description", (data as Item).description));
                    keyValues.Add(new KeyValuePair<string, string>("price", (data as Item).price.ToString()));
                }
                else if (action == "deleteItem")
                {
                    keyValues.Add(new KeyValuePair<string, string>("id", (data as Item).id.ToString()));
                }
                else
                {
                    keyValues.Add(new KeyValuePair<string, string>("idItem", (data as Item).id.ToString()));
                    keyValues.Add(new KeyValuePair<string, string>("idUser", LoggedUser.id.ToString()));
                }
            }
            else if (typeof(T) == typeof(Order))
            {
                keyValues.Add(new KeyValuePair<string, string>("idOrder", (data as Order).id.ToString()));
            }

            // send task, load response
            request.Content = new FormUrlEncodedContent(keyValues);
            var response = await client.SendAsync(request);
            string json = await response.Content.ReadAsStringAsync();

            if (typeof(T) == typeof(User))
            {
                try
                {
                    LoggedUser = JsonConvert.DeserializeObject<User>(json);
                }
                catch
                {
                    LoggedUser = new User();
                    LoggedUser.name = "";
                }

                if (LoggedUser.name == "")
                    return false;
            }
            return true;
        }

        public async Task<List<T>> GetData<T>() where T : DBitem
        {
            HttpResponseMessage response;
            string json;
            List<T> returnList = new List<T>();

            if (typeof(T) == typeof(Order))
            {
                response = await client.GetAsync(rootPath + "?parentId=" + LoggedUser.id.ToString() + "&table=orders");
                json = await response.Content.ReadAsStringAsync();
                Orders = JsonConvert.DeserializeObject<List<Order>>(json);

                foreach (Order order in Orders)
                {
                    if (order.visible == 1)
                        returnList.Add((order as T));
                }
            }
            else if (typeof(T) == typeof(Item))
            {
                response = await client.GetAsync(rootPath + "?table=items");
                json = await response.Content.ReadAsStringAsync();
                Items = JsonConvert.DeserializeObject<List<Item>>(json);

                foreach (Item item in Items)
                {
                    returnList.Add((item as T));
                }
            }

            return returnList;
        }
    }
}

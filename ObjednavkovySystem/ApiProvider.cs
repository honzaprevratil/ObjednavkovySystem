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

        public async void GetUsers()
        {
            var response = await client.GetAsync(rootPath);

            string json = await response.Content.ReadAsStringAsync();
            List<User> userList = JsonConvert.DeserializeObject<List<User>>(json);
            string name = userList[0].name;
        }

        public async void Login(string nick, string password)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, rootPath);
            var keyValues = new List<KeyValuePair<string, string>>();

            // set action to login
            keyValues.Add(new KeyValuePair<string, string>("action", "login"));

            // pass login parameters
            string hashedPass = Hash.sha256_hash(password);
            keyValues.Add(new KeyValuePair<string, string>("nick", nick));
            keyValues.Add(new KeyValuePair<string, string>("password", hashedPass));

            request.Content = new FormUrlEncodedContent(keyValues);

            var response = await client.SendAsync(request);

            string json = await response.Content.ReadAsStringAsync();
            LoggedUser = JsonConvert.DeserializeObject<User>(json);
            string name = LoggedUser.name;


            GetData("orders");
        }

        public async void GetData(string table)
        {
            string json;

            switch (table)
            {
                case "orders":
                    var response1 = await client.GetAsync(rootPath + "?parentId=" + LoggedUser.id.ToString() + "&table=" + table);
                    json = await response1.Content.ReadAsStringAsync();
                    List<Order> orders = JsonConvert.DeserializeObject<List<Order>>(json);
                    string y = orders[0].ToString();
                    break;
            }
        }
    }
}

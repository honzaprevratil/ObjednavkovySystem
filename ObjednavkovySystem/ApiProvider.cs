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

        public async void GetData()
        {
            var response = await client.GetAsync("https://student.sps-prosek.cz/~prevrja15/ObjednavkovySystem/API.php?get=1");

            string json = await response.Content.ReadAsStringAsync();
            List<User> userList = JsonConvert.DeserializeObject<List<User>>(json);
            string name = userList[0].name;
        }

        public async void Login(string nick, string password)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://student.sps-prosek.cz/~prevrja15/ObjednavkovySystem/API.php");
            var keyValues = new List<KeyValuePair<string, string>>();

            // set action to login
            keyValues.Add(new KeyValuePair<string, string>("action", "login"));

            // pass login parameters
            string hashedPass = Hash.sha256_hash("password");
            keyValues.Add(new KeyValuePair<string, string>("nick", nick));
            keyValues.Add(new KeyValuePair<string, string>("password", hashedPass));

            request.Content = new FormUrlEncodedContent(keyValues);

            var response = await client.SendAsync(request);

            string json = await response.Content.ReadAsStringAsync();
            User user = JsonConvert.DeserializeObject<User>(json);
            string name = user.name;
        }
    }
}

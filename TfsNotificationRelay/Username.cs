using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace DevCore.TfsNotificationRelay
{
    public class Username
    {

        public static string getSlackName(string TfsUsername) {
            string jsonString = new WebClient().DownloadString("https://slack.com/api/users.list?token=xoxp-4780571987-5169200945-6585715173-2dc98e");
            JObject json = JObject.Parse(jsonString);

            JObject[] members = json.GetValue("members").ToObject<JObject[]>();
            foreach(JObject member in members) {
                JObject profile = member.GetValue("profile").ToObject<JObject>();
                JToken title;
                // check that title exists before proceeding
                if (profile.TryGetValue("title", out title))
                {
                    string strtitle = title.ToString();
                    string[] words = strtitle.Split(new Char[] { ' ' });

                    foreach (string word in words)
                    {
                        if (word.Length > 1)
                        {
                            if (word[0] == '@' && word.Substring(1) == TfsUsername)
                            {
                                Console.WriteLine(member.GetValue("name").ToString());
                                return member.GetValue("name").ToString();
                            }
                        }

                    }
                }

            }

            return TfsUsername;


        }
        

        
    }
}

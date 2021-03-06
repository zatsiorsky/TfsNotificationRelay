﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json.Linq;

namespace DevCore.TfsNotificationRelay
{
    public class UserMap
    {
        private static string SlackAPIToken = "xoxp-4780571987-5169200945-6585715173-2dc98e";

        // Takes a TFS username, and returns the corresponding Slack username if it exists, 
        // else return the TFS username
        public static string TfsToSlack(string TfsUsername)
        {
            string jsonString = new WebClient().DownloadString("https://slack.com/api/users.list?token=" + SlackAPIToken);
            JObject json = JObject.Parse(jsonString);

            JObject[] members = json.GetValue("members").ToObject<JObject[]>();
            foreach (JObject member in members)
            {
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
                                Console.WriteLine(member.GetValue("id").ToString());
                                return "<@" + member.GetValue("id").ToString() +">";
                            }
                        }
                    }
                }
            }

            return TfsUsername;
        }
    }
}

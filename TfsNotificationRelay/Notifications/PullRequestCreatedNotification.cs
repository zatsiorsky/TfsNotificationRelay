/*
 * TfsNotificationRelay - http://github.com/kria/TfsNotificationRelay
 * 
 * Copyright (C) 2014 Kristian Adrup
 * 
 * This file is part of TfsNotificationRelay.
 * 
 * TfsNotificationRelay is free software: you can redistribute it and/or 
 * modify it under the terms of the GNU General Public License as published 
 * by the Free Software Foundation, either version 3 of the License, or 
 * (at your option) any later version. See included file COPYING for details.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.Git.Server;

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;

using Newtonsoft.Json.Linq;
using System.Net;

namespace DevCore.TfsNotificationRelay.Notifications
{
    public class PullRequestCreatedNotification : BaseNotification
    {
        protected readonly static Configuration.SettingsElement settings = Configuration.TfsNotificationRelaySection.Instance.Settings;

        public string UniqueName { get; set; }
        public string DisplayName { get; set; }
        public string ProjectName { get; set; }
        public string RepoUri { get; set; }
        public string RepoName { get; set; }
        public int PrId { get; set; }
        public string PrUrl { get; set; }
        public string PrTitle { get; set; }

        public string UserName
        {
            get {
                return settings.StripUserDomain ? UserMap.TfsToSlack(Utils.StripDomain(UniqueName)) : UserMap.TfsToSlack(UniqueName); 
            }
        }

        public string RepoId
        {
            get
            {
                // get the url 
                string[] repoSplit = RepoUri.Split(new char[] { '/' });
                string account = repoSplit[2];
                string collection = repoSplit[4];

                string newUri = "http://" + account + "/tfs/" + collection +
                 "/_apis/git/repositories?api-version=1.0";

                WebClient myClient = new WebClient();
                myClient.Credentials = CredentialCache.DefaultCredentials;


                string jsonString = myClient.DownloadString(newUri);
                JObject json = JObject.Parse(jsonString);
                JObject[] repos = json.GetValue("value").ToObject<JObject[]>();
                foreach (JObject repo in repos)
                {
                    if (repo.GetValue("name").ToString() == RepoName)
                    {
                        return repo.GetValue("id").ToString();
                    }
                }
                return "";
            }
        }

        // gets the first reviewer
        public string Reviewer
        {
            get
            {
                string[] repoSplit = RepoUri.Split(new char[] { '/' });
                string account = repoSplit[2];
                string collection = repoSplit[4];

                string url = "http://" + account + "/tfs/" + collection +
                 "/_apis/git/repositories/" + RepoId + "/pullrequests/" + PrId + "?api-version=1.0";
                WebClient myClient = new WebClient();
                myClient.Credentials = CredentialCache.DefaultCredentials;

                string jsonString = myClient.DownloadString(url);
                JObject json = JObject.Parse(jsonString);
               
                JToken reviewers;
                if (json.TryGetValue("reviewers", out reviewers)) {
                    JObject[] reviewersArr = reviewers.ToObject<JObject[]>();
                    int len = reviewersArr.Length;
                    string[] uniquenames = new string[len];
                    for (int i = 0; i < len; i++)
                    {
                        string uniquename = reviewersArr[i].GetValue("uniqueName").ToString();
                        uniquenames[i] = settings.StripUserDomain ? UserMap.TfsToSlack(Utils.StripDomain(uniquename)) : UserMap.TfsToSlack(uniquename); 
                    }

                    if (len == 1)
                    {
                        return uniquenames[0];
                    }
                    else if (len == 2)
                    {
                        return uniquenames[0] + " and " + uniquenames[1];
                    }
                    else
                    {
                        string result = string.Join(", ", uniquenames, 0, len-1);
                        return result + ", and " + uniquenames[len - 1];
                    }



                    
               }

                // only get here if there are no reviewers
                return "no one";
            }
        } 

        public override IList<string> ToMessage(Configuration.BotElement bot, Func<string, string> transform)
        {
            var formatter = new
            {
                TeamProjectCollection = transform(this.TeamProjectCollection),
                DisplayName = transform(this.DisplayName),
                ProjectName = transform(this.ProjectName),
                RepoUri = this.RepoUri,
                RepoName = transform(this.RepoName),
                PrId = this.PrId,
                PrUrl = this.PrUrl,
                PrTitle = transform(this.PrTitle),
                UserName = transform(this.UserName),
                Reviewer = transform(this.Reviewer)
            };

            return new[] { bot.Text.PullRequestCreatedFormat.FormatWith(formatter) };
        }

        public override bool IsMatch(string collection, Configuration.EventRuleCollection eventRules)
        {
            var rule = eventRules.FirstOrDefault(r => r.Events.HasFlag(TfsEvents.PullRequestCreated)
                && collection.IsMatchOrNoPattern(r.TeamProjectCollection)
                && ProjectName.IsMatchOrNoPattern(r.TeamProject)
                && RepoName.IsMatchOrNoPattern(r.GitRepository));

            if (rule != null) return rule.Notify;

            return false;
        }
    }
}

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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.TeamFoundation.Client;

namespace DevCore.TfsNotificationRelay.Notifications
{
    public class WorkItemChangedNotification : BaseNotification
    {
        protected static Configuration.SettingsElement settings = Configuration.TfsNotificationRelaySection.Instance.Settings;

        public bool IsNew { get; set; }
        public string UniqueName { get; set; }
        public string DisplayName { get; set; }
        public string WiUrl { get; set; }
        public string WiType { get; set; }
        public int WiId { get; set; }
        public string WiTitle { get; set; }
        public string ProjectName { get; set; }
        public bool IsStateChanged { get; set; }
        public bool IsAssignmentChanged { get; set; }
        public string AssignedTo { get; set; }
        public string State { get; set; }
        public string Reason { get; set; }

        
        public string History
        {
            get
            {
                var wi = GetWorkItemDetails(WiId);
                string history = "\n";

                // loop through all revisions, if necessary
                for (int i = 0, len = wi.Revisions.Count; i < len; i++) {
                    // found the latest message
                    if (wi.Revisions[len - 1 - i]["History"].ToString() != "")
                    {
                        history += wi.Revisions[len - 1 - i]["History"].ToString();
                        break;
                    }

                    // have we reached the end of the revisions?
                    if (i == len - 1)
                    {
                        history = "None";
                    }
                    
                }

                return history;
            }
        }

        
  
        public string UserName
        {
            get { return settings.StripUserDomain ? Utils.StripDomain(UniqueName) : UniqueName; }
        }

        private string FormatAction(Configuration.BotElement bot)
        {
            return IsNew ? bot.Text.Created : bot.Text.Updated;
        }

        public override IList<string> ToMessage(Configuration.BotElement bot, Func<string, string> transform)
        {
            var lines = new List<string>();
            var formatter = new
            {
                TeamProjectCollection = transform(this.TeamProjectCollection),
                DisplayName = transform(this.DisplayName),
                ProjectName = transform(this.ProjectName),
                WiUrl = this.WiUrl,
                WiType = transform(this.WiType),
                WiId = this.WiId,
                WiTitle = transform(this.WiTitle),
                IsStateChanged = this.IsStateChanged,
                IsAssignmentChanged = this.IsAssignmentChanged,
                AssignedTo = transform(UserMap.TfsToSlack(this.AssignedTo)),
                State = transform(this.State),
                UserName = transform(UserMap.TfsToSlack(this.UserName)),
                Action = FormatAction(bot),
                Reason = transform(this.Reason),
                History = transform(this.History)
            };
            lines.Add(bot.Text.WorkItemchangedFormat.FormatWith(formatter));
            lines.Add(String.Format("State: {0}", State));
            if (AssignedTo != "")
            {
                lines.Add(String.Format("AssignedTo: {0} ", AssignedTo));
            }
            if (History != "None")
            {
                lines.Add(String.Format("HistoryMessage: {0}", History));
            }
            
            lines.Add(String.Format("Reason: {0}", Reason));

            return lines;
        }

        public override bool IsMatch(string collection, Configuration.EventRuleCollection eventRules)
        {
            var rule = eventRules.FirstOrDefault(r =>
                (r.Events.HasFlag(TfsEvents.WorkItemStateChange) && IsStateChanged
                || r.Events.HasFlag(TfsEvents.WorkItemAssignmentChange) && IsAssignmentChanged)
                && collection.IsMatchOrNoPattern(r.TeamProjectCollection)
                && ProjectName.IsMatchOrNoPattern(r.TeamProject)
                && !(r.ExcludeType.Contains(WiType)) // check if excluding type
                && !(IsNew && r.ExcludeNew) // check if excluding new
                && !(r.ExcludeState.Contains(State)) // check if excluding state
                );
            


            if (rule != null) return rule.Notify;

            return false;
        }

        
        public WorkItem GetWorkItemDetails(int id)
        {

            TfsTeamProjectCollection tfs = new TfsTeamProjectCollection(
                new Uri(@"http://bil-qa-tfs2013:8080/tfs/TFSCollection01"), true);

            var service = tfs.GetService<WorkItemStore>();

            return service.GetWorkItem(id);
        }
        
    }
}

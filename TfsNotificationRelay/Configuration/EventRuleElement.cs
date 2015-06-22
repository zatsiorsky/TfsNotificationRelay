﻿/*
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
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevCore.TfsNotificationRelay.Configuration
{
    public class EventRuleElement : ConfigurationElement, IKeyedConfigurationElement
    {
        public object Key { get { return this; } }
        
        [ConfigurationProperty("events", IsRequired = true)]
        public TfsEvents Events
        {
            get { return (TfsEvents)this["events"]; }
        }

        [ConfigurationProperty("notify")]
        public bool Notify
        {
            get { return (bool)this["notify"]; }
        }

        [ConfigurationProperty("excludeType")]
        public string ExcludeType
        {
            get
            {
                return (string)this["excludeType"];
            }
        }

        [ConfigurationProperty("excludeNew", DefaultValue = false)]
        public bool ExcludeNew
        {
            get
            {
                return (bool)this["excludeNew"];
            }
        }

        [ConfigurationProperty("teamProjectCollection")]
        public string TeamProjectCollection
        {
            get { return (string)this["teamProjectCollection"]; }
        }

        [ConfigurationProperty("teamProject")]
        public string TeamProject
        {
            get { return (string)this["teamProject"]; }
        }

        [ConfigurationProperty("gitRepository")]
        public string GitRepository
        {
            get { return (string)this["gitRepository"]; }
        }

        [ConfigurationProperty("buildDefinition")]
        public string BuildDefinition
        {
            get { return (string)this["buildDefinition"]; }
        }
    }
}

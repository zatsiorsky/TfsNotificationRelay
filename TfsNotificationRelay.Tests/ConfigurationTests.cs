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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;
using DevCore.TfsNotificationRelay.Configuration;
using DevCore.TfsNotificationRelay;

namespace TfsNotificationRelay.Tests
{
    [TestClass]
    public class ConfigurationTests
    {
        [TestMethod]
        public void Load_DefaultConfiguration_ShouldSucceed()
        {
            var config = ConfigurationManager.GetSection("tfsNotificationRelay") as TfsNotificationRelaySection;
            
            Assert.IsNotNull(config, "Unable to load section");
            Assert.IsTrue(config.Settings.MaxLines > 0, "Too low MaxLines");
            Assert.IsTrue(config.Bots[0].EventRules.Count > 0, "no rules");
            Assert.IsTrue(config.Bots.Count > 0, "No bots");
            Assert.IsTrue(config.Texts.Count > 0, "No texts");
        }
    }

    [TestClass]
    public class ExtensionTests
    {
        [TestMethod]
        public void TestTfsToSlack()
        {
            string realname = "James.Zatsiorsky";
            Assert.AreEqual(UserMap.TfsToSlack(realname), "@james.z");

            string fakename = "Not.A.Real.User";
            Assert.AreEqual(UserMap.TfsToSlack(fakename), "Not.A.Real.User");
        }
    }
}

﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Axe.Windows.Automation;

namespace UITests
{
    [TestClass]
    public class GettingStartedPage : AIWinSession
    {
        [TestMethod]
        public void VerifyGettingStartedTitle()
        {
            Assert.AreEqual(session.Title, "Accessibility Insights for Windows - ");
        }

        [TestMethod]
        public void VerifyAccessibility()
        {
            var result = SnapshotCommand.Execute(new Dictionary<string, string>
            {
                { CommandConstStrings.TargetProcessId, processId.ToString() },
            });
            Assert.AreEqual(result.ScanResultsPassedCount, 0);
        }

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) => Setup(context);

        [ClassCleanup]
        public static void ClassCleanup() => TearDown();
    }
}

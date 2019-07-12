﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;

namespace UITests
{
    [TestClass]
    public class LiveMode : AIWinSession
    {
        readonly string TestAppPath = Path.GetFullPath("../../../../tools/WildlifeManager/WildlifeManager.exe");
        Process _wildlifeManager;

        /// <summary>
        /// The entry point for this test scenario. Every TestMethod will restart ai-win, so
        /// we want to use them sparingly.
        /// </summary>
        [TestMethod]
        [TestCategory("NoStrongName")]
        [TestCategory("UITest")]
        public void LiveModeTests()
        {
            TestLiveMode();
            TestTestResults();
        }

        private void TestTestResults()
        {
            RunTests();
            ScanAccessibility();
            ValidateTestModeTitle();
            driver.TestMode.AutomatedChecks.ValidateAutomatedChecks(12);
            driver.TestMode.AutomatedChecks.GoToAutomatedChecksElementDetails(4);
            ValidateResultsInUIATree();
        }

        private void ValidateResultsInUIATree()
        {
            driver.TestMode.ResultsInUIATree.ValidateResults(false, 2, 11);
            driver.TestMode.ResultsInUIATree.SwitchToDetailsTab();
            driver.TestMode.ResultsInUIATree.ValidateDetails("SynchronizedInputPattern", "Name, Property does not exist", 1, 10);
            driver.TestMode.ResultsInUIATree.ValidateTree("pane 'Desktop 1'", 16);
        }

        private void RunTests()
        {
            driver.LiveMode.RunTests();
            var testsComplete = WaitFor(() => !driver.Title.Contains("(Scanning)"), new TimeSpan(0, 0, 0, 0, 500), 10);

            Assert.IsTrue(testsComplete);
        }

        private void TestLiveMode()
        {
            var appOpened = WaitFor(() => _wildlifeManager.MainWindowTitle == "Wildlife Manager 2.0", new TimeSpan(0, 0, 1), 10, _wildlifeManager.Refresh);
            var appSelected = WaitFor(() => driver.LiveMode.SelectedElementText == "window 'Wildlife Manager 2.0'", new TimeSpan(0, 0, 1), 5);
            driver.LiveMode.TogglePause();

            Assert.IsTrue(appOpened);
            Assert.IsTrue(appSelected);
            VerifyLiveModeTitle();
        }

        private void VerifyLiveModeTitle()
        {
            Assert.AreEqual("Accessibility Insights for Windows - Inspect - Live", driver.Title);
        }

        private void ValidateTestModeTitle()
        {
            Assert.AreEqual("Accessibility Insights for Windows - Test - Test results", driver.Title);
        }

        private void ScanAccessibility()
        {
            var issueCount = driver.ScanAIWin(TestContext, "LiveMode");
            Assert.AreEqual(0, issueCount);
        }

        [TestInitialize]
        public void TestInitialize()
        {
            Setup();

            driver.GettingStarted.DismissTelemetry();
            driver.GettingStarted.DismissStartupPage();

            _wildlifeManager = Process.Start(TestAppPath);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _wildlifeManager?.CloseMainWindow();
            _wildlifeManager?.Kill();
            TearDown();
        }
    }
}
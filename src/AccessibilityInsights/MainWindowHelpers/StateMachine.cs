// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Actions;
using AccessibilityInsights.Enums;
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Desktop.Settings;
using AccessibilityInsights.Desktop.Telemetry;
using AccessibilityInsights.Desktop.Utility;
using AccessibilityInsights.SharedUx.Dialogs;
using AccessibilityInsights.SharedUx.Interfaces;
using AccessibilityInsights.SharedUx.Settings;
using AccessibilityInsights.SharedUx.Utilities;
using AccessibilityInsights.SharedUx.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Automation;
using AccessibilityInsights.Actions.Sarif;

namespace AccessibilityInsights
{
    /// <summary>
    /// this is partial class for MainWindow
    /// this portion will contain State machine logic
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// Current Page variable to keep track of state.
        /// - Startup
        /// - Inspect
        /// - Test
        /// </summary>
        public AppPage CurrentPage { get; set; } = AppPage.Start;

        /// <summary>
        /// Current view in current page. 
        /// e.g.
        /// Inspect
        ///  . Live
        ///  . TestDetail
        ///  . Test
        /// </summary>
        public dynamic CurrentView { get; set; } = null;


        /// <summary>
        /// Current mode control
        /// </summary>
        IModeControl ctrlCurMode = null;

        /// <summary>
        /// Handle Target Selection Change
        /// </summary>
        /// <param name="ec"></param>
        private void HandleTargetSelectionChanged()
        {
            if (this.CurrentPage == AppPage.Inspect && (InspectView)this.CurrentView == InspectView.Live)
            {
                StartInspectMode(InspectView.Live);
                UpdateTabSelection();
                UpdateTitleString();
            }
        }

        /// <summary>
        /// Handle Test request by hotkey (Shift + F8)
        /// it is due by HotKey.
        /// </summary>
        void HandleRunTestsByHotkey()
        {
            if (CurrentPage != AppPage.Start && CurrentPage != AppPage.Exit && IsInSelectingState())
            {
                HandleSnapshotRequest(TestRequestSources.HotKey);
            }
        }

        /// <summary>
        /// Handle Recording Request by Hotkey (Shift + F7)
        /// </summary>
        void HandleRequestRecordingByHotkey()
        {
            if (this.CurrentPage == AppPage.Test && (TestView)this.CurrentView == TestView.TabStop)
            {
                ctrlTestMode.ctrlTabStop.ToggleRecording();
            }
            else if (this.CurrentPage == AppPage.Events)
            {
                ctrlEventMode.ctrlEvents.ToggleRecording();
            }
            else if (this.IsInSelectingState())
            {
                var sa = SelectAction.GetDefaultInstance();

                if (sa.HasPOIElement())
                {
                    var poi = GetDataAction.GetElementContext(sa.GetSelectedElementContextId().Value);
                    this.StartEventsMode(poi.Element);
                    UpdateMainWindowUI();
                }
                else
                {
                    MessageDialog.Show(Properties.Resources.HandleRequestRecordingByHotkeySelectElementMessage);
                }
            }
        }

        /// <summary>
        /// Handle Mode Change request due to snapshot timer
        /// </summary>
        void HandleModeChangeRequestByTimer()
        {
            if (IsInSelectingState())
            {
                HandleSnapshotRequest(TestRequestSources.Timer);
            }
        }

        /// <summary>
        /// Bring app back to selecting State
        /// </summary>
        public void HandleBackToSelectingState()
        {
            // if coming from startup, restore left nav bar
            if (this.CurrentPage == AppPage.Start)
            {
                this.bdLeftNav.IsEnabled = true;
                this.gdModes.Visibility = Visibility.Visible;
                this.btnPause.Visibility = Visibility.Visible;
            }

            // clear selected element highliter
            HighlightAction.GetInstance(AccessibilityInsights.Actions.Enums.HighlighterType.Selected).Clear();
            HighlightImageAction.ClearDefaultInstance();

            // this can be disabled if previous action was loading old data format. 
            // bring it back to enabled state. 
            this.btnHilighter.IsEnabled = true;

            // since we comes back to live mode, enable highlighter by default. 
            SelectAction.GetDefaultInstance().ClearSelectedContext();

            // turn highlighter on once you get back to selection mode. 
            SetHighlightBtnState(true);
            var ha = HighlightAction.GetDefaultInstance();
            ha.HighlighterMode = ConfigurationManager.GetDefaultInstance().AppConfig.HighlighterMode;
            ha.IsEnabled = true;
            ha.SetElement(null);

            /// make sure that all Mode UIs are clean since new selection will be done. 
            CleanUpAllModeUIs();

            if (this.CurrentPage == AppPage.Start
                || (this.CurrentPage == AppPage.CCA)
                || (this.CurrentPage == AppPage.Test)
                || (this.CurrentPage == AppPage.Inspect && this.CurrentView != InspectView.Live)
                || (this.CurrentPage == AppPage.Events)
                || (this.CurrentPage == AppPage.Inspect && SelectAction.GetDefaultInstance().IsPaused))
            {
                SelectAction.GetDefaultInstance().Scope = ConfigurationManager.GetDefaultInstance().AppConfig.IsUnderElementScope ? AccessibilityInsights.Actions.Enums.SelectionScope.Element : AccessibilityInsights.Actions.Enums.SelectionScope.App;
                cbSelectionScope.SelectedIndex = (SelectAction.GetDefaultInstance().Scope == AccessibilityInsights.Actions.Enums.SelectionScope.Element) ? 0 : 1;
                StartInspectMode(InspectView.Live); // clean up data when we get back to selection mode.
                this.CurrentPage = AppPage.Inspect;
                this.CurrentView = InspectView.Live;
            }

            // garbage collection for any UI elements
            GC.Collect();

            // enable element selector
            EnableElementSelector();

            // if it was open when the switch back button is clicked. 
            HideConfigurationMode();

            UpdateMainWindowUI();
        }

        /// <summary>
        /// Make sure that Live mode UI is clean. 
        /// </summary>
        private void CleanUpAllModeUIs()
        {
            this.ctrlEventMode.ctrlEvents.StopRecordEvents();
            this.ctrlEventMode.Clear();
            this.ctrlLiveMode.Clear();
            this.ctrlTestMode.Clear();
            this.ctrlSnapMode.Clear();
        }

        /// <summary>
        /// Checking whether app is still in Selecting an element or not. 
        /// Old live mode
        /// </summary>
        /// <returns></returns>
        private bool IsInSelectingState()
        {
            return (this.CurrentPage == AppPage.Inspect && (InspectView)this.CurrentView == InspectView.Live);
        }

        /// <summary>
        /// indicate the type of TestRequestSources
        /// </summary>
        internal enum TestRequestSources
        {
            HotKey,
            Beaker,
            HierarchyNode,
            Timer,
        }

        /// <summary>
        /// When snapshot is invoked, Request appropriate mode change.
        /// </summary>
        /// <param name="method">by which way : Beaker or Hotkey</param>
        internal void HandleSnapshotRequest(TestRequestSources method)
        {
            if (this.CurrentPage == AppPage.Inspect && this.CurrentView == InspectView.Live)
            {
                if (ctrlLiveMode.SelectedInHierarchyElement != null)
                {
                    if (ctrlLiveMode.SelectedInHierarchyElement.Item2 == 0)
                    {
                        SetDataAction.ReleaseDataContext(SelectAction.GetDefaultInstance().GetSelectedElementContextId().Value);
                    }
                    else
                    {
                        var sa = SelectAction.GetDefaultInstance();
                        sa.SetCandidateElement(ctrlLiveMode.SelectedInHierarchyElement.Item1, ctrlLiveMode.SelectedInHierarchyElement.Item2);
                        sa.Select();
                    }
                }

                // Based on Ux model feedback from PM team, we decided to go to AutomatedTestResults as default page view for snapshot.
                StartTestMode(TestView.AutomatedTestResults);

                Logger.PublishTelemetryEvent(TelemetryAction.Test_Requested, new Dictionary<TelemetryProperty, string>
                {
                    { TelemetryProperty.By, method.ToString() },
                    { TelemetryProperty.Scope, SelectAction.GetDefaultInstance().Scope.ToString() }
                });
            }
            HighlightAction.GetDefaultInstance().Clear();
            UpdateMainWindowUI();
        }

        /// <summary>
        /// Handle Inspect Tab Click
        /// </summary>
        private void HandleInspectTabClick()
        {
            // make sure that configuration page is closed. 
            HideConfigurationMode();

            HandleBackToSelectingState();

            UpdateMainWindowUI();
        }

        /// <summary>
        /// Handle Test Tab Click
        /// </summary>
        private void HandleTestTabClick()
        {
            // make sure that configuration page is closed. 
            HideConfigurationMode();

            switch (this.CurrentPage)
            {
                case AppPage.Inspect:
                    switch ((InspectView)this.CurrentView)
                    {
                        case InspectView.CapturingData:
                            // no op while capturing data.
                            break;
                        default:
                            StartTestMode(TestView.NoSelection);
                            break;
                    }
                    break;
                case AppPage.Test:
                    if ((TestView)this.CurrentView == TestView.ElementHowToFix || (TestView)this.CurrentView == TestView.ElementDetails)
                    {
                        StartTestMode(TestView.AutomatedTestResults);
                    }
                    break;
                case AppPage.Events:
                    StartTestMode(TestView.NoSelection);
                    break;
                case AppPage.CCA:
                    StartTestMode(TestView.NoSelection);
                    break;
            }

            UpdateMainWindowUI();
        }

        private void HandleCCATabClick()
        {
            HideConfigurationMode();
            ctrlCurMode.HideControl();
            ctrlCurMode = ctrlCCAMode;
            ctrlCurMode.ShowControl();
            CurrentView = null;
            CurrentPage = AppPage.CCA;
            UpdateMainWindowUI();

        }

        /// <summary>
        /// Delegate type for methods to load given file
        /// </summary>
        /// <param name="path">Path to file</param>
        /// <param name="selectedElementId">ElementId to select after loading. May not be applicable.</param>
        delegate void FileLoadHandler(string path, int? selectedElementId = null);

        /// <summary>
        /// Handle Load snapshot data and Request Ux Change
        /// </summary>
        /// <param name="path"></param>
        /// <param name="selectedEelement"></param>
        internal void HandleLoadingSnapshotData(string path, int? selectedElementId = null)
        {
            HandlePauseButtonToggle(true);

            StartLoadingSnapshot(path, selectedElementId);
            UpdateMainWindowUI();
        }

        /// <summary>
        /// Handle Load snapshot data and Request Ux Change
        /// </summary>
        /// <param name="path"></param>
        /// <param name="selectedEelement">Not used</param>
        private void HandleLoadingEventData(string fileName, int? selectedElementId = null)
        {
            HandlePauseButtonToggle(true);

            StartEventsWithLoadedData(fileName);
            Logger.PublishTelemetryEvent(TelemetryAction.Event_Load);

            UpdateMainWindowUI();
        }

        /// <summary>
        /// Load a file with sarif data by identifying
        /// the first internal .a11ytest file and opening
        /// it as a snapshot
        /// </summary>
        /// <param name="path"></param>
        /// <param name="selectedElementId"></param>
        private void HandleLoadingSarifData(string path, int? selectedElementId = null)
        {
            var allyFileData = OpenSarif.ExtractA11yTestFile(path);

            string tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            try
            {
                File.WriteAllBytes(tempPath, Convert.FromBase64String(allyFileData));
                HandleLoadingSnapshotData(tempPath, selectedElementId);
            }
            catch (Exception)
            {
                File.Delete(tempPath);
            }
        }

        /// <summary>
        /// Attempts to open a file as either a test or events file
        /// </summary>
        /// <returns>true if file opened successfully; otherwise, false.</returns>
        private bool TryOpenFile(string fileName, int? selectedElementId = null)
        {
            // array of file handlers to be attempted in order. If one throws an exception, the next will be tried
            FileLoadHandler[] fileHandlers = { HandleLoadingSarifData, HandleLoadingSnapshotData, HandleLoadingEventData };

            foreach(var fileHandler in fileHandlers)
            {
                try
                {
                    fileHandler(fileName, selectedElementId);
                    return true;
                }
                catch { }
            }

            return false;
        }

        /// <summary>
        /// Select an element in Snapshot from Test Mode(Automated Check) to show "How to fix" info. 
        /// </summary>
        internal void HandleSelectElementToShowHowToFix()
        {
            StartTestMode(TestView.ElementHowToFix);

            UpdateMainWindowUI();
        }

        /// <summary>
        /// Switch to test results in UIA tree, run tests on selected element, then open file bug window
        /// </summary>
        internal void HandleFileBugLiveMode()
        {
            HandlePauseButtonToggle(true);

            if (ctrlLiveMode.SelectedInHierarchyElement != null)
            {
                if (ctrlLiveMode.SelectedInHierarchyElement.Item2 == 0)
                {
                    SetDataAction.ReleaseDataContext(SelectAction.GetDefaultInstance().GetSelectedElementContextId().Value);
                }
                else
                {
                    var sa = SelectAction.GetDefaultInstance();
                    sa.SetCandidateElement(ctrlLiveMode.SelectedInHierarchyElement.Item1, ctrlLiveMode.SelectedInHierarchyElement.Item2);
                    sa.Select();
                }
            }

            StartElementHowToFixView(() =>
            {
                Dispatcher.Invoke(() => ctrlSnapMode.ctrlHierarchy.FileBug());
            });

            UpdateMainWindowUI();
        }

        /// <summary>
        /// Bring back to Inspect StartUp
        /// </summary>
        internal void HandleBackToLiveFromEventPage()
        {
            StartInspectMode(InspectView.Live);
            HighlightAction.GetInstance(AccessibilityInsights.Actions.Enums.HighlighterType.Selected).Clear();
            HighlightAction.GetDefaultInstance().IsEnabled = ConfigurationManager.GetDefaultInstance().AppConfig.IsHighlighterOn;

            UpdateMainWindowUI();
        }

        /// <summary>
        /// Move from Live to Events mode
        /// </summary>
        /// <param name="selectedElementId"></param>
        internal void HandleLiveToEvents(A11yElement e)
        {
            StartEventsMode(e);
            UpdateMainWindowUI();
        }

        /// <summary>
        /// Handle End of app
        /// </summary>
        private void HandleExitMode()
        {
            this.CurrentPage = AppPage.Exit;
        }

        /// <summary>
        /// check whether current mode allow selection
        /// </summary>
        /// <returns></returns>
        private bool IsCurrentModeAllowingSelection()
        {
            return (this.CurrentPage == AppPage.Inspect && (InspectView)this.CurrentView == InspectView.Live);
        }

        /// <summary>
        /// Handle File open request
        /// if it was based on double clicking.  open file. 
        /// </summary>
        /// <return>true if file open was handled. otherwise, false.</return>
        private bool HandleFileAssociationOpenRequest()
        {
            try
            {
                var path = GetFilePathForOpenAction();
                if (path != null)
                {
                    return HandleFileDiskOpen(path, null);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Handle File open request from disk if it was based on double clicking,
        /// or the temp file saved after downloading the file from the client UI
        /// </summary>
        /// <return>true if file open was handled. otherwise, false.</return>
        private bool HandleFileDiskOpen(string path, int? selectedElementId = null)
        {
            if (path != null)
            {
                this.bdLeftNav.IsEnabled = true;
                this.gdModes.Visibility = Visibility.Visible;

                return TryOpenFile(path, selectedElementId);
            }

            return false;
        }

        /// <summary>
        /// Return the file path
        /// </summary>
        /// <returns></returns>
        private static string GetFilePathForOpenAction()
        {
            string path = null;

            var args = Environment.GetCommandLineArgs();

            if (args.Length > 1)
            {
                path = args[1];
            }
            return path;
        }

        /// <summary>
        /// Handle configuration changed 
        /// - apply changes as needed
        /// - Send telemetry
        /// </summary>
        /// <param name="testconfig"></param>
        internal void HandleConfigurationChanged(IReadOnlyDictionary<PropertyInfo, Object> changes)
        {
            HotkeyHandler?.Dispose();
            InitHotKeys();

            var configManager = ConfigurationManager.GetDefaultInstance();

            SelectAction.GetDefaultInstance().IntervalMouseSelector = configManager.AppConfig.MouseSelectionDelayMilliSeconds;
            this.Topmost = configManager.AppConfig.AlwaysOnTop;
            this.ctrlTestMode.ctrlTabStop.SetHotkeyText(configManager.AppConfig.HotKeyForRecord);

            HighlightAction.GetDefaultInstance().HighlighterMode = configManager.AppConfig.HighlighterMode;

            configManager.TestConfig = TestSetting.GenerateSuiteConfiguration(AccessibilityInsights.RuleSelection.SuiteConfigurationType.Default);

            InitSelectActionMode();

            configManager.AppConfig.TraceConfigurationIntoTelemetry();

            HideConfigurationMode();

            Type type = typeof(AccessibilityInsights.SharedUx.Settings.ConfigurationModel);
            if (changes != null && changes.ContainsKey(type.GetProperty("FontSize")))
            {
                SetFontSize();
            }

            UpdateMainWindowUI();
            this.btnConfig.Focus();
        }

        /// <summary>
        /// Hide(close) configuration mode
        /// </summary>
        internal void HideConfigurationMode()
        {
            if (this.gridlayerConfig.Visibility == Visibility.Visible)
            {
                this.gridlayerConfig.Visibility = Visibility.Collapsed;
                this.spBreadcrumbs.Visibility = Visibility.Visible;
                this.ctrlCurMode.ShowControl();
                this.AllowFurtherAction = true;
                EnableElementSelector();
            }
        }

        /// <summary>
        /// Start configuration mode with connection screen
        /// </summary>
        internal void HandleConnectionConfigurationStart()
        {
            HandleConfigurationModeStart(true);
        }

        /// <summary>
        /// Start Configuration mode. 
        /// - if connection is true, routes to connection configuration
        /// </summary>
        private void HandleConfigurationModeStart(bool connection)
        {
            this.AllowFurtherAction = false;
            this.ctrlCurMode.HideControl();
            DisableElementSelector();
            this.ctrlConfigurationMode.ShowControl(connection);

            gridLayerModes.Children.Remove(gridlayerConfig);
            gridLayerModes.Children.Add(gridlayerConfig);
            this.spBreadcrumbs.Visibility = Visibility.Collapsed;
            this.gridlayerConfig.Visibility = Visibility.Visible;
            this.btnPause.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        ///  check whether we are curring capturing data or not.
        /// </summary>
        /// <returns></returns>
        internal bool IsCapturingData()
        {
            return (CurrentPage == AppPage.Inspect && ((InspectView)CurrentView) == InspectView.CapturingData) ||
                (CurrentPage == AppPage.Test && ((TestView)CurrentView) == TestView.CapturingData);
        }

        /// <summary>
        /// Shows a dialog with an error message and then goes back to selecting state
        /// </summary>
        public void HandleFailedSelectionReset()
        {
            MessageDialog.Show(Properties.Resources.HandleFailedSelectionResetConnectionLostMessage);
            HandleBackToSelectingState();
        }


        /// <summary>
        /// Shows a dialog with an error message and then goes back to selecting state
        /// </summary>
        internal void HandlePauseButtonToggle(bool enabled)
        {
            if (enabled)
            {
                this.vmLiveModePauseResume.State = ButtonState.On;
                SelectAction.GetDefaultInstance().ResumeUIATreeTracker();
                AutomationProperties.SetName(btnPause, Properties.Resources.btnPauseAutomationPropertiesNameOn);
            }
            else
            {
                this.vmLiveModePauseResume.State = ButtonState.Off;
                SelectAction.GetDefaultInstance().PauseUIATreeTracker();
                AutomationProperties.SetName(btnPause, Properties.Resources.btnPauseAutomationPropertiesNameOff);
            }
            UpdateMainWindowUI();
        }
    }
}

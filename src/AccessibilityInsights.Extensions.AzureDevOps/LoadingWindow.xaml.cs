﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AccessibilityInsights.Extensions.AzureDevOps
{
    /// <summary>
    /// Interaction logic for LoadingWindow.xaml
    /// </summary>
    public partial class LoadingWindow : Window
    {
        private Func<Task> Operation { get; set; }

        public LoadingWindow(Window owner, Func<Task> operation)
        {
            InitializeComponent();
            this.Owner = owner;
            this.Operation = operation;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await Operation().ConfigureAwait(true);
                this.DialogResult = true; // crash in case dialog closed before this(?)
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                this.DialogResult = false;
            }
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
        }
    }
}

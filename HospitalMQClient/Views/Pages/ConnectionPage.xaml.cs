using HospitalMQClient.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Wpf.Ui.Abstractions.Controls;

namespace HospitalMQClient.Views.Pages
{
    public partial class ConnectionPage : INavigableView<ConnectionViewModel>
    {
        public ConnectionViewModel ViewModel { get; }

        public ConnectionPage(ConnectionViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }
    }
}

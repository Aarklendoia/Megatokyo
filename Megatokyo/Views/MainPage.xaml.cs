using System;

using Megatokyo.ViewModels;

using Windows.UI.Xaml.Controls;

namespace Megatokyo.Views
{
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; } = new MainViewModel();

        public MainPage()
        {
            InitializeComponent();
        }
    }
}

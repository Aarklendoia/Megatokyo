using System;

using Megatokyo.Client.ViewModels;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Megatokyo.Client.Views
{
    public sealed partial class RantsPage : Page
    {
        public RantsViewModel ViewModel { get; } = new RantsViewModel();

        public RantsPage()
        {
            InitializeComponent();
            Loaded += RantsPage_Loaded;
        }

        private async void RantsPage_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.LoadDataAsync(ListDetailsViewControl.ViewState);
        }
    }
}

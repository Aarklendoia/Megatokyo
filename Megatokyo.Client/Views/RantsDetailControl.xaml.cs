
using Megatokyo.Client.Core.Models;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Megatokyo.Client.Views
{
    public sealed partial class RantsDetailControl : UserControl
    {
        public Rant ListMenuItem
        {
            get { return GetValue(ListMenuItemProperty) as Rant; }
            set { SetValue(ListMenuItemProperty, value); }
        }

        public static readonly DependencyProperty ListMenuItemProperty = DependencyProperty.Register("ListMenuItem", typeof(Rant), typeof(RantsDetailControl), new PropertyMetadata(null, OnListMenuItemPropertyChanged));

        public RantsDetailControl()
        {
            InitializeComponent();
        }

        private static void OnListMenuItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as RantsDetailControl;
            control.ForegroundElement.ChangeView(0, 0, 1);
        }
    }
}

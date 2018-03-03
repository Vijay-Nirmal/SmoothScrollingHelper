using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SmoothScrollingHelper
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private ObservableCollection<int> ItemsSourceCollection = new ObservableCollection<int>();

        public MainPage()
        {
            this.InitializeComponent();

            for (int i = 0; i < 800; i++)
            {
                ItemsSourceCollection.Add(i);
            }

            var enumValue = Enum.GetValues(typeof(ItemPlacement)).Cast<ItemPlacement>();
            ItemPlacementComboBox.ItemsSource = enumValue.ToList();
            ItemPlacementComboBox.SelectedItem = ItemPlacement.Default;
        }

        private async void GoToButton_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(GoToIntex.Text, out int goToIntex);
            ItemPlacement itemPlacement = (ItemPlacement)ItemPlacementComboBox.SelectedItem;
            bool disableAnimation = DisableAnimationToggleSwitch.IsOn;
            bool scrollIfVisibile = ScrollIfVisibileToggleSwitch.IsOn;
            int.TryParse(AdditionalHorizontalOffsetTextBox.Text, out int additionalHorizontalOffset);
            int.TryParse(AdditionalHorizontalOffsetTextBox.Text, out int additionalVerticalOffset);

            await MyGridView.SmoothScrollIntoViewWithIndex(goToIntex, itemPlacement, disableAnimation, scrollIfVisibile, additionalHorizontalOffset, additionalVerticalOffset);
        }

        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            MyGridView.SmoothScrollNavigation(105, ScrollNavigationDirection.Left);
        }

        private void UpButton_Click(object sender, RoutedEventArgs e)
        {
            MyGridView.SmoothScrollNavigation(105, ScrollNavigationDirection.Up);
        }

        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            MyGridView.SmoothScrollNavigation(105, ScrollNavigationDirection.Right);
        }

        private void DownButton_Click(object sender, RoutedEventArgs e)
        {
            MyGridView.SmoothScrollNavigation(105, ScrollNavigationDirection.Down);
        }
    }
}
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SmoothScrollingHelper
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private ObservableCollection<ItemSource> ItemsSourceCollection = new ObservableCollection<ItemSource>();
        int intex = 0;

        public MainPage()
        {
            this.InitializeComponent();

            for (int i = 0; i < 800; i++)
            {
                ItemsSourceCollection.Add(new ItemSource { Name = i.ToString() });
            }

            CurrentIntex.Text = "CurrentIntex - " + intex.ToString();
        }

        private void GoToButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.Parse(GoToIntex.Text) >= 0 && int.Parse(GoToIntex.Text) < MyGridView.Items.Count)
            {
                MyGridView.SelectedIndex = int.Parse(GoToIntex.Text);
            }
        }

        private async void MyGridView_SelectionChangedAsync(object sender, SelectionChangedEventArgs e)
        {
            if (MyGridView.SelectedItem == null)
                return;

            intex = MyGridView.SelectedIndex;
            CurrentIntex.Text = "CurrentIntex - " + intex.ToString();
            await MyGridView.SmoothScrollIntoViewWithIndex(MyGridView.SelectedIndex, ItemPosition.Default);

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

    public class ItemSource
    {
        public string Name { get; set; }
    }

}
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

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            MyGridView.SelectedIndex = intex + 1;
            NextPreEnableCheck();
        }

        private void PreButton_Click(object sender, RoutedEventArgs e)
        {
            MyGridView.SelectedIndex = intex - 1;
            NextPreEnableCheck();
        }

        private void GoToButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.Parse(GoToIntex.Text) >= 0 && int.Parse(GoToIntex.Text) < MyGridView.Items.Count)
            {
                MyGridView.SelectedIndex = int.Parse(GoToIntex.Text);
                NextPreEnableCheck();
            }
        }

        private async void MyGridView_SelectionChangedAsync(object sender, SelectionChangedEventArgs e)
        {
            NextPreEnableCheck();

            if (MyGridView.SelectedItem == null)
                return;

            intex = MyGridView.SelectedIndex;
            CurrentIntex.Text = "CurrentIntex - " + intex.ToString();
            await MyGridView.SetItemPositionWithIndex(MyGridView.SelectedIndex, ItemPosition.Left);

        }

        public void NextPreEnableCheck()
        {
            if (intex <= 0 && intex >= MyGridView.Items.Count - 1)
            {
                PreButton.IsEnabled = false;
                NextButton.IsEnabled = false;
            }
            else
            {
                if (intex <= 0)
                {
                    PreButton.IsEnabled = false;
                }
                else if (intex >= MyGridView.Items.Count - 1)
                {
                    NextButton.IsEnabled = false;
                }
                else
                {
                    PreButton.IsEnabled = true;
                    NextButton.IsEnabled = true;
                }
            }
        }

    }

    public class ItemSource
    {
        public string Name { get; set; }
    }

    public static class Helpers
    {
        public static ScrollViewer GetScrollViewer(this DependencyObject element)
        {
            if (element is ScrollViewer)
            {
                return (ScrollViewer)element;
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var child = VisualTreeHelper.GetChild(element, i);

                var result = GetScrollViewer(child);
                if (result == null)
                {
                    continue;
                }
                else
                {
                    return result;
                }
            }
            return null;
        }

        public static async Task SetItemPositionWithIndex(this ListViewBase listViewBase, int index, ItemPosition itemPosition = ItemPosition.Default, bool disableAnimation = false)
        {
            bool isVirtualizing = default(bool);
            double previousXOffset = default(double), previousYOffset = default(double);

            var scrollViewer = listViewBase.GetScrollViewer();
            var selectorItem = listViewBase.ContainerFromIndex(index) as SelectorItem;

            if (selectorItem == null)
            {
                isVirtualizing = true;

                previousXOffset = scrollViewer.HorizontalOffset;
                previousYOffset = scrollViewer.VerticalOffset;

                var tcs = new TaskCompletionSource<object>();

                EventHandler<ScrollViewerViewChangedEventArgs> viewChanged = (s, e) => tcs.TrySetResult(null);
                try
                {
                    scrollViewer.ViewChanged += viewChanged;
                    listViewBase.ScrollIntoView(listViewBase.Items[index], ScrollIntoViewAlignment.Leading);
                    await tcs.Task;
                }
                finally
                {
                    scrollViewer.ViewChanged -= viewChanged;
                }

                selectorItem = (SelectorItem)listViewBase.ContainerFromIndex(index);
            }

            var transform = selectorItem.TransformToVisual((UIElement)scrollViewer.Content);
            var position = transform.TransformPoint(new Point(0, 0));

            if (isVirtualizing)
            {
                var tcs = new TaskCompletionSource<object>();

                EventHandler<ScrollViewerViewChangedEventArgs> viewChanged = (s, e) => tcs.TrySetResult(null);
                try
                {
                    scrollViewer.ViewChanged += viewChanged;
                    scrollViewer.ChangeView(previousXOffset, previousYOffset, null, true);
                    await tcs.Task;
                }
                finally
                {
                    scrollViewer.ViewChanged -= viewChanged;
                }
            }

            var listViewBaseWidth = listViewBase.ActualWidth;
            var selectorItemWidth = selectorItem.ActualWidth;
            var listViewBaseHeight = listViewBase.ActualHeight;
            var selectorItemHeight = selectorItem.ActualHeight;
            previousXOffset = scrollViewer.HorizontalOffset;
            previousYOffset = scrollViewer.VerticalOffset;

            if (itemPosition == ItemPosition.Left)
            {
                scrollViewer.ChangeView(position.X, previousYOffset, null, disableAnimation);
            }
            else if (itemPosition == ItemPosition.Top)
            {
                scrollViewer.ChangeView(previousXOffset, position.Y, null, disableAnimation);
            }
            else if (itemPosition == ItemPosition.Centre)
            {
                var CentreX = (listViewBaseWidth - selectorItemWidth) / 2.0;
                var CentreY = (listViewBaseHeight - selectorItemHeight) / 2.0;
                var finalXPosition = position.X - CentreX;
                var finalYPosition = position.Y - CentreY;
                scrollViewer.ChangeView(finalXPosition, finalYPosition, null, disableAnimation);
            }
            else if (itemPosition == ItemPosition.Right)
            {
                var finalXPosition = position.X - listViewBaseWidth + selectorItemWidth;
                scrollViewer.ChangeView(finalXPosition, previousYOffset, null, disableAnimation);
            }
            else if (itemPosition == ItemPosition.Bottom)
            {
                var finalYPosition = position.Y - listViewBaseHeight + selectorItemHeight;
                scrollViewer.ChangeView(previousXOffset, finalYPosition, null, disableAnimation);
            }
            else if (itemPosition == ItemPosition.Default)
            {
                var bottomXPosition = position.X - listViewBaseWidth + selectorItemWidth;
                var finalXPosition = position.X;

                if (previousXOffset < position.X && previousXOffset > bottomXPosition)
                    finalXPosition = previousXOffset;
                else if (Math.Abs(previousXOffset - bottomXPosition) < Math.Abs(previousXOffset - position.X))
                    finalXPosition = bottomXPosition;

                var rightYPosition = position.Y - listViewBaseHeight + selectorItemHeight;
                var finalYPosition = position.Y;

                if (previousYOffset < position.Y && previousYOffset > rightYPosition)
                    finalYPosition = previousYOffset;
                else if (Math.Abs(previousYOffset - rightYPosition) < Math.Abs(previousYOffset - position.Y))
                    finalYPosition = rightYPosition;

                scrollViewer.ChangeView(finalXPosition, finalYPosition, null, disableAnimation);
            }
        }
    }

    public enum ItemPosition
    {
        /// <summary>
        /// If visible then it will not scroll, if not then item will be aligned to the nearest edge
        /// </summary>
        [EnumString("Default")]
        Default,

        /// <summary>
        /// Aligned left
        /// </summary>
        [EnumString("Left")]
        Left,

        /// <summary>
        /// Aligned top
        /// </summary>
        [EnumString("Top")]
        Top,

        /// <summary>
        /// Aligned centre
        /// </summary>
        [EnumString("Centre")]
        Centre,

        /// <summary>
        /// Aligned right
        /// </summary>
        [EnumString("Right")]
        Right,

        /// <summary>
        /// Aligned bottom
        /// </summary>
        [EnumString("Bottom")]
        Bottom
    }

    internal class EnumStringAttribute : Attribute
    {
        private string v;

        public EnumStringAttribute(string v)
        {
            this.v = v;
        }
    }

}
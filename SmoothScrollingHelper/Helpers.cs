using Microsoft.Toolkit.Uwp.UI.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace SmoothScrollingHelper
{
    public static class Helpers
    {
        /// <summary>
        /// Smooth scrolling the list to bring the specified index into view 
        /// </summary>
        /// <param name="listViewBase">List to scroll</param>
        /// <param name="index">The intex to bring into view</param>
        /// <param name="itemPlacement">Set the item placement after scrolling</param>
        /// <param name="disableAnimation">Set true to disable animation</param>
        /// <param name="scrollIfVisibile">Set true to disable scrolling when the corresponding item is in view</param>
        /// <param name="additionalHorizontalOffset">Adds additional horizontal offset</param>
        /// <param name="additionalVerticalOffset">Adds additional vertical offset</param>
        /// <returns>Note: Even though this return <see cref="Task"/>, it will not wait until the scrolling completes</returns>
        public static async Task SmoothScrollIntoViewWithIndex(this Windows.UI.Xaml.Controls.ListViewBase listViewBase, int index, ItemPlacement itemPlacement = ItemPlacement.Default, bool disableAnimation = false, bool scrollIfVisibile = true, int additionalHorizontalOffset = 0, int additionalVerticalOffset = 0)
        {
            if (Math.Abs(index) > listViewBase.Items.Count)
            {
                throw new IndexOutOfRangeException("Index can't be greater than number of items in " + listViewBase.GetType().Name);
            }

            index = (index < 0) ? (index + listViewBase.Items.Count) : index;

            bool isVirtualizing = default(bool);
            double previousXOffset = default(double), previousYOffset = default(double);

            var scrollViewer = listViewBase.FindDescendant<ScrollViewer>();
            var selectorItem = listViewBase.ContainerFromIndex(index) as SelectorItem;

            if (selectorItem == null)
            {
                isVirtualizing = true;

                previousXOffset = scrollViewer.HorizontalOffset;
                previousYOffset = scrollViewer.VerticalOffset;

                var tcs = new TaskCompletionSource<object>();

                void viewChanged(object s, ScrollViewerViewChangedEventArgs e) => tcs.TrySetResult(null);

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

                void viewChanged(object s, ScrollViewerViewChangedEventArgs e) => tcs.TrySetResult(null);

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

            var minXPosition = position.X - listViewBaseWidth + selectorItemWidth;
            var minYPosition = position.Y - listViewBaseHeight + selectorItemHeight;

            var maxXPosition = position.X;
            var maxYPosition = position.Y;

            double finalXPosition, finalYPosition;

            if (!scrollIfVisibile && (previousXOffset <= maxXPosition && previousXOffset >= minXPosition) && (previousYOffset <= maxYPosition && previousYOffset >= minYPosition))
            {
                finalXPosition = previousXOffset;
                finalYPosition = previousYOffset;
            }
            else
            {
                switch (itemPlacement)
                {
                    case ItemPlacement.Default:
                        if (previousXOffset <= maxXPosition && previousXOffset >= minXPosition)
                        {
                            finalXPosition = previousXOffset + additionalHorizontalOffset;
                        }
                        else if (Math.Abs(previousXOffset - minXPosition) < Math.Abs(previousXOffset - maxXPosition))
                        {
                            finalXPosition = minXPosition + additionalHorizontalOffset;
                        }
                        else
                        {
                            finalXPosition = maxXPosition + additionalHorizontalOffset;
                        }

                        if (previousYOffset <= maxYPosition && previousYOffset >= minYPosition)
                        {
                            finalYPosition = previousYOffset + additionalVerticalOffset;
                        }
                        else if (Math.Abs(previousYOffset - minYPosition) < Math.Abs(previousYOffset - maxYPosition))
                        {
                            finalYPosition = minYPosition + additionalVerticalOffset;
                        }
                        else
                        {
                            finalYPosition = maxYPosition + additionalVerticalOffset;
                        }

                        break;

                    case ItemPlacement.Left:
                        finalXPosition = maxXPosition + additionalHorizontalOffset;
                        finalYPosition = previousYOffset + additionalVerticalOffset;
                        break;

                    case ItemPlacement.Top:
                        finalXPosition = previousXOffset + additionalHorizontalOffset;
                        finalYPosition = maxYPosition + additionalVerticalOffset;
                        break;

                    case ItemPlacement.Centre:
                        var centreX = (listViewBaseWidth - selectorItemWidth) / 2.0;
                        var centreY = (listViewBaseHeight - selectorItemHeight) / 2.0;
                        finalXPosition = maxXPosition - centreX + additionalHorizontalOffset;
                        finalYPosition = maxYPosition - centreY + additionalVerticalOffset;
                        break;

                    case ItemPlacement.Right:
                        finalXPosition = minXPosition + additionalHorizontalOffset;
                        finalYPosition = previousYOffset + additionalVerticalOffset;
                        break;

                    case ItemPlacement.Bottom:
                        finalXPosition = previousXOffset + additionalHorizontalOffset;
                        finalYPosition = minYPosition + additionalVerticalOffset;
                        break;

                    default:
                        finalXPosition = previousXOffset + additionalHorizontalOffset;
                        finalYPosition = previousYOffset + additionalVerticalOffset;
                        break;
                }
            }

            scrollViewer.ChangeView(finalXPosition, finalYPosition, null, disableAnimation);
        }

        /// <summary>
        /// Smooth scrolling the list to bring the specified data item into view 
        /// </summary>
        /// <param name="listViewBase">List to scroll</param>
        /// <param name="index">The data item to bring into view</param>
        /// <param name="itemPlacement">Set the item placement after scrolling</param>
        /// <param name="disableAnimation">Set true to disable animation</param>
        /// <param name="ScrollIfVisibile">Set true to disable scrolling when the corresponding item is in view</param>
        /// <param name="additionalHorizontalOffset">Adds additional horizontal offset</param>
        /// <param name="additionalVerticalOffset">Adds additional vertical offset</param>
        /// <returns>Note: Even though this return <see cref="Task"/>, it will not wait until the scrolling completes</returns>
        public static async Task SmoothScrollIntoViewWithItem(this Windows.UI.Xaml.Controls.ListViewBase listViewBase, object item, ItemPlacement itemPlacement = ItemPlacement.Default, bool disableAnimation = false, bool scrollIfVisibile = true, int additionalHorizontalOffset = 0, int additionalVerticalOffset = 0)
        {
            await SmoothScrollIntoViewWithIndex(listViewBase, listViewBase.Items.IndexOf(item), itemPlacement, disableAnimation, scrollIfVisibile, additionalHorizontalOffset, additionalVerticalOffset);
        }

        public static void SmoothScrollNavigation(this Windows.UI.Xaml.Controls.ListViewBase listViewBase, int scrollAmount, ScrollNavigationDirection scrollNavigationDirection, bool disableAnimation = false)
        {
            var scrollViewer = listViewBase.FindDescendant<ScrollViewer>();

            if (scrollNavigationDirection == ScrollNavigationDirection.Left)
            {
                scrollViewer.ChangeView(scrollViewer.HorizontalOffset - scrollAmount, scrollViewer.VerticalOffset, null, disableAnimation);
            }
            else if (scrollNavigationDirection == ScrollNavigationDirection.Up)
            {
                scrollViewer.ChangeView(scrollViewer.HorizontalOffset, scrollViewer.VerticalOffset - scrollAmount, null, disableAnimation);
            }
            else if (scrollNavigationDirection == ScrollNavigationDirection.Right)
            {
                scrollViewer.ChangeView(scrollViewer.HorizontalOffset + scrollAmount, scrollViewer.VerticalOffset, null, disableAnimation);
            }
            else if (scrollNavigationDirection == ScrollNavigationDirection.Down)
            {
                scrollViewer.ChangeView(scrollViewer.HorizontalOffset, scrollViewer.VerticalOffset + scrollAmount, null, disableAnimation);
            }
        }
    }

    /// <summary>
    /// Item Position
    /// </summary>
    public enum ItemPlacement
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

    /// <summary>
    /// Scroll Direction
    /// </summary>
    public enum ScrollNavigationDirection
    {
        /// <summary>
        /// Scroll left
        /// </summary>
        [EnumString("Left")]
        Left,

        /// <summary>
        /// Scroll up
        /// </summary>
        [EnumString("Up")]
        Up,

        /// <summary>
        /// Scroll right
        /// </summary>
        [EnumString("Right")]
        Right,

        /// <summary>
        /// Scroll down
        /// </summary>
        [EnumString("Down")]
        Down,
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

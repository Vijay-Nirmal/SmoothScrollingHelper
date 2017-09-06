# Smooth Scrolling Helpers
Use Smooth Scrolling Helpers to smooth scroll in ListViewBase(GridView, ListView).
1. SmoothScrollIntoView Helper
2. SmoothScrollNavigation Helper
3. GetScrollViewer Helper

# SmoothScrollIntoView Helper
Use SmoothScrollIntoView Helper to scroll the item into the view with animation. Specify the ItemPosition property to align the item.

## Syntax

```csharp
await MyGridView.SmoothScrollIntoViewWithIndex(MyGridView.SelectedIndex, ItemPosition.Default, false);
```

## Sample Output

![SmoothScrollIntoView Helper](https://github.com/Vijay-Nirmal/SmoothScrollingHelper/blob/master/SmoothScrollIntoViewSampleOutput.gif)

## Properties

| Properties | Type | Description |
|------------|------|-------------|
| Intex      | int  | Intex of the item to be scrolled |
| ItemPosition | Enum | Specify the position of the Item after scrolling |
| DisableAnimation | bool | To disable the scrolling animation |

### ItemPosition

| ItemPosition | Description |
|--------------|-------------|
| Default | If visible then it will not scroll, if not then item will be aligned to the nearest edge |
| Left | Aligned left |
| Top | Aligned top |
| Centre | Aligned centre |
| Right | Aligned right |
| Bottom | Aligned bottom |

# SmoothScrollNavigation Helper
Use SmoothScrollNavigation Helper to scroll the ListViewBase with buttons

## Syntax

```csharp
await MyGridView.SmoothScrollIntoViewWithIndex(MyGridView.SelectedIndex, ItemPosition.Default, false);
```

## Sample Output

![SmoothScrollIntoView Helper](https://github.com/Vijay-Nirmal/SmoothScrollingHelper/blob/master/SmoothScrollNavigationSampleOutput.gif)

## Properties

| Properties | Type | Description |
|------------|------|-------------|
| ScrollAmount | int  | Amount to be scrolled |
| ScrollNavigationDirection | Enum | Direction of the scrolling |
| DisableAnimation | bool | To disable the scrolling animation |

### ItemPosition

| ItemPosition | Description |
|--------------|-------------|
| Left | Scroll left |
| Up | Scroll top |
| Right | Scroll right |
| Down | Scroll bottom |

# GetScrollViewer Helper
Use GetScrollViewer Helper to get the ScrollViewer inside the ListViewBase

## Syntax

```csharp
var scrollViewer = listViewBase.GetScrollViewer();
```

### Spcial thanks to  
[@JustinXinLiu](https://github.com/JustinXinLiu) - [StackOverflow Answer](https://stackoverflow.com/a/32559623/7331395)

# SmoothScrollingHelper
Use SmoothScrollingHelper to smooth scroll in ListViewBase(GridView, ListView)

## Syntax

```csharp
await MyGridView.SetItemPositionWithIndex(MyGridView.SelectedIndex, ItemPosition.Left);
```

## Sample Output

![SmoothScrolling Helper](https://github.com/Vijay-Nirmal/SmoothScrollingHelper/blob/master/SampleOutput.gif)

## Properties

| Properties | Type | Description |
|------------|------|-------------|
| Intex      | int  | Intex of the item to be scrolled |
| ItemPosition | Enum | Specify the position of the Item after scrolling |
| DisableAnimation | bool | Give to disable the scrolling animation |

### ItemPosition

| ItemPosition | Description |
|--------------|-------------|
| Default | If visible then it will not scroll, if not then item will be aligned to the nearest edge |
| Left | Aligned left |
| Top | Aligned top |
| Centre | Aligned centre |
| Right | Aligned right |
| Bottom | Aligned bottom |

### Spcial thanks to  
[@JustinXinLiu](https://github.com/JustinXinLiu) - [StackOverflow Answer](https://stackoverflow.com/a/32559623/7331395)

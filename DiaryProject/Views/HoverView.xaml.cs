using System.Windows;
using System.Windows.Input;
using DiaryProject.Events;

namespace DiaryProject.Views;

public partial class HoverView : Window
{
    private readonly IEventAggregator _aggregator;

    public HoverView(IEventAggregator aggregator)
    {
        _aggregator = aggregator;
        InitializeComponent();
    }

    private void OnDrag(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }

    private void OnClose(object? sender, EventArgs e)
    {
        Close();
    }

    private void OnLoad(object sender, RoutedEventArgs e)
    {
        Topmost = true;
        Left = 300;
        Top = 300;
    }

    private void PopupOptions(object sender, MouseButtonEventArgs e)
    {
        Menu.IsOpen = true;
    }

    private void HideHover(object sender, RoutedEventArgs e)
    {
        App.IsHoverVisible = false;
        _aggregator.GetEvent<HoverStatusChanged>().Publish(new HoverStatusModel { IsVisible = HoverStatus.Hide });
        Menu.IsOpen = false;
        Hide();
    }
}
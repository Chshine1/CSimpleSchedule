using System.Windows;
using System.Windows.Input;

namespace DiaryProject.Views;

public partial class HoverView : Window
{
    public HoverView()
    {
        InitializeComponent();
    }

    private void OnDragged(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        Close();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        Topmost = true;
        Left = 300;
        Top = 300;
    }
}
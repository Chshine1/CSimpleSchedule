using System.Diagnostics.CodeAnalysis;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using DiaryProject.Events;
using DiaryProject.Shared.Utils;
using MaterialDesignThemes.Wpf;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace DiaryProject.Views;

[SuppressMessage("ReSharper", "RedundantExtendsListEntry")]
public partial class MainView : Window
{
    private readonly System.Timers.Timer _timer = new(1000);

    private const string StartUpView = "LoginView";
    
    public MainView(IEventAggregator aggregator, IRegionManager regionManager)
    {
        InitializeComponent();
        aggregator.GetEvent<PageNavigatedTo>().Subscribe(arg =>
        {
            MenuBarCommand.IsEnabled = false;
            MenuBar.SelectedItem = arg.Uri.OriginalString switch
            {
                "LoginView" => MenuBar.Items[0],
                "CalendarView" => MenuBar.Items[1],
                "MemoEditorView" => MenuBar.Items[2],
                _ => MenuBar.SelectedItem
            };
            MenuBarCommand.IsEnabled = true;
        });
        MinimizeButton.Click += (_, _) => WindowState = WindowState.Minimized;
        MaximizeButton.Click += (_, _) =>
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            MaximizeIcon.Kind = WindowState == WindowState.Maximized ? PackIconKind.WindowRestore : PackIconKind.WindowMaximize;
        };
        CloseButton.Click += (_, _) => Close();
        SecondPointer.Angle = DateTime.Now.Millisecond / (double) 1000 * 6 + 180;
        MinutePointer.Angle = DateTime.Now.Minute * 6 + 180;
        HourPointer.Angle = DateTime.Now.Hour * 30 + DateTime.Now.Minute * 0.5 + 180;
        TimeBlock.Text = DateTime.Now.ToLongTimeString();
        DateBlock.Text = DateTime.Now.ToString("yyyy/M/d") + " " + DateTime.Now.GetChineseDayOfWeek();
        _timer.Elapsed += TimeElapsed!;
        _timer.Start();
        Loaded += (_, _) =>
        {
            MenuBar.SelectedItem = MenuBar.Items[0];
            regionManager.Regions["MainPanel"].RequestNavigate(nameof(LoginView));
        };
    }

    private void Border_OnDragOver(object sender, MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed) DragMove();
    }
    
    private void TimeElapsed(object sender, ElapsedEventArgs e)
    {
        Dispatcher.Invoke(DispatcherPriority.Normal, () =>
        {
            SecondPointer.Angle = DateTime.Now.Second * 6 + 180;
            MinutePointer.Angle = DateTime.Now.Minute * 6 + 180;
            HourPointer.Angle = DateTime.Now.Hour * 30 + DateTime.Now.Minute * 0.5 + 180;
            TimeBlock.Text = DateTime.Now.ToLongTimeString();
            DateBlock.Text = DateTime.Now.ToString("yyyy/M/d") + " " + DateTime.Now.GetChineseDayOfWeek();
        });
    }
}
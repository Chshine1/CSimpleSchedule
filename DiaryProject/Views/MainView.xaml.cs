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
    // ReSharper disable once NullableWarningSuppressionIsUsed
    private HoverView _hoverView = null!;
    private readonly System.Timers.Timer _timer = new(1000);

    private const string StartUpView = nameof(LoginView);
    
    public MainView(IEventAggregator aggregator, IRegionManager regionManager, IContainerProvider container)
    {
        InitializeComponent();
        InitializeClock();
        
        MinimizeButton.Click += (_, _) => WindowState = WindowState.Minimized;
        MaximizeButton.Click += (_, _) =>
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            MaximizeIcon.Kind = WindowState == WindowState.Maximized ? PackIconKind.WindowRestore : PackIconKind.WindowMaximize;
        };
        CloseButton.Click += (_, _) =>
        {
            Close();
        };
        Loaded += (_, _) =>
        {
            MenuBar.SelectedItem = MenuBar.Items[0];
            regionManager.Regions["MainPanel"].RequestNavigate(StartUpView);
            _hoverView = container.Resolve<HoverView>();
            _hoverView.Hide();
            _hoverView.Owner = null;
        };
        Closed += (_, _) =>
        {
            _hoverView.Close();
        };
        aggregator.GetEvent<HoverStatusChanged>().Subscribe(arg =>
        {
            switch (arg.IsVisible)
            {
                case HoverStatus.RevertVisibility:
                    if (_hoverView.IsVisible) _hoverView.Hide();
                    else _hoverView.Show();
                    break;
                case HoverStatus.Show:
                    _hoverView.Show();
                    break;
                case HoverStatus.Hide:
                    _hoverView.Hide();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(arg));
            }
        });
        
        aggregator.GetEvent<PageNavigatedTo>().Subscribe(arg =>
        {
            MenuBarCommand.IsEnabled = false;
            MenuBar.SelectedItem = arg.Uri.OriginalString switch
            {
                nameof(LoginView) => MenuBar.Items[0],
                nameof(UserView) => MenuBar.Items[0],
                nameof(CalendarView) => MenuBar.Items[1],
                nameof(MemoEditorView) => MenuBar.Items[2],
                nameof(HoverSettingsView) => MenuBar.Items[3],
                _ => MenuBar.SelectedItem
            };
            MenuBarCommand.IsEnabled = true;
        });
    }

    private void InitializeClock()
    {
        SecondPointer.Angle = DateTime.Now.Millisecond / (double) 1000 * 6 + 180;
        MinutePointer.Angle = DateTime.Now.Minute * 6 + 180;
        HourPointer.Angle = DateTime.Now.Hour * 30 + DateTime.Now.Minute * 0.5 + 180;
        TimeBlock.Text = DateTime.Now.ToLongTimeString();
        DateBlock.Text = DateTime.Now.ToString("yyyy/M/d") + " " + DateTime.Now.GetChineseDayOfWeek();
        
        // ReSharper disable once NullableWarningSuppressionIsUsed
        _timer.Elapsed += TimeElapsed!;
        _timer.Start();
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

    private void Border_OnDragOver(object sender, MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed) DragMove();
    }
}
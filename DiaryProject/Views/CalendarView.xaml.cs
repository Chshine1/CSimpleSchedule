using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls;
using UserControl = System.Windows.Controls.UserControl;

namespace DiaryProject.Views;

[SuppressMessage("ReSharper", "RedundantExtendsListEntry")]
public partial class CalendarView : UserControl
{
    public CalendarView()
    {
        InitializeComponent();
    }
}
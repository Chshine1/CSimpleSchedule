using System.Diagnostics.CodeAnalysis;
using UserControl = System.Windows.Controls.UserControl;

namespace DiaryProject.Views;

[SuppressMessage("ReSharper", "RedundantExtendsListEntry")]
public partial class UserView : UserControl
{
    public UserView()
    {
        InitializeComponent();
    }
}
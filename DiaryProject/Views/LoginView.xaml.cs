using System.Windows.Controls;
using System.Windows.Input;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using UserControl = System.Windows.Controls.UserControl;

namespace DiaryProject.Views;

public partial class LoginView : UserControl
{
    public LoginView()
    {
        InitializeComponent();
    }

    private void EndUserNameInput(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter) return;
        PasswordBox.Focus();
    }

    private void EndPasswordInput(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter) return;
        FocusHolder.Focus();
    }
}
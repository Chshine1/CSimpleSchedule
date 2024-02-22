using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls;
using System.Windows.Input;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using UserControl = System.Windows.Controls.UserControl;

namespace DiaryProject.Views;

[SuppressMessage("ReSharper", "RedundantExtendsListEntry")]
public partial class MemoEditorView : UserControl
{
    public MemoEditorView()
    {
        InitializeComponent();
    }

    private void EndTextBoxInput(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter) return;
        FocusHolder.Focus();
    }
}
using System.Windows;
using System.Windows.Documents;
using RichTextBox = System.Windows.Controls.RichTextBox;

namespace DiaryProject.Utils;

public class RichTextBoxHelper : RichTextBox
{
    public new FlowDocument Document
    {
        get => (FlowDocument) GetValue(DocumentProperty);
        set => SetValue(DocumentProperty, value);
    }
    
    public static readonly DependencyProperty DocumentProperty =
        DependencyProperty.Register(nameof(Document), typeof(FlowDocument), typeof(RichTextBoxHelper),
            new FrameworkPropertyMetadata(null, OnDocumentChanged));
    
    private static void OnDocumentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var rtb = (RichTextBox)d;
        rtb.Document = (FlowDocument)e.NewValue;
    }
}

public static class FlowDocumentExtension
{
    public static FlowDocument ConvertToFlowDocument(this string str)
    {
        var flowDocument = new FlowDocument();
        var t = str.Split(Environment.NewLine);
        foreach (var s in t)
        {
            var paragraph = new Paragraph();
            paragraph.Inlines.Add(new Run(s));
            flowDocument.Blocks.Add(paragraph);
        }
        return flowDocument;
    }
}
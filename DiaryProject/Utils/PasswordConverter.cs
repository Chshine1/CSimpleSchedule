using System.Globalization;
using System.Windows.Data;

namespace DiaryProject.Utils;

public class PasswordConverter : IValueConverter
{
    private string? _realWord = "";

    private char _replaceChar = '*';

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (parameter != null)
        {
            var temp = parameter.ToString();
            if (!string.IsNullOrEmpty(temp))
            {
                _replaceChar = temp.First();
            }
        }
        if (value != null)
        {
            _realWord = value.ToString();
        }
        
        var replaceWord = "";
        if (_realWord == null) return replaceWord;
        for (var index = 0; index < _realWord.Length; ++index)
        {
            replaceWord += _replaceChar;
        }
        return replaceWord;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var backValue = "";
        if (value == null) return backValue;
        var strValue = value.ToString();
        if (strValue == null || _realWord == null) return backValue;
        for (var index = 0; index < strValue.Length; ++index)
        {
            backValue += strValue.ElementAt(index) == _replaceChar
                ? _realWord.ElementAt(index)
                : strValue.ElementAt(index);
        }
        return backValue;
    }

}

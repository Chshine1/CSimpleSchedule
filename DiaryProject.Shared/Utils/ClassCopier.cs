namespace DiaryProject.Shared.Utils;

public static class ClassCopier
{
    public static void CopyFrom<T>(this T target, T from)
    {
        var pA = typeof(T).GetProperties();
        foreach (var t in pA)
        {
            if(t.CanWrite) t.SetValue(target, t.GetValue(from, null));
        }
    }
}
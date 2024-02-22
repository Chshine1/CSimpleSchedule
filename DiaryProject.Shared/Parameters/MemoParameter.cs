using System.Diagnostics.CodeAnalysis;

namespace DiaryProject.Shared.Parameters;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class MemoParameter : BaseParameter
{
    public long Timestamp { get; set; }

    /// <summary>
    /// 0-No range limits, 1-Day, 2-Month, 3-Year
    /// </summary>
    public int Range { get; set; }
    
    /// <summary>
    /// 0-No category limits
    /// </summary>
    public int Category { get; set; }
}
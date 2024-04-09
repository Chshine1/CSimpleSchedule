namespace DiaryProject.Models;

public class HoverConfiguration
{
    public bool ShowOnRegistered { get; set; } = true;
    public bool SetActiveOnAdded { get; set; } = false;
    public int DefaultMemoCategory { get; set; } = 0;
}
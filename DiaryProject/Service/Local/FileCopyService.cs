using System.IO;

namespace DiaryProject.Service.Local;

public class FileCopyService(string directory)
{
    public void CopyFile(string source)
    {
        var ext = Path.GetFileName(source);
        var target = Path.Join(directory, ext);
        File.Copy(source, target);
    }
}
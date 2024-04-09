using System.IO;
using DiaryProject.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DiaryProject.Service.Local;

public class FileCopyService(string directory)
{
    private const string FileName = "configuration.json";

    public HoverConfiguration ReadHoverConfiguration()
    {
        var file = Path.Join(directory, FileName);
        if (!Path.Exists(file)) return new HoverConfiguration();
        var content = File.ReadAllText(file);
        return JsonConvert.DeserializeObject<HoverConfiguration>(content) ?? new HoverConfiguration();
    }

    public void WriteHoverConfiguration(HoverConfiguration configuration)
    {
        var file = Path.Join(directory, FileName);
        var json = JsonConvert.SerializeObject(configuration);
        File.WriteAllText(file, json);
    }
}
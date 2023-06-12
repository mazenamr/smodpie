using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Smodpie.Utils;

public static class SZZ
{
    public class FileInformation
    {
        public string? OldName { get; set; }
        public string? NewName { get; set; }
        public List<string>? DeletedLines { get; set; }
        public Dictionary<string, string>? AddedLines { get; set; }
        public int? AuthorCount { get; set; }
        public int? LineCountAfterChange { get; set; }
        public int? LineCountBeforeChange { get; set; }
    }

    public static Dictionary<string, List<string>> GetTrainingLines(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentException("File path cannot be null or empty.");
        }

        if (!File.Exists(path))
        {
            throw new FileNotFoundException("File not found.", path);
        }

        string jsonContent = File.ReadAllText(path);
        Dictionary<string, List<string>>? parsedData = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(jsonContent);

        if (parsedData == null)
        {
            throw new InvalidOperationException($"Failed to parse {path}");
        }

        return parsedData;
    }

    public static List<string> GetFileNames(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentException("File path cannot be null or empty.");
        }

        if (!File.Exists(path))
        {
            throw new FileNotFoundException("File not found.", path);
        }

        var jsonContent = File.ReadAllText(path);
        var jsonObject = JObject.Parse(jsonContent);

        var filesObject = jsonObject["Files"] ?? throw new InvalidOperationException($"Failed to parse {path}");

        var filesArray = filesObject.ToObject<JArray>() ?? throw new InvalidOperationException($"Failed to parse {path}");

        var fileInformationList = filesArray.ToObject<List<FileInformation>>() ?? throw new InvalidOperationException($"Failed to parse {path}");

        var fileNames = new List<string>();
        foreach (var fileInfo in fileInformationList)
        {
            if (fileInfo.NewName == null)
            {
                if (fileInfo.OldName == null)
                    throw new InvalidOperationException($"Failed to parse {path}");
                fileNames.Add(fileInfo.OldName);
            }
            else
                fileNames.Add(fileInfo.NewName);
        }

        return fileNames;
    }
}

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

    public static List<string> GetFileNames(string newCommitJson)
    {
        var jsonContent = File.ReadAllText(newCommitJson);
        var jsonObject = JObject.Parse(jsonContent);

        var filesObject = jsonObject["Files"] ?? throw new InvalidOperationException($"Failed to parse {newCommitJson}");

        var filesArray = filesObject.ToObject<JArray>() ?? throw new InvalidOperationException($"Failed to parse {newCommitJson}");

        var fileInformationList = filesArray.ToObject<List<FileInformation>>() ?? throw new InvalidOperationException($"Failed to parse {newCommitJson}");

        var fileNames = new List<string>();
        foreach (var fileInfo in fileInformationList)
        {
            if (fileInfo.NewName == null)
            {
                if (fileInfo.OldName == null)
                    throw new InvalidOperationException($"Failed to parse {newCommitJson}");
                fileNames.Add(fileInfo.OldName);
            }
            else
                fileNames.Add(fileInfo.NewName);
        }

        return fileNames;
    }
}

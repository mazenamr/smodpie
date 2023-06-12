using System.Text.Json.Serialization;

namespace Smodpie.Parser;

/// <summary>
/// Interface for parsing text into a structured format.
/// </summary>
/// <remarks>
/// A Parser is responsible for parsing text into a structured format, such as an array of strings.
/// It provides methods for parsing a single block of text, an array of text lines, a file, or all files in a directory.
/// </remarks>
[JsonDerivedType(typeof(CharacterParser), 1)]
[JsonDerivedType(typeof(WordParser), 2)]
public interface IParser
{
    /// <summary>
    /// Parses a single block of text into a structured format.
    /// </summary>
    /// <param name="text">The text to parse.</param>
    /// <returns>An array of strings representing the parsed text.</returns>
    string[] Parse(string text);

    /// <summary>
    /// Parses an array of text lines into a structured format.
    /// </summary>
    /// <param name="lines">The array of text lines to parse.</param>
    /// <returns>A jagged array of strings representing the parsed text.</returns>
    string[][] ParseLines(string[] lines)
    {
        if (lines == null)
            throw new ArgumentNullException(nameof(lines), "The array of text lines to parse cannot be null.");

        var parsedLines = new string[lines.Length][];
        for (int i = 0; i < lines.Length; i++)
            parsedLines[i] = Parse(lines[i]);

        return parsedLines;
    }

    /// <summary>
    /// Parses a file into a structured format.
    /// </summary>
    /// <param name="filePath">The path to the file to parse.</param>
    /// <returns>A jagged array of strings representing the parsed text.</returns>
    string[][]? ParseFile(string filePath)
    {
        if (filePath == null)
            throw new ArgumentNullException(nameof(filePath), "The path to the file to parse cannot be null.");

        try
        {
            var lines = File.ReadAllLines(filePath);
            return ParseLines(lines);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error parsing file {filePath}: {ex.Message}");
        }
    }

    /// <summary>
    /// Parses all files in a directory into a structured format.
    /// </summary>
    /// <param name="directoryPath">The path to the directory to parse.</param>
    /// <param name="recursive">Whether to parse files in subdirectories as well. Default is false.</param>
    /// <returns>A jagged array of strings representing the parsed text for each file.</returns>
    string[][][]? ParseDirectory(string directoryPath, bool recursive = false)
    {
        if (directoryPath == null)
            throw new ArgumentNullException(nameof(directoryPath), "The path to the directory to parse cannot be null.");

        try
        {
            var files = Directory.GetFiles(directoryPath);
            if (recursive)
                files = Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories);

            var parsedFiles = new List<string[][]>(files.Length);
            foreach (var file in files)
            {
                try
                {
                    var lines = File.ReadAllLines(file);
                    parsedFiles.Add(ParseLines(lines));
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error parsing file {file}: {ex.Message}");
                }
            }
            return parsedFiles.ToArray();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error parsing directory {directoryPath}: {ex.Message}");
        }
    }
}

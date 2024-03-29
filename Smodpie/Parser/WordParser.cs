namespace Smodpie.Parser;

/// <summary>
/// Parses a string into a list of tokens by splitting on whitespace and punctuation.
/// </summary>
/// <remarks>
/// The separators used by the parser can be configured by setting the Separators property.
/// </remarks>
public class WordParser : IParser
{
    private string[] _separators = new string[] { " ", "\t", "\r", "\n", ",", ".", "?", "<", ">", "=", "+", "-", "*", "/", "|", "&", "^", "%", "(", ")", "[", "]", "{", "}", "\"", "'", ":", ";", "!", "@", "#", "$" };

    public string? Comment { get; set; } = null;

    public string[] NoisePrefixes { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Gets or sets the separators used by the parser to split the input text into tokens.
    /// </summary>
    public string[] Separators
    {
        get => _separators;
        set
        {
            if (value == null)
                throw new ArgumentNullException(nameof(Separators), "Separators cannot be null.");

            _separators = value;
        }
    }

    /// <summary>
    /// Parses a single block of text into a structured format.
    /// </summary>
    /// <param name="text">The text to parse.</param>
    /// <returns>An array of strings representing the parsed text.</returns>
    public string[] Parse(string text)
    {
        if (NoisePrefixes.Any(p => text.Trim().ToLowerInvariant().StartsWith(p)))
            return Array.Empty<string>();

        if (text == null)
            throw new ArgumentNullException(nameof(text));

        var tokens = text.Split(Separators, StringSplitOptions.RemoveEmptyEntries);

        if (Comment != null)
            for (int i = 0; i < tokens.Length; i++)
                if (tokens[i].Trim().StartsWith(Comment))
                {
                    Array.Resize(ref tokens, i);
                    break;
                }


        return tokens;
    }
}

namespace Smodpie.Parser;

/// <summary>
/// Parses a string into a list of tokens by splitting on whitespace and punctuation.
/// </summary>
/// <remarks>
/// The separators used by the parser can be configured by setting the Separators property.
/// </remarks>
public class WordParser : IParser
{
    private string[] _separators = new string[] { " ", "\t", "\r", "\n", ",", ".", "?", "<", ">", "=", "+", "-", "*", "/", "|", "&", "^", "%" };

    /// <summary>
    /// Gets or sets the separators used by the parser to split the input text into tokens.
    /// </summary>
    /// <remarks>
    /// The default separators are whitespace characters, comma, period, question mark, less than, greater than, equal sign, plus sign, minus sign, asterisk, forward slash, vertical bar, ampersand, caret, and percent sign.
    /// </remarks>
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
        if (text == null)
            throw new ArgumentNullException(nameof(text));

        return text.Split(Separators, StringSplitOptions.RemoveEmptyEntries);
    }
}

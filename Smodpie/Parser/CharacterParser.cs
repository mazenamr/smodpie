namespace Smodpie.Parser;

/// <summary>
/// Parses text into an array of tokens, where each token is a single character.
/// </summary>
public class CharacterParser : IParser
{
    private int _tokenLength = 1;

    /// <summary>
    /// Gets or sets the length of each token.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the value is less than 1.</exception>
    public int TokenLength
    {
        get => _tokenLength;

        set
        {
            if (value < 1)
                throw new ArgumentOutOfRangeException("TokenLength", "TokenLength must be greater than 0.");

            _tokenLength = value;
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
            throw new ArgumentNullException("text");

        return text.ToCharArray().Select((c, i) => new { c, i })
            .GroupBy(x => x.i / _tokenLength)
            .Select(g => new string(g.Select(x => x.c).ToArray()))
            .ToArray();
    }
}

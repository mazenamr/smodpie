using Newtonsoft.Json;
using Smodpie.Counter;
using Smodpie.Parser;

namespace Smodpie.Utils;

public static class IO
{
    public static void SaveCounterToFile(this TrieCounter counter, string path)
    {
        if (counter == null)
            throw new ArgumentNullException(nameof(counter));
        if (string.IsNullOrEmpty(path))
            throw new ArgumentNullException(nameof(path));

        using var stream = File.CreateText(path);
        JsonSerializer jsonSerializer = new();
        jsonSerializer.Serialize(stream, counter);
    }

    public static TrieCounter LoadCounterFromFile(string path)
    {
        if (string.IsNullOrEmpty(path))
            throw new ArgumentNullException(nameof(path));
        if (!File.Exists(path))
            throw new FileNotFoundException($"File not found: {path}");

        using var stream = File.OpenText(path);
        JsonSerializer jsonSerializer = new();
        var counter = jsonSerializer.
            Deserialize(stream, typeof(TrieCounter)) as TrieCounter;

        if (counter == null)
            throw new InvalidOperationException($"Failed to deserialize {path}");

        return counter;
    }

    public static void SaveTokenizerToFile(this Tokenizer tokenizer, string path)
    {
        if (tokenizer == null)
            throw new ArgumentNullException(nameof(tokenizer));
        if (string.IsNullOrEmpty(path))
            throw new ArgumentNullException(nameof(path));

        using var stream = File.CreateText(path);
        JsonSerializer jsonSerializer = new();
        jsonSerializer.Serialize(stream, tokenizer);
    }

    public static Tokenizer LoadTokenizerFromFile(string path)
    {
        if (string.IsNullOrEmpty(path))
            throw new ArgumentNullException(nameof(path));
        if (!File.Exists(path))
            throw new FileNotFoundException($"File not found: {path}");

        using var stream = File.OpenText(path);
        JsonSerializer jsonSerializer = new();
        var tokenizer = jsonSerializer.
            Deserialize(stream, typeof(Tokenizer)) as Tokenizer;

        if (tokenizer == null)
            throw new InvalidOperationException($"Failed to deserialize {path}");

        return tokenizer;
    }
}

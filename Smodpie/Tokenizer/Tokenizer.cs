namespace Smodpie.Tokenizer;

public class Tokenizer
{
    private const string UnknownToken = "<UNKNOWN>";

    private readonly IDictionary<string, int> _tokenIndices = new Dictionary<string, int>();
    private readonly IList<string> _tokens = new List<string>();
    private readonly IList<int> _counts = new List<int>();

    private bool _closed = false;

    public int Store(string token, int count = 1)
    {
        if (TryGetIndex(token, out var index))
        {
            _counts[index] = count;
        }
        else
        {
            index = _tokens.Count;
            _tokenIndices.Add(token, index);
            _tokens.Add(token);
            _counts.Add(count);
        }

        return index;
    }

    public void Close() => _closed = true;
    public void Open() => _closed = false;

    public int GetIndex(string token) => TryGetIndex(token, out var index) ? index : _closed ? _tokenIndices[UnknownToken] : Store(token);
    public bool TryGetIndex(string token, out int index) => _tokenIndices.TryGetValue(token, out index);
    public IEnumerable<int> GetIndices(IEnumerable<string> tokens) => tokens.Select(GetIndex);

    public string GetWord(int index) => _tokens[index];
    public IEnumerable<string> GetWords(IEnumerable<int> indices) => indices.Select(GetWord);

    public int GetCount(int index) => _counts[index];
    public IEnumerable<int> GetCounts(IEnumerable<int> indices) => indices.Select(GetCount);
    public int GetCount(string token) => TryGetIndex(token, out var index) ? GetCount(index) : 0;
    public IEnumerable<int> GetCounts(IEnumerable<string> tokens) => tokens.Select(GetCount);
}

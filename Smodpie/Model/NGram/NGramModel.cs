using Smodpie.Config;
using Smodpie.Counter;
using System.Buffers;
using System.Text.Json.Serialization;

namespace Smodpie.Model.NGram;

/// <summary>
/// NGramModel is an implementation of an n-gram language model with support for online learning.
/// The model can learn and forget tokens, and can be used to predict and model the probability and confidence
/// of tokens in a given sequence.
/// </summary>
/// <remarks>
/// The NGramModel class provides methods to learn and forget tokens, as well as to predict and model
/// the probability and confidence of tokens in a given sequence. It supports different smoothing techniques
/// and can be used with online learning.
/// </remarks>
public class NGramModel : OnlineModel, IModel
{
    private readonly ArrayPool<ArraySegment<int>> _arrayPool = ArrayPool<ArraySegment<int>>.Shared;

    public int Order { get; init; }
    public ICounter Counter { get; init; }
    public Smoothing.SmoothingTypes SmoothingType { get; init; }

    [JsonConstructor]
    public NGramModel(int order, ICounter counter,
        Smoothing.SmoothingTypes smoothingType = Smoothing.SmoothingTypes.JelinekMercer,
        bool onlineLearning = false) : base(onlineLearning)
    {
        Order = order;
        Counter = counter;
        SmoothingType = smoothingType;
    }

    public NGramModel(int order, bool onlineLearning = false) : this(order, new TrieCounter(), onlineLearning: onlineLearning) { }

    public NGramModel(bool onlineLearning = false) : this(Constant.DEFAULT_NGRAM_ORDER, onlineLearning) { }

    /// <summary>
    /// Instructs the model to learn the provided tokens.
    /// </summary>
    /// <param name="tokens">A list of lexed and translated input tokens.</param>
    public void LearnTokens(in int[] tokens)
    {
        ArraySegment<int>[] ngrams = GetNGrams(tokens);
        Parallel.ForEach(ngrams, index => Counter.IncrementCount(index));
        _arrayPool.Return(ngrams);
    }

    /// <summary>
    /// Instructs the model to learn a specific token at the given index.
    /// </summary>
    /// <param name="tokens">A list of lexed and translated input tokens.</param>
    /// <param name="index">The index of the token to learn.</param>
    public override void LearnToken(in int[] tokens, int index)
    {
        var ngram = GetNGram(tokens, index);
        for (var i = 0; i < ngram.Count; i++)
        {
            Counter.IncrementCount(ngram);
            ngram = ngram.Slice(1);
        }
    }

    /// <summary>
    /// Instructs the model to forget the provided tokens.
    /// </summary>
    /// <param name="tokens">A list of lexed and translated input tokens.</param>
    public void ForgetTokens(in int[] tokens)
    {
        ArraySegment<int>[] ngrams = GetNGrams(tokens);
        Parallel.ForEach(ngrams, index => Counter.DecrementCount(index));
        _arrayPool.Return(ngrams);
    }

    /// <summary>
    /// Instructs the model to forget a specific token at the given index.
    /// </summary>
    /// <param name="tokens">A list of lexed and translated input tokens.</param>
    /// <param name="index">The index of the token to forget.</param>
    public override void ForgetToken(in int[] tokens, int index)
    {
        var ngram = GetNGram(tokens, index);
        for (var i = 0; i < ngram.Count; i++)
        {
            Counter.IncrementCount(ngram);
            ngram = ngram.Slice(1);
        }
    }

    /// <summary>
    /// Internal implementation of modeling a single token at the specified index.
    /// </summary>
    /// <param name="tokens">A list of lexed and translated input tokens.</param>
    /// <param name="index">The index of the token to model.</param>
    /// <returns>Probability and confidence values for the token at the specified index.</returns>
    protected override (double Probability, double Confidence) ModelTokenInternal(in int[] tokens, int index)
    {
        var ngram = GetNGram(tokens, index);
        double probability = 0;
        double mass = 0;
        int hits = 0;

        for (var i = ngram.Count; i >= 0; i--)
        {
            var n = ngram.Slice(i);
            var counts = Counter.GetCounts(n);
            if (counts.Count == 0)
                if (counts.ContextCount == 0)
                    break;
                else
                    continue;

            var (p, c) = Smoothing.Smooth(SmoothingType, counts, n, Counter);

            mass = (1 - c) * mass + c;
            probability = (1 - c) * probability + c * p;

            hits++;
        }

        if (mass > 0)
            probability /= mass;

        double confidence = 1 - Math.Pow(2, -hits);

        return (probability, confidence);
    }

    /// <summary>
    /// Internal implementation of generating predictions for a single token at the specified index.
    /// </summary>
    /// <param name="tokens">A list of lexed and translated input tokens.</param>
    /// <param name="index">The index of the token to predict.</param>
    /// <returns>A dictionary containing the top N predictions for the token at the specified index with their probability and confidence values.</returns>
    protected override Dictionary<int, (double Probability, double Confidence)> PredictTokenInternal(in int[] tokens, int index)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns a list of all n-grams in the provided tokens.
    /// </summary>
    /// <param name="tokens">A list of lexed and translated input tokens.</param>
    /// <returns>A list of all n-grams in the provided tokens.</returns>
    private ArraySegment<int>[] GetNGrams(in int[] tokens)
    {
        ArraySegment<int>[] ngrams = _arrayPool.Rent(tokens.Length);
        for (var i = 0; i < tokens.Length; i++)
        {
            int take = Math.Min(Order, tokens.Length - i);
            ngrams[i] = new ArraySegment<int>(tokens, i, take);
        }
        return ngrams;
    }

    /// <summary>
    /// Returns an n-gram of the provided tokens starting at the specified index.
    /// </summary>
    /// <param name="tokens">A list of lexed and translated input tokens.</param>
    /// <param name="index">The starting index of the n-gram.</param>
    /// <returns>An n-gram of the provided tokens starting at the specified index.</returns>
    private ArraySegment<int> GetNGram(in int[] tokens, int index)
    {
        int take = Math.Min(Order, tokens.Length - index);
        ArraySegment<int> ngram = new ArraySegment<int>(tokens, index, take);
        return ngram;
    }

    /// <summary>
    /// Creates a new clean instance of the model with the same settings as the current instance.
    /// </summary>
    /// <returns>A new instance of the model with the same settings.</returns>
    protected override OnlineModel NewInstanceInternal()
    {
        return new NGramModel(Order, Counter.NewInstance(), SmoothingType, OnlineLearning);
    }
}

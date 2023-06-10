using Smodpie.Config;
using Smodpie.Counter;

namespace Smodpie.Model;

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
public class NGramModel : OnlineModel
{
    public int Order { get; init; }
    public ICounter Counter { get; init; }
    public Smoothing.SmoothingTypes SmoothingType { get; init; }

    public NGramModel (int order, ICounter counter,
        Smoothing.SmoothingTypes smoothingType = Smoothing.SmoothingTypes.JelinekMercer,
        bool onlineLearning = false) : base(onlineLearning)
    {
        Order = order;
        Counter = counter;
        SmoothingType = smoothingType;
    }

    public NGramModel(int order, bool onlineLearning = false) : this(order, new Counter.TrieCounter(), onlineLearning: onlineLearning) { }

    public NGramModel(bool onlineLearning = false) : this(Constant.DEFAULT_NGRAM_ORDER, onlineLearning) { }

    /// <summary>
    /// Instructs the model to learn the provided tokens.
    /// </summary>
    /// <param name="tokens">A list of lexed and translated input tokens.</param>
    public void LearnTokens(in IReadOnlyList<int> tokens)
    {
        Counter.IncrementCountBatch(GetNGrams(tokens));
    }

    /// <summary>
    /// Instructs the model to learn a specific token at the given index.
    /// </summary>
    /// <param name="tokens">A list of lexed and translated input tokens.</param>
    /// <param name="index">The index of the token to learn.</param>
    public override void LearnToken(in IReadOnlyList<int> tokens, int index)
    {
        var ngram = GetNGram(tokens, index);
        for (var i = 0; i < ngram.Count; i++)
            Counter.IncrementCount(ngram.Skip(i).ToArray());
    }

    /// <summary>
    /// Instructs the model to forget the provided tokens.
    /// </summary>
    /// <param name="tokens">A list of lexed and translated input tokens.</param>
    public void ForgetTokens(in IReadOnlyList<int> tokens)
    {
        Counter.DecrementCountBatch(GetNGrams(tokens));
    }

    /// <summary>
    /// Instructs the model to forget a specific token at the given index.
    /// </summary>
    /// <param name="tokens">A list of lexed and translated input tokens.</param>
    /// <param name="index">The index of the token to forget.</param>
    public override void ForgetToken(in IReadOnlyList<int> tokens, int index)
    {
        var ngram = GetNGram(tokens, index);
        for (var i = 0; i < ngram.Count; i++)
            Counter.DecrementCount(ngram.Skip(i).ToArray());
    }

    /// <summary>
    /// Internal implementation of modeling a single token at the specified index. This method should be overridden by derived classes.
    /// </summary>
    /// <param name="tokens">A list of lexed and translated input tokens.</param>
    /// <param name="index">The index of the token to model.</param>
    /// <returns>Probability and confidence values for the token at the specified index.</returns>
    protected override (double Probability, double Confidence) ModelTokenInternal(in IReadOnlyList<int> tokens, int index)
    {
        var ngram = GetNGram(tokens, index);
        double probability = 0;
        double mass = 0;
        int hits = 0;

        for (var i = ngram.Count; i >= 0; i--)
        {
            var n = ngram.Skip(i).ToArray();
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
    /// Internal implementation of generating predictions for a single token at the specified index. This method should be overridden by derived classes.
    /// </summary>
    /// <param name="tokens">A list of lexed and translated input tokens.</param>
    /// <param name="index">The index of the token to predict.</param>
    /// <returns>A dictionary containing the top N predictions for the token at the specified index with their probability and confidence values.</returns>
    protected override IReadOnlyDictionary<int, (double Probability, double Confidence)> PredictTokenInternal(in IReadOnlyList<int> tokens, int index)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns a list of all n-grams in the provided tokens.
    /// </summary>
    /// <param name="tokens">A list of lexed and translated input tokens.</param>
    /// <returns>A list of all n-grams in the provided tokens.</returns>
    private IReadOnlyList<IReadOnlyList<int>> GetNGrams(in IReadOnlyList<int> tokens)
    {
        int[][] ngrams = new int[tokens.Count][];
        for (var i = 0; i < tokens.Count; i++)
        {
            int take = Math.Min(Order, tokens.Count - i);
            ngrams[i] = tokens.Skip(i).Take(take).ToArray();
        }
        return ngrams;
    }

    /// <summary>
    /// Returns an n-gram of the provided tokens starting at the specified index.
    /// </summary>
    /// <param name="tokens">A list of lexed and translated input tokens.</param>
    /// <param name="index">The starting index of the n-gram.</param>
    /// <returns>An n-gram of the provided tokens starting at the specified index.</returns>
    private IReadOnlyList<int> GetNGram(in IReadOnlyList<int> tokens, int index)
    {
        int take = Math.Min(Order, tokens.Count - index);
        return tokens.Skip(index).Take(take).ToArray();
    }
}

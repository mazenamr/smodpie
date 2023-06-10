namespace Smodpie.Model;

/// <summary>
/// Interface for modeling input tokens.
/// It provides methods for learning, forgetting, modeling, and predicting tokens in the input.
/// </summary>
public interface IModel
{
    /// <summary>
    /// Instructs the model to learn the provided tokens.
    /// </summary>
    /// <param name="tokens">A list of lexed and translated input tokens.</param>
    public void LearnTokens(in IReadOnlyList<int> tokens)
    {
        for (int i = 0; i < tokens.Count; i++)
            LearnToken(tokens, i);
    }

    /// <summary>
    /// Instructs the model to learn a specific token at the given index.
    /// </summary>
    /// <param name="tokens">A list of lexed and translated input tokens.</param>
    /// <param name="index">The index of the token to learn.</param>
    public void LearnToken(in IReadOnlyList<int> tokens, int index);

    /// <summary>
    /// Instructs the model to forget the provided tokens.
    /// </summary>
    /// <param name="tokens">A list of lexed and translated input tokens.</param>
    public void ForgetTokens(in IReadOnlyList<int> tokens)
    {
        for (int i = 0; i < tokens.Count; i++)
            ForgetToken(tokens, i);
    }

    /// <summary>
    /// Instructs the model to forget a specific token at the given index.
    /// </summary>
    /// <param name="tokens">A list of lexed and translated input tokens.</param>
    /// <param name="index">The index of the token to forget.</param>
    public void ForgetToken(in IReadOnlyList<int> tokens, int index);

    /// <summary>
    /// Models each token in the input and returns a list of probability and confidence values.
    /// </summary>
    /// <param name="tokens">A list of lexed and translated input tokens.</param>
    /// <returns>A list of probability and confidence values for each token in the input.</returns>
    public IReadOnlyList<(double Probability, double Confidence)> ModelTokens(in IReadOnlyList<int> tokens)
    {
        var result = new (double Probability, double Confidence)[tokens.Count];
        for (int i = 0; i < tokens.Count; i++)
            result[i] = ModelToken(tokens, i);
        return result;
    }

    /// <summary>
    /// Models a single token at the specified index and returns its probability and confidence values.
    /// </summary>
    /// <param name="tokens">A list of lexed and translated input tokens.</param>
    /// <param name="index">The index of the token to model.</param>
    /// <returns>Probability and confidence values for the token at the specified index.</returns>
    public (double Probability, double Confidence) ModelToken(in IReadOnlyList<int> tokens, int index);

    /// <summary>
    /// Generates predictions for each token in the input and returns a list of dictionaries with probability and confidence values.
    /// </summary>
    /// <param name="tokens">A list of lexed and translated input tokens.</param>
    /// <returns>A list of dictionaries containing the top N predictions for each token in the input with their probability and confidence values.</returns>
    public IReadOnlyList<IReadOnlyDictionary<int, (double Probability, double Confidence)>> PredictTokens(in IReadOnlyList<int> tokens)
    {
        var result = new IReadOnlyDictionary<int, (double Probability, double Confidence)>[tokens.Count];
        for (int i = 0; i < tokens.Count; i++)
            result[i] = PredictToken(tokens, i);
        return result;
    }

    /// <summary>
    /// Generates predictions for a single token at the specified index and returns a dictionary with probability and confidence values.
    /// </summary>
    /// <param name="tokens">A list of lexed and translated input tokens.</param>
    /// <param name="index">The index of the token to predict.</param>
    /// <returns>A dictionary containing the top N predictions for the token at the specified index with their probability and confidence values.</returns>
    public IReadOnlyDictionary<int, (double Probability, double Confidence)> PredictToken(in IReadOnlyList<int> tokens, int index);
}

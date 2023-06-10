namespace Smodpie.Model;

/// <summary>
/// An abstract class representing a model that can learn, forget, model, and predict input tokens.
/// </summary>
/// <remarks>
/// This class adds the ability for online learning, which is the ability to learn from the input tokens as they are modeled.
/// </remarks>
public abstract class OnlineModel : IModel
{
    private enum OnlineLearningStatus
    {
        Disabled,
        Paused,
        Enabled
    }

    private OnlineLearningStatus _onlineLearning = OnlineLearningStatus.Disabled;
    private int _onlineLearningDepth = 0;

    /// <summary>
    /// Gets or sets a value indicating whether online learning is enabled.
    /// </summary>
    public bool OnlineLearning
    {
        get => _onlineLearning == OnlineLearningStatus.Enabled;
        set
        {
            _onlineLearning = value ?
                OnlineLearningStatus.Enabled :
                OnlineLearningStatus.Disabled;
            _onlineLearningDepth = 0;
        }
    }

    /// <summary>
    /// Initializes a new instance of the Model class with the specified online learning setting.
    /// </summary>
    /// <param name="onlineLearning">A boolean value indicating whether online learning should be enabled.</param>
    public OnlineModel(bool onlineLearning = false)
    {
        OnlineLearning = onlineLearning;
    }

    /// <summary>
    /// Pauses online learning by incrementing the online learning depth and setting the online learning status to paused.
    /// </summary>
    private void PauseOnlineLearning()
    {
        _onlineLearningDepth++;
        _onlineLearning = OnlineLearningStatus.Paused;
    }

    /// <summary>
    /// Resumes online learning by decrementing the online learning depth and setting the online learning status to enabled if the depth is zero.
    /// </summary>
    private void ResumeOnlineLearning()
    {
        _onlineLearningDepth = Math.Max(0, _onlineLearningDepth - 1);
        if (_onlineLearningDepth == 0 && _onlineLearning != OnlineLearningStatus.Paused)
        {
            _onlineLearning = OnlineLearningStatus.Enabled;
        }
    }

    /// <summary>
    /// Instructs the model to learn a specific token at the given index.
    /// </summary>
    /// <param name="tokens">A list of lexed and translated input tokens.</param>
    /// <param name="index">The index of the token to learn.</param>
    abstract public void LearnToken(in IReadOnlyList<int> tokens, int index);

    /// <summary>
    /// Instructs the model to forget the provided tokens.
    /// </summary>
    /// <param name="tokens">A list of lexed and translated input tokens.</param>
    abstract public void ForgetToken(in IReadOnlyList<int> tokens, int index);

    /// <summary>
    /// Models a single token at the specified index and returns its probability and confidence values.
    /// </summary>
    /// <param name="tokens">A list of lexed and translated input tokens.</param>
    /// <param name="index">The index of the token to model.</param>
    /// <returns>Probability and confidence values for the token at the specified index.</returns>
    public (double Probability, double Confidence) ModelToken(in IReadOnlyList<int> tokens, int index)
    {
        var result = ModelTokenInternal(tokens, index);

        if (OnlineLearning)
            LearnToken(tokens, index);

        return result;
    }

    /// <summary>
    /// Generates predictions for a single token at the specified index and returns a dictionary with probability and confidence values.
    /// </summary>
    /// <param name="tokens">A list of lexed and translated input tokens.</param>
    /// <param name="index">The index of the token to predict.</param>
    /// <returns>A dictionary containing the top N predictions for the token at the specified index with their probability and confidence values.</returns>
    public IReadOnlyDictionary<int, (double Probability, double Confidence)> PredictToken(in IReadOnlyList<int> tokens, int index)
    {
        PauseOnlineLearning();
        var result = PredictTokenInternal(tokens, index);
        ResumeOnlineLearning();

        if (OnlineLearning)
            LearnToken(tokens, index);

        return result;
    }

    /// <summary>
    /// Internal implementation of modeling a single token at the specified index. This method should be overridden by derived classes.
    /// </summary>
    /// <param name="tokens">A list of lexed and translated input tokens.</param>
    /// <param name="index">The index of the token to model.</param>
    /// <returns>Probability and confidence values for the token at the specified index.</returns>
    abstract protected (double Probability, double Confidence) ModelTokenInternal(in IReadOnlyList<int> tokens, int index);

    /// <summary>
    /// Internal implementation of generating predictions for a single token at the specified index. This method should be overridden by derived classes.
    /// </summary>
    /// <param name="tokens">A list of lexed and translated input tokens.</param>
    /// <param name="index">The index of the token to predict.</param>
    /// <returns>A dictionary containing the top N predictions for the token at the specified index with their probability and confidence values.</returns>
    abstract protected IReadOnlyDictionary<int, (double Probability, double Confidence)> PredictTokenInternal(in IReadOnlyList<int> tokens, int index);
}

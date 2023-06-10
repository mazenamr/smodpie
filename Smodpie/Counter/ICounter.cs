namespace Smodpie.Counter;

/// <summary>
/// Interface for counters for models that use frequency counts of sequences of tokens.
/// </summary>
/// <remarks>
/// A Counter is responsible for counting sequences of tokens and providing access
/// to various statistics related to those counts. It provides methods for incrementing
/// and decrementing counts, as well as for retrieving counts and statistics for
/// specific sequences of tokens.
/// Count-based models use Counters to build statistical models of language, where
/// the frequency of occurrence of different sequences of tokens is used to predict
/// the likelihood of future sequences. Counters are used to store and manipulate
/// these frequency counts, and to provide access to various statistics that can be
/// used to build and evaluate models.
/// </remarks>
public interface ICounter
{
    /// <summary>
    /// Gets the count of the sequence of tokens represented by the given list of indices.
    /// </summary>
    /// <param name="sequence">A list of stored, translated tokens representing the sequence to count.</param>
    /// <returns>A tuple containing the context count and the count of the sequence of tokens represented by the given list of indices.</returns>
    public (long ContextCount, long Count) GetCounts(in IEnumerable<int> sequence);

    /// <summary>
    /// Gets the number of unique successors of the context represented by the given list of indices.
    /// </summary>
    /// <param name="context">A list of stored, translated tokens representing the context to consider.</param>
    /// <returns>The number of unique successors of the context.</returns>
    public int GetSuccessorCount(in IEnumerable<int> context);

    /// <summary>
    /// Gets a list of the top successors of the context represented by the given list of indices sorted in descending order of their counts.
    /// </summary>
    /// <param name="context">A list of stored, translated tokens representing the context to consider.</param>
    /// <param name="limit">The maximum number of successors to return.</param>
    /// <returns>A list of the top successors of the context, sorted in descending order of their counts.</returns>
    public IReadOnlyList<int> GetTopSuccessors(in IEnumerable<int> context, int limit);

    /// <summary>
    /// Gets the number of sequences of length n that have been seen exactly 'count' times.
    /// </summary>
    /// <param name="n">The length of the sequences to consider.</param>
    /// <param name="count">The exact number of times the sequences should have been seen.</param>
    /// <returns>The number of sequences of length n seen 'count' times.</returns>
    public int GetNumberOfSequencesWithCount(int n, int count);

    /// <summary>
    /// Gets a dictionary of counts of the number of distinct sequences of length n that have been seen exactly 'count' times, for all counts in the range [1, range].
    /// </summary>
    /// <param name="range">The maximum count to consider.</param>
    /// <param name="context">A list of stored, translated tokens representing the context to consider.</param>
    /// <returns>A dictionary of counts of the number of distinct sequences of length n seen 'count' times, for all counts in the range [1, range].</returns>
    public Dictionary<int, int> GetDistinctSequenceCounts(int range, in IEnumerable<int> context);

    /// <summary>
    /// Increments the count of the sequence of tokens represented by the given list of indices.
    /// </summary>
    /// <param name="sequence">A list of stored, translated tokens representing the sequence to count.</param>
    public void IncrementCount(in IEnumerable<int> sequence);

    /// <summary>
    /// Increments the count of each sequence of tokens represented by the given list of indices.
    /// </summary>
    /// <param name="sequence">A list of lists of stored, translated tokens representing the sequences to count.</param>
    public void IncrementCountBatch(in IEnumerable<IEnumerable<int>> sequence)
    {
        if (sequence == null)
            throw new ArgumentNullException(nameof(sequence));

        Parallel.ForEach(sequence, index => IncrementCount(index));
    }

    /// <summary>
    /// Increments the count of each sequence of tokens represented by the given list of indices asynchronously.
    /// </summary>
    /// <param name="sequence">A list of lists of stored, translated tokens representing the sequences to count.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task IncrementCountBatchAsync(IEnumerable<IEnumerable<int>> sequence)
    {
        if (sequence == null)
            throw new ArgumentNullException(nameof(sequence));

        await Parallel.ForEachAsync(sequence, async (index, cancellationToken) =>
        {
            await Task.Run(() => IncrementCount(index));
        });
    }

    /// <summary>
    /// Decrements the count of the sequence of tokens represented by the given list of indices.
    /// The count will not be decremented below 0, if the count is already 0, it will not be changed.
    /// </summary>
    /// <param name="sequence">A list of stored, translated tokens representing the sequence to uncount.</param>
    public void DecrementCount(in IEnumerable<int> sequence);

    /// <summary>
    /// Decrements the count of each sequence of tokens represented by the given list of indices.
    /// </summary>
    /// <param name="sequence">A list of lists of stored, translated tokens representing the sequences to uncount.</param>
    public void DecrementCountBatch(in IEnumerable<IEnumerable<int>> sequence)
    {
        if (sequence == null)
            throw new ArgumentNullException(nameof(sequence));

        Parallel.ForEach(sequence, index => DecrementCount(index));
    }

    /// <summary>
    /// Decrements the count of each sequence of tokens represented by the given list of indices asynchronously.
    /// </summary>
    /// <param name="sequence">A list of lists of stored, translated tokens representing the sequences to uncount.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task DecrementCountBatchAsync(IEnumerable<IEnumerable<int>> sequence)
    {
        if (sequence == null)
            throw new ArgumentNullException(nameof(sequence));

        await Parallel.ForEachAsync(sequence, async (index, cancellationToken) =>
        {
            await Task.Run(() => DecrementCount(index));
        });
    }

    /// <summary>
    /// Sets the count of the sequence of tokens represented by the given list of indices.
    /// </summary>
    /// <param name="sequence">A list of stored, translated tokens representing the sequence to set the count for.</param>
    /// <param name="count">The count to set.</param>
    public void SetCount(in IEnumerable<int> sequence, long count);

    /// <summary>
    /// Clears all counts by setting them to 0.
    /// </summary>
    public void ClearCounts();

    /// <summary>
    /// Creates a new instance of the counter.
    /// </summary>
    /// <returns>A new instance of the counter.</returns>
    public ICounter NewInstance();
}

namespace Smodpie.Counter;

public class TrieCounter : ICounter
{
    private readonly TrieNode _root;

    public TrieCounter()
    {
        _root = new TrieNode();
    }

    /// <summary>
    /// Gets the count of the sequence of tokens represented by the given list of indices.
    /// </summary>
    /// <param name="sequence">A list of stored, translated tokens representing the sequence to count.</param>
    /// <returns>A tuple containing the context count and the count of the sequence of tokens represented by the given list of indices.</returns>
    public (long ContextCount, long Count) GetCounts(in IEnumerable<int> sequence)
    {
        if (sequence == null)
            throw new ArgumentNullException(nameof(sequence));

        TrieNode node = _root;
        foreach (int index in sequence)
        {
            if (node.Children.TryGetValue(index, out TrieNode? child))
                node = child;
            else
                return (0, 0);
        }

        return (node.ContextCount, node.Count);
    }

    /// <summary>
    /// Gets the number of unique successors of the context represented by the given list of indices.
    /// </summary>
    /// <param name="context">A list of stored, translated tokens representing the context to consider.</param>
    /// <returns>The number of unique successors of the context.</returns>
    public int GetSuccessorCount(in IEnumerable<int> context)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        TrieNode node = _root;
        foreach (int index in context)
        {
            if (node.Children.TryGetValue(index, out TrieNode? child))
                node = child;
            else
                return 0;
        }

        return node.Children.Count;
    }

    /// <summary>
    /// Gets a list of the top successors of the context represented by the given list of indices sorted in descending order of their counts.
    /// </summary>
    /// <param name="context">A list of stored, translated tokens representing the context to consider.</param>
    /// <param name="limit">The maximum number of successors to return.</param>
    /// <returns>A list of the top successors of the context, sorted in descending order of their counts.</returns>
    public IReadOnlyList<int> GetTopSuccessors(in IEnumerable<int> context, int limit)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));
        if (limit < 0)
            throw new ArgumentOutOfRangeException(nameof(limit));

        TrieNode node = _root;
        foreach (int index in context)
        {
            if (node.Children.TryGetValue(index, out TrieNode? child))
                node = child;
            else
                return new List<int>();
        }

        var topSuccessors = new SortedSet<(int Key, long Count)>(
            Comparer<(int Key, long Count)>.Create((x, y) =>
                y.Count.CompareTo(x.Count) == 0 ?
                    x.Key.CompareTo(y.Key) : y.Count.CompareTo(x.Count)));

        foreach (var child in node.Children)
        {
            topSuccessors.Add((child.Key, child.Value.Count));
            if (topSuccessors.Count > limit)
                topSuccessors.Remove(topSuccessors.Max);
        }

        return topSuccessors.Select(x => x.Key).ToList();
    }

    /// <summary>
    /// Gets a dictionary of counts of the number of distinct sequences of length n that have been seen exactly 'count' times, for all counts in the range [1, range].
    /// </summary>
    /// <param name="range">The maximum count to consider.</param>
    /// <param name="context">A list of stored, translated tokens representing the context to consider.</param>
    /// <returns>A dictionary of counts of the number of distinct sequences of length n seen 'count' times, for all counts in the range [1, range].</returns>
    public int GetNumberOfSequencesWithCount(int n, int count)
    {
        if (n < 0)
            throw new ArgumentOutOfRangeException(nameof(n));
        if (count < 0)
            throw new ArgumentOutOfRangeException(nameof(count));

        return _root.GetNumberOfSequencesWithCount(n, count, new Dictionary<(int, int, TrieNode), int>());
    }

    /// <summary>
    /// Sets the count of the sequence of tokens represented by the given list of indices.
    /// </summary>
    /// <param name="sequence">A list of stored, translated tokens representing the sequence to set the count for.</param>
    /// <param name="count">The count to set.</param>
    public Dictionary<int, int> GetDistinctSequenceCounts(int range, in IEnumerable<int> context)
    {
        if (range < 0)
            throw new ArgumentOutOfRangeException(nameof(range));
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        TrieNode node = _root;
        foreach (int index in context)
        {
            if (node.Children.TryGetValue(index, out TrieNode? child))
                node = child;
            else
                return new Dictionary<int, int>();
        }

        Dictionary<int, int> result = new Dictionary<int, int>();
        for (int i = 1; i <= range; i++)
            result[i] = node.GetNumberOfSequencesWithCount(1, i, new Dictionary<(int, int, TrieNode), int>());

        return result;
    }

    /// <summary>
    /// Increments the count of the sequence of tokens represented by the given list of indices.
    /// </summary>
    /// <param name="sequence">A list of stored, translated tokens representing the sequence to count.</param>
    public void IncrementCount(in IEnumerable<int> sequence)
    {
        if (sequence == null)
            throw new ArgumentNullException(nameof(sequence));

        TrieNode node = _root;

        foreach (int index in sequence)
        {
            if (!node.Children.ContainsKey(index))
                node.AddChild(index);

            node.UpdateCount(1);
            node = node.Children[index];
        }

        node.UpdateCount(1);
    }

    /// <summary>
    /// Decrements the count of the sequence of tokens represented by the given list of indices.
    /// The count will not be decremented below 0, if the count is already 0, it will not be changed.
    /// </summary>
    /// <param name="sequence">A list of stored, translated tokens representing the sequence to uncount.</param>
    public void DecrementCount(in IEnumerable<int> sequence)
    {
        if (sequence == null)
            throw new ArgumentNullException(nameof(sequence));

        TrieNode node = _root;

        foreach (int index in sequence)
        {
            if (node.Children.TryGetValue(index, out TrieNode? child))
                node = child;
            else return;
        }

        node = _root;

        foreach (int index in sequence)
        {
            node.UpdateCount(-1);
            node = node.Children[index];
        }

        node.UpdateCount(-1);
    }

    /// <summary>
    /// Sets the count of the sequence of tokens represented by the given list of indices.
    /// </summary>
    /// <param name="sequence">A list of stored, translated tokens representing the sequence to set the count for.</param>
    /// <param name="count">The count to set.</param>
    public void SetCount(in IEnumerable<int> sequence, long count)
    {
        if (sequence == null)
            throw new ArgumentNullException(nameof(sequence));
        if (count < 0)
            throw new ArgumentOutOfRangeException(nameof(count));

        TrieNode node = _root;
        long diff = count - node.Count;

        if (diff == 0)
            return;

        foreach (int index in sequence)
        {
            if (!node.Children.ContainsKey(index))
                node.AddChild(index);

            node.UpdateCount(diff);
            node = node.Children[index];
        }

        node.UpdateCount(diff);
    }

    /// <summary>
    /// Clears all counts by setting them to 0.
    /// </summary>
    public void ClearCounts()
    {
        _root.Clear();
    }

    /// <summary>
    /// Creates a new instance of the counter.
    /// </summary>
    /// <returns>A new instance of the counter.</returns>
    public ICounter NewInstance()
    {
        return new TrieCounter();
    }

    private class TrieNode
    {
        public long Count { get; private set; }
        public long ContextCount { get; private set; }
        public Dictionary<int, TrieNode> Children { get; }

        private readonly object _updateLock = new object();
        private readonly object _addChildLock = new object();

        public TrieNode()
        {
            Count = 0;
            ContextCount = 0;
            Children = new Dictionary<int, TrieNode>();
        }

        /// <summary>
        /// Updates the count of the TrieNode and its children by adding the specified difference to the current count.
        /// If the difference is 0, the method does nothing.
        /// If the difference is negative and greater than the current count, the count will be set to 0.
        /// </summary>
        /// <param name="diff">The difference to add to the current count.</param>
        public void UpdateCount(long diff)
        {
            lock (_updateLock)
            {
                if (diff == 0)
                    return;

                diff = Math.Max(-Count, diff);

                Count += diff;

                foreach (TrieNode child in Children.Values)
                    child.ContextCount += diff;
            }
        }

        /// <summary>
        /// Adds a child TrieNode to the current TrieNode with the given index.
        /// If a child with the same index already exists, the method does nothing.
        /// </summary>
        /// <param name="index">The index of the child to add.</param>
        public void AddChild(int index)
        {
            lock (_addChildLock)
            {
                if (!Children.ContainsKey(index))
                    Children[index] = new()
                    {
                        Count = 0,
                        ContextCount = Count
                    };
            }
        }

        /// <summary>
        /// Clears the TrieNode and its children by setting their counts to 0 and removing all children.
        /// </summary>
        public void Clear()
        {
            Count = 0;
            ContextCount = 0;

            foreach (TrieNode child in Children.Values)
                child.Clear();

            Children.Clear();
        }

        /// <summary>
        /// Gets the number of sequences with the specified count and length.
        /// </summary>
        /// <param name="n">The length of the sequences to count.</param>
        /// <param name="count">The count of the sequences to count.</param>
        /// <returns>The number of sequences with the specified count and length.</returns>
        public int GetNumberOfSequencesWithCount(int n, int count, Dictionary<(int, int, TrieNode), int> memo)
        {
            if (n == 0)
                return Count == count ? 1 : 0;

            if (memo.ContainsKey((n, count, this)))
                return memo[(n, count, this)];

            int result = 0;
            foreach (TrieNode child in Children.Values)
                result += child.GetNumberOfSequencesWithCount(n - 1, count, memo);

            memo[(n, count, this)] = result;
            return result;
        }
    }
}

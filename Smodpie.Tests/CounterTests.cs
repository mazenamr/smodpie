namespace Smodpie.Counter.Tests;

public class CounterTests
{
    [Fact]
    public void TestIncrementCount()
    {
        // Arrange
        ICounter counter = new TrieCounter();
        var sequence1 = new List<int> { 1, 2, 3 };
        var sequence2 = new List<int> { 1, 2, 4 };

        // Act
        counter.IncrementCount(sequence1);
        counter.IncrementCount(sequence1);
        counter.IncrementCount(sequence2);
        var counts1 = counter.GetCounts(sequence1);
        var counts2 = counter.GetCounts(sequence2);

        // Assert
        Assert.Equal(2, counts1.Count);
        Assert.Equal(1, counts2.Count);
        Assert.Equal(3, counts1.ContextCount);
        Assert.Equal(3, counts2.ContextCount);
    }

    [Fact]
    public void TestDecrementCount()
    {
        // Arrange
        ICounter counter = new TrieCounter();
        var sequence = new List<int> { 1, 2, 3 };
        counter.IncrementCount(sequence);

        // Act
        counter.DecrementCount(sequence);
        var counts = counter.GetCounts(sequence);

        // Assert
        Assert.Equal(0, counts.Count);
        Assert.Equal(0, counts.ContextCount);
    }

    [Fact]
    public void TestDecrementBelowZeroCount()
    {
        // Arrange
        ICounter counter = new TrieCounter();
        var sequence = new List<int> { 1, 2, 3 };
        counter.IncrementCount(sequence);

        // Act
        counter.DecrementCount(sequence);
        counter.DecrementCount(sequence);
        counter.DecrementCount(sequence);
        var counts = counter.GetCounts(sequence);

        // Assert
        Assert.Equal(0, counts.Count);
        Assert.Equal(0, counts.ContextCount);
    }

    [Fact]
    public void TestIncrementDecrementCount()
    {
        // Arrange
        ICounter counter = new TrieCounter();
        var sequence1 = new List<int> { 1, 2, 3 };
        var sequence2 = new List<int> { 1, 2, 4 };

        // Act
        counter.IncrementCount(sequence1);
        counter.IncrementCount(sequence1);
        counter.IncrementCount(sequence2);
        counter.IncrementCount(sequence2);
        counter.IncrementCount(sequence2);
        counter.DecrementCount(sequence1);
        var counts1 = counter.GetCounts(sequence1);
        var counts2 = counter.GetCounts(sequence2);

        // Assert
        Assert.Equal(1, counts1.Count);
        Assert.Equal(3, counts2.Count);
        Assert.Equal(4, counts1.ContextCount);
        Assert.Equal(4, counts2.ContextCount);
    }

    [Fact]
    public void TestSetCount()
    {
        // Arrange
        ICounter counter = new TrieCounter();
        var sequence = new List<int> { 1, 2, 3 };

        // Act
        counter.SetCount(sequence, 5);
        var counts = counter.GetCounts(sequence);

        // Assert
        Assert.Equal(5, counts.Count);
        Assert.Equal(5, counts.ContextCount);
    }

    [Fact]
    public void TestClearCounts()
    {
        // Arrange
        ICounter counter = new TrieCounter();
        var sequence = new List<int> { 1, 2, 3 };
        counter.IncrementCount(sequence);

        // Act
        counter.ClearCounts();
        var counts = counter.GetCounts(sequence);

        // Assert
        Assert.Equal(0, counts.Count);
    }

    [Fact]
    public void TestGetNumberOfSequencesWithCount()
    {
        // Arrange
        ICounter counter = new TrieCounter();
        var sequence1 = new List<int> { 1, 2, 3 };
        var sequence2 = new List<int> { 4, 5, 6 };
        counter.IncrementCount(sequence1);
        counter.IncrementCount(sequence2);

        // Act
        int result1 = counter.GetNumberOfSequencesWithCount(3, 1);
        int result2 = counter.GetNumberOfSequencesWithCount(2, 1);

        // Assert
        Assert.Equal(2, result1);
        Assert.Equal(2, result2);
    }

    [Fact]
    public void TestGetSuccessorCount()
    {
        // Arrange
        ICounter counter = new TrieCounter();
        var context = new List<int> { 1, 2 };
        var sequence1 = new List<int> { 1, 2, 3 };
        var sequence2 = new List<int> { 1, 2, 4 };
        counter.IncrementCount(sequence1);
        counter.IncrementCount(sequence2);

        // Act
        int result = counter.GetSuccessorCount(context);

        // Assert
        Assert.Equal(2, result);
    }

    [Fact]
    public void TestGetTopSuccessors()
    {
        // Arrange
        ICounter counter = new TrieCounter();
        var context = new List<int> { 1, 2 };
        var sequence1 = new List<int> { 1, 2, 3 };
        var sequence2 = new List<int> { 1, 2, 4 };
        counter.IncrementCount(sequence1);
        counter.IncrementCount(sequence2);
        counter.IncrementCount(sequence2);

        // Act
        IReadOnlyList<int> result = counter.GetTopSuccessors(context, 2);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal(4, result[0]);
        Assert.Equal(3, result[1]);
    }

    [Fact]
    public void TestGetDistinctSequenceCounts()
    {
        // Arrange
        ICounter counter = new TrieCounter();
        var context = new List<int> { 1, 2 };
        var sequence1 = new List<int> { 1, 2, 3 };
        var sequence2 = new List<int> { 1, 2, 4 };
        counter.IncrementCount(sequence1);
        counter.IncrementCount(sequence2);
        counter.IncrementCount(sequence2);

        // Act
        Dictionary<int, int> result = counter.GetDistinctSequenceCounts(3, context);

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal(1, result[1]);
        Assert.Equal(1, result[2]);
    }

    [Fact]
    public void TestIncrementCountBatch()
    {
        // Arrange
        ICounter counter = new TrieCounter();
        var sequences = new List<IEnumerable<int>>
        {
            new List<int> { 1, 2, 3 },
            new List<int> { 4, 5, 6 }
        };

        // Act
        counter.IncrementCountBatch(sequences);
        var counts1 = counter.GetCounts(sequences[0]);
        var counts2 = counter.GetCounts(sequences[1]);

        // Assert
        Assert.Equal(1, counts1.Count);
        Assert.Equal(1, counts2.Count);
    }

    [Fact]
    public void TestDecrementCountBatch()
    {
        // Arrange
        ICounter counter = new TrieCounter();
        var sequences = new List<IEnumerable<int>>
        {
            new List<int> { 1, 2, 3 },
            new List<int> { 4, 5, 6 }
        };
        counter.IncrementCountBatch(sequences);

        // Act
        counter.DecrementCountBatch(sequences);
        var counts1 = counter.GetCounts(sequences[0]);
        var counts2 = counter.GetCounts(sequences[1]);

        // Assert
        Assert.Equal(0, counts1.Count);
        Assert.Equal(0, counts2.Count);
    }

    [Fact]
    public void TestIncrementAndDecrementCountComplex()
    {
        // Arrange
        ICounter counter = new TrieCounter();
        var sequence1 = new List<int> { 1, 2, 3 };
        var sequence2 = new List<int> { 1, 2, 4 };
        var sequence3 = new List<int> { 4, 5, 6 };

        var context1 = new List<int> { 1, 2 };
        var context2 = new List<int> { 4 };

        // Act
        counter.IncrementCount(sequence1);
        counter.IncrementCount(sequence1);
        counter.IncrementCount(sequence2);
        counter.DecrementCount(context1);
        counter.IncrementCount(sequence3);
        counter.IncrementCount(sequence3);
        counter.DecrementCount(context2);
        counter.IncrementCount(context1);

        var counts1 = counter.GetCounts(sequence1);
        var counts2 = counter.GetCounts(sequence2);
        var counts3 = counter.GetCounts(sequence3);

        // Assert
        Assert.Equal(2, counts1.Count);
        Assert.Equal(1, counts2.Count);
        Assert.Equal(2, counts3.Count);
        Assert.Equal(3, counts1.ContextCount);
        Assert.Equal(3, counts2.ContextCount);
        Assert.Equal(2, counts3.ContextCount);

    }

    [Fact]
    public void TestGetTopSuccessorsComplex()
    {
        // Arrange
        ICounter counter = new TrieCounter();
        var context = new List<int> { 1, 2 };
        var sequence1 = new List<int> { 1, 2, 3 };
        var sequence2 = new List<int> { 1, 2, 4 };
        var sequence3 = new List<int> { 1, 2, 5 };
        counter.IncrementCount(sequence1);
        counter.IncrementCount(sequence1);
        counter.IncrementCount(sequence2);
        counter.IncrementCount(sequence2);
        counter.IncrementCount(sequence2);
        counter.IncrementCount(sequence3);

        // Act
        IReadOnlyList<int> result = counter.GetTopSuccessors(context, 3);

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal(4, result[0]);
        Assert.Equal(3, result[1]);
        Assert.Equal(5, result[2]);
    }

    [Fact]
    public void TestGetDistinctSequenceCountsComplex()
    {
        // Arrange
        ICounter counter = new TrieCounter();
        var context = new List<int> { 1, 2 };
        var sequence1 = new List<int> { 1, 2, 3 };
        var sequence2 = new List<int> { 1, 2, 4 };
        var sequence3 = new List<int> { 1, 2, 5 };
        counter.IncrementCount(sequence1);
        counter.IncrementCount(sequence1);
        counter.IncrementCount(sequence2);
        counter.IncrementCount(sequence2);
        counter.IncrementCount(sequence2);
        counter.IncrementCount(sequence3);

        // Act
        Dictionary<int, int> result = counter.GetDistinctSequenceCounts(3, context);

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal(1, result[1]);
        Assert.Equal(1, result[2]);
        Assert.Equal(1, result[3]);
    }

    [Fact]
    public void TestBatchOperationsComplex()
    {
        // Arrange
        ICounter counter = new TrieCounter();
        var sequences = new List<IEnumerable<int>>
            {
                new List<int> { 1, 2, 3 },
                new List<int> { 1, 2, 4 },
                new List<int> { 4, 5, 6 }
            };

        // Act
        counter.IncrementCountBatch(sequences);
        counter.IncrementCountBatch(sequences);
        counter.DecrementCountBatch(sequences);

        var counts1 = counter.GetCounts(sequences[0]);
        var counts2 = counter.GetCounts(sequences[1]);
        var counts3 = counter.GetCounts(sequences[2]);

        // Assert
        Assert.Equal(1, counts1.Count);
        Assert.Equal(1, counts2.Count);
        Assert.Equal(1, counts3.Count);
        Assert.Equal(2, counts1.ContextCount);
        Assert.Equal(2, counts2.ContextCount);
        Assert.Equal(1, counts3.ContextCount);
    }

    [Fact]
    public void TestBatchOperationsBig()
    {
        // Arrange
        ICounter counter = new TrieCounter();
        var sequence1 = new List<int> { 1, 2, 3 };
        var sequence2 = new List<int> { 1, 2, 4 };
        var sequence3 = new List<int> { 4, 5, 6 };

        var sequences = new List<IEnumerable<int>>();

        for (int i = 0; i < 10000000; i++)
        {
            sequences.Add(sequence1);
            sequences.Add(sequence2);
            sequences.Add(sequence3);
        }

        // Act
        counter.IncrementCountBatch(sequences);
        counter.IncrementCountBatch(sequences);
        counter.DecrementCountBatch(sequences);

        var counts1 = counter.GetCounts(sequence1);
        var counts2 = counter.GetCounts(sequence2);
        var counts3 = counter.GetCounts(sequence3);

        // Assert
        Assert.Equal(10000000, counts1.Count);
        Assert.Equal(10000000, counts2.Count);
        Assert.Equal(10000000, counts3.Count);
        Assert.Equal(20000000, counts1.ContextCount);
        Assert.Equal(20000000, counts2.ContextCount);
        Assert.Equal(10000000, counts3.ContextCount);
    }
}

using Smodpie.Config;
using Smodpie.Counter;

namespace Smodpie.Model.NGram;

public static class Smoothing
{
    public enum SmoothingTypes
    {
        JelinekMercer,
        WittenBell,
        AbsoluteDiscounting,
        KneserNey,
        ModifiedKneserNey,
        AbsoluteDiscountingModified
    }

    public static (double Probability, double Confidence) Smooth(SmoothingTypes type, (long ContextCount, long Count) counts, in IReadOnlyList<int> tokens, in ICounter counter)
    {
        return type switch
        {
            SmoothingTypes.JelinekMercer => JelinekMercer(counts, tokens, counter),
            SmoothingTypes.WittenBell => WittenBell(counts, tokens, counter),
            SmoothingTypes.AbsoluteDiscounting => AbsoluteDiscounting(counts, tokens, counter),
            SmoothingTypes.KneserNey => KneserNey(counts, tokens, counter),
            SmoothingTypes.ModifiedKneserNey => ModifiedKneserNey(counts, tokens, counter),
            SmoothingTypes.AbsoluteDiscountingModified => AbsoluteDiscountingModified(counts, tokens, counter),
            _ => throw new ArgumentException($"Unknown smoothing type: {type}"),
        };
    }

    public static (double Probability, double Confidence) JelinekMercer((long ContextCount, long Count) counts, in IReadOnlyList<int> tokens, in ICounter counter)
    {
        var lambda = Constant.JELINEK_MERCER_LAMBDA;
        double probability = (double)counts.Count / counts.ContextCount;
        var confidence = lambda;
        return (probability, confidence);
    }

    public static (double Probability, double Confidence) WittenBell((long ContextCount, long Count) counts, in IReadOnlyList<int> tokens, in ICounter counter)
    {
        throw new NotImplementedException();
    }

    public static (double Probability, double Confidence) AbsoluteDiscounting((long ContextCount, long Count) counts, in IReadOnlyList<int> tokens, in ICounter counter)
    {
        throw new NotImplementedException();
    }

    public static (double Probability, double Confidence) KneserNey((long ContextCount, long Count) counts, in IReadOnlyList<int> tokens, in ICounter counter)
    {
        throw new NotImplementedException();
    }

    public static (double Probability, double Confidence) ModifiedKneserNey((long ContextCount, long Count) counts, in IReadOnlyList<int> tokens, in ICounter counter)
    {
        throw new NotImplementedException();
    }

    public static (double Probability, double Confidence) AbsoluteDiscountingModified((long ContextCount, long Count) counts, in IReadOnlyList<int> tokens, in ICounter counter)
    {
        throw new NotImplementedException();
    }
}

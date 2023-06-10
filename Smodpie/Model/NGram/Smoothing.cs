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
        switch (type)
        {
            case SmoothingTypes.JelinekMercer:
                return JelinekMercer(counts, tokens, counter);
            case SmoothingTypes.WittenBell:
                return WittenBell(counts, tokens, counter);
            case SmoothingTypes.AbsoluteDiscounting:
                return AbsoluteDiscounting(counts, tokens, counter);
            case SmoothingTypes.KneserNey:
                return KneserNey(counts, tokens, counter);
            case SmoothingTypes.ModifiedKneserNey:
                return ModifiedKneserNey(counts, tokens, counter);
            case SmoothingTypes.AbsoluteDiscountingModified:
                return AbsoluteDiscountingModified(counts, tokens, counter);
            default:
                throw new ArgumentException($"Unknown smoothing type: {type}");
        }
    }

    public static (double Probability, double Confidence) JelinekMercer((long ContextCount, long Count) counts, in IReadOnlyList<int> tokens, in ICounter counter)
    {
        var lambda = Constant.JELINEK_MERCER_LAMBDA;
        double probability = (double)counts.Count / (double)counts.ContextCount;
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

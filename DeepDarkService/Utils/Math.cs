using System.Numerics;

namespace DeepDarkService.Utils;

public static class Math 
{
    public static T DotProduct<T>(IEnumerable<T> a, IEnumerable<T> b) where T : INumber<T>
    {
        var enumerable = a.ToList();
        var numbers = b.ToList();
        if (enumerable.Count != numbers.Count) throw new ArgumentException("Both arrays must have the same length");
        return enumerable.Zip(numbers, (x, y) => x * y).Aggregate((a, b) => a + b);
    }
    public static double Cos(IEnumerable<double> a, IEnumerable<double> b) 
        => DotProduct(a, b) / System.Math.Sqrt(DotProduct(a, a) * DotProduct(b, b));
}
namespace Ecomtik.Strategies;

public class TrapezoidalStrategy : IIntegrationStrategy
{
    public double Compute(Func<double, double> f, double a, double b, int n)
    {
        double h = (b - a) / n;
        double sum = 0.5 * (f(a) + f(b));
        for (int i = 1; i < n; i++)
            sum += f(a + i * h);
        return sum * h;
    }
}
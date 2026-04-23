namespace Ecomtik.Strategies;

public class MonteCarloStrategy : IIntegrationStrategy
{
 
    public double Compute(Func<double, double> f, double a, double b, int n)
    {
        Random rnd = new();
        double sum = 0;
        for (int i = 0; i < n; i++)
        {
            double x = a + rnd.NextDouble() * (b - a);
            sum += f(x);
        }
        return sum * (b - a) / n;
    }
}
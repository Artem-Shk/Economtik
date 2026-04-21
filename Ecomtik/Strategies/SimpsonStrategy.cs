namespace Ecomtik.Strategies;

public class SimpsonStrategy:IIntegrationStrategy
{
    public double Compute(Func<double, double> f, double a, double b, int n)
    {
        if (n % 2 == 1) n++;
        double h = (b - a) / n;
        double sum = f(a) + f(b);
        for (int i = 1; i < n; i++)
        {
            double x = a + i * h;
            sum += (i % 2 == 0 ? 2 : 4) * f(x);
        }
        return sum * h / 3;
    }
}
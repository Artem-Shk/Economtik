namespace Ecomtik.Strategies;

public interface IIntegrationStrategy
{
    double Compute(Func<double, double> f, double a, double b, int n);
}
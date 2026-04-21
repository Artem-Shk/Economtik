namespace Ecomtik.Strategies;

public class IntegrationFactory : IIntegrationFactory
{
    private readonly Dictionary<string, IIntegrationStrategy> _strategies = new()
    {
        ["simpson"]     = new SimpsonStrategy(),
        ["trapezoidal"] = new TrapezoidalStrategy(),
        ["monte_carlo"] = new MonteCarloStrategy()
    };

    public IIntegrationStrategy GetStrategy(string code) => _strategies[code];
}
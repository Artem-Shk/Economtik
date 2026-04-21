namespace Ecomtik.Strategies;

public interface IIntegrationFactory
{
    IIntegrationStrategy GetStrategy(string code);
}
namespace Ecomtik.Strategies;

public interface IIntegrationStrategy
{
    /// <summary>
    ///  подсчёт интеграла методом Монтекарло
    /// </summary>
    /// <param name="f">Функция расчёта</param>
    /// <param name="a">Функция расчёта</param>
    /// <param name="b">Функция расчёта</param>
    /// <param name="n">Функция расчёта</param>
    /// <returns>Возвращает результат вычесления интеграла</returns>
    /// <example>
    /// Пример использования: Compute(x*x, 1, 2, 3) ->
    /// <code>
    /// var result = await methodName(args);
    /// </code>
    /// </example>
    double Compute(Func<double, double> f, double a, double b, int n);
}
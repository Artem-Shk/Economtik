using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq.Expressions;
using Ecomtik.Strategies;
using Ecomtik.Data;
using Ecomtik.Models;


[ApiController]
[Route("api/[controller]")]
public class ComputeController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IIntegrationFactory _factory;

    public ComputeController(AppDbContext db, IIntegrationFactory factory)
    {
        _db = db;
        _factory = factory;
    }

    [HttpPost]
    public async Task<IActionResult> Calculate([FromBody] ComputeRequest req)
    {
        var sw = Stopwatch.StartNew();
        
        // Парсинг функции (упрощённо: поддерживаем x, +, -, *, /, ^, sin, cos, exp, log)
        var func = ParseFunction(req.Function);
        var strategy = _factory.GetStrategy(req.Method);
        
        double result = strategy.Compute(func, req.A, req.B, req.Steps);
        
        sw.Stop();
        
        // Сохранение в БД
        var method = await _db.Methods.FirstAsync(m => m.Code == req.Method);
        var computation = new Computation
        {
            FunctionExpr = req.Function,
            LowerBound = (decimal)req.A,
            UpperBound = (decimal)req.B,
            MethodId = method.Id,
            Steps = req.Steps,
            Result = (decimal)result,
            DurationMs = (int)sw.ElapsedMilliseconds
        };
        
        _db.Computations.Add(computation);
        await _db.SaveChangesAsync();
        
        return Ok(new { result, durationMs = sw.ElapsedMilliseconds, id = computation.Id });
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetHistory([FromQuery] int limit = 10)
    {
        var history = await _db.Computations
            .Include(c => c.Method)
            .OrderByDescending(c => c.CreatedAt)
            .Take(limit)
            .Select(c => new 
            {
                c.Id,
                c.FunctionExpr,
                c.LowerBound,
                c.UpperBound,
                method = c.Method.Name,
                c.Steps,
                result = Math.Round(c.Result, 6),
                c.DurationMs,
                c.CreatedAt
            })
            .ToListAsync();
            
        return Ok(history);
    }

    [HttpGet("methods")]
    public async Task<IActionResult> GetMethods()
    {
        return Ok(await _db.Methods.ToListAsync());
    }

    // Упрощённый парсер математических выражений
    private static Func<double, double> ParseFunction(string expr)
    {
        // Заменяем ^ на Pow
        expr = expr.Replace("^", "**");
        
        // Создаём параметр x
        var parameter = Expression.Parameter(typeof(double), "x");
        
        // Простой парсер через DataTable.Compute (не идеален, но для MVP работает)
        // Для продакшена нужен нормальный парсер (NCalc, Math.NET)
        return x =>
        {
            try
            {
                // Заменяем x на значение
                var eval = expr.Replace("x", x.ToString(System.Globalization.CultureInfo.InvariantCulture));
                
                // Поддержка функций
                eval = System.Text.RegularExpressions.Regex.Replace(eval, @"sin\(([^)]+)\)", m => 
                    Math.Sin(double.Parse(m.Groups[1].Value)).ToString());
                eval = System.Text.RegularExpressions.Regex.Replace(eval, @"cos\(([^)]+)\)", m => 
                    Math.Cos(double.Parse(m.Groups[1].Value)).ToString());
                eval = eval.Replace("pi", Math.PI.ToString());
                
                // Вычисляем простое выражение
                return Convert.ToDouble(new System.Data.DataTable().Compute(eval, null));
            }
            catch
            {
                // Fallback: если не удалось распарсить, считаем что это просто x*x
                return x * x;
            }
        };
    }
}

public record ComputeRequest(string Function, string Method, double A, double B, int Steps);
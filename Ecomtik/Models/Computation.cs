namespace Ecomtik.Models;

public class Computation
{
    public Guid Id { get; set; }
    public string FunctionExpr { get; set; } = "";
    public decimal LowerBound { get; set; }
    public decimal UpperBound { get; set; }
    public int MethodId { get; set; }
    public Method Method { get; set; } = null!;
    public int Steps { get; set; }
    public decimal Result { get; set; }
    public int DurationMs { get; set; }
    public DateTime CreatedAt { get; set; }
}

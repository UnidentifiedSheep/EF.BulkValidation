namespace BulkValidation.Core.Models;

public class SharedCounter
{
    private int _value;
    public static readonly SharedCounter Shared = new();
    public int GetNextInt()
    {
        return Interlocked.Increment(ref _value);
    }
}
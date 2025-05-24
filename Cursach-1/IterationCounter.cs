namespace Cursach_1;

public class IterationCounter
{
    public int TotalIterations { get; private set; }
    public int CurrentMethodIterations { get; private set; }

    public void Reset()
    {
        TotalIterations = 0;
        CurrentMethodIterations = 0;
    }

    public void ResetCurrentMethod()
    {
        CurrentMethodIterations = 0;
    }

    public void AddIteration()
    {
        TotalIterations++;
        CurrentMethodIterations++;
    }

    public void AddIterations(int count)
    {
        TotalIterations += count;
        CurrentMethodIterations += count;
    }

    public override string ToString()
    {
        return $"Total iterations: {TotalIterations}, Current method iterations: {CurrentMethodIterations}";
    }
}

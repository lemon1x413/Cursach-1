namespace Cursach_1;

public class IterationCounter
{
    private int TotalIterations { get; set; }
    public int CurrentMethodIterations { get; private set; }
    
    public void ResetCurrentMethod()
    {
        CurrentMethodIterations = 0;
    }

    public void AddIteration()
    {
        TotalIterations++;
        CurrentMethodIterations++;
    }
    
}

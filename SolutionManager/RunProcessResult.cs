namespace SolutionManager;

public struct RunProcessResult
{
    public RunProcessResult(int code, string text)
    {
        Code = code;
        Text = text;
    }

    public int Code { get; }
    public string Text { get; }
}
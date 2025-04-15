public abstract class Subgoal
{
    public int Target { get; private set; }
    public abstract int Progress { get; }
    public bool Complete => Progress >= Target;
}
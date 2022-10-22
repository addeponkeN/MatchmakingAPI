namespace Rovio.Utility;

public class IdGenerator
{
    private int _idPool = 1;
    private Stack<int> _stack;

    public IdGenerator()
    {
        _stack = new();
    }

    public int GetId()
    {
        if(_stack.Count > 0)
            _stack.Pop();
        return ++_idPool;
    }

    public void ReturnId(int id)
    {
        _stack.Push(id);
    }
}
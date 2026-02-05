public class VM : IDisposable
{
    public IntPtr L { get; private set; }
    private bool _disposed = false;

    public VM()
    {
        L = Luau.luaL_newstate();
        if (L == IntPtr.Zero)
        {
            throw new InvalidOperationException("Failed to initialize Luau VM: Native pointer is null.");
        }

        Luau.luaL_openlibs(L);
    }

    public void InspectStack()
    {
        UserData.ProcessStackItem(L, -1);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (L != IntPtr.Zero)
            {
                Luau.lua_close(L);
                L = IntPtr.Zero;
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~VM()
    {
        Dispose(false);
    }
}
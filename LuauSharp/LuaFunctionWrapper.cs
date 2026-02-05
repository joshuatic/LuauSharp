public static class LuaFunctionWrapper
{
    public static int SafeCall(IntPtr L, Func<IntPtr, int> implementation)
    {
        try
        {
            return implementation(L);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in C# Callback: {ex.Message}");
            return 0;
        }
    }
}
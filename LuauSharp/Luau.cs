using System.Runtime.InteropServices;

public static class Luau
{
    private const string LibName = "LuauShim"; 
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int LuaCFunction(IntPtr L);
    
    public const int LUA_TNIL = 0;
    public const int LUA_TBOOLEAN = 1;
    public const int LUA_TLIGHTUSERDATA = 2;
    public const int LUA_TNUMBER = 3;
    public const int LUA_TSTRING = 4;
    public const int LUA_TTABLE = 5;
    public const int LUA_TFUNCTION = 6;
    public const int LUA_TUSERDATA = 7;
    public const int LUA_TTHREAD = 8;
    
    public const int LUA_GLOBALSINDEX = -10002;
    
    [DllImport(LibName, EntryPoint = "bridge_luaL_newstate", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr luaL_newstate();

    [DllImport(LibName, EntryPoint = "bridge_lua_close", CallingConvention = CallingConvention.Cdecl)]
    public static extern void lua_close(IntPtr L);

    [DllImport(LibName, EntryPoint = "bridge_luaL_openlibs", CallingConvention = CallingConvention.Cdecl)]
    public static extern void luaL_openlibs(IntPtr L);
    
    [DllImport(LibName, EntryPoint = "bridge_lua_type", CallingConvention = CallingConvention.Cdecl)]
    public static extern int lua_type(IntPtr L, int idx);

    [DllImport(LibName, EntryPoint = "bridge_lua_pushcclosurek", CallingConvention = CallingConvention.Cdecl)]
    public static extern void lua_pushcclosurek(IntPtr L, [MarshalAs(UnmanagedType.FunctionPtr)] LuaCFunction fn, [MarshalAs(UnmanagedType.LPStr)] string debugname, int nup, IntPtr cont);

    [DllImport(LibName, EntryPoint = "bridge_lua_tonumberx", CallingConvention = CallingConvention.Cdecl)]
    public static extern double lua_tonumberx(IntPtr L, int idx, IntPtr isnum);

    [DllImport(LibName, EntryPoint = "bridge_lua_toboolean", CallingConvention = CallingConvention.Cdecl)]
    public static extern bool lua_toboolean(IntPtr L, int idx);

    [DllImport(LibName, EntryPoint = "bridge_lua_tolstring", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr lua_tolstring(IntPtr L, int idx, out UIntPtr len);

    [DllImport(LibName, EntryPoint = "bridge_lua_setfield", CallingConvention = CallingConvention.Cdecl)]
    public static extern void lua_setfield(IntPtr L, int idx, [MarshalAs(UnmanagedType.LPStr)] string k);

    [DllImport(LibName, EntryPoint = "bridge_lua_settop", CallingConvention = CallingConvention.Cdecl)]
    public static extern void lua_settop(IntPtr L, int idx);
    
    [DllImport(LibName, EntryPoint = "bridge_free", CallingConvention = CallingConvention.Cdecl)]
    public static extern void bridge_free(IntPtr ptr);
    
    public static bool lua_isnil(IntPtr L, int n) => lua_type(L, n) == LUA_TNIL;
    public static bool lua_isnumber(IntPtr L, int n) => lua_type(L, n) == LUA_TNUMBER;
    public static bool lua_isstring(IntPtr L, int n) => lua_type(L, n) == LUA_TSTRING;
    public static bool lua_isboolean(IntPtr L, int n) => lua_type(L, n) == LUA_TBOOLEAN;
    public static bool lua_isfunction(IntPtr L, int n) => lua_type(L, n) == LUA_TFUNCTION;
    
    public static string? lua_tostring(IntPtr L, int idx)
    {
        IntPtr ptr = lua_tolstring(L, idx, out UIntPtr len);
        return ptr == IntPtr.Zero ? null : Marshal.PtrToStringAnsi(ptr, (int)len);
    }

    public static void lua_register(IntPtr L, string name, LuaCFunction f)
    {
        lua_pushcclosurek(L, f, name, 0, IntPtr.Zero);
        lua_setfield(L, LUA_GLOBALSINDEX, name);
    }
}
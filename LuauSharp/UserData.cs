using System.ComponentModel;
using System.Dynamic;
using System.Reflection;
using System.Runtime.InteropServices;

namespace LuauSharp;

public unsafe class UserData : IDisposable
{
    private Luau.lua_State* state;
    internal List<GCHandle> typeHandles = new();
    private static Dictionary<(Type, ExpandoObject), Dictionary<string, int>> staticClassCache = new();

    internal UserData(Luau.lua_State* state) => this.state = state;

    private static string? ReadString(Luau.lua_State* state, int index)
    {
        IntPtr len;
        IntPtr ptr = Luau.lua_tolstring(state, index, out len);
        if (ptr == IntPtr.Zero) return null;
        return Marshal.PtrToStringAnsi(ptr, len.ToInt32());
    }

    public T? ReadLightUserDataTagged<T>(TypeConverter converter, int index, int tag)
    {
        IntPtr ptr = Luau.lua_tolightuserdatatagged(state, index, tag);
        if (ptr == IntPtr.Zero) return default;
        GCHandle handle = GCHandle.FromIntPtr(ptr);
        return (T?)converter.ConvertFrom(handle.Target);
    }

    public Delegate? ReadFunction(int index)
    {
        IntPtr ptr = Luau.lua_tocfunction(state, index);
        if (ptr == IntPtr.Zero) return null;
        GCHandle handle = GCHandle.FromIntPtr(ptr);
        return handle.Target as Delegate;
    }

    internal static object? GetLuaValue(Luau.lua_State* state, int index, int tag = 0)
    {
        if (Luau.lua_isnil(state, index))
            return null;
        if (Luau.lua_isnumber(state, index) == 1)
            return Luau.lua_tonumberx(state, index, out _);
        if (Luau.lua_isstring(state, index) == 1)
            return ReadString(state, index);
        if (Luau.lua_isboolean(state, index))
            return Luau.lua_toboolean(state, index);
        if (Luau.lua_isfunction(state, index))
            return new LuaFunctionWrapper(state, index);
        if (Luau.lua_isuserdata(state, index) == 1)
        {
            IntPtr ptr = Luau.lua_tolightuserdatatagged(state, index, tag);
            if (ptr == IntPtr.Zero) return null;
            return GCHandle.FromIntPtr(ptr).Target;
        }
        return null;
    }

    public void Dispose()
    {
        foreach (var h in typeHandles)
            if (h.IsAllocated) h.Free();
    }
}

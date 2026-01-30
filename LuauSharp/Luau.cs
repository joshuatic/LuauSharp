using System.Runtime.InteropServices;

namespace LuauSharp;

internal unsafe class Luau
{
#if MSVC
    private const string VM = "Luau.VM.dll";
    private const string COMPILER = "Luau.Compiler.dll";
#elif MAC
    private const string VM = "libLuau.VM.dylib";
    private const string COMPILER = "libLuau.Compiler.dylib";
#else
    private const string VM = "libLuau.VM.so";
    private const string COMPILER = "libLuau.Compiler.so";
#endif

    public const int LUA_MULTRET = -1;
    public const int LUAI_MAXCSTACK = 8000;
    public const int LUA_REGISTRYINDEX = -LUAI_MAXCSTACK - 2000;
    public const int LUA_ENVIRONINDEX = -LUAI_MAXCSTACK - 2001;
    public const int LUA_GLOBALSINDEX = -LUAI_MAXCSTACK - 2002;

    public enum lua_Type
    {
        LUA_TNIL = 0,
        LUA_TBOOLEAN = 1,
        LUA_TLIGHTUSERDATA,
        LUA_TNUMBER,
        LUA_TVECTOR,
        LUA_TSTRING,
        LUA_TTABLE,
        LUA_TFUNCTION,
        LUA_TUSERDATA,
        LUA_TTHREAD,
        LUA_TBUFFER,
        LUA_TPROTO,
        LUA_TUPVAL,
        LUA_TDEADKEY,
        LUA_T_COUNT = LUA_TPROTO
    };

    public enum lua_Status : uint
    {
        LUA_OK,
        LUA_YIELD,
        LUA_ERRRUN,
        LUA_ERRSYNTAX,
        LUA_ERRMEM,
        LUA_ERRERR,
        LUA_BREAK,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct lua_State
    {
        public byte tt;
        public byte marked;
        public byte memcat;
        public byte status;
        public byte activememcat;
        [MarshalAs(UnmanagedType.I1)] public bool isactive;
        [MarshalAs(UnmanagedType.I1)] public bool singlestep;

        public IntPtr top;
        public IntPtr @base;
        public IntPtr global;
        public IntPtr ci;
        public IntPtr stack_last;
        public IntPtr stack;
        public IntPtr end_ci;
        public IntPtr base_ci;

        public int stacksize;
        public int size_ci;

        public ushort nCcalls;
        public ushort baseCcalls;

        public int cachedslot;

        public IntPtr gt;
        public IntPtr openupval;
        public IntPtr gclist;
        public IntPtr namecall;
        public IntPtr userdata;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct lua_CompileOptions
    {
        public int optimizationLevel;
        public int debugLevel;
        public int typeInfoLevel;
        public int coverageLevel;

        public IntPtr vectorLib;
        public IntPtr vectorCtor;
        public IntPtr vectorType;

        public IntPtr mutableGlobals;
        public IntPtr userdataTypes;

        // FIXED: assign fields, not locals
        public lua_CompileOptions()
        {
            optimizationLevel = 1;
            debugLevel = 1;
            typeInfoLevel = 0;
            coverageLevel = 0;
        }
    }
}

public class UserData
{
    public string Name { get; set; } = "Unknown";
    public int ID { get; set; }

    public static void ProcessStackItem(IntPtr L, int index)
    {
        int type = Luau.lua_type(L, index);

        switch (type)
        {
            case Luau.LUA_TNIL:
                Console.WriteLine($"[Stack {index}] Type: Nil");
                break;
            case Luau.LUA_TNUMBER:
                double val = Luau.lua_tonumberx(L, index, IntPtr.Zero);
                Console.WriteLine($"[Stack {index}] Type: Number, Value: {val}");
                break;
            case Luau.LUA_TBOOLEAN:
                bool b = Luau.lua_toboolean(L, index);
                Console.WriteLine($"[Stack {index}] Type: Boolean, Value: {b}");
                break;
            case Luau.LUA_TSTRING:
                string? s = Luau.lua_tostring(L, index);
                Console.WriteLine($"[Stack {index}] Type: String, Value: {s}");
                break;
            case Luau.LUA_TFUNCTION:
                Console.WriteLine($"[Stack {index}] Type: Function");
                break;
            case Luau.LUA_TUSERDATA:
                Console.WriteLine($"[Stack {index}] Type: UserData");
                break;
            default:
                Console.WriteLine($"[Stack {index}] Type: Other ({type})");
                break;
        }
    }
}
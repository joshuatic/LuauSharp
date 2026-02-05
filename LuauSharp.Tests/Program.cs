using System;
using System.Runtime.InteropServices;

class Program
{
    private static readonly Luau.LuaCFunction _testFunc = MyCSharpFunction;

    static void Main(string[] args)
    {
        Console.WriteLine("Starting Luau VM...");
        
        using (VM vm = new VM())
        {
            Luau.lua_register(vm.L, "printHello", _testFunc);
            
            Console.WriteLine("Function 'printHello' registered.");
        }

        Console.WriteLine("VM Closed.");
    }

    static int MyCSharpFunction(IntPtr L)
    {
        Console.WriteLine("Hello from C#!");
        return 0; // Successful Build
    }
}
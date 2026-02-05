# LuauSharp

LuauSharp is a C# wrapper for implementing Roblox's [luau](https://github.com/luau-lang/luau) language into your .NET applications.

## Using

Firstly, you'll need to have binaries of luau that can be referenced by LuauSharp's DllImports. Please see the Building section for more information on how to build luau.

### Setup VM

Once you have imported all the native binaries, you can create a VM. A VM manages all of luau's native functions and states and whatnot.

```cs
using VM vm = new VM(Console.WriteLine, Console.WriteLine, Console.WriteLine);
```

> [!WARNING]
>
> Do not forget to dispose (`vm.Dispose();`) the VM when you are done with it!

The three parameters of VM are as follows:

1. print
    - The print function to log from luau to managed
2. warn
    - The warn function to warn from luau to managed
3. error
    - The error function to error from luau to managed
    - This can throw an exception if you want it to

### Create UserData

After you have your VM created, you need to forward any types from the **UserData** object you want luau to be able to access.

> [!CAUTION]
>
> - **NEVER** expose any classes that may cause damage to the host machine.
> - Be careful when exposing .NET types, because some may be dangerous

```cs
vm.UserData.ForwardType<MyClass>();
vm.UserData.ForwardType<MyEnum>();

public class MyClass
{
    // Visible to UserData
    public int Number;
    // Not Visible to UserData
    internal string NameOfClass;
    private int fromLua;

    // Visible to UserData
    public void Increase()
    {
        __Increase();
        fromLua++;
    }
    // Not Visible to UserData
    internal void __Increase() => Number++;

    // Visible to UserData
    public static void IsComposed(MyEnum e) => e == MyEnum.Composed;
}

public enum MyEnum
{
    NotComposed = 0x0,
    Composed = 1
}
```

You can also push individual objects and functions as globals

```cs
vm.UserData.PushFunction("unixtime", () => DateTimeOffset.UtcNow.ToUnixTimeSeconds());
// Do not Use List<T>
vm.UserData.PushObjectToLua<string[]>(new string[2]{"Joe", "Donald"}, "names");
// Dictionary<TKey,TValue> is okay
vm.UserData.PushPbject<Dictionary<string, float>>(new Dictionary<string, float>
{
    ["Smile"] = 1,
    ["EyesWiden"] = 0.65f
}, "faceweights");
```

### Loading a Script

There are two ways you can load a script.

1. From text
    - Slower but more dynamic
2. From compiled bytecode
    - Faster but not dynamic

> [!NOTE]
>
> It is always recommended to pre-compile luau code and load it from its compiled bytecode, as it is much faster.

For the two examples, lets assume this is the script (`const string MY_SCRIPT_TEXT;`)

```lua
local composed = MyEnum.NotComposed
local myObject = MyClass.new()
for i = 1, 3 do
    -- Use : for functions
    myObject:Increase()
end
composed = MyEnum.Composed
print("Is Composed: "..tostring(MyClass:IsComposed(composed)))
print("Number: "..tostring(myObject.Number))
```

#### From Script

```cs
// Where text.luau is the script name
bool didWork = vm.DoText("text.luau", MY_SCRIPT_TEXT);
if(!didWork)
{
    // Handle if the script did not load correctly
}
```

#### From Compilation

> [!NOTE]
>
> Compiling a script would look like this
> 
> ```cs
> byte[] compiled = vm.Compile(MY_SCRIPT_TEXT);
> // Now you can save to file or do whatever you want to do with the compiled code
> ```

Simply get the compiled bytecode however you need to, then load it

```cs
// Example case of pulling from file. Any byte array would work as long as it's for luau
byte[] compiled = File.ReadAllBytes("path/to/script.luau");
int loadStatus = vm.Load("script.luau", compiled);
if(loadStatus != 0)
{
    // Handle if the script did not load correctly
}
```

### Executing a Script

Once you have the script loaded properly, to start executing the script, simply call `vm.Execute();`

### Full Example

```cs
const string MY_SCRIPT_TEXT =
"""
local composed = MyEnum.NotComposed
local myObject = MyClass.new()
for i = 1, 3 do
    -- Use : for functions
    myObject:Increase()
end
composed = MyEnum.Composed
print("Is Composed: "..tostring(MyClass:IsComposed(composed)))
print("Number: "..tostring(myObject.Number))
""";

using VM vm = new VM(Console.WriteLine, Console.WriteLine, Console.WriteLine);
vm.UserData.ForwardType<MyClass>();
vm.UserData.ForwardType<MyEnum>();
vm.UserData.PushFunction("unixtime", () => DateTimeOffset.UtcNow.ToUnixTimeSeconds());
vm.UserData.PushObjectToLua<string[]>(new string[2]{"Joe", "Donald"}, "names");
vm.UserData.PushPbject<Dictionary<string, float>>(new Dictionary<string, float>
{
    ["Smile"] = 1,
    ["EyesWiden"] = 0.65f
}, "faceweights");
bool didWork = vm.DoText("text.luau", MY_SCRIPT_TEXT);
if(!didWork)
{
    throw new Exception("Failed to load script text.luau!");
}
vm.Execute();

public class MyClass
{
    public int Number;
    internal string NameOfClass;
    private int fromLua;

    public void Increase()
    {
        __Increase();
        fromLua++;
    }
    internal void __Increase() => Number++;

    public static void IsComposed(MyEnum e) => e == MyEnum.Composed;
}

public enum MyEnum
{
    NotComposed = 0x0,
    Composed = 1
}
```

## Building

Building the Luau environment is streamlined through the use of a native bridge. **LuauSharp** utilizes [**LuauShim**](https://github.com/joshuatic/LuauShim) to handle the complexities of cross-language communication.

### 1. Build the Native Shim
Instead of manually modifying Luau's core source code, use the shim to generate the necessary binaries:
1. Clone the [LuauShim](https://github.com/joshuatic/LuauShim) repository.
2. Build the project using **CLion** or **CMake**.
   - This automatically handles `extern "C"` wrapping.
   - It manages `__declspec(dllexport)` for MSVC/Windows builds to prevent symbol mangling.
   - It ensures the correct `-fPIC` options are applied for position-independent code.

### 2. Include the Binaries
Once the build is complete, you will have a `LuauShim.dll` (Windows) or `.so` (Linux/Unix).
- **Standard .NET:** Place the binary in your execution directory (next to your `.dll`/`.exe`).
- **Unity:** Add the binary to your `Assets/Plugins` folder.
- **Advanced:** Use [LoadLibrary](https://learn.microsoft.com/en-us/windows/win32/api/libloaderapi/nf-libloaderapi-loadlibrarya) if you need to load the shim from a non-standard path at runtime.

### 3. Build LuauSharp
After the native binaries are in place, simply build the **LuauSharp** project using the dotnet CLI:
```bash
dotnet build --configuration Release
```
> [!NOTE] Dynamic Object Support LuauSharp utilizes ExpandoObject to provide a flexible API. If your target platform (e.g., certain AOT-restricted environments) does not support dynamic objects, LuauSharp will fail.
## License

As its parent project, LuauSharp is distributed under the terms of the [MIT License](https://github.com/TigersUniverse/LuauSharp/blob/main/LICENSE).

When implemented into projects, please honor both the [luau License](https://github.com/luau-lang/luau/blob/master/LICENSE.txt) and our license.

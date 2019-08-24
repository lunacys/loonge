# Language Syntax

## General

### Comments

```rust
// loonge (loonacuse's language, *.lo)
// "It's all about data"

// Single-line Comment
/*
 * Multi-line comment
 */
```

### Available type aliases

| Type Alias | Size (in bits) | Description | Notes |
| --- | --- | --- | --- |
| `i16` | 16 | Signed integer | Like `short` in C# |
| `i32` | 32 | Signed integer | Like `int` in C# |
| `i64` | 64 | Signed integer | Like `long` in C# |
| `f32` | 32 | Signed floating-point number | Like `float` in C# |
| `f64` | 64 | Signed floating-point number with double precision | Like `double` in C# |
| `f128` | 128 | Signed floating-point number with quad precision, useful for high precision arithmetics | Like `decimal` in C# |
| `ui16` | 16 | Unsigned integer | Like `ushort` in C# |
| `ui32` | 32 | Unsigned integer | Like `uint` in C# |
| `ui64` | 64 | Unsigned integer | Like `ulong` in C# |
| `bool` | 2 | Unsigned integer | `true` or `false` |
| `char` | 16 | Any symbol/character in Unicode, signed integer | |
| `string` | Depending on string length, size is not limited | Immutable string (UTF-16) | |
| `object` | Depending on object members | Any instance of any type | |

Any type with square braces is array:

```rust
i32[] // array of 32-bit integers
```

Square braces AND types within them is table:

```rust
[i32, i64, f128] // table with 3 columns
```

Types delimited with comma within parentheses is tuple:

```rust
(i32, string, object) // tuple with 3 elements
```

Braces are for anonymous (typeless) objects (which can be useful for JSON for example):

```rust
{ a: i32, b: i64, c: i16 }
```

### Variables

```rust
let x1 = 1; // i32
let x2: i32 = 15;
let x3: f32 = 15.6f;
// let x4: i32 = 15.1; // Compilation error: type mismatch
let myStr = "Hello, world!";

let x5 = 3.14; // f64
let x6 = 3.14f; // f32

let myChar = '\n'; // char

let myBool = true; // bool

let myArray: i32[] = [ 1, 2, 3, 4, 5 ]; // immutable array of size 5

let myTwoDimensionalArray: i32[,] = 

const myConst = 3.14; // immutable object of type f64

let square = (number: i32) => { return number * number }; // lambda
let doubleNumber: (number: i32) => i32 = (number: i32) => number * 2; // lambda with implicit type and bodyless expression

let myEmptyArray: i32[] = [16]; // Empty array of size 16 initializated with default values (in this case it is 0)
myEmptyArray[0] = 10;
// myEmptyArray[1] = 15.5; // Compilation error: type mismatch
// myEmptyArray[-1] = 1; // Exception: OutOfBounds

// let a; // Compilation error: unknown type for variable 'a'
let b: i32; // Define variable with default value (in this case it is 0)

let tuple: (i32, f32, f64) = (12, 12.3, 123.45);
```

### Tables

```rust
let table: [string, string, string] = [ "Col1", "Col2", "Col3" ]; // Empty mutable table of strings, every column must contain rows of type 'string'

// let table2 = [ ]; // Compilation error: table type is not specified

let table3: [string, string, string] = [ ]; // Compilation error: type of column is not specified

// let table4 = [ "Col1", "Col2", "Col3" ]; // Compilation error: ambiguous declaration, either is possible: array of strings or table 

let table5 = ["FullName": string, "FullAge": i16, "IsMarried": bool ]; // Auto type selection

// The next table is an analog to the previous one:
let table5: [string, i16, bool] = [ "FullName", "FullAge", "IsMarried" ];

table[0] = [ "First", "SecondColumn", "ThirdColumn" ]; // Add a new row into the table with id = 0
// Table:
// | Id | ColName1    | ColName2     | ColName3    |
// -------------------------------------------------
// | 0  | FirstColumn | SecondColumn | ThirdColumn |

table[2] = [ "First", null, "Third" ];
// Table:
// | Id | ColName1    | ColName2     | ColName3    |
// -------------------------------------------------
// | 0  | FirstColumn | SecondColumn | ThirdColumn |
// | 1  | null        | null         | null        |
// | 2  | First       | null         | Third       |

table[3] = [ "First" ];
// Table:
// | Id | ColName1    | ColName2     | ColName3    |
// -------------------------------------------------
// | 0  | FirstColumn | SecondColumn | ThirdColumn |
// | 1  | null        | null         | null        |
// | 2  | First       | null         | Third       |
// | 3  | First       | null         | null        |

table.Collapse(true, true); // Collapse(removeEmptyColumns: bool, shiftIds: bool): void
// Table:
// | Id | ColName1    | ColName2     | ColName3    |
// -------------------------------------------------
// | 0  | FirstColumn | SecondColumn | ThirdColumn |
// | 1  | First       | null         | Third       |
// | 2  | First       | null         | null        |

table.CollapseAll(); // CollapseAll(shiftIds: bool): void
// Table:
// | Id | ColName1    | ColName2     | ColName3    |
// -------------------------------------------------
// | 0  | FirstColumn | SecondColumn | ThirdColumn |
// | 1  | First       | Third        |
// | 2  | First       |

table.AddColumn("ColName4");
// Table:
// | Id | ColName1    | ColName2     | ColName3    | ColName4 |
// ------------------------------------------------------------
// | 0  | FirstColumn | SecondColumn | ThirdColumn | null     |
// | 1  | First       | Third        | null        | null     |
// | 2  | First       | null         | null        | null     |

table[2]["ColName4"] = "Fourth";
// Table:
// | Id | ColName1    | ColName2     | ColName3    | ColName4 |
// ------------------------------------------------------------
// | 0  | FirstColumn | SecondColumn | ThirdColumn | null     |
// | 1  | First       | Third        | null        | null     |
// | 2  | First       | null         | null        | Fourth   |

use Console from System; 

Console.WriteLine(table[2]);
// > OUTPUT:
// > First,null,null,Fourth
Console.WriteLine(table[2]["ColName4"]);
// > OUTPUT:
// > Fourth

table.AddRow([ "First" ]);
table += [ "First", "Second" ];
// Table:
// | Id | ColName1    | ColName2     | ColName3    | ColName4 |
// ------------------------------------------------------------
// | 0  | FirstColumn | SecondColumn | ThirdColumn | null     |
// | 1  | First       | Third        | null        | null     |
// | 2  | First       | null         | null        | Fourth   |
// | 3  | First       | null         | null        | null     |
// | 4  | First       | Second       | null        | null     |

table[0] = null; // Remove the whole row with id = 0 and move other rows up
// Table:
// | Id | ColName1    | ColName2     | ColName3    | ColName4 |
// ------------------------------------------------------------
// | 0  | First       | Third        | null        | null     |
// | 1  | First       | null         | null        | Fourth   |
// | 2  | First       | null         | null        | null     |
// | 3  | First       | Second       | null        | null     |

// Add range:
table += 
[ 
    [ "First1", "Second1", "Third1", "Fourth1" ],
    [ "First2", "Second2", "Third2", "Fourth2" ],
    [ "First3", "Second3", "Third3", "Fourth3" ],
    [ "First4", "Second4", "Third4", "Fourth4" ],
];
// Table:
// | Id | ColName1    | ColName2     | ColName3    | ColName4 |
// ------------------------------------------------------------
// | 0  | First       | Third        | null        | null     |
// | 1  | First       | null         | null        | Fourth   |
// | 2  | First       | null         | null        | null     |
// | 3  | First       | Second       | null        | null     |
// | 4  | First1      | Second1      | Third1      | Fourth1  |
// | 5  | First2      | Second2      | Third2      | Fourth2  |
// | 6  | First3      | Second3      | Third3      | Fourth3  |
// | 7  | First4      | Second4      | Third4      | Fourth4  |

```

### Advanced table usage: selections, joins

```rust
let usersTable: [string, string, DateTime, string, string] = [ "FirstName", "LastName", "Birthday", "Login", "PasswordHash" ];
let tradesTable: [i32, f128, f128 | null, DateTime, f64 ] = [ "UserId", "OpenPrice", "ClosePrice", "OpenDate", "Volume" ];

fn AddNewUser(firstName: string, lastName: string, birthday: DateTime, login: string, password: string) 
{
    let pwdHasd = Md5.Encode(password);

    usersTable += [ firstName, lastName, birthday, login, pwdHash ];
}

fn OpenTrade(userId: i32, openPrice: f128, volume: f64) 
{
    let currentTime = DateTime.UtcNow();

    tradesTable += [ userId, openPrice, null, currentTime, volume ];
}

fn CloseTrade(tradeId: i32, currentPrice: f128)
{
    tradesTable.Find((trade) => trade["Id"] == tradeId).ClosePrice = currentPrice;
}

// Seed data
AddNewUser("Anton", "Smith", DateTime.FromString("1982-05-19"), "executor_221", "masterkey");
AddNewUser("Andrey", "Rock", DateTime.FromString("1991-04-11"), "everlasting_summer", "asdf456");
AddNewUser("Misha", "Shisha", DateTime.FromString("1989-09-04"), "pro_trader_plus", "1234567");
AddNewUser("Vasya", "Pupkin", DateTime.FromString("1986-01-10"), "vpupkin_", "091283asd!");

OpenTrade(usersTable.FindByColumn("FirstName", "Anton").First()["Id"], 1.2345m, 100_000);
OpenTrade(usersTable[3]["Id"], 1.3345m, 100_100);
OpenTrade(1, 1.6345m, 1_500_000);

CloseTrade(0, 1.1101);

let closedTrades = tradesTable.SelectAll((trade) => trade.ClosePrice != null);

let openPrices = tradesTable.SelectColumn("OpenPrice");
```

### Lists

```rust
let numList = new List<i32>(); // Create a new list
numList.Add(15);
numList.AddRange({ 12, 10, 5, 2 });
```

### Functions

Any function that is **not** inside a class is global for *the current scope*. Max scope possible (without any changes) is a file. Local scopes are function in other functions. In fact, every function is a named (non-anonymous) lambda expression.

Some examples below:

```rust
fn DoSomething(): void // return nothing
{
    Console.WriteLine("We're doing something!");
}

fn CaclAdd(a: i32, b: i32): i32
{
    return a + b;
}

fn Square(a: i32): i32 
{
    return a * a;
}

fn FuncInsideAnotherFunc(testStr: string) // if you don't specify the return type, it stays as 'void'
{
    fn TestInsideFunc(testStr2: string) 
    {
        Console.WriteLine($"You just wrote: {testStr2} while testStr is {testStr}"); // String interpolation
    }

    TestInsideFund("Hello, World!");
}

// TestInsideFunc("test"); // Compilation error: 'TestInsideFunc' is not defined
```

You can create pointers to any function and methods. In loonge they are named delegates, like C#:

```rust
pub delegate AddDelegate(a: i32, b: i32): i32;
```

Delegates cannot have body. Delegates descibe signature of any function that can fit in. With the previous example, the allowed signature will be: `<fn_name>(<param1>: i32, <param2>: i32): i32`. Example:

```rust
pub delegate AddDelegate(a: i32, b: i32): i32;

fn Add(a: i32, b: i32): i32 
{
    return a + b;
}

let myDelegate: AddDelegate = Add;
// Now you can call 'myDelegate' as function:
let result = myDelegate(15, 20);
Console.WriteLine($"Result of 'myDelegate' is: " + result);
// OUTPUT:
// > Result of 'myDelegate' is: 35
```

Delegates accept access modifiers as they can be found **outside** of the current file.

### Anonymous (typeless) objects

```rust
let jsonSchema: {i32, bool, bool, i32} = 
{
    name: "Test",
    isMarried: true,
    extended: true,
    errorCode: 200
};

jsonSchema = Json.Parse("{\"name\": \"hellothere\", \"isMarried\": true, \"extended\": false, \"errorCode\": 404 }");

// Extended usage:
let jsonSchemaExtended: {i32, {i32, string}, string[]} = 
{
    requestId: 1542001,
    response: {
        errorCode: 404,
        message: "Not Found"
    },
    data: [
        "hello",
        "there",
        "i think",
        "you're wrong",
        "try something better"
    ]
};
```

OR

```rust
 // Objects
let Math = 
{ 
    // Single-line function
    export fn Abs(num: i32): i32 => { return (num < 0) ? -num : num; }; // ?: operator

    // Braces are optional
    export fn Sqr(num: i32): i32 => num * num;

    // Default style functions
    export fn Min(nums: i32[]): i32
    {
        let min = nums[0]; // the first element
        for let i = 1..nums.length 
        {
            if nums[i] < min 
                min = nums[i];
        }
        return min;
    }
}
// Type is: { (i32) => i32, (i32) => i32, (i32[]) => i32 }

Console.WriteLine($"Abs(1): {Math.Abs(1)}");
// OUTPUT:
// > 1

Console.WriteLine($"Abs(-1): {Math.Abs(-1)}");
// OUTPUT:
// > 1

let arrayOfInts: i32[] = { 12, 8, 16, 21, 1, -8, 2 };
Console.WriteLine($"Min(arrayOfInts): {Math.Min(arrayOfInts)}");
// OUTPUT:
// > -8
```

### Access modifiers

Please note, that functions can have an access modifier, but the max visibility of a function will always (for a few exceptions) be current file (`pub` modifier), see [Modules](Modules) chapter.

These access modifiers are allowed:

| Keyword | Allowed scope(s) |
| --- | --- |
| `pub` | Entire project and all other projects |
| `protected` | Childs of a class |
| `internal` | Current project |
| `private` | Current scope |

### Modules and exports

Modules helps you to logically split your code.

In fact, a module is a namespace provider (similiarly to `namepsace` in C# or `package` in Java). 

In a single file there can be any amount of modules.

```rust
module MyModule
{
    fn Add(a: i32, b: i32): i32 => a + b;
    fn Sub(a: i32, b: i32): i32 => a - b;
    fn Mul(a: i32, b: i32): i32 => a * b;
    fn Div(a: i32, b: i32): f32 => a / b;
}

fn Calc(): void 
{
    Console.WriteLine($"5 + 5 = {MyModule.Add(5, 5)}");
    Console.WriteLine($"5 - 5 = {MyModule.Sub(5, 5)}");
    Console.WriteLine($"5 * 5 = {MyModule.Mul(5, 5)}");
    Console.WriteLine($"5 / 5 = {MyModule.Div(5, 5)}");
}

Calc();

// OUTPUT: 
// > 5 + 5 = 10
// > 5 - 5 = 0
// > 5 * 5 = 25
// > 5 / 5 = 1
```

Exports provide any function or module to be visible externally.

```rust
// Math.lun
export module Math
{
    fn Add(a: i32, b: i32): i32 => a + b;
    fn Sub(a: i32, b: i32): i32 => a - b;
    fn Mul(a: i32, b: i32): i32 => a * b;
    fn Div(a: i32, b: i32): f32 => a / b;
}

export fn Calc(): void 
{
    Console.WriteLine($"5 + 5 = {MyModule.Add(5, 5)}");
    Console.WriteLine($"5 - 5 = {MyModule.Sub(5, 5)}");
    Console.WriteLine($"5 * 5 = {MyModule.Mul(5, 5)}");
    Console.WriteLine($"5 / 5 = {MyModule.Div(5, 5)}");
} 

// Create an alias to module
export Math as M;

// or you can export module later:
// export Math;
// export Calc;

// Main.lun
use Math, M from "./Math";
use Console from System;

Console.WriteLine("11 + 15 = " + Math.Add(11, 15));
Console.WriteLine("1 - 2 = " + M.Sub(1 - 2));

Calc();
// OUTPUT: 
// > 11 + 15 = 26
// > 1 - 2 = -1
// > 5 + 5 = 10
// > 5 - 5 = 0
// > 5 * 5 = 25
// > 5 / 5 = 1
```

### Module extensions and implementations

### Conditional statements

`if condition doSomething(); else doSomethingElse();`

`if condition1 || condition2 { doSomething1(); doSomething2(); }`

### Matches

### Enums

### Classes

### Interfaces

### Generics


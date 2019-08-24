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

let myArray: i32[] = { 1, 2, 3, 4, 5 }; // immutable array of size 5

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

table[0] = null; // Remove the whole row with id = 0
```

### Lists

```rust
let numList = new List<i32>(); // Create a new list
numList.Add(15);
numList.AddRange({ 12, 10, 5, 2 });
```

### Functions

### Modules 

### Classes

### Methods
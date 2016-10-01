# Fortuo
Stack based interpreted programming language

## Fortuo Word List
In Fortuo a command is called a word. Several commands inside curly brackets are called WordSets.					
Comments start with % and span for the rest of the line.					
					
### Reset / Clear Operations
|Name|Consumes|Result|Description|
|---|:---:|:---:|---|
|`clear`|-|-|Resets the interpreter. Clears stack, dictionary.|
|`delstack`|-|-|Deletes the contents of the stack.|
|`deldict`|-|-|Deletes the contents of the global dictionary.|	
|`ccon`|-|-|Clears the console.|

### Output
|Name|Consumes|Result|Description|
|---|:---:|:---:|---|
|`.`|object|-|Pops the top object of the stack and prints it to the output.|
|`h`|Int|-|Prints integer in hex format (base 16).|
|`pstack`|-|-|Prints the stack to the standard output.|
|`pdict`|-|-|Prints the global dictionary to the output.|
|`cr`|-|-|Writes **\r\n** to the standard output.|

### Integer Manipulation
|Name|Consumes|Result|Description|
|---|:---:|:---:|---|
|`+`|Int, Int|Int|Addition **a + b**|
|`-`|Int, Int|Int|Subtraction **a - b**|
|`*`|Int, Int|Int|Multiplication **a * b**|
|`/`|Int, Int|Int|Integer Division **a / b**|
|`mod`|Int, Int|Int|Remainder of Integer Division **a % b**|

### Stack Manipulation 
|Name|Consumes|Result|Description|
|---|:---:|:---:|---|
|`dup`|-|-|Duplicates the top object of the stack.|
|`swap`|-|-|Exchanges the 2 top objects of the stack.|
|`drop`|object|-|Removes the top item from the stack.|

### Definition 
|Name|Consumes|Result|Description|
|---|:---:|:---:|---|
|`/<name>`|-|NameDef|Pushes a NameDef object (Value = <name>) on the stack.|
|`def`|NameDef, object|-|Adds a entry to the global dictionary, which associates the object with the Name specified by the NameDef object.|
|`undef`|NameDef|-|Removes a entry from the global dictionary.|
 
### WordSet 
|Name|Consumes|Result|Description|
|---|:---:|:---:|---|
|`{`|-|StartSet|Pushes a StartSet object on the stack and starts recording a wordset.|
|`}`|objects|WordSet|Ends recording a wordset and pushes the resulting WordSet object on the stack.|

### Values 
|Name|Consumes|Result|Description|
|---|:---:|:---:|---|
|`true, false`|-|Bool|Pushes bool on the stack.|
|`Integer`|-|Int|Pushes an integer on the stack.|
|`"<string>"`|-|String|Strings are enclosed by quotes. When pushing on the stack, the quotes are removed.|

### Integer Comparison 
|Name|Consumes|Result|Description|
|---|:---:|:---:|---|
|`neg`|Int|Int|Changes the sign. **a = -a**|
|`=`| Int, Int|Bool|equal **a == b**|
|`>`| Int, Int|Bool|greater than **a > b**|
|`<`| Int, Int|Bool|less than **a < b**|

### Bool Comparison
|Name|Consumes|Result|Description|
|---|:---:|:---:|---|
|`not`|Bool|Bool|Inverts value. **a = !a**|
|`and`|Bool, Bool|Bool|logical and **a && b**|
|`or`|Bool, Bool|Bool|logical or **a \|\| b**|

### Flow Control
|Name|Consumes|Result|Description|
|---|:---:|:---:|---|
|`if`|Bool, WordSet|-|Executes the Wordset if Bool on the stack is true.|
|`ifelse`|Bool, WordSet, WordSet|-|Executes the first WordSet if the Bool on the stack is true, the seconds, if the Bool is false.|

### Files
|Name|Consumes|Result|Description|
|---|:---:|:---:|---|
|`exec`|String|-|Executes the file, specified by the String.|

### Input
|Name|Consumes|Result|Description|
|---|:---:|:---:|---|
|`linein`|-|String|Reads a line from the standard input and pushes it to the stack.|

### String Manipulation 
|Name|Consumes|Result|Description|
|---|:---:|:---:|---|
|`len`|String|Int|Pushes the length of the top string on the stack.|
|`concat`|String, String|String|Pops 2 strings from the stack, concatenates them and pushes the result on the stack.|
|`comp`|String, String|Bool|Compares 2 strings.|
|`trim`|String|String|Removes all leading or trailing spaces.|
|`getchar`|String, Int|Int|Pushes the value of the character at the specified index of the string to the stack.|
|`substr`|String, Int, Int|String|Pushes a substring of the given string on the stack. Integer on top of stack is length, seconds integer is the startindex.|

### Loops
|Name|Consumes|Result|Description|
|---|:---:|:---:|---|
|`repeat`|Int, WordSet|-|Executes the Wordset a given number of times.|
|`while`|Bool, WordSet|-|Executes a Wordset as long as Bool on top of stack is true.|

### Conversion
|Name|Consumes|Result|Description|
|---|:---:|:---:|---|
|`tostr`|Int or Bool|String Converts an Int or Bool to a String.|
|`tobool`|Int|Bool|Converts an Int to a Bool. Result is true if integer is non-zero.|

### List
|Name|Consumes|Result|Description|
|---|:---:|:---:|---|
|`list`|-|List|Pushes an empty list on the stack.|
|`count`|List|List, Int|Pushes the count of objects in list to the stack.|
|`add`|List, Object|List|Adds an object to the list.|
|`get`|List, Int|List, Object|Pushes the object of list at given index to the stack.|
|`set`|List, Object, Int|List|Sets the object at a given index.|
|`remove`|List, Int|List|Removes object at given index from list.|
|`[`|-|StartList|Pushes a StartList object to the stack and starts recording a list.|
|`]`|objects|List|Ends recording a list and pushes the resulting list to the stack.|
 

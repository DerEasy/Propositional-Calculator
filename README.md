# propcalc
Full-fledged console application for calculating propositional calculus formulas. Can create truth tables, show steps to result, check for validity, etc.

## How to use
You write a file (.txt, but any extension works except .pcl) containing proper syntax, then you call the executable in a console window and it does the rest for you.
- The first program argument must be the file you want to process
- Any further program arguments can be placed in any order you like
propcalc also accepts .pcl files, which are its own compilations. They contain all the necessary data to perform the same calculations as the source code in a binary format.

### Possible arguments
|Argument|Effect|
|--------|------|
|no_table|Prevents a table from being printed.|
|props   |Shows whether the formula is valid, satisfiable, invalid.|
|compile |Compiles the code to a .pcl file and runs that. Does nothing if run with a .pcl file.|
|steps   |Prints all the steps to arrive at the result. (Can also be easier to read for humans if the formula is complex.)|
|out     |Makes all print commands write to a .txt file. If your input file is called `prop.txt`, it will appear as `prop_out.txt`|

## Syntax
|Binary operators|Explanation|
|-|-|
|and|AND gate|
|nand|NAND gate|
|or|OR gate|
|nor|NOR gate|
|xor|XOR gate|
|xnor|XNOR gate|
|impl|Material implication|

The operator precedence is in that exact order.

|Unary operator||
|-|-|
|!|NOT gate/Negation/Inverter; inverts the value to the _right_ of it|

Can negate variables, constants and brackets (must be placed left of the left bracket).


|Brackets|
|-|
|(|
|)|

Can be used to change the precedence of operations or to make it easier to read the formula.

|Constants|
|-|
|false|
|true|

Can be used to substitute a variable, does not change its value over the course of the program though.

Variables can be denoted with following characters:
- Lowercase English alphabet a-z
- Uppercase English alphabet A-Z
- Common digits 0-9

The first character may not be a digit.
Variables can be of any character length you desire.

By default, variables are sorted alphabetically when they are printed next to the truth table. You can change the order by making use of the order instruction anywhere in the file: `order c b a;`. The order instruction _must_ end with at least one semicolon. You cannot leave out any variables in the instruction. The variables must be separated with spaces.

## Working example formulas
Formulas may include as many spaces and line breaks as you want.

`a and !(b or !c)`

`!b`

`true`

`(example impl !probablyWorks) xnor !false and u123`

`a or b`

`order b a; a or b`





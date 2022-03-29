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

Can negate variables, constants and brackets (must be placed left of the left bracket). Adjacent ones negate each other.


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

## What to expect
Pretend we have a file logical.txt with following content:
```
order b a c;

a and !(b or !c)
```
Now when we run:
`propcalc logical.txt steps props`

We get:
```
  0 = b or !c
out = a and !0

Valid: False
Satisfiable: True
Invalid: False

[b a c]  a and !(b or !c)
[0 0 0]  0
[0 0 1]  0
[0 1 0]  0
[0 1 1]  1
[1 0 0]  0
[1 0 1]  0
[1 1 0]  0
[1 1 1]  0
```

As you can see, propcalc prints the steps, then the properties of the formula, then the truth table with the correct variable order. It also shows the formula it has just calculated. If you ran the program with a .pcl file, this makes it possible to see what is actually being calculated here.

## Known bugs
The program works well if you use the correct syntax. The syntax also isn't really that difficult and it allows for as much whitespace as you need. Nevertheless, I have yet to implement exceptions that catch incorrect syntax, like two variables/binary operators next to each other, negating a binary operator, leaving out brackets, passing a file that does not contain a formula, etc. Some of these problems will cause the program to crash, others will result in an infinite loop. None will just continue and pretend nothing happened (to my knowledge).

using propcalc;


bool[] mode = new bool[7];
const int no_table = 0;
const int props = 1;
const int compile = 2;
const int steps = 3;
const int output = 4;
const int shell = 5;
const int no_calc = 6;

bool fileIsCompiled = false;
string rawCode = "";
string extension;
string filenoext;


// Program execution
getArguments();
if (mode[shell] && onShell()) 
    exitProgram(0);
if (mode[output]) 
    onOutput();
if (onFile()) 
    exitProgram(0);
// On success end program



void getArguments() {
    if (args.Length == 0)
        args = new[] {"shell"}; // Default execution path is shell
    
    extension = Path.GetExtension(args[0]);
    filenoext = Path.GetFileNameWithoutExtension(args[0]);
    
    for (int i = 0; i < args.Length; ++i) {
        switch (args[i]) {
        case "no_table": mode[no_table] = true; break;
        case "props"   : mode[props]    = true; break;
        case "compile" : mode[compile]  = true; break;
        case "steps"   : mode[steps]    = true; break;
        case "out"     : mode[output]   = true; break;
        case "shell"   : mode[shell]    = true; break;
        case "no_calc" : mode[no_calc]  = true; break;
        }
    }
}


bool onShell() {
    Console.WriteLine("Enter a formula:");
    rawCode = Console.ReadLine() ?? string.Empty;
    Console.WriteLine();

    if (mode[output])
        onOutput();
    
    parse();
    return true;
}


void onOutput() {
    FileStream stream = new(filenoext + "_out.txt", FileMode.Create);
    StreamWriter writer = new(stream);
    writer.AutoFlush = false;
    Console.SetOut(writer);
}


bool onFile() {
    switch (extension) {
    case ".pcl":
        fileIsCompiled = true;
        interpret(args[0]);
        break;
    default:
        fileIsCompiled = false;
        parse();
        break;
    }
    return true;
}


void parse() {
    var (tokens, metadata) = Tokenizer.tokenize(args[0], ref rawCode, mode[shell]);
    if (rawCode.Length == 0) // No code, exit program
        exitProgram(3);
    var split = Splitter.splitBrackets(tokens);
    var (atoms, atomBools) = Atomizer.atomize(split);
    compileCode(atoms, atomBools, metadata);
}


void compileCode(
    Deque< Deque<string> > atoms, 
    Deque< (bool, bool) > atomBools,
    string[] metadata) 
{
    if (mode[steps])
        Steps.showSteps(atoms, atomBools);
    if (mode[compile]) {
        string compiledFilename = Compiler.compileToFile(atoms, atomBools, metadata, rawCode);
        interpret(compiledFilename);
    }
    else {
        Deque<object> compilation = Compiler.compileInMemory(atoms, atomBools, metadata, rawCode);
        interpret(compilation);
    }
}


void interpret(object interpretableObject) {
    Interpreter interpreter;
    
    if (interpretableObject is string compiledFilename)
        interpreter = new Interpreter(compiledFilename);
    else if (interpretableObject is Deque<object> compilation)
        interpreter = new Interpreter(compilation);
    else throw new Exception("Object cannot be interpreted.");
    
    if (!mode[no_calc])
        interpreter.interpret();

    if (fileIsCompiled && mode[steps])
        Steps.showSteps(args[0]);
    if (mode[props] && !mode[no_calc])
        interpreter.properties();
    if (!mode[no_table] && !mode[no_calc])
        interpreter.table();
}


void exitProgram(int exitCode) {
    Console.Out.Flush();
    Environment.Exit(exitCode);
}
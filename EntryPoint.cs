using propcalc;

if (args.Length == 0) {
    Console.WriteLine("Must specify file to open.");
    Environment.Exit(1);
}


const int no_table = 0;
const int props = 1;
const int compile = 2;
const int steps = 3;
const int output = 4;

bool[] mode = new bool[5];
for (int i = 1; i < args.Length; ++i) {
    switch (args[i]) {
    case "no_table": mode[no_table] = true; break;
    case "props"   : mode[props]    = true; break;
    case "compile" : mode[compile]  = true; break;
    case "steps"   : mode[steps]    = true; break;
    case "out"     : mode[output]   = true; break;
    }
}

bool isCompiled;
string rawCode;
string extension = Path.GetExtension(args[0]);
string filenoext = Path.GetFileNameWithoutExtension(args[0]);

if (mode[output]) {
    FileStream stream = new(filenoext + "_out.txt", FileMode.Create);
    StreamWriter writer = new(stream);
    writer.AutoFlush = false;
    Console.SetOut(writer);
}

switch (extension) {
case ".pcl":
    isCompiled = true;
    interpret(args[0]);
    break;
default:
    isCompiled = false;
    parse();
    break;
}

void parse() {
    var (tokens, metadata) = Tokenizer.tokenize(args[0], out rawCode);
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
    
    interpreter.interpret();

    if (isCompiled && mode[steps])
        Steps.showSteps(args[0]);
    if (mode[props])
        interpreter.properties();
    if (!mode[no_table])
        interpreter.table();
}

Console.Out.Flush();
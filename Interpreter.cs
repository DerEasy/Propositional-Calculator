using static propcalc.Compiler;
using static propcalc.Logic;

namespace propcalc; 

public class Interpreter {
    private readonly bool[] cache; // intermediates ... variables ...
    private readonly Statement[] statement;
    private readonly string[] varNames;
    private readonly int stmtCount;
    private readonly int varCount;
    private readonly int result; // Index of result statement
    public readonly string rawInput;
    
    // 0 - valid, 1 - satisfiable, 2 - invalid
    private bool[] props = {true, false, true};
    private const int valid = 0;
    private const int satisfiable = 1;
    private const int invalid = 2;


    public Interpreter(string filename) {
        BinaryReader file = new(File.OpenRead(filename));
        
        stmtCount = file.ReadInt32();
        varCount = file.ReadInt32();
        file.ReadBytes(bufferToStatements);
        
        statement = new Statement[stmtCount];
        varNames = new string[varCount];
        cache = new bool[stmtCount + varCount + 2]; // 2 extra for booleans
        cache[stmtCount + varCount + 1] = true; // Constant boolean true
        result = stmtCount - 1;

        for (int i = 0; i < stmtCount; ++i)
            statement[i] = new Statement(
                file.ReadInt32(),
                file.ReadInt32(),
                file.ReadInt32() - interStart,
                file.ReadInt32() - interStart,
                file.ReadBoolean(),
                file.ReadBoolean()
                );
        
        for (int i = 0; i < varCount; ++i)
            varNames[i] = file.ReadString();
        
        rawInput = file.ReadString();
        file.Dispose();
    }
    
    public Interpreter(Deque<object> compilation) {
        stmtCount = (int) compilation.PopFirst();
        varCount = (int) compilation.PopFirst();
        
        statement = new Statement[stmtCount];
        varNames = new string[varCount];
        cache = new bool[stmtCount + varCount + 2];
        cache[stmtCount + varCount + 1] = true;
        result = stmtCount - 1;
        
        for (int i = 0; i < stmtCount; ++i)
            statement[i] = new Statement(
                (int) compilation.PopFirst(),
                (int) compilation.PopFirst(),
                (int) compilation.PopFirst() - interStart,
                (int) compilation.PopFirst() - interStart,
                (bool) compilation.PopFirst(),
                (bool) compilation.PopFirst()
                );
        
        for (int i = 0; i < varCount; ++i)
            varNames[i] = (string) compilation.PopFirst();
        rawInput = (string) compilation.PopFirst();
    }

    public void interpret(bool table, bool properties) {
        if (table && properties)
            interpretTableProps();
        else if (table && !properties)
            interpretTable();
        else if (!table && properties)
            interpretProps();
    }
    
    private void interpretTable() {
        Table table = new(rawInput, varCount, varNames);
        int i = default, max = 1 << varCount;

        while (i < max) {
            executeStatements();
            table.writeOutputAndIncrement(cache[result], ++i);
            iterateVariables(i);
        }
    }
    
    private void interpretTableProps() {
        Table table = new(rawInput, varCount, varNames);
        int i = default, max = 1 << varCount;
        
        while (i < max) {
            executeStatements();
            table.writeOutputAndIncrement(cache[result], ++i);
            iterateVariables(i);
            
            if (cache[result]) {
                props[satisfiable] = true;
                props[invalid] = false;
            }   else props[valid] = false;
        }
    }
    
    private void interpretProps() {
        int i = default, max = 1 << varCount;
        
        while (i < max) {
            executeStatements();
            iterateVariables(++i);
            
            if (cache[result]) {
                props[satisfiable] = true;
                props[invalid] = false;
            }
            else props[valid] = false;
        }
    }

    private void executeStatements() {
        for (int i = 0; i < stmtCount; ++i)
            cache[i] = executeAtom(statement[i]);
    }
    
    private bool executeAtom(Statement atom) {
        bool varLeft = atom.boolLeft ? 
            !cache[atom.varLeft] : cache[atom.varLeft];
        bool varRight = atom.boolRight ? 
            !cache[atom.varRight] : cache[atom.varRight];
        
        return gate[atom.binOperator](varLeft, varRight);
    }

    private void iterateVariables(int n) {
        for (int i = 0; i < varCount; ++i)
            cache[i + stmtCount] =
                ((n >> (varCount - i - 1)) & 1) == 1;
    }
    
    public void properties() {
        Console.WriteLine(
            $"\n{"Valid:", -12} {props[valid]}" +
            $"\n{"Satisfiable:", -12} {props[satisfiable]}" +
            $"\n{"Invalid:", -12} {props[invalid]}"
        );
    }
}
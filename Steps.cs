using System.Text;
using static propcalc.Compiler;

namespace propcalc; 

public static class Steps {
    public static void showSteps(
        Deque< Deque<string> > atoms, 
        Deque< (bool, bool) > atomBools) 
    {
        StringBuilder sb = new(64);
    
        for (int i = 0; i < atoms.Size(); ++i) {
            if (i < atoms.Size() - 1)
                sb.Append($"{i,3} = ");
            else sb.Append($"{"out",3} = ");
        
            if (atomBools[i].Item1)
                sb.Append('!');
            sb.Append($"{atoms[i][0]} {atoms[i][1]} ");
            if (atomBools[i].Item2)
                sb.Append('!');
            sb.Append(atoms[i][2]);
        
            Console.WriteLine(sb);
            sb.Clear();
        } Console.WriteLine();
    }
    
    
    public static void showSteps(string filename) {
        BinaryReader file = new(File.OpenRead(filename));
        int stmtCount = file.ReadInt32();
        int varCount = file.ReadInt32();
        file.ReadBytes(bufferToStatements);
        
        Statement[] statement = new Statement[stmtCount];
        string[] varNames = new string[varCount];
        
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
        file.Dispose();
        
        showSteps(statement, varNames);
    }
    
    private static void showSteps(Statement[] statement, string[] varNames) {
        StringBuilder sb = new(64);
        int stmtCount = statement.Length;
        
        for (int i = 0; i < stmtCount; ++i) {
            if (i < stmtCount - 1)
                sb.Append($"{i,3} = ");
            else sb.Append($"{"out",3} = ");
            
            if (statement[i].boolLeft)
                sb.Append('!');
            
            sb.Append(
                $"{extractVar(varNames, stmtCount, statement[i].varLeft)} " +
                $"{extractOperator(statement[i].binOperator)} ");
            
            if (statement[i].boolRight)
                sb.Append('!');
            
            sb.Append(extractVar(varNames, stmtCount, statement[i].varRight));
            
            Console.WriteLine(sb);
            sb.Clear();
        }
        Console.WriteLine();
    }
    
    private static string extractVar(string[] varNames, int stmtCount, int pos) {
        int varCount = varNames.Length;
        
        if (pos < stmtCount)
            return pos.ToString();
        else if (pos < stmtCount + varCount)
            return varNames[pos - stmtCount];
        else if (pos == stmtCount + varCount)
            return "false";
        else if (pos == stmtCount + varCount + 1)
            return "true";
        else throw new IndexOutOfRangeException("Variable does not exist.");
    }
    
    private static string extractOperator(int code) {
        return code switch {
            0 => "and",
            1 => "nand",
            2 => "or",
            3 => "nor",
            4 => "xor",
            5 => "xnor",
            6 => "impl",
            _ => throw new IndexOutOfRangeException("Operator does not exist.")
        };
    }
}
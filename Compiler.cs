namespace propcalc; 

public class Compiler {
    private const int undef = -1;
    private const int binOp = 1;
    private const int varLeft = 0;
    private const int varRight = 2;
    public const int interStart = 7;
    public const int bufferToStatements = 10;
    
    private const int and = 0;
    private const int nand = 1;
    private const int or = 2;
    private const int nor = 3;
    private const int xor = 4;
    private const int xnor = 5;
    private const int impl = 6;
    


    public static string compileToFile(
        Deque< Deque<string> > atoms,
        Deque< (bool, bool) > atomBools,
        string[] metadata,
        string rawInput) 
    {
        string filename = Path.GetFileNameWithoutExtension(metadata[0]) + ".pcl";
        int intermediateCount = atoms.Size();
        int varCount = metadata.Length - 1;
        
        string[] vars = new string[varCount];
        Array.Copy(metadata, 1, vars, 0, varCount);
        
        File.Delete(filename); // Delete old file if there is one
        BinaryWriter file = new(File.OpenWrite(filename));
        
        file.Write(intermediateCount); // 4 Bytes
        file.Write(varCount); // 4 Bytes
        file.Write(new byte[bufferToStatements]); // Leaves 10 Bytes to reach default of 18

        for (int i = 0; i < atoms.Size(); ++i) {
            var (boolLeft, boolRight) = atomBools[i];
            
            file.Write(i); // Current intermediate value
            file.Write(strToCode(atoms[i][binOp], intermediateCount, vars));
            file.Write(strToCode(atoms[i][varLeft], intermediateCount, vars));
            file.Write(strToCode(atoms[i][varRight], intermediateCount, vars));
            file.Write(boolLeft);
            file.Write(boolRight);
        }
        
        foreach (string variable in vars)
            file.Write(variable);
        
        file.Write(rawInput);
        
        file.Dispose();
        return filename;
    }
    
    
    public static Deque<object> compileInMemory(
        Deque< Deque<string> > atoms,
        Deque< (bool, bool) > atomBools,
        string[] metadata,
        string rawInput) 
    {
        Deque<object> compilation = new();
        int intermediateCount = atoms.Size();
        int varCount = metadata.Length - 1;
        
        string[] vars = new string[varCount];
        Array.Copy(metadata, 1, vars, 0, varCount);
        
        compilation.Append(intermediateCount);
        compilation.Append(varCount);
        
        for (int i = 0; i < atoms.Size(); ++i) {
            var (boolLeft, boolRight) = atomBools[i];
            
            compilation.Append(i);
            compilation.Append(strToCode(atoms[i][binOp], intermediateCount, vars));
            compilation.Append(strToCode(atoms[i][varLeft], intermediateCount, vars));
            compilation.Append(strToCode(atoms[i][varRight], intermediateCount, vars));
            compilation.Append(boolLeft);
            compilation.Append(boolRight);
        }
        
        foreach (string variable in vars)
            compilation.Append(variable);
        
        compilation.Append(rawInput);
        return compilation;
    }

    
    private static int strToCode(string str, int intermediateCount, string[] vars) {
        switch (str) { // First check if it's a boolean or a logic gate
        case "false":
            return interStart + intermediateCount + vars.Length;
        case "true" : 
            return interStart + intermediateCount + vars.Length + 1;
        case "and"  : return and;
        case "nand" : return nand;
        case "or"   : return or;
        case "nor"  : return nor;
        case "xor"  : return xor;
        case "xnor" : return xnor;
        case "impl" : return impl;
        }

        // Then check if it's a variable
        int varIndex = strIsVar(str, vars);
        if (varIndex != undef) // Definition of var mem location
            return interStart + intermediateCount + varIndex;
        
        // Else it must be an intermediate value
        return interStart + int.Parse(str);
    }
    
    private static int strIsVar(string str, string[] vars) {
        for (int i = 0; i < vars.Length; ++i)
            if (str.Equals(vars[i]))
                return i;
        return undef;
    }
}
using System.Text;

namespace propcalc; 

public class Table {
    private readonly string rawInput;
    private readonly int varCount;
    private readonly string[] varNames;
    private readonly int[] varCache;
    private readonly Deque<bool>.DequeIterator it;
    
    public Table(
        string rawInput,
        int varCount,
        string[] varNames,
        Deque<bool>.DequeIterator it
        ) {
        this.rawInput = rawInput;
        this.varCount = varCount;
        this.varNames = varNames;
        this.it = it;
        varCache = new int[varCount];
    }
    
    private void iterateVariables(int n) {
        for (int i = 0; i < varCount; ++i)
            varCache[i] = (n >> (varCount - i - 1)) & 1;
    }
    
    private string[] spaceWidthForVars() {
        var spaces = new string[varCount];
        for (int i = 0; i < varCount; ++i)
            spaces[i] = new string(' ', varNames[i].Length);
        
        //Last one needs one space less because of the square bracket
        if (spaces.Length > 0)
            spaces[^1] = spaces[^1].Remove(spaces[^1].Length - 1);
        return spaces;
    }

    private static string joinStringArrays(string[] separators, int[] values) {
        StringBuilder sb = new();
        for (int i = 0; i < values.Length; ++i) {
            sb.Append(values[i]);
            sb.Append(separators[i]);
        }
        return sb.ToString();
    }
    
    private string topRow() => 
        $"[{string.Join(' ', varNames)}]  {rawInput}";
    
    private string row(string[] spaces, bool result) => 
        $"[{joinStringArrays(spaces, varCache)}]  {(result ? 1 : 0)}";

    public void createTable() {
        int n = 0;
        var spaces = spaceWidthForVars();
        
        Console.WriteLine(topRow());
        while (it.ToNext()) {
            Console.WriteLine(row(spaces, it.Get()));
            iterateVariables(++n);
        }
    }
}
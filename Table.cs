using System.Text;

namespace propcalc; 

public class Table {
    private readonly string rawInput;
    private readonly string[] varNames;
    private readonly string[] spaces;
    private readonly int varCount;
    private readonly int[] varCache;
    private Task ioTask = new(() => {});

    public Table(string rawInput, int varCount, string[] varNames) {
        this.rawInput = rawInput;
        this.varCount = varCount;
        this.varNames = varNames;
        spaces = spaceWidthForVars();
        varCache = new int[varCount];
        ioTask.Start();
        
        Console.WriteLine(topRow());
    }

    private string[] spaceWidthForVars() {
        var pSpaces = new string[varCount];
        for (int i = 0; i < varCount; ++i)
            pSpaces[i] = new string(' ', varNames[i].Length);
        
        //Last one needs one space less because of the square bracket
        if (pSpaces.Length > 0)
            pSpaces[^1] = pSpaces[^1].Remove(pSpaces[^1].Length - 1);
        return pSpaces;
    }
    
    private void iterateVariables(int n) {
        for (int i = 0; i < varCount; ++i)
            varCache[i] = (n >> (varCount - i - 1)) & 1;
    }
    
    private string joinStringArrays() {
        StringBuilder sb = new();
        for (int i = 0; i < varCache.Length; ++i) {
            sb.Append(varCache[i]);
            sb.Append(spaces[i]);
        }
        return sb.ToString();
    }
    
    public void writeOutputAndIncrement(bool result, int iteration) {
        ioTask.Wait();
        ioTask = Console.Out.WriteLineAsync(row(result ? 1 : 0));
        iterateVariables(iteration);
    }
    
    private string topRow() => $"[{string.Join(' ', varNames)}]  {rawInput}";
    
    private string row(int result) => $"[{joinStringArrays()}]  {result}";
}
using System.Text.RegularExpressions;
using static propcalc.Syntax;
using static propcalc.Syntax.TYPE;

namespace propcalc; 

public class Tokenizer {
    private const string regexOrder = "order +([a-zA-Z][a-zA-Z0-9]* *)+;+";
    private const string regexVar = "[a-zA-Z][a-zA-Z0-9]*";
    private const string regexSpecialSpaces = "\n|\r|\t";
    private const string regexReduceSpaces = " {2,}";
    
    
    public static (Deque<string>, string[]) tokenize(string filename, ref string raw, bool isShell) {
        string str = isShell ? raw : File.ReadAllText(filename);
        string[] metadata = extractExplicitOrder(str, filename);
            
        if (metadata.Length <= 1)
            metadata = extractImplicitOrder(str, filename);
        else str = str.Replace(
            Regex.Matches(str, regexOrder)
                .Select(m => m.Value)
                .ToArray()[0]
                .ToString(),
            ""); // Remove order statement from source code
        
        str = Regex.Replace(str, regexSpecialSpaces, " ");
        str = Regex.Replace(str, regexReduceSpaces, " ");
        raw = str.Trim();

        string[] split = // Split the source code
            Regex.Matches(str, "[\\(\\)!]|[a-zA-Z0-9]+")
            .Select(m => m.Value)
            .ToArray();
        
        var deq = new Deque<string>(split); // Save it in a deque
        deq.Prepend(strOf(brl)); // Obligatory brackets
        deq.Append(strOf(brr));
        
        return (deq, metadata);
    }
    
    private static string[] extractImplicitOrder(string str, string filename) { 
        var a = 
            new Deque<string>(
                Regex.Matches(str, regexVar)
                .Select(m => m.Value)
                .ToArray()
            )
            .ConvertToSet();

        while (a.iterator.HasNext()) {
            if (typeOf(a.iterator.GetNext()) != Var)
                a.iterator.RemoveNext();
            else
                a.iterator.ToNext();
        }
        
        var sorted = a.ToArray();
        Array.Sort(sorted);
        return sorted.Prepend(filename).ToArray();
    }
    
    private static string[] extractExplicitOrder(string str, string filename) {
        if (!Regex.IsMatch(str, regexOrder))
            return new [] {filename};
        
        string statement = 
            Regex.Matches(str, regexOrder)
                .Select(m => m.Value)
                .ToArray()[0]
                .ToString();
        
        string[] metadata =
            Regex.Matches(statement, "[^order]([a-zA-Z][a-zA-Z0-9]* *)+")
                .Select(m => m.Value)
                .ToArray()[0]
                .ToString()
                .Split(Array.Empty<char>(), StringSplitOptions.RemoveEmptyEntries);
        
        return metadata.Prepend(filename).ToArray();
    }
}
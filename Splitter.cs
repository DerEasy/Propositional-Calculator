using static propcalc.Syntax;

namespace propcalc;

public static class Splitter {
    public static Deque< Deque<string> > splitBrackets(Deque<string> str) {
        
        Deque<int> indexOfBrackets = new(); // Gets trashed after return
        Deque< Deque<string> > cutBrackets = new(); // Will return
        int replacement = 0; // Intermediate counter

        
        for (int i = 0; i < str.Size(); ++i) {
            if (str[i].Equals(strOf(brl)))
                indexOfBrackets.Append(i);
            
            else if (str[i].Equals(strOf(brr))) {
                int left = indexOfBrackets.PopLast() + 1;
                int right = i - 1;
                
                cutBrackets.Append(new());
                int amountOfBinOps = 0;
                
                for (int j = left; j <= right; ++j) {
                    cutBrackets.ReadLast().Append(str[j]); // Create new intermediate
                    if (isBinOp(str[j])) 
                        ++amountOfBinOps;
                }

                if (amountOfBinOps == 0)
                    amountOfBinOps = 1; // To make it work with unary not clauses
                
                str.iterator.To(left - 2);
                replaceBracket(str, replacement + amountOfBinOps - 1); // Replace bracket with intermediate
                replacement += amountOfBinOps;
                i = left - 1;
            }
        }
        
        return cutBrackets;
    }
    
    private static void replaceBracket(Deque<string> str, int replacement) {
        while (!str.iterator.GetNext().Equals(strOf(brr)))
            str.iterator.RemoveNext();
        
        str.iterator.ToNext();
        str.iterator.Set(replacement.ToString());
    }
}
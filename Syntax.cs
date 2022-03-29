using System.Text.RegularExpressions;
using static propcalc.Syntax.TYPE;

namespace propcalc; 

public static class Syntax {
    private static string[] key = {
        "(", ")", "!", "and", "nand", "or", "nor", "xor", "xnor", "impl", "false", "true"
    };

    public const int undef = -1;
    public const int brl = 0;
    public const int brr = 1;
    public const int not = 2;
    public const int and = 3;
    public const int nand = 4;
    public const int or = 5;
    public const int nor = 6;
    public const int xor = 7;
    public const int xnor = 8;
    public const int impl = 9;
    public const int fal = 10;
    public const int tru = 11;

    public enum TYPE {
        Error,
        Key,
        Var
    }
    
    public static string strOf(int k)
        =>key[k];
    public static bool isIntermediate(string v)
        => !key.Any(k => k.Equals(v)) && Regex.IsMatch(v, "[a-zA-Z0-9]+") || isBool(v);

    
    public static TYPE typeOf(string k) {
        if (key.Any(v => v.Equals(k)))
            return Key;
        if (Regex.IsMatch(k, "[a-zA-Z][a-zA-Z0-9]*"))
            return Var;
        return Error;
    }
    
    public static bool isBool(string b) {
        return b.Equals(strOf(fal)) ||
               b.Equals(strOf(tru));
    }
    
    public static bool isBinOp(string b) {
        for (int i = and; i <= impl; ++i)
            if (key[i].Equals(b))
                return true;
        return false;
    }
    
    public static int binOpInt(string b) {
        for (int i = and; i <= impl; ++i)
            if (key[i].Equals(b))
                return i;
        return undef;
    }
}
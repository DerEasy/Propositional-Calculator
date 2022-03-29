namespace propcalc; 

public static class Logic {
    public delegate bool logicGate(bool a, bool b);
    
    public static readonly logicGate[] gate = {
        and,
        nand,
        or,
        nor,
        xor,
        xnor,
        impl
    };
    
    public static bool and(bool a, bool b)
        => a && b;
    public static bool nand(bool a, bool b) 
        => !(a && b);
    public static bool or(bool a, bool b)
        => a || b;
    public static bool nor(bool a, bool b)
        => !(a || b);
    public static bool xor(bool a, bool b)
        => a != b;
    public static bool xnor(bool a, bool b)
        => a == b;
    public static bool impl(bool a, bool b)
        => !a || b;
}
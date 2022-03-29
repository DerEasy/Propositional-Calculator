namespace propcalc; 

public readonly record struct Statement(
    int intermediate,
    int binOperator,
    int varLeft,
    int varRight,
    bool boolLeft,
    bool boolRight
    );
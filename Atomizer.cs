using System.Diagnostics;
using static propcalc.Syntax;

namespace propcalc;

public static class Atomizer {
    public static (Deque< Deque<string> >, Deque< (bool, bool) >) atomize(Deque< Deque<string> > bracket) {
        Deque< Deque<string> > atomized = new();
        Deque< (bool, bool) >  atomizedNegs = new();
        Deque< Deque<bool> >   negInter = removeNegations(bracket);
        int intermediate = 0;
        
        //    1    0         2         3         4
        // a or b and c | b nor c | 1 xor 2 | a or 3 | 4
        // 0    1     0 | 0     1 | 1     0 | 0    0 | 1
        
        for (int i = 0; i < bracket.Size(); ++i) {
            Deque<string> whole = bracket[i];
            Deque<bool>   neg = negInter[i];
            
            if (whole.Size() == 1) { // Single negation, no binary operator
                whole.Append(strOf(and));
                whole.Append(whole.ReadFirst()); // Idempotence law
                neg.Append(neg.ReadFirst()); // Duplicate negation value
            }
            
            for (int j = 0; j < neg.Size(); ++j) {
                var pos = positionOfSignificantOp(whole);
                var (atom, atomBool) = cutNextOperation(whole, neg, pos, ref intermediate);
                atomized.Append(atom);
                atomizedNegs.Append(atomBool);
            }
        }
        
        return (atomized, atomizedNegs);
    }
    
    private static (Deque<string>, (bool, bool)) cutNextOperation(
        Deque<string> whole, Deque<bool> booleans, (int, int) positions, ref int intermediate) 
    {
        const int distanceLeftToOp = 2;
        const int atomSubParts = 3;
        
        var (operatorPos, boolPos) = positions;
        Deque<string> atom = new();

        whole.iterator.To(operatorPos - distanceLeftToOp);
        for (int i = 0; i < atomSubParts; ++i) {
            atom.Append(whole.iterator.GetNext());
            whole.iterator.RemoveNext();
        } 
        whole.iterator.AddNext(intermediate++.ToString());
        
        
        booleans.iterator.To(boolPos);
        (bool, bool) linkedBools = (booleans.iterator.Get(), booleans.iterator.GetNext());
        
        booleans.iterator.RemoveNext();
        booleans.iterator.Set(false); // Do not negate the resulting clause
        
        return (atom, linkedBools);
    }
    
    private static (int, int) positionOfSignificantOp(Deque<string> whole) {
        int lowest = int.MaxValue;
        int position = undef;
        //int boolPosition = undef; boolPositions can be calculated with position / 2
        
        for (int i = 0; i < whole.Size() - 1; ++i) {
            if (isIntermediate(whole[i])) {
                //++boolPosition;
                continue;
            }
            
            int opInt = binOpInt(whole[i]);
            if (opInt == undef) continue;
            
            if (opInt < lowest) {
                lowest = opInt;
                position = i;
                if (lowest == and)
                    return (position, position / 2);
            }
        }
        
        return (position, position / 2);
    }

    private static Deque< Deque<bool> > removeNegations(Deque< Deque<string> > bracket) {
        Deque< Deque<bool> > negationRecord = new();
        
        for (int i = 0; i < bracket.Size(); ++i) {
            Deque<string> single = bracket[i];
            Deque<bool> negations = new();

            for (int j = -1; j < single.Size() - 1; ++j) {
                if (isBinOp(single[j + 1]))
                    continue;
                
                int notCount = 0;
                while (single[j + 1].Equals(strOf(not))) {
                    single.iterator.ToPrev(); // Indexing moved iterator one to the right
                    single.iterator.RemoveNext();
                    ++notCount;
                }
                
                if (!isIntermediate(single[j + 1]))
                    throw new Exception("Can only negate intermediates.");
                
                negations.Append((notCount & 1) == 1); // Check if intermediate really _is_ negated
            }
            
            negationRecord.Append(negations);
        }
        
        return negationRecord;
    }
}
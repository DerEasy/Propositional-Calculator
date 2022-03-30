using System.Text;

namespace propcalc; 

public class Deque<T> {
    public class Node {
        public T data;
        public Node prev, next;

        public Node() {
            data = default!; prev = default!; next = default!;
        }
        public Node(T d, Node p, Node n) {
            data = d; prev = p; next = n;
        }
    }

    public readonly DequeIterator iterator;
    private readonly Node head, tail;
    private int size;

    public Deque() {
        head = new Node();
        tail = new Node();
        head.next = tail;
        head.prev = head;
        tail.prev = head;
        tail.next = tail;
        iterator = new DequeIterator(this);
    }
    
    public Deque(IEnumerable<T> init) {
        head = new Node();
        tail = new Node();
        head.next = tail;
        head.prev = head;
        tail.prev = head;
        tail.next = tail;
        iterator = new DequeIterator(this);
        foreach (var i in init)
            Append(i);
    }

    private Node First() 
        => head.next;
    private Node Last() 
        => tail.prev;
    public int Size() 
        => size;

    public T ReadFirst() 
        => First().data;
    public T ReadLast() 
        => Last().data;
    public void SetLast(T data) 
        => Last().data = data;
    public void SetFirst(T data) 
        => First().data = data;
    
    
    
    public T this[int index] {
        get => iterator.To(index);
        set { 
            iterator.To(index); 
            iterator.Set(value);
        }
    }

    public void Append(T data) {
        var n = new Node(data, Last(), tail);
        tail.prev.next = n;
        tail.prev = n;
        ++size;
    }

    public void Prepend(T data) {
        var n = new Node(data, head, First());
        head.next.prev = n;
        head.next = n;
        ++size;
    }

    public T PopFirst() {
        var t = ReadFirst();
        head.next = First().next;
        First().next.prev = head;
        --size;
        return t;
    }

    public T PopLast() {
        var t = ReadLast();
        tail.prev = Last().prev;
        Last().prev.next = tail;
        --size;
        return t;
    }
    
    private void Remove(Node n) {
        n.next.prev = n.prev;
        n.prev.next = n.next;
        --size;
    }

    public void Clear() {
        head.next = tail;
        tail.prev = head;
        size = 0;
    }
    
    public void RemoveDuplicates(T data) {
        var n = First();
        bool found = false;
        
        while (n != tail) {
            if (found && n.data!.Equals(data))
                Remove(n);
            if (n.data!.Equals(data))
                found = true;
            n = n.next;
        }
    }
    
    public Deque<T> ConvertToSet() {
        var i = GetIterator();
        while (i.ToNext())
            RemoveDuplicates(i.Get());
        return this;
    }
    
    public T[] ToArray() {
        var n = First();
        T[] arr = new T[size];
        
        for (int i = 0; i < size; n = n.next)
            arr[i++] = n.data;
        
        return arr;
    }
    
    public override string ToString() {
        StringBuilder s = new();
        
        var n = First();
        while (n != tail) {
            s.Append(n.data + " ");
            n = n.next;
        }

        return s.ToString();
    }
    
    public DequeIterator GetIterator() {
        return new DequeIterator(this);
    }
    
    public class DequeIterator {
        private Node curr;
        private readonly Deque<T> deque;
        private int index = -1;
        
        public DequeIterator(Deque<T> deq) {
            curr = deq.head;
            deque = deq;
        }
        
        public void Reset() => curr = deque.head;
        public bool HasNext() => curr != deque.Last();
        public bool HasPrev() => curr != deque.First();
        public T GetNext() => curr.next.data;
        public T GetPrev() => curr.prev.data;
        public T Get() => curr.data;
        public void Set(T data) => curr.data = data;

        public T To(int i) {
            if (i == 0) {
                index = 0;
                curr = deque.First();
                return Get();
            }

            if (i > index)
                while (index < i)
                    ToNext();
            else if (i < index)
                while (index > i)
                    ToPrev();
            return Get();
        }
        
        public bool ToNext() {
            curr = curr.next;
            if (curr == deque.tail) index = deque.Size();
            else ++index;
            return curr != deque.tail;
        }
        
        public bool ToPrev() {
            curr = curr.prev;
            if (curr == deque.head) index = -1;
            else --index;
            return curr != deque.head;
        }
        
        public bool RemoveNext() {
            if (curr == deque.tail.prev || curr.next == deque.tail)
                return false;
            curr.next.next.prev = curr;
            curr.next = curr.next.next;
            --deque.size;
            return true;
        }
        
        public void AddNext(T data) {
            Node n = new(data, curr, curr.next);
            curr.next.prev = n;
            curr.next = n;
            ++deque.size;
        }
    }
}
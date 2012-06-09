using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Planning
{

    class PriorityQueue<T, V> where V : IComparable
    {
        SortedList<Pair<V, int>, T> _list;
        int count;

        public PriorityQueue()
        {
            _list = new SortedList<Pair<V, int>, T>(new PairComparer<V>());
        }

        public void Enqueue(T item, V priority)
        {
            _list.Add(new Pair<V, int>(priority, count), item);
            count++;
        }

        public T Dequeue()
        {
            T item = _list[_list.Keys[0]];
            _list.RemoveAt(0);
            return item;
        }

        public Boolean isEmpty() 
        {
            return (_list.Count == 0);
        }
    }

    class Pair<T,Integer>
    {
        public T First { get; private set; }
        public int Second { get; private set; }

        public Pair(T first, int second)
        {
            First = first;
            Second = second;
        }

        public override int GetHashCode()
        {
            return First.GetHashCode() ^ Second.GetHashCode();
        }

        public override bool Equals(object other)
        {
            Pair<T,int> pair = other as Pair<T,int>;
            if (pair == null)
            {
                return false;
            }
            return (this.First.Equals(pair.First) && this.Second.Equals(pair.Second));
        }
    }

    class PairComparer<T> : IComparer<Pair<T,int>> where T : IComparable
    {
        public int Compare(Pair<T, int> x, Pair<T, int> y)
        {
            if (x.First.CompareTo(y.First) < 0)
            {
                return -1;
            }
            else if (x.First.CompareTo(y.First) > 0)
            {
                return 1;
            }
            else
            {
                return x.Second.CompareTo(y.Second);
            }
        }
    }

}

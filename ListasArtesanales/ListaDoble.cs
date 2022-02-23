using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListasArtesanales
{
    public class ListaDoble<T> : IEnumerable<T> where T : IComparable
    {
        public NodoDoble<T> First { get; private set; }
        public NodoDoble<T> Last { get; private set; }
        public int Count { get; set; }

        public ListaDoble()
        {
            First = null;
            Last = null;
            Count = 0;
        }

        public void Add(T Value)
        {
            NodoDoble<T> Temp = new NodoDoble<T>(Value);
            if (Count == 0)
            {
                First = Temp;
                Last = Temp;
                First.Next = Last;
                First.Prev = Last;
                Last.Next = First;
                Last.Prev = First;
            }
            else
            {
                Temp.Prev = Last;
                Last.Next = Temp;
                Temp.Next = First;
                First.Prev = Temp;
                Last = Temp;
            }
            Count++;
        }

        public void RemoveFirst()
        {
            if (Count == 1)
            {
                First = null;
                Last = null;
                Count--;
            }
            else if (Count >= 2)
            {
                Last.Next = First.Next;
                First.Next.Prev = Last;
                First = First.Next;
                Count--;
            }
        }

        public void RemoveLast()
        {
            if (Count == 1)
            {
                RemoveFirst();
            }
            else if (Count >= 2)
            {
                First.Prev = Last.Prev;
                Last.Prev.Next = First;
                Last = Last.Prev;
                Count--;
            }
        }

        public bool Remove(T Value)
        {
            if (Count == 0)
            {
                return false;
            }
            else
            {
                NodoDoble<T> Temp = First;
                int i = 0;
                do
                {
                    if (Temp.Value.Equals(Value))
                    {
                        if (i == 0)
                        {
                            RemoveFirst();
                            return true;
                        }
                        if (i == Count - 1)
                        {
                            RemoveLast();
                            return true;
                        }
                        Temp.Prev.Next = Temp.Next;
                        Temp.Next.Prev = Temp.Prev;
                        Temp = null;
                        Count--;
                        return true;
                    }
                    Temp = Temp.Next;
                    i++;
                } while (i < Count);
                return false;
            }
        }

        public void RemoveAll(T Value)
        {
            bool done = Remove(Value);
            while (done)
            {
                done = Remove(Value);
            }
        }

        public void Clear()
        {
            Count = 0;
            First = null;
            Last = null;
        }

        public ListaDoble<T> Search(T Value)
        {
            ListaDoble<T> list = new ListaDoble<T>();
            NodoDoble<T> Temp = First;
            int i = 0;
            do
            {
                if (Temp.Value.Equals(Value))
                {
                    list.Add(Temp.Value);
                }
                Temp = Temp.Next;
                i++;
            } while (i < Count);
            return list;
        }

        public ListaDoble<T> Search(Predicate<T> anon)
        {
            ListaDoble<T> list = new ListaDoble<T>();
            NodoDoble<T> Temp = First;
            int i = 0;
            do
            {
                if (anon(Temp.Value))
                {
                    list.Add(Temp.Value);
                }
                Temp = Temp.Next;
                i++;
            } while (i < Count);
            return list;
        }

        public T[] ToArray()
        {
            return ToList().ToArray();
        }

        public List<T> ToList()
        {
            if (Count == 0)
            {
                return null;
            }
            else
            {
                List<T> list = new List<T>();
                int i = 0;
                NodoDoble<T> Temp = First;
                do
                {
                    list.Add(Temp.Value);
                    Temp = Temp.Next;
                    i++;
                } while (i < Count);
                return list;
            }
        }

        private void SortAsArray(T[] array, int floor, int top ,Delegate comparer)
        {

            int start = floor;
            int end = top;
            int mid = (floor + top) / 2;

            T Aux = default(T);
            T Pivot = array[mid];
            do
            {
                while ((int)comparer.DynamicInvoke(array[start], Pivot) < 0) start++;
                //while (array[start].CompareTo(Pivot) < 0) start++;
                while ((int)comparer.DynamicInvoke(array[start], Pivot) > 0) end++;
                //while (array[end].CompareTo(Pivot) > 0) end--;
                if (start <= end)
                {
                    Aux = array[start];
                    array[start] = array[end];
                    array[end] = Aux;
                    start++;
                    end--;
                }
            } while (start <= end);
            if (floor < end)
            {
                SortAsArray(array, floor, end, comparer);
            }
            if (start < top)
            {
                SortAsArray(array, start, top, comparer);
            }
        }

        public void Sort(Delegate comparer)
        {
            if (Count > 0)
            {
                T[] array = ToArray();
                SortAsArray(array, 0, Count - 1, comparer);
                NodoDoble<T> Temp = First;
                for (int i = 0; i < Count; i++)
                {
                    Temp.Value = array[i];
                    Temp = Temp.Next;
                }
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            NodoDoble<T> current = First;
            int i = 0;
            while (current != null && i < Count)
            {
                yield return current.Value;
                current = current.Next;
                i++;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

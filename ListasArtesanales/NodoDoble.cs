using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListasArtesanales
{
    public class NodoDoble<T>
    {
        public NodoDoble<T> Next;
        public NodoDoble<T> Prev;
        public T Value;

        public NodoDoble(T Value, NodoDoble<T> Next, NodoDoble<T> Prev)
        {
            this.Value = Value;
            this.Prev = Prev;
            this.Next = Next;
        }

        public NodoDoble(T Value) : this(Value, null, null)
        {
        }

        public override bool Equals(object obj)
        {
            try
            {
                NodoDoble<T> nodo = obj as NodoDoble<T>;
                return nodo.Value.Equals(Value);
            }
            catch
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}

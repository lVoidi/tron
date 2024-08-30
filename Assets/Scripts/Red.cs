using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public class Red
    {
        public Nodo[,] RedNodos;

        public Red(int espacios)
        {
            RedNodos = new Nodo[espacios, espacios];
            for (int i = 0; i < espacios; i++)
            {
                for (int j = 0; j < espacios; j++)
                {
                    Nodo nodo = new Nodo();
                    nodo.x = i;
                    nodo.y = j;
                    RedNodos[i, j] = nodo;

                    
                }
            }
            for (int x = 0; x < espacios; x ++)
            {
                for (int y = 0; y < espacios; y++)
                {
                    if (x > 0)
                    {
                        RedNodos[x, y].Izquierda = RedNodos[x - 1, y];
                    }
                    if (x < espacios - 1)
                    {
                        RedNodos[x, y].Derecha = RedNodos[x + 1, y];
                    }
                    if (y > 0)
                    {
                        RedNodos[x, y].Abajo = RedNodos[x, y - 1];
                    }
                    if (y < espacios - 1)
                    {
                        RedNodos[x, y].Arriba = RedNodos[x, y + 1];
                    }
                }
            }
        }

    }
    public class Nodo
    {
        public Nodo Arriba = null;
        public Nodo Abajo = null;
        public Nodo Izquierda = null;
        public Nodo Derecha = null;

        public int x;
        public int y;
    }
}

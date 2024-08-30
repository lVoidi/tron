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

        public Red(int espacios_x, int espacios_y)
        {
            RedNodos = new Nodo[espacios_x, espacios_y];
            for (int i = 0; i < espacios_x; i++)
            {
                for (int j = 0; j < espacios_y; j++)
                {
                    Nodo nodo = new Nodo();
                    nodo.x = i;
                    nodo.y = j;
                    RedNodos[i, j] = nodo;

                    
                }
            }
            for (int x = 0; x < espacios_x; x ++)
            {
                for (int y = 0; y < espacios_y; y++)
                {
                    if (x > 0)
                    {
                        RedNodos[x, y].Izquierda = RedNodos[x - 1, y];
                    } else
                    {
                        RedNodos[x, y].Izquierda = RedNodos[espacios_x - 1, y];
                    }
                    if (x < espacios_x - 1)
                    {
                        RedNodos[x, y].Derecha = RedNodos[x + 1, y];
                    } else
                    {
                        RedNodos[x, y].Derecha = RedNodos[0, y];
                    }
                    if (y > 0)
                    {
                        RedNodos[x, y].Abajo = RedNodos[x, y - 1];
                    } else
                    {
                        RedNodos[x, y].Abajo = RedNodos[x, espacios_y - 1];
                    }
                    if (y < espacios_y - 1)
                    {
                        RedNodos[x, y].Arriba = RedNodos[x, y + 1];
                    } else
                    {
                        RedNodos[x, y].Arriba = RedNodos[x, 0];
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
        public bool esObstaculo = false;
        public bool esCabeza = false;

        public int x;
        public int y;
    }
}

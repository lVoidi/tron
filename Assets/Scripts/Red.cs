/*
 * Clase que representa una red de nodos que simula un espacio toroidal
 * Cada nodo tiene referencias a sus nodos adyacentes
 * Cada nodo tiene un id que representa su posición en la red
 * Cada nodo tiene un booleano que indica si es un obstáculo
 * Cada nodo tiene un booleano que indica si es la cabeza de la serpiente
 * Cada nodo tiene un método que obtiene el nodo adyacente a partir de una dirección
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using System.Text;
using System.Threading.Tasks;

using Vector3 = UnityEngine.Vector3;

namespace Assets.Scripts
{
    public class Red
    {
        public Nodo[,] RedNodos;

        public int largo;
        public int ancho;



        public Red(int espacios_x, int espacios_y)
        {
            // Inicializa la red de nodos

            largo = espacios_x;
            ancho = espacios_y;

            RedNodos = new Nodo[espacios_x, espacios_y];
            for (int i = 0; i < espacios_x; i++)
            {
                for (int j = 0; j < espacios_y; j++)
                {
                    Nodo nodo = new Nodo();
                    nodo.id = new Vector2Int(i, j);
                    RedNodos[i, j] = nodo;
                }
            }

            // Conecta los nodos exteriores para simular un espacio toroidal
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
            SpawnearPoder();
            SpawnearPoder();
            SpawnearPoder();
            SpawnearPoder();
        }

        // Funcion para spawnear un poder en un nodo aleatorio
        public void SpawnearPoder()
        {
            System.Random rnd = new System.Random();
            int x = rnd.Next(2, largo - 2);
            int y = rnd.Next(2, ancho - 2);
            Nodo nodo = RedNodos[x, y];

            while (nodo.esObstaculo || nodo.esCabeza || nodo.poder != null || nodo.item != null)
            {
                x = rnd.Next(2, largo - 2);
                y = rnd.Next(2, ancho - 2);
                nodo = RedNodos[x, y];
            }
            nodo.poder = new Poder();

            // El nombre del poder debe ser uno aleatorio entre "Escudo" y "Velocidad"
            nodo.poder.nombre = (rnd.Next(0, 2) == 0) ? "Escudo" : "Velocidad";

            Vector3 CoordenadaRelativa = CoordenadaACentro(new Vector2Int(x, y));

            if (nodo.poder.nombre == "Escudo")
            {
                Texturas.instancia.EscudoPoder.transform.position = CoordenadaRelativa;
            } else
            {
                Texturas.instancia.VelocidadPoder.transform.position = CoordenadaRelativa;
            }

        }

        // Función que toma una coordenada y le mueve el centro, ya que en unity 
        // el centro de la pantalla está en el medio y no en una esquina

        public Vector3 CoordenadaACentro(Vector2Int coord)
        {
            return new Vector3(coord.x - largo / 2, coord.y - ancho / 2, 10);
        }

        public Nodo ObtenerNodo(Vector2Int id)
        {
            return RedNodos[id.x, id.y];
        }

    }
    public class Nodo
    {
        // Nodos adyacentes
        public Nodo Arriba = null;
        public Nodo Abajo = null;
        public Nodo Izquierda = null;
        public Nodo Derecha = null;

        // Propiedades del nodo
        public bool esObstaculo = false;
        public bool esCabeza = false;
        public Item item = null;
        public Poder poder = null;

        public Vector2Int id;

        // Obtiene el nodo adyacente a partir de una dirección
        public Nodo ObtenerNodoAdyacente(Vector2Int dir)
        {
            if (dir.x == 1)
            {
                return Derecha;
            }
            if (dir.x == -1)
            {
                return Izquierda;
            }
            if (dir.y == 1)
            {
                return Arriba;
            }
            if (dir.y == -1)
            {
                return Abajo;
            }
            return null;

        }
    }
}

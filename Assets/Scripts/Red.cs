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

        private GameObject[] objetos = {
            Texturas.instancia.EscudoPoder,
            Texturas.instancia.VelocidadPoder,
            Texturas.instancia.CombustibleItem,
            Texturas.instancia.EstelaItem,
            Texturas.instancia.BombaItem
        };

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

            GenerarObjetos();

        }
        
        public void GenerarObjetos()
        {
            // Posiciona todos los objetos en lugares aleatorios del mapa
            foreach (GameObject objeto in objetos)
            {
                Vector2Int coord = new Vector2Int(UnityEngine.Random.Range(0, largo), UnityEngine.Random.Range(0, ancho));
                while (RedNodos[coord.x, coord.y].esObstaculo || RedNodos[coord.x, coord.y].item != null)
                {
                    coord = new Vector2Int(UnityEngine.Random.Range(0, largo), UnityEngine.Random.Range(0, ancho));
                }
                PosicionarObjetoEnMapa(coord, objeto);
                CategorizarNodo(objeto, coord);
            }
        }

        public void CategorizarNodo(GameObject objeto, Vector2Int coord)
        {

            if (objeto == Texturas.instancia.EscudoPoder)
            {
                RedNodos[coord.x, coord.y].poder = new Poder
                {
                    nombre = "Escudo",
                };
            }
            else if (objeto == Texturas.instancia.VelocidadPoder)
            {
                RedNodos[coord.x, coord.y].poder = new Poder
                {
                    nombre = "Velocidad",
                };
            }
            else if (objeto == Texturas.instancia.BombaItem)
            {
                RedNodos[coord.x, coord.y].item = new Item
                {
                    nombre = "Bomba",
                };
            }
            else if (objeto == Texturas.instancia.CombustibleItem)
            {
                RedNodos[coord.x, coord.y].item = new Item
                {
                    nombre = "Combustible",
                };
            }
            else if (objeto == Texturas.instancia.EstelaItem)
            {
                RedNodos[coord.x, coord.y].item = new Item
                {
                    nombre = "Estela",
                };

            }
        }

        public void PosicionarObjetoEnMapa(Vector2Int coord, GameObject objeto)
        {
            objeto.transform.position = CoordenadaACentro(coord);
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

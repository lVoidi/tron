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

            largo = espacios_x;
            ancho = espacios_y;

            RedNodos = new Nodo[largo, ancho];
            GenerarNodos();
            ActualizarConexionesNodos();
            GenerarObjetos();

        }

        public void GenerarNodos()
        {
            // Genera los nodos de la red
            for (int i = 0; i < largo; i++)
            {
                for (int j = 0; j < ancho; j++)
                {
                    Nodo nodo = new();
                    nodo.id = new Vector2Int(i, j);
                    RedNodos[i, ancho-j-1] = nodo;
                }
            }
        }


        public void ActualizarConexionesNodos()
        {
            // Conecta los nodos exteriores para simular un espacio toroidal
            for (int x = 0; x < largo; x++)
            {
                for (int y = ancho-1; y >= 0; y--)
                {
                    Nodo nodo = RedNodos[x, y];
                    if (x > 0)
                    {
                        nodo.Izquierda = RedNodos[x - 1, y];
                    }
                    else
                    {
                        nodo.Izquierda = RedNodos[largo - 1, y];
                    }
                    if (x < ancho - 1)
                    {
                        nodo.Derecha = RedNodos[x + 1, y];
                    }
                    else
                    {
                        nodo.Derecha = RedNodos[0, y];
                    }
                    if (y > 0)
                    {
                        nodo.Abajo = RedNodos[x, y - 1];
                    }
                    else
                    {
                        nodo.Abajo = RedNodos[x, ancho - 1];
                    }
                    if (y < ancho - 1)
                    {
                        nodo.Arriba = RedNodos[x, y + 1];
                    }
                    else
                    {
                        nodo.Arriba = RedNodos[x, 0];
                    }
                }
            }
        }

        
        public void GenerarObjetos()
        {
            // Posiciona todos los objetos en lugares aleatorios del mapa
            foreach (GameObject objeto in objetos)
            {
                Vector2Int coord = new Vector2Int(UnityEngine.Random.Range(0, largo), UnityEngine.Random.Range(0, ancho));
                Nodo nodo = RedNodos[coord.x, coord.y];
                while (nodo.esObstaculo || nodo.item != null || nodo.poder != null)
                {
                    coord = new Vector2Int(UnityEngine.Random.Range(0, largo), UnityEngine.Random.Range(0, ancho));
                }
                PosicionarObjetoEnMapa(coord, objeto);
                CategorizarNodo(objeto, coord);
                Debug.Log($"Objeto {objeto.name} en {coord.x},{coord.y}");

            }
        }


        public void CategorizarNodo(GameObject objeto, Vector2Int coord)
        {

            Nodo nodo = RedNodos[coord.x, coord.y];

            if (objeto == Texturas.instancia.EscudoPoder)
            {
                nodo.poder = new Poder
                {
                    nombre = "Escudo",
                };
            }
            else if (objeto == Texturas.instancia.VelocidadPoder)
            {
                nodo.poder = new Poder
                {
                    nombre = "Velocidad",
                };
            }
            else if (objeto == Texturas.instancia.BombaItem)
            {
                nodo.item = new Item
                {
                    nombre = "Bomba",
                };
            }
            else if (objeto == Texturas.instancia.CombustibleItem)
            {
                nodo.item = new Item
                {
                    nombre = "Combustible",
                };
            }
            else if (objeto == Texturas.instancia.EstelaItem)
            {
                nodo.item = new Item
                {
                    nombre = "Estela",
                };

            }
            
        }

        public void PosicionarObjetoEnMapa(Vector2Int coord, GameObject objeto)
        {
            Nodo nodo = ObtenerNodo(coord);
            nodo.id = coord;
            objeto.transform.position = CoordenadaACentro(coord);
        }

        // Función que toma una coordenada y le mueve el centro, ya que en unity 
        // el centro de la pantalla está en el medio y no en una esquina

        public Vector3 CoordenadaACentro(Vector2Int coord)
        {
            return new Vector3(coord.x - largo / 2, coord.y - ancho / 2, 1);
        }

        public Vector3 CentroACoordenada(Vector2Int centro)
        {
            return new Vector3(centro.x + largo / 2, centro.y + ancho / 2, 1);
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

        public GameObject ObjectoAsignado = null;

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
            else if (dir.x == -1)
            {
                return Izquierda;
            }
            else if (dir.y == 1)
            {
                return Arriba;
            }
            else
            {
                return Abajo;
            }

        }

        public Nodo ObtenerNodoDireccionContraria(Vector2Int dir)
        {
            if (dir.x == 1)
            {
                return Izquierda;
            } else if (dir.x == -1)
            {
                return Derecha;
            } else if (dir.y == 1)
            {
                return Abajo;
            } else
            {
                return Arriba;
            }
        }
    }
}

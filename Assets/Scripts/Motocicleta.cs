/*
 * Clase que representa una motocicleta en el juego de snake
 * 
 * La motocicleta se mueve en un espacio de nodos, donde cada nodo puede ser un obstaculo, un espacio vacio o la cabeza de la motocicleta.
 * La motocicleta tiene una dirección de movimiento, un tamaño de estela, una velocidad, un combustible, un estado de inmunidad, un estado de vida y un estado de jugador.
 * La motocicleta puede utilizar items y poderes que le permiten aumentar su combustible, aumentar su tamaño de estela, aumentar su velocidad, poner obstaculos en el espacio y volverse inmune.
 * La motocicleta se mueve en la dirección de movimiento, intercambiando la cabeza de la motocicleta por el nodo adyacente en la dirección de movimiento.
 * La motocicleta genera una estela de tamaño de estela, que se va actualizando cada vez que la motocicleta se mueve.
 * La motocicleta muere si la cabeza de la motocicleta se encuentra con un obstaculo o con la estela de la motocicleta.
 * La motocicleta muere si se queda sin combustible.
 * La motocicleta puede utilizar un poder de la pila de poderes, que le permite aumentar su velocidad o volverse inmune.
 * La motocicleta puede utilizar un item de la cola de items, que le permite aumentar su combustible, aumentar su tamaño de estela o poner obstaculos en el espacio.
 * 
 * La motocicleta se utiliza en el juego de snake, donde el jugador controla la motocicleta y debe evitar chocar con obstaculos y con la estela de la motocicleta.
 * El jugador puede recoger items que le permiten aumentar su combustible, aumentar su tamaño de estela o poner obstaculos en el espacio.
 * El jugador puede recoger poderes que le permiten aumentar su velocidad o volverse inmune.
 * El jugador gana si logra sobrevivir el tiempo suficiente y recoger la mayor cantidad de items y poderes.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using System.Text;
using System.Threading.Tasks;

using Random = System.Random;
namespace Assets.Scripts
{
    public class Motocicleta
    {
        // Componente que representa la cabeza de la motocicleta
        public Componente Cabeza;

        // Lista enlazada de componentes que representan la estela de la motocicleta
        public LinkedList<Componente> Estela;

        // Cola de items que la motocicleta puede utilizar
        public ColaItems items;

        // Pila de poderes que la motocicleta puede utilizar
        public PilaPoderes poderes;

        // Red que representa el espacio en el que se mueve la motocicleta
        public Red Espacio;

        // Direccion en la que se mueve la motocicleta
        public Vector2Int direccion;

        // Atributos de la motocicleta
        public int tamagnoEstela = 3;
        public int velocidad = 5;
        public int combustible = 100;
        public bool esInmune = false;
        public bool estaVivo = true;
        public bool esJugador = true;

        // Generador de numeros aleatorios
        private Random rng = new Random();

        public Motocicleta(Vector2Int dir)
        {
            // Inicializa la red
            Espacio = new Red(31, 13);

            // Genera la motocicleta y asigna la cabeza
            Cabeza = new Componente();
            Cabeza.nodo = Espacio.RedNodos[0, 0];
            Cabeza.nodo.esCabeza = true;

            // Inicializa la estela
            Estela = new LinkedList<Componente>();

            // Inicializa la direccion
            direccion = dir;

            // Inicializa la cola de items
            items = new ColaItems();

            // Inicializa la pila de poderes
            poderes = new PilaPoderes();

            // Crea una estela inicial de tamagnoEstela
            for (int i = 1; i < tamagnoEstela; i++)
            {
                Componente componente = new Componente();
                componente.nodo = Espacio.RedNodos[i, 0];
                componente.nodo.esObstaculo = true;
                Estela.AddLast(componente);
            }

        }

        public bool Mover()
        {

            // Analiza si el jugador sigue vivo, mediante el combustible. Si es así, va a intercambiar
            // la cabeza de la motocicleta por el nodo adyacente en la dirección de movimiento.
            if (combustible > 0)
            {
                // Obtiene las coordenadas de la cabeza de la motocicleta actual en la matriz
                int x = (int)Cabeza.nodo.id.x;
                int y = (int)Cabeza.nodo.id.y;

                // Quita la cabeza de la motocicleta
                Espacio.RedNodos[x, y].esCabeza = false;

                // Asigna la nueva cabeza de la motocicleta
                Cabeza.nodo = Cabeza.nodo.ObtenerNodoAdyacente(direccion);

                // Comprueba si la nueva cabeza de la motocicleta es un obstaculo
                // Si es un obstaculo, la motocicleta muere
                if (Cabeza.nodo.esObstaculo | Cabeza.nodo.esCabeza)
                {
                    estaVivo = false;
                    return false;
                }

                Cabeza.nodo.esCabeza = true;


                // Actualiza la red de nodos. El nodo antiguo pasa de ser cabeza a parte de la estela
                Espacio.RedNodos[x, y] = Espacio.RedNodos[x, y].ObtenerNodoAdyacente(direccion);

            }
            // Si la motocicleta se queda sin combustible, muere
            else
            {
                estaVivo = false;
                return false;
            }

            // Actualiza la estela
            Componente nuevoComponente = new Componente();
            nuevoComponente.nodo = Cabeza.nodo;
            Estela.AddFirst(nuevoComponente);

            // Si la estela es más grande que el tamaño de la estela, elimina el último componente
            if (Estela.Count > tamagnoEstela)
            {
                Componente ultimoComponente = Estela.Last.Value;
                ultimoComponente.nodo.esObstaculo = false;
                Estela.RemoveLast();
            }
            return true;
        }



        public void UtilizarPoder()
        {
            // Saca un poder de la pila de poderes
            Poder poder = poderes.SacarPoder();

            // Si no hay poder, no hace nada
            if (poder == null)
            {
                return;
            }

            // Si el poder es inmunidad, la motocicleta se vuelve inmune
            if (poder.nombre == "Inmunidad")
            {
                esInmune = true;
            }

            // Si el poder es velocidad, aumenta la velocidad de la 
            if (poder.nombre == "Velocidad")
            {
                velocidad += rng.Next(1, 10-velocidad);
            }
        }

        public void UtilizarItem()
        {
            // Saca un item de la cola de items
            Item item = items.SacarItem();

            // Si no hay item, no hace nada
            if (item == null)
            {
                return;
            }

            // Si el item es combustible, aumenta el combustible de la motocicleta en un valor aleatorio
            if (item.nombre == "Combustible")
            {
                combustible += rng.Next(1, 100-combustible);
            }

            // Si el item es bomba, pone un obstaculo en la dirección contraria a la que se mueve la motocicleta
            else if (item.nombre == "Bomba")
            {
                if (direccion == new Vector2Int(-1, 0))
                {
                    Cabeza.nodo.Derecha.esObstaculo = true;
                } else if (direccion == new Vector2Int(1, 0))
                {
                    Cabeza.nodo.Izquierda.esObstaculo = true;
                }
                else if (direccion == new Vector2Int(0, -1))
                {
                    Cabeza.nodo.Arriba.esObstaculo = true;
                }
                else if (direccion == new Vector2Int(0, 1))
                {
                    Cabeza.nodo.Abajo.esObstaculo = true;
                }
            }

            // Si el item es estela, aumenta el tamaño de la estela de la motocicleta en 1
            else if (item.nombre == "Estela")
            {
                tamagnoEstela++;
            }
        }
    }


    public class Componente
    {
        public Nodo nodo;
    }

    public class PilaPoderes
    {
        public LinkedList<Poder> poderes;

        public PilaPoderes()
        {
            poderes = new LinkedList<Poder>();
        }

        public void AgregarPoder(Poder poder)
        {
            poderes.AddFirst(poder);
        }

        public Poder SacarPoder()
        {
            if (poderes.Count > 0)
            {
                Poder poder = poderes.First.Value;
                poderes.RemoveFirst();
                return poder;
            }
            return null;
        }
    }

    // Clase que representa un poder
    public class Poder
    {
        public string nombre;
    }

    // Clase que representa una cola de items
    public class ColaItems
    {
        // Arreglo de items
        public Item[] items;
        public int cantidadItems = 0;
        public int maxItems = 10;

        public ColaItems()
        {
            items = new Item[maxItems];
        }

        // Agrega un item a la cola
        public void AgregarItem(Item item)
        {
            if (cantidadItems < maxItems)
            {
                items[cantidadItems] = item;
                cantidadItems++;
            }
        }

        // Saca un item de la cola
        public Item SacarItem()
        {
            if (cantidadItems > 0)
            {
                Item item = items[0];
                for (int i = 0; i < cantidadItems - 1; i++)
                {
                    items[i] = items[i + 1];
                }
                cantidadItems--;
                return item;
            }
            return null;
        }
    }

    // Clase que representa un item
    public class Item
    {
        public string nombre;
    }
}



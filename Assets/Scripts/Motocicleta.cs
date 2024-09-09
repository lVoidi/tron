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
        public Nodo Cabeza;

        // Lista enlazada de componentes que representan la estela de la motocicleta
        public LinkedList<Nodo> Estela;

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

        public int cantidadEscudos = 0;
        public int cantidadVelocidades = 0;
        public int cantidadCombustibles = 0;
        public int cantidadEstelas = 0;
        public int cantidadBombas = 0;


        // Generador de numeros aleatorios
        private Random rng = new Random();

        public Motocicleta(Vector2Int dir, Red espacio)
        {
            Espacio = espacio;
            // Inicializa la estela
            Estela = new LinkedList<Nodo>();

            // Inicializa la direccion
            direccion = dir;

            // Inicializa la cola de items
            items = new ColaItems();

            // Inicializa la pila de poderes
            poderes = new PilaPoderes();
        }

        public void AsignarCabeza(int x, int y)
        {
            Cabeza = Espacio.RedNodos[x, y];
            Cabeza.esCabeza = true;
        }

        public void ComprobarCabeza()
        {
            
            if (Cabeza.item != null)
            {
                Item item = Cabeza.item;
                Cabeza.item = null;
                items.AgregarItem(item);
                Vector2Int PosicionAleatoriaNueva = new Vector2Int(rng.Next(2, Espacio.largo - 2), rng.Next(2, Espacio.ancho - 2));
                if (item.nombre == "Combustible")
                {
                    Espacio.PosicionarObjetoEnMapa(PosicionAleatoriaNueva, Texturas.instancia.CombustibleItem);
                    Espacio.CategorizarNodo(Texturas.instancia.CombustibleItem, PosicionAleatoriaNueva);
                    cantidadCombustibles++;
                }
                else if (item.nombre == "Estela")
                {
                    Espacio.PosicionarObjetoEnMapa(PosicionAleatoriaNueva, Texturas.instancia.EstelaItem);
                    Espacio.CategorizarNodo(Texturas.instancia.EstelaItem, PosicionAleatoriaNueva);
                    cantidadEstelas++;
                }
                else if (item.nombre == "Bomba")
                {
                    Espacio.PosicionarObjetoEnMapa(PosicionAleatoriaNueva, Texturas.instancia.BombaItem);
                    Espacio.CategorizarNodo(Texturas.instancia.BombaItem, PosicionAleatoriaNueva);
                    cantidadBombas++;
                }
                Debug.Log($"Posicion: {PosicionAleatoriaNueva.x},{PosicionAleatoriaNueva.y}");
            }
            else if (Cabeza.poder != null)
            {
                Poder poder = Cabeza.poder;
                Cabeza.poder = null;
                poderes.AgregarPoder(poder);
                Vector2Int PosicionAleatoriaNueva = new Vector2Int(rng.Next(2, Espacio.largo - 2), rng.Next(2, Espacio.ancho - 2));
                if (poder.nombre == "Velocidad")
                {
                    Espacio.PosicionarObjetoEnMapa(PosicionAleatoriaNueva, Texturas.instancia.VelocidadPoder);
                    Espacio.CategorizarNodo(Texturas.instancia.VelocidadPoder, PosicionAleatoriaNueva);
                    cantidadVelocidades++;
                }
                else if (poder.nombre == "Escudo")
                {
                    Espacio.PosicionarObjetoEnMapa(PosicionAleatoriaNueva, Texturas.instancia.EscudoPoder);
                    Espacio.CategorizarNodo(Texturas.instancia.EscudoPoder, PosicionAleatoriaNueva);
                    cantidadEscudos++;
                }
            }
        }

        public void Esquivar()
        {
            // Los bots van a esquivar los obstaculos y la estela de la motocicleta. Para esto, cambia el vector direccion
            if (esJugador)
            {
                return;
            }
            int MakeDecision = rng.Next(0, 5);
            if (MakeDecision == 0)
            {
                return;
            }

            if (Cabeza.ObtenerNodoAdyacente(direccion).esObstaculo)
            {
                
                if (direccion == new Vector2Int(-1, 0) || direccion == new Vector2Int(1, 0))
                {
                    int Decision = rng.Next(0, 2);
                    if (Decision == 0)
                    {
                        direccion = new Vector2Int(0, 1);
                    }
                    else
                    {
                        direccion = new Vector2Int(0, -1);
                    }
                    return;
                }

                // Si va hacia arriba o hacia abajo, va a elegir aleatoriamente si ir hacia la izquierda o a la derecha
                if (direccion == new Vector2Int(0, -1) || direccion == new Vector2Int(0, 1))
                {
                    int Decision = rng.Next(0, 2);
                    if (Decision == 0)
                    {
                        direccion = new Vector2Int(1, 0);
                    }
                    else
                    {
                        direccion = new Vector2Int(-1, 0);
                    }
                    return;
                }

            }

        }

        public bool Mover(Vector2Int pos)
        {

            // Analiza si el jugador sigue vivo, mediante el combustible. Si es así, va a intercambiar
            // la cabeza de la motocicleta por el nodo adyacente en la dirección de movimiento.
            if (combustible > 0 && estaVivo)
            {
                // Obtiene las coordenadas de la cabeza de la motocicleta actual en la matriz

                UnityEngine.Vector3 posNueva = Espacio.CentroACoordenada(pos);

                int x = (int)posNueva.x;
                int y = (int)posNueva.y;

                // Quita la cabeza de la motocicleta
                Cabeza.esCabeza = false;

                Cabeza = Espacio.RedNodos[x,y];
                if (Cabeza.esObstaculo || Cabeza.esCabeza)
                {
                    estaVivo = false;
                    return false;
                }
                ComprobarCabeza();
                Esquivar();
                // Asigna la nueva cabeza de la motocicleta
                Cabeza = Cabeza.ObtenerNodoAdyacente(direccion);

                // Comprueba si la nueva cabeza de la motocicleta es un obstaculo
                // Si es un obstaculo, la motocicleta muere
                
                Cabeza.esCabeza = true;
            }

            // Si la motocicleta se queda sin combustible, muere
            else
            {
                estaVivo = false;
                return false;
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
                cantidadEscudos--;
            }


            // Si el poder es velocidad, aumenta la velocidad de la 
            if (poder.nombre == "Velocidad")
            {
                int nuevaVelocidad = velocidad + rng.Next(1, 11 - velocidad);
                if (nuevaVelocidad > 10)
                {
                    nuevaVelocidad = 10;
                }
                velocidad = nuevaVelocidad;
                cantidadVelocidades--;
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
                cantidadCombustibles--;
            }

            // Si el item es bomba, pone un obstaculo en la dirección contraria a la que se mueve la motocicleta
            else if (item.nombre == "Bomba")
            {
                if (direccion == new Vector2Int(-1, 0))
                {
                    Cabeza.Derecha.esObstaculo = true;
                } else if (direccion == new Vector2Int(1, 0))
                {
                    Cabeza.Izquierda.esObstaculo = true;
                }
                else if (direccion == new Vector2Int(0, -1))
                {
                    Cabeza.Arriba.esObstaculo = true;
                }
                else if (direccion == new Vector2Int(0, 1))
                {
                    Cabeza.Abajo.esObstaculo = true;
                }
                cantidadBombas--;
            }

            // Si el item es estela, aumenta el tamaño de la estela de la motocicleta en 1
            else if (item.nombre == "Estela")
            {
                tamagnoEstela++;
                cantidadEstelas--;
            }
        }
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
        public LinkedList<Item> items;

        public ColaItems()
        {
            items = new();
        }

        // Agrega un item a la cola
        public void AgregarItem(Item item)
        {

            items.AddLast(item);
            
        }

        // Saca un item de la cola
        public Item SacarItem()
        {
            if (items.Count > 0)
            {
                Item item = items.First.Value;
                items.RemoveFirst();
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



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
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;
using static UnityEditor.PlayerSettings;
using UnityEngine.Rendering.Universal;
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
        public Vector2Int posicion;

        // Atributos de la motocicleta
        public int tamagnoEstela = 3;
        public int velocidad = 5;
        public int combustible = 100;

        public bool esInmune = false;
        public bool estaVivo = true;
        public bool esJugador = true;
        public bool bombaExplotando = false;
        public bool activarCambioSprite = false;

        public int cantidadEscudos = 0;
        public int cantidadVelocidades = 0;
        public int cantidadCombustibles = 0;
        public int cantidadEstelas = 0;
        public int cantidadBombas = 0;

        public float TiempoDesdeUsoVelocidad = 0;
        public float TiempoLimiteVelocidad = 5f;
        public float TiempoDesdeUsoInmunidad = 0;
        public float TiempoDesdeUsoBomba = 0;
        public float TiempoDesdeExplosionNuclear = 0;

        public int PoderUtilizado = 1; // Toma valor de 1 o -1, donde 1 es la velocidad y -1 es la inmunidad

        public GameObject bomba;
        public GameObject explosion;
        public Nodo nodoBomba;

        // Generador de numeros aleatorios
        private Random rng = new Random();

        public ControladorAudio controladorAudio;

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
            controladorAudio = GameObject.Find("ManejadorAudio").GetComponent<ControladorAudio>();
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
                if (item.nombre == "Combustible")
                {
                    GenerarObjetoEnPosicionAleatoria("Combustible");
                    cantidadCombustibles++;
                }
                else if (item.nombre == "Estela")
                {
                    GenerarObjetoEnPosicionAleatoria("Estela");
                    cantidadEstelas++;
                }
                else if (item.nombre == "Bomba")
                {
                    GenerarObjetoEnPosicionAleatoria("Bomba");
                    cantidadBombas++;
                }
                GameObject.Destroy(Cabeza.ObjectoAsignado);
                controladorAudio.ReproducirSonido(controladorAudio.RecogerItemGenerico);

            }
            else if (Cabeza.poder != null)
            {
                Poder poder = Cabeza.poder;
                Cabeza.poder = null;
                poderes.AgregarPoder(poder);
                if (poder.nombre == "Velocidad")
                {
                    GenerarObjetoEnPosicionAleatoria("Velocidad");
                    cantidadVelocidades++;
                }
                else if (poder.nombre == "Escudo")
                {
                    GenerarObjetoEnPosicionAleatoria("Escudo");
                    cantidadEscudos++;
                }
                GameObject.Destroy(Cabeza.ObjectoAsignado);
                controladorAudio.ReproducirSonido(controladorAudio.RecogerPoderGenerico);

            }


        }

        public void GenerarObjetoEnPosicionAleatoria(string nombre)
        {
            Vector2Int coord = new Vector2Int(rng.Next(0, Espacio.largo), rng.Next(0, Espacio.ancho));
            Nodo nodo = Espacio.RedNodos[coord.x, coord.y];
            while (nodo.esObstaculo || nodo.item != null || nodo.poder != null)
            {
                coord = new Vector2Int(rng.Next(0, Espacio.largo), rng.Next(0, Espacio.ancho));
                nodo = Espacio.RedNodos[coord.x, coord.y];
            }

            GameObject gameObject = new GameObject();
            gameObject.AddComponent<SpriteRenderer>();
            // Crea el gameobject a partir del sprite
            if (nombre == "Escudo")
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = Texturas.instancia.Escudo;
                nodo.poder = new Poder
                {
                    nombre = "Escudo",
                };
            }
            else if (nombre == "Velocidad")
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = Texturas.instancia.Velocidad;
                nodo.poder = new Poder
                {
                    nombre = "Velocidad",
                };
            }
            else if (nombre == "Combustible")
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = Texturas.instancia.Combustible;
                nodo.item = new Item
                {
                    nombre = "Combustible",
                };
            }
            else if (nombre == "Estela")
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = Texturas.instancia.Estela;
                nodo.item = new Item
                {
                    nombre = "Estela",
                };
            }
            else if (nombre == "Bomba")
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = Texturas.instancia.Bomba;
                nodo.item = new Item
                {
                    nombre = "Bomba",
                };
            }
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = 10;
            gameObject.transform.localScale = Texturas.instancia.escala;
            gameObject.transform.position = Espacio.CoordenadaACentro(coord);
            nodo.ObjectoAsignado = gameObject;
        }


        public void ActualizarEstela(Vector2Int pos)
        {
            Vector3 posicionEspacio = Espacio.CentroACoordenada(pos);
            Nodo ultimaEstela = Espacio.RedNodos[(int)posicionEspacio.x, (int)posicionEspacio.y];
            ultimaEstela.id = new Vector2Int((int)pos.x, (int)pos.y);
            Estela.AddFirst(ultimaEstela);
            ultimaEstela.id = Estela.Last.Value.id;
            ultimaEstela.esCabeza = false;
            ultimaEstela.esObstaculo = true;
            ultimaEstela.ObjectoAsignado = new GameObject();
            ultimaEstela.ObjectoAsignado.AddComponent<SpriteRenderer>();
            ultimaEstela.ObjectoAsignado.GetComponent<SpriteRenderer>().sprite = Texturas.instancia.EstelaGenerica;
            ultimaEstela.ObjectoAsignado.transform.localScale = new Vector3(.3f, .3f, .3f);
            ultimaEstela.ObjectoAsignado.transform.position = new Vector3(pos.x, pos.y, 10);
            ultimaEstela.ObjectoAsignado.GetComponent<SpriteRenderer>().sortingOrder = 1;


            if (Estela.Count > tamagnoEstela)
            {
                ultimaEstela = Estela.Last.Value;
                ultimaEstela.esCabeza = false;
                ultimaEstela.esObstaculo = false;
                Estela.RemoveLast();
                GameObject.Destroy(ultimaEstela.ObjectoAsignado);
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

        public void BotarItemsYPoderesDeLaMotocicletaPorElMapaAlMorir()
        {
            // Bota los poderes
            foreach (Poder poder in poderes.poderes)
            {
                GenerarObjetoEnPosicionAleatoria(poder.nombre);
            }

            // Bota los items
            foreach (Item item in items.items)
            {
                GenerarObjetoEnPosicionAleatoria(item.nombre);
            }
        }

        public bool Mover(Vector2Int pos)
        {
            posicion = pos;
            Vector3 posicionEspacio = Espacio.CentroACoordenada(pos);
            Cabeza = Espacio.RedNodos[(int)posicionEspacio.x, (int)posicionEspacio.y];
            // Analiza si el jugador sigue vivo, mediante el combustible. Si es así, va a intercambiar
            // la cabeza de la motocicleta por el nodo adyacente en la dirección de movimiento.
            if (combustible > 0 && estaVivo)
            {
                if (TiempoDesdeUsoVelocidad >= TiempoLimiteVelocidad)
                {
                    activarCambioSprite = true;
                    velocidad = 5;
                    controladorAudio.PararSonidoEfectos();
                    TiempoDesdeUsoVelocidad = 0;
                }

                if (TiempoDesdeUsoInmunidad >= 5f)
                {
                    activarCambioSprite = true;
                    esInmune = false;
                    TiempoDesdeUsoInmunidad = 0;
                }

                if (TiempoDesdeUsoBomba >= 3f)
                {
                    GameObject.Destroy(bomba);
                    // Explota la bomba
                    if (bombaExplotando == false)
                    {
                        explosion = new();
                        explosion.AddComponent<SpriteRenderer>();
                        SpriteRenderer visualizacion = explosion.GetComponent<SpriteRenderer>();
                        visualizacion.sprite = Texturas.instancia.BombaNuclearTiradaPorElEstadoDeIsraelEnEspacioCivilPalestino;
                        explosion.transform.localScale = new Vector3(0.954f / 3, 1.465f / 3, 1f / 3);
                        visualizacion.sortingOrder = 1;
                        Vector3 tempCoord = Espacio.CoordenadaACentro(nodoBomba.id);
                        explosion.transform.position = tempCoord;

                        nodoBomba.esObstaculo = true;
                        nodoBomba.Abajo.esObstaculo = true;
                        nodoBomba.Abajo.Izquierda.esObstaculo = true;
                        nodoBomba.Abajo.Derecha.esObstaculo = true;
                        nodoBomba.Arriba.esObstaculo = true;
                        nodoBomba.Arriba.Derecha.esObstaculo = true;
                        nodoBomba.Arriba.Izquierda.esObstaculo = true;

                        controladorAudio.ReproducirSonido(controladorAudio.ExplosionBomba);

                    }
                    bombaExplotando = true;
                    if (TiempoDesdeExplosionNuclear >= 1f)
                    {
                        GameObject.Destroy(explosion);
                        bombaExplotando = false;
                        TiempoDesdeExplosionNuclear = 0f;
                        TiempoDesdeUsoBomba = 0;

                        nodoBomba.esObstaculo = false;
                        nodoBomba.Abajo.esObstaculo = false;
                        nodoBomba.Abajo.Izquierda.esObstaculo = false;
                        nodoBomba.Abajo.Derecha.esObstaculo = false;
                        nodoBomba.Arriba.esObstaculo = false;
                        nodoBomba.Arriba.Derecha.esObstaculo = false;
                        nodoBomba.Arriba.Izquierda.esObstaculo = false;
                    }

                }

                Vector3 posNueva = Espacio.CentroACoordenada(pos);

                int x = (int)posNueva.x;
                int y = (int)posNueva.y;

                Cabeza.esCabeza = false;

                Cabeza = Espacio.RedNodos[x, y];
                if (Cabeza.esObstaculo || Cabeza.esCabeza)
                {
                    // Va a generar una explosion en la cabeza y en cada posicion de la estela
                    // La posicion de la estela es estela.id, pero debe pasarse a centro de coordenada
                    if (esInmune)
                    {
                        esInmune = false;
                        TiempoDesdeUsoInmunidad = 0;
                        activarCambioSprite = true;
                    }
                    else
                    {
                        foreach (Nodo nodo in Estela)
                        {
                            GameObject.Destroy(nodo.ObjectoAsignado);
                        }
                        estaVivo = false;
                        if (esJugador)
                        {
                            controladorAudio.PararTodosLosSondos();
                        }
                        controladorAudio.ReproducirSonido(controladorAudio.ExplosionJugador);
                        BotarItemsYPoderesDeLaMotocicletaPorElMapaAlMorir();
                        return false;
                    }
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
                if (estaVivo)
                {
                    if (esJugador)
                    {
                        controladorAudio.PararTodosLosSondos();
                    }
                    BotarItemsYPoderesDeLaMotocicletaPorElMapaAlMorir();
                    controladorAudio.ReproducirSonido(controladorAudio.ExplosionJugador);
                }
                estaVivo = false;
                return false;
            }

            return true;
        }


        public void UtilizarPoder()
        {
            // Saca un poder de la pila de poderes
            Poder poder;

            if (PoderUtilizado == 1)
            {
                poder = poderes.SacarPoderEspecifico("Velocidad");
            }
            else
            {
                poder = poderes.SacarPoderEspecifico("Escudo");
            }

            if (poder == null)
            {
                return;
            }

            // Si el poder es inmunidad, la motocicleta se vuelve inmune
            if (poder.nombre == "Escudo")
            {
                esInmune = true;
                cantidadEscudos--;
                controladorAudio.ReproducirSonido(controladorAudio.UtilizarEscudo);
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
                TiempoLimiteVelocidad = (float)rng.Next(3, 7);
                cantidadVelocidades--;
                controladorAudio.ReproducirSonido(controladorAudio.UtilizarAumentoVelocidad);
                controladorAudio.ReproducirMusica(controladorAudio.SonidoFondoVelocidad);
            }
        }

        public void UtilizarItem(Vector2Int pos)
        {
            if (items.BuscarItem("Combustible") != null)
            {
                combustible += rng.Next(1, 100 - combustible);
                cantidadCombustibles--;
                controladorAudio.ReproducirSonido(controladorAudio.UtilizarGasolina);
                return;
            }

            Item item = items.SacarItem();

            if (item == null)
            {
                return;
            }

            // Si el item es bomba, pone un obstaculo en la dirección contraria a la que se mueve la motocicleta
            else if (item.nombre == "Bomba")
            {
                if (cantidadBombas == 0 || bomba != null)
                {
                    return;
                }
                Vector3 posDesCentrada = Espacio.CentroACoordenada(pos);
                pos = new((int)posDesCentrada.x, (int)posDesCentrada.y);
                Vector2Int nuevaPos = new(pos.x - direccion.x, pos.y - direccion.y);
                nodoBomba = Espacio.ObtenerNodo(nuevaPos);
                nodoBomba.id = nuevaPos;
                bomba = new GameObject();
                bomba.AddComponent<SpriteRenderer>();
                bomba.GetComponent<SpriteRenderer>().sprite = Texturas.instancia.BombaRoja;
                bomba.transform.localScale = new Vector3(.3f, .3f, .3f);
                bomba.transform.position = Espacio.CoordenadaACentro(nuevaPos);
                bomba.GetComponent<SpriteRenderer>().sortingOrder = 1;
                cantidadBombas--;
                controladorAudio.ReproducirSonido(controladorAudio.UtilizarBomba);
            }

            // Si el item es estela, aumenta el tamaño de la estela de la motocicleta en 1
            else if (item.nombre == "Estela")
            {
                tamagnoEstela++;
                cantidadEstelas--;
                controladorAudio.ReproducirSonido(controladorAudio.UtilizarAumentoEstela);
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

        public Poder SacarPoderEspecifico(string nombre)
        {
            foreach (Poder poder in poderes)
            {
                if (poder.nombre == nombre)
                {
                    poderes.Remove(poder);
                    return poder;
                }
            }
            return null;
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


        public Item BuscarItem(string nombre)
        {
            foreach (Item item in items)
            {
                if (item.nombre == nombre)
                {
                    items.Remove(item);
                    return item;
                }
            }
            return null;
        }

        // Agrega un item a la cola
        public void AgregarItem(Item item)
        {

            items.AddLast(item);

        }

        public void EliminarItemEspecifico(string nombre)
        {
            foreach (Item item in items)
            {
                if (item.nombre == nombre)
                {
                    items.Remove(item);
                    return;
                }
            }
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



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
using JetBrains.Annotations;
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

        public GameObject BotRojo;
        public GameObject BotAzul;
        public GameObject BotAmarillo;
        public GameObject BotRosado;

        public Nodo BotRojoCabeza;
        public Nodo BotAzulCabeza;
        public Nodo BotAmarilloCabeza;
        public Nodo BotRosadoCabeza;

        public LinkedList<Nodo> EstelaBotRojo = new();
        public LinkedList<Nodo> EstelaBotAzul = new();
        public LinkedList<Nodo> EstelaBotAmarillo = new();
        public LinkedList<Nodo> EstelaBotRosado = new();

        public int TamagnoEstelaBotRojo = 3;
        public int TamagnoEstelaBotAzul = 3;
        public int TamagnoEstelaBotAmarillo = 3;
        public int TamagnoEstelaBotRosado = 3;

        public bool estaVivoRojo = true;
        public bool estaVivoAzul = true;
        public bool estaVivoAmarillo = true;
        public bool estaVivoRosado = true;

        public Vector2Int PosicionBotRojo, DireccionBotRojo;
        public Vector2Int PosicionBotAzul, DireccionBotAzul;
        public Vector2Int PosicionBotAmarillo, DireccionBotAmarillo;
        public Vector2Int PosicionBotRosado, DireccionBotRosado;

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
            CrearBots();

        }

        public void CrearBots()
        {
            /*
             * Los bots se comportan como gusanos, se crean en las esquinas de la matriz y se mueven en una direccion aleatoria
             * Estos pueden agrandar su estela de manera aleatoria y van a tratar de esquivar los obstaculos y la estela de la motocicleta
             * Esta funcion agarra los nodos en las esquinas y les asigna una direccion aleatoria
             * Esto dentro del espacio de nodos
             * El bot rojo está en (0,0), el azul en 0,ancho-1, el amarillo en largo-1,0 y el rosado en largo-1,ancho-1
             */

            // Instancia los bots, estos simplemente se moveran y esquivaran obstaculos
            BotRojo = new GameObject();
            BotRojo.AddComponent<SpriteRenderer>().sprite = Texturas.instancia.BotRojo;
            BotRojo.GetComponent<SpriteRenderer>().sortingOrder = 10;

            BotAzul = new GameObject();
            BotAzul.AddComponent<SpriteRenderer>().sprite = Texturas.instancia.BotAzul;
            BotAzul.GetComponent<SpriteRenderer>().sortingOrder = 10;

            BotAmarillo = new GameObject();
            BotAmarillo.AddComponent<SpriteRenderer>().sprite = Texturas.instancia.BotAmarillo;
            BotAmarillo.GetComponent<SpriteRenderer>().sortingOrder = 10;

            BotRosado = new GameObject();
            BotRosado.AddComponent<SpriteRenderer>().sprite = Texturas.instancia.BotRosado;
            BotRosado.GetComponent<SpriteRenderer>().sortingOrder = 10;

            BotRojo.transform.localScale = Texturas.instancia.escala;
            BotAzul.transform.localScale = Texturas.instancia.escala;
            BotAmarillo.transform.localScale = Texturas.instancia.escala;
            BotRosado.transform.localScale = Texturas.instancia.escala;

            // Los bots se posicionan en las esquinas de la matriz
            PosicionBotRojo = new Vector2Int(5, 2);
            PosicionBotAzul = new Vector2Int(5, Espacio.ancho - 3);
            PosicionBotAmarillo = new Vector2Int(Espacio.largo - 6, 2);
            PosicionBotRosado = new Vector2Int(Espacio.largo - 6, Espacio.ancho - 3);

            DireccionBotRojo = new Vector2Int(0, 1);
            DireccionBotAzul = new Vector2Int(1, 0);
            DireccionBotAmarillo = new Vector2Int(0, -1);
            DireccionBotRosado = new Vector2Int(-1, 0);

            BotRojo.transform.position = Espacio.CoordenadaACentro(PosicionBotRojo);
            BotAzul.transform.position = Espacio.CoordenadaACentro(PosicionBotAzul);
            BotAmarillo.transform.position = Espacio.CoordenadaACentro(PosicionBotAmarillo);
            BotRosado.transform.position = Espacio.CoordenadaACentro(PosicionBotRosado);

            BotRojo.transform.eulerAngles = new Vector3(0, 0, ObtenerAnguloAPartirDeVectorDireccion(DireccionBotRojo) - 90);
            BotAzul.transform.eulerAngles = new Vector3(0, 0, ObtenerAnguloAPartirDeVectorDireccion(DireccionBotAzul) - 90);
            BotAmarillo.transform.eulerAngles = new Vector3(0, 0, ObtenerAnguloAPartirDeVectorDireccion(DireccionBotAmarillo) - 90);
            BotRosado.transform.eulerAngles = new Vector3(0, 0, ObtenerAnguloAPartirDeVectorDireccion(DireccionBotRosado) - 90);

            BotRojoCabeza = Espacio.RedNodos[5, 5];
            BotAmarilloCabeza = Espacio.RedNodos[Espacio.largo - 6, 5];
            BotAzulCabeza = Espacio.RedNodos[5, Espacio.ancho - 6];
            BotRosadoCabeza = Espacio.RedNodos[Espacio.largo - 6, Espacio.ancho - 6];
        }
        private float ObtenerAnguloAPartirDeVectorDireccion(Vector2Int dir)
        {
            float anguloObtenidoAPartirDeCalculo = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (anguloObtenidoAPartirDeCalculo < 0)
            {
                anguloObtenidoAPartirDeCalculo += 360;
            }
            return anguloObtenidoAPartirDeCalculo;
        }

        public void MoverBots()
        {
            if (estaVivoRojo)
            {
                // Hace un movimiento aleatorio
                if (rng.Next(0, 7) == 0)
                {
                    if (DireccionBotRojo.x != 0)
                    {
                        DireccionBotRojo = new Vector2Int(0, rng.Next(0, 1) == 0 ? 1 : -1);
                    }
                    else
                    {
                        DireccionBotRojo = new Vector2Int(rng.Next(0, 1) == 0 ? 1 : -1, 0);
                    }
                }

                DireccionBotRojo = Esquivar(DireccionBotRojo, BotRojoCabeza);
                PosicionBotRojo += DireccionBotRojo;
                PosicionBotRojo = SimularEspacioToroidalPara(PosicionBotRojo);
                BotRojo.transform.position = Espacio.CoordenadaACentro(PosicionBotRojo);
                BotRojo.transform.eulerAngles = new Vector3(0, 0, ObtenerAnguloAPartirDeVectorDireccion(DireccionBotRojo) - 90);
                ActualizarEstelaParaBot("Rojo");
                BotRojoCabeza.esCabeza = false;
                BotRojoCabeza = Espacio.ObtenerNodo(PosicionBotRojo);
                if (BotRojoCabeza.esObstaculo || BotRojoCabeza.esCabeza)
                {
                    MatarBot("Rojo");
                    return;
                }
                BotRojoCabeza.esCabeza = true;
                Espacio.RedNodos[PosicionBotRojo.x, PosicionBotRojo.y] = BotRojoCabeza;
                // Actualiza los transforms de todos los bots
            }
            if (estaVivoAzul)
            {
                if (rng.Next(0, 4) == 0)
                {
                    if (DireccionBotAzul.x != 0)
                    {
                        DireccionBotAzul = new Vector2Int(0, rng.Next(0, 1) == 0 ? 1 : -1);
                    }
                    else
                    {
                        DireccionBotAzul = new Vector2Int(rng.Next(0, 1) == 0 ? 1 : -1, 0);
                    }
                }


                DireccionBotAzul = Esquivar(DireccionBotAzul, BotAzulCabeza);
                PosicionBotAzul += DireccionBotAzul;
                PosicionBotAzul = SimularEspacioToroidalPara(PosicionBotAzul);
                BotAzul.transform.position = Espacio.CoordenadaACentro(PosicionBotAzul);
                BotAzul.transform.eulerAngles = new Vector3(0, 0, ObtenerAnguloAPartirDeVectorDireccion(DireccionBotAzul) - 90);
                ActualizarEstelaParaBot("Azul");
                BotAzulCabeza.esCabeza = false;
                BotAzulCabeza = Espacio.ObtenerNodo(PosicionBotAzul);
                if (BotAzulCabeza.esObstaculo || BotAzulCabeza.esCabeza)
                {
                    MatarBot("Azul");
                    return;
                }
                Espacio.RedNodos[PosicionBotAzul.x, PosicionBotAzul.y] = BotAzulCabeza;
                BotAzulCabeza.esCabeza = true;
            }
            if (estaVivoAmarillo)
            {

                if (rng.Next(0, 10) == 0)
                {
                    if (DireccionBotAmarillo.x != 0)
                    {
                        DireccionBotAmarillo = new Vector2Int(0, rng.Next(0, 1) == 0 ? 1 : -1);
                    }
                    else
                    {
                        DireccionBotAmarillo = new Vector2Int(rng.Next(0, 1) == 0 ? 1 : -1, 0);
                    }
                }
                DireccionBotAmarillo = Esquivar(DireccionBotAmarillo, BotAmarilloCabeza);

                PosicionBotAmarillo += DireccionBotAmarillo;
                PosicionBotAmarillo = SimularEspacioToroidalPara(PosicionBotAmarillo);
                BotAmarillo.transform.position = Espacio.CoordenadaACentro(PosicionBotAmarillo);
                BotAmarillo.transform.eulerAngles = new Vector3(0, 0, ObtenerAnguloAPartirDeVectorDireccion(DireccionBotAmarillo) - 90);
                ActualizarEstelaParaBot("Amarillo");
                BotAmarilloCabeza.esCabeza = false;
                BotAmarilloCabeza = Espacio.ObtenerNodo(PosicionBotAmarillo);
                if (BotAmarilloCabeza.esObstaculo || BotAmarilloCabeza.esCabeza)
                {
                    MatarBot("Amarillo");
                    return;
                }
                Espacio.RedNodos[PosicionBotAmarillo.x, PosicionBotAmarillo.y] = BotAmarilloCabeza;
                BotAmarilloCabeza.esCabeza = true;
            }
            if (estaVivoRosado)
            {

                if (rng.Next(0, 11) == 0)
                {
                    if (DireccionBotRosado.x != 0)
                    {
                        DireccionBotRosado = new Vector2Int(0, rng.Next(0, 1) == 0 ? 1 : -1);
                    }
                    else
                    {
                        DireccionBotRosado = new Vector2Int(rng.Next(0, 1) == 0 ? 1 : -1, 0);
                    }
                }
                DireccionBotRosado = Esquivar(DireccionBotRosado, BotRosadoCabeza);

                PosicionBotRosado += DireccionBotRosado;
                PosicionBotRosado = SimularEspacioToroidalPara(PosicionBotRosado);
                BotRosado.transform.position = Espacio.CoordenadaACentro(PosicionBotRosado);
                BotRosado.transform.eulerAngles = new Vector3(0, 0, ObtenerAnguloAPartirDeVectorDireccion(DireccionBotRosado) - 90);
                ActualizarEstelaParaBot("Rosado");
                BotRosadoCabeza.esCabeza = false;
                BotRosadoCabeza = Espacio.ObtenerNodo(PosicionBotRosado);
                if (BotRosadoCabeza.esObstaculo || BotRosadoCabeza.esCabeza)
                {
                    MatarBot("Rosado");
                    return;
                }
                Espacio.RedNodos[PosicionBotRosado.x, PosicionBotRosado.y] = BotRosadoCabeza;
                BotRosadoCabeza.esCabeza = true;
            }
        }

        public void MatarBot(string nombre)
        {
            controladorAudio.ReproducirSonido(controladorAudio.ExplosionJugador);
            if (nombre == "Rojo")
            {
                BotRojoCabeza.esCabeza = false;
                estaVivoRojo = false;
                Espacio.RedNodos[PosicionBotRojo.x, PosicionBotRojo.y] = BotRojoCabeza;
                GameObject.Destroy(BotRojo);
                foreach (Nodo nodo in EstelaBotRojo)
                {
                    GameObject.Destroy(nodo.ObjectoAsignado);
                    nodo.esCabeza = false;
                    nodo.esObstaculo = false;
                    Espacio.RedNodos[nodo.id.x, nodo.id.y] = nodo;
                }
                ParticleSystem explosion = Texturas.instancia.Explosion;
                Vector3 pos = Espacio.CoordenadaACentro(PosicionBotRojo);
                explosion.transform.position = new Vector3(pos.x, pos.y, explosion.transform.position.z);
                explosion.Play();
            }
            else if (nombre == "Azul")
            {
                BotAzulCabeza.esCabeza = false;
                estaVivoAzul = false;
                Espacio.RedNodos[PosicionBotAzul.x, PosicionBotAzul.y] = BotAzulCabeza;
                GameObject.Destroy(BotAzul);
                foreach (Nodo nodo in EstelaBotAzul)
                {
                    GameObject.Destroy(nodo.ObjectoAsignado);
                    nodo.esCabeza = false;
                    nodo.esObstaculo = false;
                    Espacio.RedNodos[nodo.id.x, nodo.id.y] = nodo;
                }
                ParticleSystem explosion = Texturas.instancia.Explosion;
                Vector3 pos = Espacio.CoordenadaACentro(PosicionBotAzul);
                explosion.transform.position = new Vector3(pos.x, pos.y, explosion.transform.position.z);
                explosion.Play();
            }
            else if (nombre == "Amarillo")
            {
                BotAmarilloCabeza.esCabeza = false;
                estaVivoAmarillo = false;
                Espacio.RedNodos[PosicionBotAmarillo.x, PosicionBotAmarillo.y] = BotAmarilloCabeza;
                GameObject.Destroy(BotAmarillo);
                foreach (Nodo nodo in EstelaBotAmarillo)
                {
                    GameObject.Destroy(nodo.ObjectoAsignado);
                    nodo.esCabeza = false;
                    nodo.esObstaculo = false;
                    Espacio.RedNodos[nodo.id.x, nodo.id.y] = nodo;
                }
                ParticleSystem explosion = Texturas.instancia.Explosion;
                Vector3 pos = Espacio.CoordenadaACentro(PosicionBotAmarillo);
                explosion.transform.position = new Vector3(pos.x, pos.y, explosion.transform.position.z);
                explosion.Play();
            }
            else
            {
                BotRosadoCabeza.esCabeza = false;
                estaVivoRosado = false;
                Espacio.RedNodos[PosicionBotRosado.x, PosicionBotRosado.y] = BotRosadoCabeza;
                GameObject.Destroy(BotRosado);
                foreach (Nodo nodo in EstelaBotRosado)
                {
                    GameObject.Destroy(nodo.ObjectoAsignado);
                    nodo.esCabeza = false;
                    nodo.esObstaculo = false;
                    Espacio.RedNodos[nodo.id.x, nodo.id.y] = nodo;
                }
                ParticleSystem explosion = Texturas.instancia.Explosion;
                Vector3 pos = Espacio.CoordenadaACentro(PosicionBotRosado);
                explosion.transform.position = new Vector3(pos.x, pos.y, explosion.transform.position.z);
                explosion.Play();
            }
        }

        public void ActualizarEstelaParaBot(string nombreBot)
        {
            Vector2Int pos;
            int tamagno;
            LinkedList<Nodo> estela;
            bool autorizar;
            if (nombreBot == null)
            {
                return;
            }
            else if (nombreBot == "Rojo")
            {
                pos = PosicionBotRojo - DireccionBotRojo;
                tamagno = TamagnoEstelaBotRojo;
                estela = EstelaBotRojo;
                autorizar = estaVivoRojo;
            }
            else if (nombreBot == "Azul")
            {
                pos = PosicionBotAzul - DireccionBotAzul;
                tamagno = TamagnoEstelaBotAzul;
                estela = EstelaBotAzul;
                autorizar = estaVivoAzul;
            }
            else if (nombreBot == "Amarillo")
            {
                pos = PosicionBotAmarillo - DireccionBotAmarillo;
                tamagno = TamagnoEstelaBotAmarillo;
                estela = EstelaBotAmarillo;
                autorizar = estaVivoAmarillo;
            }
            else
            {
                pos = PosicionBotRosado - DireccionBotRosado;
                tamagno = TamagnoEstelaBotRosado;
                estela = EstelaBotRosado;
                autorizar = estaVivoRosado;
            }

            pos = SimularEspacioToroidalPara(pos);

            Nodo ultimaEstela = Espacio.RedNodos[(int)pos.x, (int)pos.y];
            ultimaEstela.id = new Vector2Int((int)pos.x, (int)pos.y);
            estela.AddFirst(ultimaEstela);
            ultimaEstela.esCabeza = false;
            ultimaEstela.esObstaculo = true;
            ultimaEstela.ObjectoAsignado = new GameObject();
            ultimaEstela.ObjectoAsignado.AddComponent<SpriteRenderer>();
            ultimaEstela.ObjectoAsignado.GetComponent<SpriteRenderer>().sprite = Texturas.instancia.EstelaGenerica;
            ultimaEstela.ObjectoAsignado.transform.localScale = new Vector3(.3f, .3f, .3f);
            Espacio.RedNodos[(int)pos.x, (int)pos.y] = ultimaEstela;
            Vector3 PosicionArreglada = Espacio.CoordenadaACentro(pos);

            ultimaEstela.ObjectoAsignado.transform.position = new Vector3(PosicionArreglada.x, PosicionArreglada.y, 1);
            ultimaEstela.ObjectoAsignado.GetComponent<SpriteRenderer>().sortingOrder = 1;


            if (estela.Count > tamagno && autorizar)
            {
                ultimaEstela = estela.Last.Value;
                ultimaEstela.esCabeza = false;
                ultimaEstela.esObstaculo = false;
                Espacio.RedNodos[ultimaEstela.id.x, ultimaEstela.id.y] = ultimaEstela;
                estela.RemoveLast();
                GameObject.Destroy(ultimaEstela.ObjectoAsignado);
            }

            // Actualiza la estela
            if (nombreBot == "Rojo")
            {
                EstelaBotRojo = estela;
            }
            else if (nombreBot == "Azul")
            {
                EstelaBotAzul = estela;
            }
            else if (nombreBot == "Amarillo")
            {
                EstelaBotAmarillo = estela;
            }
            else
            {
                EstelaBotRosado = estela;
            }

        }

        public Vector2Int SimularEspacioToroidalPara(Vector2Int posicion)
        {
            Vector2Int NuevaPosicion = posicion;

            // Si la posicion se sale del espacio, la regresa al otro lado
            if (posicion.x < 0)
            {
                NuevaPosicion.x = Espacio.largo - 1;
            }
            else if (posicion.x >= Espacio.largo)
            {
                NuevaPosicion.x = 0;
            }
            else if (posicion.y < 0)
            {
                NuevaPosicion.y = Espacio.ancho - 1;
            }
            else if (posicion.y >= Espacio.ancho)
            {
                NuevaPosicion.y = 0;
            }
            return NuevaPosicion;
        }

        public void AsignarCabeza(int x, int y)
        {
            Cabeza = Espacio.RedNodos[x, y];
            Cabeza.esCabeza = true;
        }

        public void ComprobarCabeza(Nodo cabeza = null)
        {
            if (cabeza == null)
            {
                cabeza = Cabeza;
            }

            if (cabeza.item != null)
            {
                GameObject.Destroy(cabeza.ObjectoAsignado);
                Item item = cabeza.item;
                cabeza.item = null;
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
                controladorAudio.ReproducirSonido(controladorAudio.RecogerItemGenerico);

            }
            else if (cabeza.poder != null)
            {
                GameObject.Destroy(cabeza.ObjectoAsignado);
                Poder poder = cabeza.poder;
                cabeza.poder = null;
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

        public Vector2Int Esquivar(Vector2Int DireccionInicial, Nodo CabezaBot)
        {
            Nodo siguienteNodo = CabezaBot.ObtenerNodoAdyacente(DireccionInicial).ObtenerNodoAdyacente(DireccionInicial);
            if (siguienteNodo.esObstaculo || siguienteNodo.esCabeza)
            {
                Vector2Int NuevaDireccion;
                if (DireccionInicial == new Vector2Int(-1, 0) || DireccionInicial == new Vector2Int(1, 0))
                {
                    int Decision = rng.Next(0, 2);
                    if (Decision == 0 && DireccionInicial.y != -1)
                    {
                        NuevaDireccion = new Vector2Int(0, 1);
                    }
                    else
                    {
                        NuevaDireccion = new Vector2Int(0, -1);
                    }
                }

                // Si va hacia arriba o hacia abajo, va a elegir aleatoriamente si ir hacia la izquierda o a la derecha
                else
                {
                    int Decision = rng.Next(0, 2);
                    if (Decision == 0 && DireccionInicial.x != -1)
                    {
                        NuevaDireccion = new Vector2Int(1, 0);
                    }
                    else
                    {
                        NuevaDireccion = new Vector2Int(-1, 0);
                    }
                }

                return NuevaDireccion;
            }
            else
            {
                return DireccionInicial;
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
                                
                // Comprueba si la nueva cabeza de la motocicleta es un obstaculo
                // Si es un obstaculo, la motocicleta muere


             


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
                foreach (Nodo estelita in Estela)
                {
                    GameObject.Destroy(estelita.ObjectoAsignado);
                }
                estaVivo = false;
                return false;
            }

            MoverBots();

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



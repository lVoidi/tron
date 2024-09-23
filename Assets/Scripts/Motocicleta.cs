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

        public int velocidadBotRojo = 5;
        public int velocidadBotAzul = 5;
        public int velocidadBotAmarillo = 5;
        public int velocidadBotRosado = 5;

        public int combustibleBotRojo = 100;
        public int combustibleBotAzul = 100;
        public int combustibleBotAmarillo = 100;
        public int combustibleBotRosado = 100;

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
        /*
         * Esta funcion obtiene el angulo de giro a partir de un vector de direccion
         * :param dir: Vector2Int que representa la direccion
         * :return: float que representa el angulo de giro
         */
        {
            float anguloObtenidoAPartirDeCalculo = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (anguloObtenidoAPartirDeCalculo < 0)
            {
                anguloObtenidoAPartirDeCalculo += 360;
            }
            return anguloObtenidoAPartirDeCalculo;
        }

        public void MoverBots(bool rojo, bool azul, bool amarillo, bool rosado)
        /*
         * Mueve los bots en el espacio de nodos
         * :param rojo: booleano que representa si el bot rojo se mueve
         * :param azul: booleano que representa si el bot azul se mueve
         * :param amarillo: booleano que representa si el bot amarillo se mueve
         * :param rosado: booleano que representa si el bot rosado se mueve
         */
        {
            if (estaVivoRojo && rojo)
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

                // Rng que aumenta la estela de vez en cuando
                if (rng.Next(0, 25) == 0)
                {
                    TamagnoEstelaBotRojo++;
                    controladorAudio.ReproducirSonido(controladorAudio.UtilizarAumentoEstela);
                }

                // Cambia la velocidad de manera aleatoria
                if (rng.Next(0, 25) == 0)
                {
                    velocidadBotRojo = rng.Next(2, 11);
                    controladorAudio.ReproducirSonido(controladorAudio.UtilizarAumentoVelocidad);
                }

                // Cambia el combustible de manera aleatoria
                if (rng.Next(0, 15) == 0)
                {
                    combustibleBotRojo = rng.Next(100 - combustibleBotRojo, 100);
                    controladorAudio.ReproducirSonido(controladorAudio.UtilizarGasolina);
                }

                // Esquiva obstaculos
                DireccionBotRojo = Esquivar(DireccionBotRojo, BotRojoCabeza);
                PosicionBotRojo += DireccionBotRojo;
                PosicionBotRojo = SimularEspacioToroidalPara(PosicionBotRojo);
                
                // Cambia el transform dependiendo de la direccion, ajusta el angulo de giro
                BotRojo.transform.position = Espacio.CoordenadaACentro(PosicionBotRojo);
                BotRojo.transform.eulerAngles = new Vector3(0, 0, ObtenerAnguloAPartirDeVectorDireccion(DireccionBotRojo) - 90);

                // Actualiza la estela
                ActualizarEstelaParaBot("Rojo");

                // Obtiene el nodo de la cabeza y lo actualiza
                BotRojoCabeza.esCabeza = false;
                BotRojoCabeza = Espacio.ObtenerNodo(PosicionBotRojo);

                // Comprueba si la cabeza es un obstaculo o si se choca con la estela
                ComprobarCabeza(BotRojoCabeza, true);

                // Mata al bot si se choca con un obstaculo, con la estela o si se queda sin combustible
                if (BotRojoCabeza.esObstaculo || BotRojoCabeza.esCabeza || combustibleBotRojo == 0)
                {
                    MatarBot("Rojo");
                    return;
                }

                // Actualiza el espacio de nodos
                BotRojoCabeza.esCabeza = true;
                Espacio.RedNodos[PosicionBotRojo.x, PosicionBotRojo.y] = BotRojoCabeza;

                // Disminuye el combustible
                combustibleBotRojo -= 1;
            }
            if (estaVivoAzul && azul)
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

                if (rng.Next(0, 25) == 0)
                {
                    TamagnoEstelaBotAzul++;
                    controladorAudio.ReproducirSonido(controladorAudio.UtilizarAumentoEstela);
                }

                if (rng.Next(0, 22) == 0)
                {
                    velocidadBotAzul = rng.Next(5, 11);
                    controladorAudio.ReproducirSonido(controladorAudio.UtilizarAumentoVelocidad);
                }

                if (rng.Next(0, 15) == 0)
                {
                    combustibleBotAzul = rng.Next(100 - combustibleBotAzul, 100);
                    controladorAudio.ReproducirSonido(controladorAudio.UtilizarGasolina);
                }

                DireccionBotAzul = Esquivar(DireccionBotAzul, BotAzulCabeza);
                PosicionBotAzul += DireccionBotAzul;
                PosicionBotAzul = SimularEspacioToroidalPara(PosicionBotAzul);
                BotAzul.transform.position = Espacio.CoordenadaACentro(PosicionBotAzul);
                BotAzul.transform.eulerAngles = new Vector3(0, 0, ObtenerAnguloAPartirDeVectorDireccion(DireccionBotAzul) - 90);
                ActualizarEstelaParaBot("Azul");
                BotAzulCabeza.esCabeza = false;
                BotAzulCabeza = Espacio.ObtenerNodo(PosicionBotAzul);
                ComprobarCabeza(BotAzulCabeza, true);
                if (BotAzulCabeza.esObstaculo || BotAzulCabeza.esCabeza || combustibleBotAzul == 0)
                {
                    MatarBot("Azul");
                    return;
                }
                Espacio.RedNodos[PosicionBotAzul.x, PosicionBotAzul.y] = BotAzulCabeza;
                BotAzulCabeza.esCabeza = true;
                combustibleBotAzul -= 1;
            }
            if (estaVivoAmarillo && amarillo)
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

                if (rng.Next(0, 25) == 0)
                {
                    TamagnoEstelaBotAmarillo++;
                    controladorAudio.ReproducirSonido(controladorAudio.UtilizarAumentoEstela);
                }

                if (rng.Next(0, 22) == 0)
                {
                    velocidadBotAmarillo = rng.Next(5, 11);
                    controladorAudio.ReproducirSonido(controladorAudio.UtilizarAumentoVelocidad);
                }
                if (rng.Next(0, 15) == 0)
                {
                    combustibleBotAmarillo = rng.Next(100 - combustibleBotAmarillo, 100);
                    controladorAudio.ReproducirSonido(controladorAudio.UtilizarGasolina);
                }

                DireccionBotAmarillo = Esquivar(DireccionBotAmarillo, BotAmarilloCabeza);

                PosicionBotAmarillo += DireccionBotAmarillo;
                PosicionBotAmarillo = SimularEspacioToroidalPara(PosicionBotAmarillo);
                BotAmarillo.transform.position = Espacio.CoordenadaACentro(PosicionBotAmarillo);
                BotAmarillo.transform.eulerAngles = new Vector3(0, 0, ObtenerAnguloAPartirDeVectorDireccion(DireccionBotAmarillo) - 90);
                ActualizarEstelaParaBot("Amarillo");
                BotAmarilloCabeza.esCabeza = false;
                BotAmarilloCabeza = Espacio.ObtenerNodo(PosicionBotAmarillo);
                ComprobarCabeza(BotAmarilloCabeza, true);
                if (BotAmarilloCabeza.esObstaculo || BotAmarilloCabeza.esCabeza || combustibleBotAmarillo == 0)
                {
                    MatarBot("Amarillo");
                    return;
                }
                Espacio.RedNodos[PosicionBotAmarillo.x, PosicionBotAmarillo.y] = BotAmarilloCabeza;
                BotAmarilloCabeza.esCabeza = true;
                combustibleBotAmarillo -= 1;
            }
            if (estaVivoRosado && rosado)
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

                if (rng.Next(0, 25) == 0)
                {
                    TamagnoEstelaBotRosado++;
                    controladorAudio.ReproducirSonido(controladorAudio.UtilizarAumentoEstela);
                }

                if (rng.Next(0, 25) == 0)
                {
                    velocidadBotRosado = rng.Next(5, 11);
                    controladorAudio.ReproducirSonido(controladorAudio.UtilizarAumentoVelocidad);
                }

                if (rng.Next(0, 15) == 0)
                {
                    combustibleBotRosado = rng.Next(100 - combustibleBotRosado, 100);
                    controladorAudio.ReproducirSonido(controladorAudio.UtilizarGasolina);
                }

                DireccionBotRosado = Esquivar(DireccionBotRosado, BotRosadoCabeza);

                PosicionBotRosado += DireccionBotRosado;
                PosicionBotRosado = SimularEspacioToroidalPara(PosicionBotRosado);
                BotRosado.transform.position = Espacio.CoordenadaACentro(PosicionBotRosado);
                BotRosado.transform.eulerAngles = new Vector3(0, 0, ObtenerAnguloAPartirDeVectorDireccion(DireccionBotRosado) - 90);
                ActualizarEstelaParaBot("Rosado");
                BotRosadoCabeza.esCabeza = false;
                BotRosadoCabeza = Espacio.ObtenerNodo(PosicionBotRosado);
                ComprobarCabeza(BotRosadoCabeza, true);
                if (BotRosadoCabeza.esObstaculo || BotRosadoCabeza.esCabeza || combustibleBotRosado == 0)
                {
                    MatarBot("Rosado");
                    return;
                }
                Espacio.RedNodos[PosicionBotRosado.x, PosicionBotRosado.y] = BotRosadoCabeza;
                BotRosadoCabeza.esCabeza = true;
                combustibleBotRosado -= 1;
            }
        }

        public void MatarBot(string nombre)
        /*
         * Mata al bot y actualiza la estela
         * :param nombre: string que representa el nombre del bot a matar.
         */
        {
            controladorAudio.ReproducirSonido(controladorAudio.ExplosionJugador);
            // Comprueba qué bot se murió exactamente
            if (nombre == "Rojo")
            {
                // Actualiza el nodo en el espacio
                BotRojoCabeza.esCabeza = false;
                estaVivoRojo = false;
                Espacio.RedNodos[PosicionBotRojo.x, PosicionBotRojo.y] = BotRojoCabeza;

                // Destruye todos los objetos asignados
                GameObject.Destroy(BotRojo);
                foreach (Nodo nodo in EstelaBotRojo)
                {
                    GameObject.Destroy(nodo.ObjectoAsignado);
                    nodo.esCabeza = false;
                    nodo.esObstaculo = false;
                    Espacio.RedNodos[nodo.id.x, nodo.id.y] = nodo;
                }
                // Crea una explosión en pos
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
        /*
         * Actualiza la estela de la motocicleta.
         * :param nombreBot: string que representa el nombre del bot a actualizar.
         */
        {
            Vector2Int pos;
            int tamagno;
            LinkedList<Nodo> estela;
            bool autorizar;

            // Ajusta las variables dependiendo de qué bot es, según nombreBot
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

            // Ajusta la posicion y la agrega a la estela
            pos = SimularEspacioToroidalPara(pos);

            Nodo ultimaEstela = Espacio.RedNodos[(int)pos.x, (int)pos.y];
            ultimaEstela.id = new Vector2Int((int)pos.x, (int)pos.y);
            estela.AddFirst(ultimaEstela);
            ultimaEstela.esCabeza = false;
            ultimaEstela.esObstaculo = true;
            ultimaEstela.ObjectoAsignado = new GameObject();
            ultimaEstela.ObjectoAsignado.name = $"{pos.x},{pos.y}";
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

                GameObject gameObjectAtPosition = GameObject.Find($"{ultimaEstela.id.x},{ultimaEstela.id.y}");
                if (gameObjectAtPosition != null)
                {
                    GameObject.Destroy(gameObjectAtPosition);
                }

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
        /*
         * Toma una posicion y la regresa al otro lado del espacio si se sale de este.
         * :param posicion: Vector2Int que representa la posicion a simular.
         * :return: Vector2Int que representa la posicion simulada.
         */
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
        /*
         * Asigna la cabeza de la motocicleta en la posición (x, y) del espacio.
         * :param x: int que representa la coordenada x de la cabeza.
         * :param y: int que representa la coordenada y de la cabeza.
         */
        {
            Cabeza = Espacio.RedNodos[x, y];
            Cabeza.esCabeza = true;
        }

        public void ComprobarCabeza(Nodo cabeza = null, bool esBot = false)
        /*
         * Toma una cabeza y comprueba si en esa posición hay un item o un poder.
         * :param cabeza: Nodo que representa la cabeza de la motocicleta.
         * :param esBot: bool que dice si la cabeza es de un bot o no.
         */
        {
            // Si no se especifica cabeza, se pone como Cabeza de default (osea, la cabeza del jugador)
            if (cabeza == null)
            {
                cabeza = Cabeza;
            }

            // Si la cabeza tiene un item, lo recoge. Si es un bot, simplemente lo mueve de posicion, ya que 
            // los bots no pueden recoger items y se comportan de manera aleatoria.
            if (cabeza.item != null)
            {
                GameObject.Destroy(cabeza.ObjectoAsignado);
                Item item = cabeza.item;
                cabeza.item = null;
                if (!esBot)
                {
                    items.AgregarItem(item);
                }
                if (item.nombre == "Combustible")
                {
                    GenerarObjetoEnPosicionAleatoria("Combustible");
                    if (!esBot)
                    {
                        cantidadCombustibles++;
                    }
                }
                else if (item.nombre == "Estela")
                {
                    GenerarObjetoEnPosicionAleatoria("Estela");

                    if (!esBot)
                    {
                        cantidadEstelas++;
                    }
                }
                else if (item.nombre == "Bomba")
                {
                    GenerarObjetoEnPosicionAleatoria("Bomba");
                    if (!esBot)
                    {
                        cantidadBombas++;
                    }
                }
                controladorAudio.ReproducirSonido(controladorAudio.RecogerItemGenerico);

            }

            // Si la cabeza tiene un poder, lo recoge. Si es un bot, simplemente lo mueve de posicion, ya que
            // los bots no pueden recoger poderes y se comportan de manera aleatoria.
            else if (cabeza.poder != null)
            {
                GameObject.Destroy(cabeza.ObjectoAsignado);
                Poder poder = cabeza.poder;
                cabeza.poder = null;
                if (!esBot)
                {
                    poderes.AgregarPoder(poder);
                }
                if (poder.nombre == "Velocidad")
                {
                    GenerarObjetoEnPosicionAleatoria("Velocidad");
                    if (!esBot)
                    {
                        cantidadVelocidades++;
                    }
                }
                else if (poder.nombre == "Escudo")
                {
                    GenerarObjetoEnPosicionAleatoria("Escudo");
                    if (!esBot)
                    {
                        cantidadEscudos++;
                    }
                }
                controladorAudio.ReproducirSonido(controladorAudio.RecogerPoderGenerico);
            }
        }

        public void GenerarObjetoEnPosicionAleatoria(string nombre)
        /*
         * Genera un objeto en una posición aleatoria del espacio. El nodo no puede ser 
         * obstaculo, cabeza, poder o item.
         * :param nombre: string que representa el nombre del objeto a generar.
         */
        {
            // Toma una coordenada aleatoria
            Vector2Int coord = new Vector2Int(rng.Next(0, Espacio.largo), rng.Next(0, Espacio.ancho));

            // Toma el nodo en esa coordenada
            Nodo nodo = Espacio.RedNodos[coord.x, coord.y];

            // Si en esa coordenada ya hay un obstáculo, un item o un poder, busca otra coordenada
            while (nodo.esObstaculo || nodo.item != null || nodo.poder != null || nodo.esCabeza)
            {
                coord = new Vector2Int(rng.Next(0, Espacio.largo), rng.Next(0, Espacio.ancho));
                nodo = Espacio.RedNodos[coord.x, coord.y];
            }

            // Crea el gameobject
            GameObject gameObject = new GameObject();
            gameObject.AddComponent<SpriteRenderer>();

            // Le pone el sprite y actualiza las propiedades del nodo, dependiendo del nombre
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
            // Actualiza las propiedades del transform y del sprite
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = 10;
            gameObject.transform.localScale = Texturas.instancia.escala;
            gameObject.transform.position = Espacio.CoordenadaACentro(coord);
            nodo.ObjectoAsignado = gameObject;

            // Actualiza la posicion en el espacio
            Espacio.RedNodos[coord.x, coord.y] = nodo;
        }

        public void ActualizarEstela(Vector2Int pos)
        /*
         * Actualiza la estela del jugador para la posicion pos.
         * - Si la estela es mayor que el tamaño de la estela, elimina el último nodo de la estela.
         * :param pos: Vector2Int que representa la posición de la cabeza.
         */
        {
            // Transforma la posición de unity (la cual esta centrada) a la posición en la matriz
            Vector3 posicionEspacio = Espacio.CentroACoordenada(pos);

            // Toma el nodo
            Nodo ultimaEstela = Espacio.RedNodos[(int)posicionEspacio.x, (int)posicionEspacio.y];

            Estela.AddFirst(ultimaEstela);

            // Actualiza las propiedades del nodo
            ultimaEstela.id = new Vector2Int((int)pos.x, (int)pos.y);
            ultimaEstela.id = Estela.Last.Value.id;
            ultimaEstela.esObstaculo = true;

            // Crea el GameObject y le pone como nombre su ubicación en el espacio
            ultimaEstela.ObjectoAsignado = new GameObject();
            ultimaEstela.ObjectoAsignado.name = $"{posicionEspacio.x},{posicionEspacio.y}";
            ultimaEstela.ObjectoAsignado.AddComponent<SpriteRenderer>();
            ultimaEstela.ObjectoAsignado.GetComponent<SpriteRenderer>().sprite = Texturas.instancia.EstelaGenerica;
            ultimaEstela.ObjectoAsignado.transform.localScale = new Vector3(.3f, .3f, .3f);
            ultimaEstela.ObjectoAsignado.transform.position = new Vector3(pos.x, pos.y, 10);
            ultimaEstela.ObjectoAsignado.GetComponent<SpriteRenderer>().sortingOrder = 1;


            // Si la estela es mayor que el tamaño de la estela, elimina el último nodo de la estela
            if (Estela.Count > tamagnoEstela)
            {
                // Actualiza las propiedades
                ultimaEstela = Estela.Last.Value;
                ultimaEstela.esCabeza = false;
                ultimaEstela.esObstaculo = false;

                // Actualiza el espacio
                Espacio.RedNodos[ultimaEstela.id.x, ultimaEstela.id.y] = ultimaEstela;

                // Elimina el GameObject
                Estela.RemoveLast();
                GameObject.Destroy(ultimaEstela.ObjectoAsignado);

                // Busca duplicados
                GameObject gameObjectAtPosition = GameObject.Find($"{ultimaEstela.id.x},{ultimaEstela.id.y}");
                if (gameObjectAtPosition != null)
                {
                    GameObject.Destroy(gameObjectAtPosition);
                }

            }
        }

        public Vector2Int Esquivar(Vector2Int DireccionInicial, Nodo CabezaBot)
        /*
         * Esta función hace que el bot esquive los obstáculos y la estela de la motocicleta.
         * Hace esto tomando los dos nodos contiguos a la cabeza del mismo, y comprueba si alguno
         * de esos es cabeza u obstáculo.
         * 
         * Parámetros:
         *  - DireccionInicial: Vector2Int que representa la dirección en la que se mueve el bot.
         *  - CabezaBot: Nodo que representa la cabeza del bot.
         * 
         * Retorna:
         *  - Vector2Int: Retorna la nueva dirección en la que se va a mover el bot.
         */
        {
            Nodo siguienteNodo = CabezaBot.ObtenerNodoAdyacente(DireccionInicial);
            Nodo siguienteSiguienteNodo = siguienteNodo.ObtenerNodoAdyacente(DireccionInicial);

            // Comprueba si los dos siguientes nodos son obstáculos o la cabeza de otro bot o jugador
            if (
                siguienteSiguienteNodo.esObstaculo || siguienteSiguienteNodo.esCabeza ||
                siguienteNodo.esCabeza || siguienteNodo.esObstaculo
            )
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
        /*
         * Esta función bota los items y poderes de la motocicleta por el mapa al morir.
         */
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
        /*
         * Esta función mueve la motocicleta a la posición pos. Si la motocicleta se queda sin combustible, muere.
         * Si la motocicleta choca con un obstáculo, muere. Si la motocicleta choca con un item, lo recoge.
         * Si la motocicleta choca con un poder, lo recoge.
         * 
         * Parámetros:
         *  - pos: Vector2Int que representa la posición a la que se va a mover la motocicleta.
         * 
         * Retorna:
         *  - bool: Retorna true si la motocicleta sigue viva, y false si la motocicleta muere.
         */
        {
            if (combustible > 0 && estaVivo)
            {
                // Se acaba el tiempo de velocidad
                if (TiempoDesdeUsoVelocidad >= TiempoLimiteVelocidad)
                {
                    activarCambioSprite = true;
                    velocidad = 5;
                    controladorAudio.PararSonidoEfectos();
                    TiempoDesdeUsoVelocidad = 0;
                }

                // Se acaba el tiempo de escudo
                if (TiempoDesdeUsoInmunidad >= 5f)
                {
                    activarCambioSprite = true;
                    esInmune = false;
                    TiempoDesdeUsoInmunidad = 0;
                }

                // Se acaba el tiempo de bomba
                if (TiempoDesdeUsoBomba >= 3f)
                {
                    // Destruye el objeto inicial de la bomba
                    GameObject.Destroy(bomba);

                    // Explota la bomba
                    if (bombaExplotando == false)
                    {
                        explosion = new();
                        explosion.AddComponent<SpriteRenderer>();
                        SpriteRenderer visualizacion = explosion.GetComponent<SpriteRenderer>();
                        visualizacion.sprite = Texturas.instancia.BombaNuclear;
                        explosion.transform.localScale = new Vector3(0.954f / 3, 1.465f / 3, 1f / 3);
                        visualizacion.sortingOrder = 1;
                        Vector3 tempCoord = Espacio.CoordenadaACentro(nodoBomba.id);
                        explosion.transform.position = tempCoord;

                        // Crea obstáculos en un area cuadrada de 3x3
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

                    // El tiempo de la explosion se acabo asi que vuelve los obstáculos a la normalidad
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

                // Cambia de cabeza por la nueva posicion
                Cabeza = Espacio.RedNodos[x, y];

                if (Cabeza.esObstaculo || Cabeza.esCabeza)
                {
                    // Comprueba si tiene escudo y si lo tiene, o quita y lo vuelve inmune
                    if (esInmune)
                    {
                        esInmune = false;
                        TiempoDesdeUsoInmunidad = 0;
                        activarCambioSprite = true;
                    }

                    // Si no es inmune, mata al jugador y destruye cada una de las partes de la estela
                    else
                    {
                        foreach (Nodo nodo in Estela)
                        {
                            GameObject.Destroy(nodo.ObjectoAsignado);
                        }
                        estaVivo = false;
                        if (esJugador)
                        {
                            controladorAudio.PararTodosLosSonidos();
                        }
                        controladorAudio.ReproducirSonido(controladorAudio.ExplosionJugador);
                        BotarItemsYPoderesDeLaMotocicletaPorElMapaAlMorir();
                        return false;
                    }
                }

                // Esto recoge los items que hayan en la cabeza
                ComprobarCabeza();
            }

            // Si la motocicleta se queda sin combustible, muere
            else
            {
                if (estaVivo)
                {
                    if (esJugador)
                    {
                        controladorAudio.PararTodosLosSonidos();
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


            return true;
        }


        public void UtilizarPoder()
        /*
         * Esta funcion utiliza un poder en la motocicleta. Siempre se prioriza la velocidad.
         */
        {
            Poder poder = null;

            // Ya que el usuario decide que poder usar con el alt, entonces se saca el poder de la pila
            // dependiendo de la eleccion del usuario
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

            // Si el poder es velocidad, aumenta la velocidad de la 
            if (poder.nombre == "Velocidad")
            {
                int nuevaVelocidad = velocidad + rng.Next(1, 11 - velocidad);

                // Si la velocidad sobrepasa el limite, se establece en el limite
                if (nuevaVelocidad > 10)
                {
                    nuevaVelocidad = 10;
                }

                velocidad = nuevaVelocidad;

                // La velocidad funciona por tiempo aleatorio
                TiempoLimiteVelocidad = (float)rng.Next(3, 7);
                cantidadVelocidades--;

                controladorAudio.ReproducirSonido(controladorAudio.UtilizarAumentoVelocidad);
                controladorAudio.ReproducirMusica(controladorAudio.SonidoFondoVelocidad);
            }

            // Si el poder es inmunidad, la motocicleta se vuelve inmune
            if (poder.nombre == "Escudo")
            {
                esInmune = true;
                cantidadEscudos--;
                controladorAudio.ReproducirSonido(controladorAudio.UtilizarEscudo);
            }

        }

        public void UtilizarItem(Vector2Int pos)
        /*
         * Esta funcion utiliza un item en la motocicleta. Siempre se prioriza la gasolina.
         * :param pos: Este parametro es necesario para determinar la posicion de la bomba,
         * la cual se va a posicionar detras del jugador
         */
        {
            // Como prioriza el item de combustible, entonces lo busca y lo saca de la cola de una vez
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
                bomba.name = "Bomba";
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
    /*
     * Esta clase representa una pila (stack) de poderes
     */
    {
        public LinkedList<Poder> poderes;

        public PilaPoderes()
        {
            poderes = new LinkedList<Poder>();
        }

        public void AgregarPoder(Poder poder)
        /*
         * Esta funcion agrega un poder a la pila de poderes
         * :param poder: poder a agregar
         */
        {
            poderes.AddFirst(poder);
        }

        public Poder SacarPoderEspecifico(string nombre)
        /*
         * Esta funcion saca un poder especifico de la pila de poderes
         * :param nombre: nombre del poder a sacar
         * :return: Poder
         */
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
        /*
         * Esta funcion saca un poder de la pila de poderes.
         * Siempre saca el primero ya que es un stack
         * :return: Poder
         */
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

    public class ColaItems
    /*
     * Esta clase representa una cola de items
     */
    {
        public LinkedList<Item> items;

        public ColaItems()
        {
            items = new();
        }


        public Item BuscarItem(string nombre)
        /*
         * Esta funcion busca un item en la cola de items
         * :param nombre: nombre del item a buscar
         */
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

    public class Item
    {
        public string nombre;
    }

    public class Poder
    {
        public string nombre;
    }


}



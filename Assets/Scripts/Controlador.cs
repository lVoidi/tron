/*
 * Este script se encarga de controlar la motocicleta del jugador y de los enemigos.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;
using Unity.VisualScripting;
using TMPro;
using JetBrains.Annotations;

public class Controlador : MonoBehaviour
{
    // Instancia de la motocicleta del jugador
    public Motocicleta InstanciaMotoJugador;
    public Red Espacio;
    // Posición de la celda en la que se encuentra la motocicleta
    public Vector2Int PosicionCelda;
    public Vector2Int PosicionNodo;
    public Nodo Cabeza;

    // Tiempo entre movimientos
    public float TiempoUltimoMovimiento;
    public float TiempoEntreMovimientos = 0.2f;

    public GameObject seleccion;

    public ControladorAudio controladorAudio;


    private void Awake()
    {
        controladorAudio = GameObject.Find("ManejadorAudio").GetComponent<ControladorAudio>();
        seleccion = GameObject.Find("Seleccion");
        Espacio = new(31, 13);
        PosicionCelda = new Vector2Int(0, 0);
        TiempoUltimoMovimiento = TiempoEntreMovimientos;
        transform.position = new Vector3(0, 0, 0);
    }

    void Start()
    {
        Debug.Log("Inicializado el controlador");
        InstanciaMotoJugador = new Motocicleta(new Vector2Int(0, 1), Espacio);
        InstanciaMotoJugador.AsignarCabeza(15, 6);
    }

    private void AdministracionDeMovimiento()
    {
        Vector2Int dir = new Vector2Int(0, 0);

        if (Input.GetKey(KeyCode.W) && InstanciaMotoJugador.direccion.y != -1)
        {
            dir.x = 0;
            dir.y = 1;
        }
        else if (Input.GetKey(KeyCode.A) && InstanciaMotoJugador.direccion.x != 1)
        {
            dir.x = -1;
            dir.y = 0;
        }
        else if (Input.GetKey(KeyCode.S) && InstanciaMotoJugador.direccion.y != 1)
        {
            dir.x = 0;
            dir.y = -1;
        }
        else if (Input.GetKey(KeyCode.D) && InstanciaMotoJugador.direccion.x != -1)
        {
            dir.x = 1;
            dir.y = 0;
        }
        else
        {
            dir.x = (int)InstanciaMotoJugador.direccion.x;
            dir.y = (int)InstanciaMotoJugador.direccion.y;
        }

        InstanciaMotoJugador.direccion.x = dir.x;
        InstanciaMotoJugador.direccion.y = dir.y;
    }

    private void AdministracionDePoderes()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            InstanciaMotoJugador.UtilizarPoder();
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            InstanciaMotoJugador.UtilizarItem(PosicionCelda);
        }
        else if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            InstanciaMotoJugador.PoderUtilizado = -InstanciaMotoJugador.PoderUtilizado;
            float PosXTransform = seleccion.transform.position.x;
            float PosYTransform = seleccion.transform.position.y;
            if (InstanciaMotoJugador.PoderUtilizado == 1)
            {
                seleccion.transform.position = new Vector3(PosXTransform-2, PosYTransform, 1);
            }
            else
            {
                seleccion.transform.position = new Vector3(PosXTransform+2, PosYTransform, 1);
            }
        }
    }

    private void AdministracionDeMovimientoDeLaMotocicletaPorCambioDeTiempo()
    {
        // Aumenta el cambio de tiempo desde la última actualización
        TiempoUltimoMovimiento += Time.deltaTime;

        // Si el tiempo desde el último movimiento es mayor al tiempo entre movimientos
        if (TiempoUltimoMovimiento >= TiempoEntreMovimientos)
        {
            Vector2Int PosicionEstela = new Vector2Int(PosicionCelda.x, PosicionCelda.y);
            // Actualiza la posición de la motocicleta en la clase Controlador
            PosicionCelda.x += (int)InstanciaMotoJugador.direccion.x;
            PosicionCelda.y += (int)InstanciaMotoJugador.direccion.y;
            // Simular espacio toroidal para 31 de largo y 13 de ancho
            if (PosicionCelda.x < -31 / 2)
            {
                PosicionCelda.x = 31 / 2;
            }
            else if (PosicionCelda.x > 31 / 2)
            {
                PosicionCelda.x = -31 / 2;
            }
            else if (PosicionCelda.y < -13 / 2)
            {
                PosicionCelda.y = 13 / 2;
            }
            else if (PosicionCelda.y > 13 / 2)
            {
                PosicionCelda.y = -13 / 2;
            }


            // Mueve la motocicleta dentro de la red
            bool MovimientoExitoso = InstanciaMotoJugador.Mover(PosicionCelda);
            if (!MovimientoExitoso && !InstanciaMotoJugador.estaVivo)
            {
                ParticleSystem explosion = Texturas.instancia.Explosion;
                explosion.transform.position = new Vector3(PosicionCelda.x, PosicionCelda.y, explosion.transform.position.z);
                explosion.Play();
                GameObject.Destroy(gameObject);
                return;
            }
            InstanciaMotoJugador.ActualizarEstela(PosicionEstela);
            // Mueve la motocicleta en la escena
            transform.position = new Vector3(PosicionCelda.x, PosicionCelda.y, 1);

            Vector2Int dir = new Vector2Int(InstanciaMotoJugador.direccion.x, InstanciaMotoJugador.direccion.y);
            transform.eulerAngles = new Vector3(0, 0, ObtenerAnguloAPartirDeVectorDireccion(dir) - 90);
            // Si el movimiento no fue exitoso y la motocicleta no está viva, se termina el juego


            // Disminuye el combustible de la motocicleta en uno
            InstanciaMotoJugador.combustible--;

            // Reinicia el tiempo desde el último movimiento
            TiempoUltimoMovimiento -= TiempoEntreMovimientos;

            Cabeza = InstanciaMotoJugador.Cabeza;

            // Actualiza la velocidad de la motocicleta
            //TiempoEntreMovimientos = ( 10 - InstanciaMotoJugador.velocidad )/25;
        }
    }

    public void CambiarSpriteEnFuncionDePoder()
    {
        // Esto significa que se empezó a utilizar la velocidad
        if (InstanciaMotoJugador.TiempoDesdeUsoVelocidad > 0 || InstanciaMotoJugador.TiempoDesdeUsoInmunidad > 0)
        {
            Sprite sprite = Texturas.instancia.SpriteCuandoTieneVelocidad;
            GetComponent<SpriteRenderer>().sprite = sprite;
        }

        if (InstanciaMotoJugador.activarCambioSprite == true)
        {
            Sprite sprite = Texturas.instancia.SpriteMotocicleta;
            GetComponent<SpriteRenderer>().sprite = sprite;
            InstanciaMotoJugador.activarCambioSprite = false;
        }
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

    public void ActualizarEstadisticas()
    {
        // Obtiene el contador de cada item
        TextMeshProUGUI ContadorVelocidad = Texturas.instancia.ContadorVelocidad;
        TextMeshProUGUI ContadorEscudo = Texturas.instancia.ContadorEscudo;
        TextMeshProUGUI ContadorGasolina = Texturas.instancia.ContadorCombustible;
        TextMeshProUGUI ContadorEstela = Texturas.instancia.ContadorEstela;
        TextMeshProUGUI ContadorBomba = Texturas.instancia.ContadorBomba;

        TextMeshProUGUI Estadisticas = Texturas.instancia.Estadisticas;

        // Actualiza el contador de cada item
        ContadorVelocidad.text = InstanciaMotoJugador.cantidadVelocidades.ToString();
        ContadorEscudo.text = InstanciaMotoJugador.cantidadEscudos.ToString();
        ContadorGasolina.text = InstanciaMotoJugador.cantidadCombustibles.ToString();
        ContadorEstela.text = InstanciaMotoJugador.cantidadEstelas.ToString();
        ContadorBomba.text = InstanciaMotoJugador.cantidadBombas.ToString();

        Estadisticas.text = $"Combustible: {InstanciaMotoJugador.combustible}\nVelocidad: {InstanciaMotoJugador.velocidad}";

    }

    private void ActualizarVariablesInternas()
    {
        PosicionNodo = new Vector2Int((int)Cabeza.id.x, (int)Cabeza.id.y);
        TiempoEntreMovimientos = 0.035f * (12 - InstanciaMotoJugador.velocidad);
        if (InstanciaMotoJugador.velocidad > 5)
        {
            InstanciaMotoJugador.TiempoDesdeUsoVelocidad += Time.deltaTime;
        }
        if (InstanciaMotoJugador.bomba != null)
        {
            InstanciaMotoJugador.TiempoDesdeUsoBomba += Time.deltaTime;
        }
        if (InstanciaMotoJugador.bombaExplotando == true)
        {
            InstanciaMotoJugador.TiempoDesdeExplosionNuclear += Time.deltaTime;
        }
        if (InstanciaMotoJugador.esInmune)
        {
            InstanciaMotoJugador.TiempoDesdeUsoInmunidad += Time.deltaTime;
        }
    }

    // Actualizacion de cada frame
    void Update()
    {
        AdministracionDeMovimiento();
        AdministracionDePoderes();
        AdministracionDeMovimientoDeLaMotocicletaPorCambioDeTiempo();
        ActualizarEstadisticas();
        ActualizarVariablesInternas();
        CambiarSpriteEnFuncionDePoder();
    }
}

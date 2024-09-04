/*
 * Este script se encarga de controlar la motocicleta del jugador y de los enemigos.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;
using Unity.VisualScripting;

public class Controlador : MonoBehaviour
{
    // Instancia de la motocicleta del jugador
    public Motocicleta InstanciaMotoJugador;

    // Posición de la celda en la que se encuentra la motocicleta
    public Vector2Int PosicionCelda;
    public Nodo Cabeza;

    // Tiempo entre movimientos
    public float TiempoUltimoMovimiento;
    public float TiempoEntreMovimientos = 0.2f;


    private void Awake()
    {
        PosicionCelda = new Vector2Int(0, 0);
        TiempoUltimoMovimiento = TiempoEntreMovimientos;
        transform.position = new Vector3(0, 0, 0);
    }

    void Start()
    {
        Debug.Log("Inicializado el controlador");
        InstanciaMotoJugador = new Motocicleta(new Vector2Int(0, 1));
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
            InstanciaMotoJugador.UtilizarItem();
        }
    }

    private void AdministracionDeMovimientoDeLaMotocicletaPorCambioDeTiempo()
    {
        // Aumenta el cambio de tiempo desde la última actualización
        TiempoUltimoMovimiento += Time.deltaTime;

        // Si el tiempo desde el último movimiento es mayor al tiempo entre movimientos
        if (TiempoUltimoMovimiento >= TiempoEntreMovimientos)
        {
            // Mueve la motocicleta dentro de la red
            bool MovimientoExitoso = InstanciaMotoJugador.Mover();

            // Muestra información de la motocicleta
            Debug.Log($"{InstanciaMotoJugador.combustible}");
            Debug.Log($"x: {InstanciaMotoJugador.direccion.x}");
            Debug.Log($"y: {InstanciaMotoJugador.direccion.y}");

            // Si el movimiento no fue exitoso y la motocicleta no está viva, se termina el juego
            if (!MovimientoExitoso && !InstanciaMotoJugador.estaVivo)
            {
                Debug.Log("Perdiste");
                return;
            }
            // Actualiza la posición de la motocicleta en la clase Controlador
            PosicionCelda.x += (int)InstanciaMotoJugador.direccion.x;
            PosicionCelda.y += (int)InstanciaMotoJugador.direccion.y;

            // Simular espacio toroidal para 31 de largo y 13 de ancho
            if (PosicionCelda.x < -31/2)
            {
                PosicionCelda.x = 31/2;
            }
            else if (PosicionCelda.x > 31/2)
            {
                PosicionCelda.x = -31/2;
            } else if (PosicionCelda.y < -13/2)
            {
                PosicionCelda.y = 13/2;
            }
            else if (PosicionCelda.y > 13/2)
            {
                PosicionCelda.y = -13/2;
            }

            // Mueve la motocicleta en la escena
            transform.position = new Vector3(PosicionCelda.x, PosicionCelda.y, 1);

            Vector2Int dir = new Vector2Int(InstanciaMotoJugador.direccion.x, InstanciaMotoJugador.direccion.y);
            transform.eulerAngles = new Vector3(0, 0, ObtenerAnguloAPartirDeVectorDireccion(dir) - 90);
            // Disminuye el combustible de la motocicleta en uno
            InstanciaMotoJugador.combustible--;

            // Reinicia el tiempo desde el último movimiento
            TiempoUltimoMovimiento -= TiempoEntreMovimientos;

            Cabeza = InstanciaMotoJugador.Cabeza.nodo;
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

    // Actualizacion de cada frame
    void Update()
    {
        AdministracionDeMovimiento();
        AdministracionDePoderes();
        AdministracionDeMovimientoDeLaMotocicletaPorCambioDeTiempo();
    }
}

/*
Clase que contiene las texturas de los objetos
*/
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Texturas : MonoBehaviour
{

    public static Texturas instancia;
    public Sprite SpriteMotocicleta;
    public Sprite EstelaGenerica;

    // Bots que simulan jugadores
    public Sprite BotRojo; 
    public Sprite BotAzul;
    public Sprite BotAmarillo;
    public Sprite BotRosado;

    public GameObject VelocidadPoder;
    public GameObject EscudoPoder;
    public GameObject CombustibleItem;
    public GameObject EstelaItem;
    public GameObject BombaItem;

    // Contadores de los objetos 
    public TextMeshProUGUI ContadorVelocidad;
    public TextMeshProUGUI ContadorEscudo;
    public TextMeshProUGUI ContadorCombustible;
    public TextMeshProUGUI ContadorEstela;
    public TextMeshProUGUI ContadorBomba;
    public TextMeshProUGUI Estadisticas;

    public Sprite GifExplosion;

    private void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
}

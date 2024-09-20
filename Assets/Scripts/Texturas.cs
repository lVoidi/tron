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
    public Sprite SpriteCuandoTieneVelocidad;
    public Sprite EstelaGenerica;

    // Bots que simulan jugadores
    public Sprite BotRojo; 
    public Sprite BotAzul;
    public Sprite BotAmarillo;
    public Sprite BotRosado;

    public Sprite Velocidad;
    public Sprite Escudo;
    public Sprite Combustible;
    public Sprite Estela;
    public Sprite Bomba;

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

    public ParticleSystem Explosion;
    // 0.954 / 1.465 / 1
    public Sprite BombaNuclearTiradaPorElEstadoDeIsraelEnEspacioCivilPalestino;
    // 1.4419 / 1.4419 / 1.4419
    public Sprite BombaRoja;

    public Vector3 escala;

    private void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
            escala = VelocidadPoder.transform.localScale;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
}

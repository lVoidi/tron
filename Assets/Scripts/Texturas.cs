/*
Clase que contiene las texturas de los objetos
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Texturas : MonoBehaviour
{

    public static Texturas instancia;
    public Sprite SpriteMotocicleta;

    public GameObject VelocidadPoder;
    public GameObject EscudoPoder;
    public GameObject CombustibleItem;
    public GameObject EstelaItem;
    public GameObject BombaItem;
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

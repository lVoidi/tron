using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class manejadorEntradaUsuario : MonoBehaviour
{
    Movimiento controladorMoto;

    void Awake()
    {
        controladorMoto = GetComponent<Movimiento>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 vectorEntrada = Vector2.zero;
        vectorEntrada.x = Input.GetAxis("Horizontal");
        vectorEntrada.y = Input.GetAxis("Vertical");
        controladorMoto.asignarVectorEntrada(vectorEntrada);

    }
}

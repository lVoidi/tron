using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movimiento : MonoBehaviour
{
    [Header("Ajustes de la motocicleta")]
    public float factorDerrape = 0.95f;
    public float factorAceleracion = 10.0f;
    public float factorGiro = 3.5f;

    float entradaAceleration = 0;
    float entradaDireccion = 0;

    float anguloRotacion = 0;
    Rigidbody2D motocicletaCuerpoRigido;

    void Awake()
    {
        motocicletaCuerpoRigido = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        aplicarMotor();
        cometerHomicidioEmContraDeLaVelocidadOrtogonal();
        aplicarCambioDireccion();
    }

    void aplicarMotor() 
    { 
        Vector2 vectorFuerzaMotor = transform.up * entradaAceleration * factorAceleracion;
        motocicletaCuerpoRigido.AddForce(vectorFuerzaMotor, ForceMode2D.Force);
    }

    void aplicarCambioDireccion()
    {
        float factorVelocidadMinimaAntesDeAutorizarGiro = motocicletaCuerpoRigido.velocity.magnitude / 8;
        factorVelocidadMinimaAntesDeAutorizarGiro = Mathf.Clamp01(factorVelocidadMinimaAntesDeAutorizarGiro);
        anguloRotacion -= entradaDireccion * factorGiro * factorVelocidadMinimaAntesDeAutorizarGiro;
        motocicletaCuerpoRigido.MoveRotation(anguloRotacion);
    }

    void cometerHomicidioEmContraDeLaVelocidadOrtogonal()
    {
        Vector2 velocidadFrontal = transform.up * Vector2.Dot(motocicletaCuerpoRigido.velocity, transform.up); 
        Vector2 velocidadLado = transform.right * Vector2.Dot(motocicletaCuerpoRigido.velocity, transform.right);
        motocicletaCuerpoRigido.velocity = velocidadFrontal + velocidadLado * factorDerrape;
    }

    public void asignarVectorEntrada(Vector2 vectorEntrada)
    {
        entradaDireccion = vectorEntrada.x;
        entradaAceleration = vectorEntrada.y;
    }
}

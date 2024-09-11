using UnityEngine;

public class ControladorAudio : MonoBehaviour
{
    [Header("Manejo de audio")]
    [SerializeField] private AudioSource FuenteMusica;
    [SerializeField] private AudioSource FuenteEfectos;

    [Header("Manejo de sonidos")]
    public AudioClip Musica;
    public AudioClip ExplosionBomba;
    public AudioClip ExplosionJugador;
    public AudioClip UtilizarGasolina;
    public AudioClip UtilizarBomba;
    public AudioClip RomperEscudo;
    public AudioClip UtilizarEscudo;
    public AudioClip UtilizarAumentoEstela;
    public AudioClip UtilizarAumentoVelocidad;
    public AudioClip SonidoFondoVelocidad;
    public AudioClip RecogerItemGenerico;
    public AudioClip RecogerPoderGenerico;

    private void Start()
    {
        FuenteMusica.clip = Musica;
        FuenteMusica.loop = true;
        FuenteMusica.volume = 0.03f;
        FuenteMusica.Play();

        FuenteEfectos.volume = 0.5f;
    }

    public void ReproducirSonido(AudioClip sonido)
    {
        FuenteEfectos.PlayOneShot(sonido);
    }

    public void PararTodosLosSondos()
    {
        FuenteMusica.Stop();
        FuenteEfectos.Stop();
    }
}

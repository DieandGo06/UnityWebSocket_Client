using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class TesisLunaManager : MonoBehaviour
{
    public enum Esculturas { feliz, triste, ambas };

    [Header("Particular")]
    [SerializeField] Esculturas estaEscultura;

    [Header("General")]
    [SerializeField] bool inicioDialogo;
    [SerializeField] Esculturas esculturaInteractuable;
    [SerializeField] AudioSource audioSource;

    [Header("Gato Feliz")]
    [SerializeField] List<AudioClip> clipsInteraccionFelices;
    [SerializeField] List<AudioClip> clipsMimitosFelices;

    [Header("Gato Tiste")]
    [SerializeField] List<AudioClip> clipsInteraccionTristes;
    [SerializeField] List<AudioClip> clipsMimitosTristes;

    [Header("Debug")]
    [SerializeField] int indexClipsInteraccion = 0;
    [SerializeField] int indexClipsMimitos = 0;
    [SerializeField] float tiempoTrasCantar = 0;



    string separador;
    float tiempoToReset = 30;





    private void Start()
    {
        separador = ">>";
        WS_Client.instance.RecibeMensaje.AddListener(RecibeMensaje);
    }

    private void Update()
    {
        if (inicioDialogo)
        {
            tiempoTrasCantar += Time.deltaTime;
            if (tiempoTrasCantar >= tiempoToReset)
            {
                ReiniciarExperiencia();
            }
        }
    }

    void RecibeMensaje(string _mensaje)
    {
        //[0] nomrbe, [1] escultura, [2] funcion
        string[] mensajeDividido = _mensaje.Split(separador);
        if (mensajeDividido.Length == 1) return;


        //Aquí van la decalaracion de todas las funciones
        for (int i = 0; i < mensajeDividido.Length; i++)
        {
            if (mensajeDividido[i] == "IniciarInteraccion")
            {
                Debug.Log("interaccion");
                IniciarInteraccion(mensajeDividido[1]);
                break;
            }
            if (mensajeDividido[i] == "IniciarMimitos")
            {
                Debug.Log("mimitos");
                IniciarMimitos(mensajeDividido[1]);
                break;
            }
        }
    }




    void IniciarInteraccion(string _escultura)
    {
        //Eventos indepediente a que escultura se acaricie
        //tiempoTrasCantar = 0;

        if (_escultura == "feliz")
        {
            if (clipsInteraccionFelices.Count > 0)
            {
                audioSource.PlayOneShot(clipsInteraccionFelices[indexClipsInteraccion]);
                indexClipsInteraccion++;
                if (indexClipsInteraccion >= clipsInteraccionFelices.Count) indexClipsInteraccion = 0;

            }
            else Debug.LogWarning("No hay referenciados clips de audio");
        }
        else if (_escultura == "triste")
        {
            if (clipsInteraccionTristes.Count > 0)
            {
                audioSource.PlayOneShot(clipsInteraccionTristes[indexClipsInteraccion]);
                indexClipsInteraccion++;
                if (indexClipsInteraccion >= clipsInteraccionTristes.Count) indexClipsInteraccion = 0;
            }
            else Debug.LogWarning("No hay referenciados clips de audio");
        }
        else
        {
            string aviso = "Uno de los ESP32 tiene mal la variable -escultura-. Solo puede ser -feliz- o -triste-";
            Debug.LogWarning(aviso);
            WS_Client.instance.ConsolePrintln(aviso);
        }
    }


    void IniciarMimitos(string _escultura)
    {
        //Eventos indepediente a que escultura se acaricie
        tiempoTrasCantar = 0;
        inicioDialogo = true;
        //------------------------------------------------------------------------


        //Cuando NO se acaricia a la escultura interactuable
        if (esculturaInteractuable != estaEscultura)
        {
            //StartCoroutine(CambiarPuedeInteractuar(8, true));
            return;
        }
        //------------------------------------------------------------------------


        //Cuando se acaricia la escultura CORRECTA
        if (esculturaInteractuable == estaEscultura)
        {

            if (_escultura == "feliz")
            {
                if (clipsMimitosFelices.Count == 0)
                {
                    Debug.LogWarning("No hay referenciados clips de audio");
                    return;
                }

                if (!audioSource.isPlaying)
                {
                    audioSource.clip = clipsMimitosFelices[indexClipsMimitos];
                    audioSource.Play();
                    indexClipsMimitos++;
                    StartCoroutine(CambiarPuedeInteractuar(8, Esculturas.triste));
                    if (indexClipsMimitos >= clipsMimitosFelices.Count) indexClipsMimitos = 0;
                }
            }

            else if (_escultura == "triste")
            {
                if (clipsMimitosFelices.Count == 0)
                {
                    Debug.LogWarning("No hay referenciados clips de audio");
                    return;
                }

                if (!audioSource.isPlaying)
                {
                    audioSource.clip = clipsMimitosTristes[indexClipsMimitos];
                    audioSource.Play();
                    indexClipsMimitos++;
                    StartCoroutine(CambiarPuedeInteractuar(8, Esculturas.feliz));
                    if (indexClipsMimitos >= clipsMimitosTristes.Count) indexClipsMimitos = 0;
                }
            }

            else
            {
                string aviso = "Uno de los ESP32 tiene mal la variable -escultura-. Solo puede ser -feliz- o -triste-";
                WS_Client.instance.ConsolePrintln(aviso);
                Debug.LogWarning(aviso);
                return;

            }
            return;
        }
        //------------------------------------------------------------------------


        //Cuando se deben acariciar las dos
        if (esculturaInteractuable == Esculturas.ambas)
        {
            //Canto en conjunto 
            return;
        }
        //------------------------------------------------------------------------

    }



    void ReiniciarExperiencia()
    {
        tiempoTrasCantar = 0;
        indexClipsMimitos = 0;
        inicioDialogo = false;
        esculturaInteractuable = Esculturas.feliz;
    }

    IEnumerator CambiarPuedeInteractuar(float timer, Esculturas nuevaEsculturaInteractuable)
    {
        yield return new WaitForSeconds(timer);
        esculturaInteractuable = nuevaEsculturaInteractuable;

        if (nuevaEsculturaInteractuable == Esculturas.feliz)
        {
            WS_Client.instance.ConsolePrintln("Ya puede interactuar con la escultura FELIZ");
        }
        else if (nuevaEsculturaInteractuable == Esculturas.triste)
        {
            WS_Client.instance.ConsolePrintln("Ya puede interactuar con la escultura TRISTE");
        }
    }
}

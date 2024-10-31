using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class TesisLunaManager : MonoBehaviour
{
    [Header("Gato Feliz")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] List<AudioClip> clipsInteraccionFelices;
    [SerializeField] List<AudioClip> clipsMimitosFelices;
    

    [Header("Gato Tiste")]
    [SerializeField] List<AudioClip> clipsInteraccionTristes;
    [SerializeField] List<AudioClip> clipsMimitosTristes;


    string separador;
    int indexClipsInteraccion = 0;
    int indexClipsMimitos = 0;




    private void Start()
    {
        separador = ">>";
        WS_Client.instance.RecibeMensaje.AddListener(RecibeMensaje);
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
            if (mensajeDividido[i] == "IniciarInteraccion")
            {
                Debug.Log("mimitos");
                IniciarMimitos(mensajeDividido[1]);
                break;
            }
        }
    }




    void IniciarInteraccion(string _escultura)
    {
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
        if (_escultura == "feliz")
        {
            if (clipsMimitosFelices.Count > 0)
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.clip = clipsMimitosFelices[indexClipsMimitos];
                    audioSource.Play();
                    indexClipsMimitos++;
                    if (indexClipsMimitos >= clipsMimitosFelices.Count) indexClipsMimitos = 0;
                }
            }
            else Debug.LogWarning("No hay referenciados clips de audio");
        }
        else if (_escultura == "triste")
        {
            if (clipsMimitosFelices.Count > 0)
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.clip = clipsMimitosTristes[indexClipsMimitos];
                    audioSource.Play();
                    indexClipsMimitos++;
                    if (indexClipsMimitos >= clipsMimitosTristes.Count) indexClipsMimitos = 0;
                }
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
}

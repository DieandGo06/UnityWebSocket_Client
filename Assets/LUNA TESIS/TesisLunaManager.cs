using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class TesisLunaManager : MonoBehaviour
{
    [Header("Gato Feliz")]
    [SerializeField] AudioSource audioSourceFeliz;
    [SerializeField] List<AudioClip> clipsFelices;
    int indexClipsFelices = 0;

    [Header("Gato Tiste")]
    [SerializeField] AudioSource audioSourceTriste;
    [SerializeField] List<AudioClip> clipsTristes;
    int indexClipsTristes = 0;


    string separador;




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
            Debug.Log("interaccion");
            if (mensajeDividido[i] == "IniciarInteraccion")
            {
                IniciarInteraccion(mensajeDividido[1]);
                break;
            }
        }
    }




    void IniciarInteraccion(string _escultura)
    {
        if (_escultura == "feliz")
        {
            if (clipsFelices.Count > 0)
            {
                audioSourceFeliz.PlayOneShot(clipsFelices[indexClipsFelices]);
                indexClipsFelices++;
                if (indexClipsFelices >= clipsFelices.Count) indexClipsFelices = 0;
                
            }
            else Debug.LogWarning("No hay referenciados clips de audio");
        }
        else if (_escultura == "triste")
        {
            if (clipsTristes.Count > 0)
            {
                audioSourceTriste.PlayOneShot(clipsTristes[indexClipsTristes]);
                indexClipsTristes++;
                if (indexClipsTristes >= clipsTristes.Count) indexClipsTristes = 0;
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

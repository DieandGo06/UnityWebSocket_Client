using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public WS_Client webSocket;
    [SerializeField] int cantidadSanguijuelas;
    public float peso;


    [Header("Audio Sources")]
    [SerializeField] AudioSource[] sanguijuelas;
    [SerializeField] AudioSource[] respiraciones;
    [SerializeField] AudioSource ambienteBueno;
    [SerializeField] AudioSource ambienteMalo;
    [SerializeField] AudioSource extras;
    [SerializeField] int entrante = 1;
    [SerializeField] int saliente = 0;

    [Header("Clips de respiraciones")]
    [SerializeField] AudioClip respiracionCalmada;
    [SerializeField] AudioClip respiracionIntermdia;
    [SerializeField] AudioClip respiracionAgitada;
    [SerializeField] AudioClip soplido;
    [SerializeField] AudioClip grito;

    [SerializeField] bool transicionRespiracion;





    void Start()
    {
        webSocket.RecibeInt.AddListener(ActualizarAudio);
        //webSocket.SeConecto.AddListener(ReproducirAudiosInciales);

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (cantidadSanguijuelas > 0)
            {
                ActualizarAudio(cantidadSanguijuelas - 1);
            }
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (cantidadSanguijuelas < 9)
            {
                ActualizarAudio(cantidadSanguijuelas + 1);
            }
        }
    }

    void ReproducirAudiosInciales()
    {

    }


    void ActualizarAudio(int nuevaCantidadSanguijuelas)
    {
        if (agregaronSanguijuelas(nuevaCantidadSanguijuelas))
        {
            extras.PlayOneShot(grito);
            sanguijuelas[nuevaCantidadSanguijuelas - 1].Play();
            Tareas.NuevaConDuracion(0, 0.1f, 0.3f, () =>
            {
                FadeCruzado(ambienteMalo, ambienteBueno, 1);
            }, 5896512);

        }
        else if (sacoronSanguijuelas(nuevaCantidadSanguijuelas))
        {
            extras.PlayOneShot(soplido);
            sanguijuelas[nuevaCantidadSanguijuelas - 1].Pause();
            Tareas.NuevaConDuracion(0, 0.1f, 0.3f, () =>
            {
                FadeCruzado(ambienteBueno, ambienteMalo, 1);
            }, 1886292);
        }


        if (nuevaCantidadSanguijuelas >= 0 && nuevaCantidadSanguijuelas < 3)
        {

        }
        else if (nuevaCantidadSanguijuelas >= 3 && nuevaCantidadSanguijuelas < 7)
        {

        }
        else if (nuevaCantidadSanguijuelas >= 7)
        {

        }
        ActualizarCantidadSanguijuelas(nuevaCantidadSanguijuelas);
    }


    public void ActualizarCantidadSanguijuelas(float num)
    {
        cantidadSanguijuelas = (int)num;
    }

    public bool agregaronSanguijuelas(int nuevaCantidadSanguijuelas)
    {
        if (nuevaCantidadSanguijuelas > cantidadSanguijuelas)
        {
            return true;
        }
        return false;
    }

    public bool sacoronSanguijuelas(int nuevaCantidadSanguijuelas)
    {
        if (nuevaCantidadSanguijuelas < cantidadSanguijuelas)
        {
            return true;
        }
        return false;
    }

    void FadeCruzado(AudioSource entrante, AudioSource saliente, float duracion)
    {
        float speed = Time.deltaTime / duracion;
        entrante.volume += speed;
        saliente.volume -= speed;
        if (entrante.volume >= 1) entrante.volume = 1;
        if (saliente.volume <= 0) saliente.volume = 0;
    }

    void CambiarAudioSourceEntranteSaliente()
    {
        //Cambiar AudioSource entrante y saliente para el fade cruzado
        if (entrante == 1 && saliente == 0)
        {
            entrante = 0;
            saliente = 1;
        }
        else if (entrante == 0 && saliente == 1)
        {
            entrante = 1;
            saliente = 0;
        }
    }
}

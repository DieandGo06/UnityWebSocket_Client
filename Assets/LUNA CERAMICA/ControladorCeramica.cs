using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControladorCeramica : MonoBehaviour
{
    public WS_Client webSocket;
    public float peso;


    [Header("Ambiente")]
    [SerializeField] AudioSource[] controladorAmbiente;
    [SerializeField] AudioClip sonidoAmbienteBueno;
    [SerializeField] AudioClip sonidoAmbienteMedio;
    [SerializeField] AudioClip sonidoAmbienteMalo;
    [SerializeField] bool transicionAmbiente; 

    [Header("Respiraciones")]
    [SerializeField] AudioSource[] controladorRespiracion;
    [SerializeField] AudioClip respiracionCalmada;
    [SerializeField] AudioClip grito;



    [SerializeField] bool transicionRespiracion;



    void Start()
    {
        //webSocket.SeConecto.AddListener(ReproducirAudiosInciales);

    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            //Estado caotico
            controladorAmbiente[0].clip = sonidoAmbienteMalo;
            controladorAmbiente[0].Play();

            //controladorRespiracion[0].clip = respiracionCalmada;
            //controladorRespiracion[0].Play();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            //Estado Intermedio
            controladorAmbiente[0].clip = sonidoAmbienteMedio;
            controladorAmbiente[0].Play();

            //controladorRespiracion[0].clip = respiracionCalmada;
            //controladorRespiracion[0].Play();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            //Estado Relajao
            controladorAmbiente[0].clip = sonidoAmbienteBueno;
            controladorAmbiente[0].Play();

            //controladorRespiracion[0].clip = respiracionCalmada;
            //controladorRespiracion[0].Play();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            controladorRespiracion[1].PlayOneShot(grito);
        }
    }

    void ReproducirAudiosInciales()
    {
        controladorAmbiente[0].clip = sonidoAmbienteMalo;
        controladorAmbiente[0].Play();
    }
}

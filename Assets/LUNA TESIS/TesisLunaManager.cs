using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class TesisLunaManager : MonoBehaviour
{
    public enum Esculturas { feliz, triste, ambas };
    public enum Fases { standBy, desarrollo, final };
    public enum Luces { parpadear, encenderTodas, apagar}


    [Header("Particular")]
    [SerializeField] Esculturas estaEscultura;

    [Space(5)]
    [Header("General")]
    [SerializeField] Fases faseActual;
    [SerializeField] Esculturas esculturaInteractuable;
    [SerializeField] AudioSource audioSource;
    [SerializeField] bool inicioDialogo;
    [SerializeField] bool iniciaronAudiosFinales;
    [SerializeField] bool felizListoParaFinal;
    [SerializeField] bool tristeListoParaFinal;

    [Space(5)]
    [Header("Gato Feliz")]
    [SerializeField] List<AudioClip> clipsInteraccionFelices;
    [SerializeField] List<AudioClip> clipsMimitosFelices;
    [SerializeField] AudioClip finalDistorsionadoFeliz;
    [SerializeField] AudioClip finalCorrectoFeliz;

    [Space(5)]
    [Header("Gato Tiste")]
    [SerializeField] List<AudioClip> clipsInteraccionTristes;
    [SerializeField] List<AudioClip> clipsMimitosTristes;
    [SerializeField] AudioClip finalDistorsionadoTriste;
    [SerializeField] AudioClip finalCorrectoTriste;

    [Space(5)]
    [Header("Debug")]
    [SerializeField] int indexClipsInteraccion = 0;
    [SerializeField] int cancionParte = 0;
    [SerializeField] float tiempoTrasCantar = 0;



    string separador;
    float tiempoToReset = 60;
    float tiempoToResetFinal = 16;
    float duracionAudios = 8;






    private void Start()
    {
        separador = ">>";
        WS_Client.instance.SeConecto.AddListener(EstadoInicialLuces);
        WS_Client.instance.RecibeMensaje.AddListener(RecibeMensaje);
        WS_Client.instance.ConsolePrintln("Controlando escultura " + estaEscultura.ToString());
    }

    private void Update()
    {
        if (inicioDialogo)
        {
            if (tiempoTrasCantar >= tiempoToReset)
            {
                ReiniciarExperiencia();
            }

            if (iniciaronAudiosFinales)
            {
                if (tiempoTrasCantar >= tiempoToResetFinal)
                {
                    ReiniciarFinal();
                }
            }
            tiempoTrasCantar += Time.deltaTime;
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
            //PrepararFinal es una señal que mandan los Unity
            if (mensajeDividido[i] == "PrepararFinal")
            {
                Debug.Log("Preparando final");
                PrepararFinal(mensajeDividido[1]);
                break;
            }
            //PrepararFinal es una señal que mandan los Unity
            if (mensajeDividido[i] == "EjecutarFinal")
            {
                Debug.Log("EjecutarFinal");
                EjecutarFinal(mensajeDividido[1]);
                break;
            }

            //En este condicional se recibe el string completo con todos los nuevos datos (Lo envian los Unity)
            if (mensajeDividido[i] == "ActualizarEstados")
            {
                Debug.Log("ActualizarEstados");
                RecibirEstadosFromUnity(_mensaje);
                break;
            }
        }
    }





    #region Logica Maquina de estados
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

        if (faseActual == Fases.standBy)
        {
            //Falta que el otro se queje cuando acarician a uno ---> IMPORTANTE
            PrimerosMimitos(_escultura);
            return;
        }

        if (faseActual == Fases.desarrollo)
        {
            MimitosDesarrollo(_escultura);
        }

        if (faseActual == Fases.final)
        {
            MimitosFinal(_escultura);
        }
    }



    //Falta que el otro se queje cuando acarician a uno ---> IMPORTANTE
    void PrimerosMimitos(string _escultura)
    {
        if (clipsMimitosFelices.Count == 0 || clipsMimitosTristes.Count == 0)
        {
            Debug.LogWarning("Falta la referencia de algun clip de audio");
            return;
        }

        if (_escultura == "feliz" && estaEscultura == Esculturas.feliz)
        {
            if (esculturaInteractuable == Esculturas.feliz || esculturaInteractuable == Esculturas.ambas)
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.clip = clipsMimitosFelices[0];
                    audioSource.Play();

                    //ESP32
                    ComportamientoComunLuces(Esculturas.feliz);

                    //Los estados cambian poco antes de terminar el audio
                    StartCoroutine(CambiarPuedeInteractuar(duracionAudios - 1, Esculturas.triste));
                    StartCoroutine(EnviarEstadoToUnity(duracionAudios + 0.2f));
                }
            }
        }
        else if (_escultura == "triste" && estaEscultura == Esculturas.triste)
        {
            if (esculturaInteractuable == Esculturas.triste || esculturaInteractuable == Esculturas.ambas)
            {
                if (!audioSource.isPlaying)
                {
                    StartCoroutine(CambiarPuedeInteractuar(duracionAudios - 1, Esculturas.feliz));
                    StartCoroutine(CambiarFase(duracionAudios - 1, Fases.desarrollo));
                    audioSource.clip = clipsMimitosTristes[0];
                    audioSource.Play();
                    cancionParte = 1;

                    //ESP32
                    ComportamientoComunLuces(Esculturas.triste);

                    //Los estados cambian poco antes de terminar el audio
                    StartCoroutine(EnviarEstadoToUnity(duracionAudios + 0.2f));
                }
            }
        }
        else
        {
            string aviso = "Uno de los ESP32 tiene mal la variable -escultura-. Solo puede ser -feliz- o -triste- o -ambas-";
            WS_Client.instance.ConsolePrintln(aviso);
            Debug.LogWarning(aviso);
            return;
        }
    }


    //Falta mandar un senal al arduino para que titilen mas laz luces del otro que no es acariciado
    void MimitosDesarrollo(string _escultura)
    {
        if (clipsMimitosFelices.Count == 0 || clipsMimitosTristes.Count == 0)
        {
            Debug.LogWarning("Falta la referencia de algun clip de audio");
            return;
        }

        //Cuando NO se acaricia a la escultura interactuable
        if (esculturaInteractuable != estaEscultura)
        {
            //StartCoroutine(CambiarPuedeInteractuar(duracionAudios-1, true));
            return;
        }
        //------------------------------------------------------------------------

        //Cuando se acaricia la escultura CORRECTA
        if (esculturaInteractuable == estaEscultura)
        {
            if (_escultura == "feliz" && estaEscultura == Esculturas.feliz)
            {
                if (!audioSource.isPlaying && cancionParte <= clipsMimitosFelices.Count - 1)
                {
                    StartCoroutine(CambiarPuedeInteractuar(duracionAudios - 1, Esculturas.triste));
                    audioSource.clip = clipsMimitosFelices[cancionParte];
                    audioSource.Play();
                    Debug.Log("Esta sonando feliz");

                    //ESP32
                    ComportamientoComunLuces(Esculturas.feliz);

                    //Los estados cambian poco antes de terminar el audio
                    StartCoroutine(EnviarEstadoToUnity(duracionAudios + 0.2f));
                }
            }
            else if (_escultura == "triste" && estaEscultura == Esculturas.triste)
            {
                if (!audioSource.isPlaying && cancionParte <= clipsMimitosTristes.Count - 1)
                {
                    StartCoroutine(CambiarPuedeInteractuar(duracionAudios - 1, Esculturas.feliz));
                    audioSource.clip = clipsMimitosTristes[cancionParte];
                    audioSource.Play();
                    cancionParte++;
                    Debug.Log("Esta sonando triste");

                    if (cancionParte < clipsMimitosTristes.Count) //Aun en el desarrollo de la historia
                    {
                        ComportamientoComunLuces(Esculturas.triste);
                    }
                    else if (cancionParte == clipsMimitosTristes.Count) //Llegando al final
                    {
                        StartCoroutine(CambiarFase(duracionAudios - 1, Fases.final));
                        StartCoroutine(CambiarPuedeInteractuar(duracionAudios - 0.2f, Esculturas.ambas));

                        //ESP32: Cambia al pasar al final
                        StartCoroutine(EnviarEstadoDeLuces(0f, Esculturas.triste, Luces.encenderTodas));
                        //Tras terminar audio
                        StartCoroutine(EnviarEstadoDeLuces(duracionAudios - 0.5f, Esculturas.triste, Luces.parpadear));
                        StartCoroutine(EnviarEstadoDeLuces(duracionAudios - 0.5f, Esculturas.feliz, Luces.parpadear));
                    }
                    //Los estados cambian poco antes de terminar el audio
                    StartCoroutine(EnviarEstadoToUnity(duracionAudios + 0.2f));
                }
            }
            else
            {
                string aviso = "Uno de los ESP32 tiene mal la variable -escultura-. Solo puede ser -feliz- o -triste- o -ambas-";
                WS_Client.instance.ConsolePrintln(aviso);
                Debug.LogWarning(aviso);
                return;
            }
        }
    }


    //Falta mandar un senal al arduino para que titilen mas laz luces del otro que no es acariciado
    void MimitosFinal(string _escultura)
    {
        if (finalCorrectoFeliz == null || finalDistorsionadoFeliz == null || finalCorrectoTriste == null || finalDistorsionadoTriste == null)
        {
            Debug.LogWarning("Falta la referencia de alguno de los audios finales");
            return;
        }

        if (_escultura == "feliz" && estaEscultura == Esculturas.feliz)
        {
            //if (!iniciaronAudiosFinales)
            //{
            iniciaronAudiosFinales = true;
            EnviarMensajeComoESP32("feliz", "PrepararFinal");

            if (!audioSource.isPlaying && !tristeListoParaFinal)
            {
                audioSource.Stop();
                audioSource.clip = finalDistorsionadoFeliz;
                audioSource.Play();
            }
            //Los estados cambian poco antes de terminar el audio
            StartCoroutine(EnviarEstadoToUnity(0.2f));
            //iniciaronAudiosFinales SE REINICIA EN EL UPDATE     
            return;
            //}
        }
        else if (_escultura == "triste" && estaEscultura == Esculturas.triste)
        {
            //if (!iniciaronAudiosFinales)
            //{
            iniciaronAudiosFinales = true;
            EnviarMensajeComoESP32("triste", "PrepararFinal");

            if (!audioSource.isPlaying && !felizListoParaFinal)
            {
                audioSource.Stop();
                audioSource.clip = finalDistorsionadoTriste;
                audioSource.Play();
            }
            //Los estados cambian poco antes de terminar el audio
            StartCoroutine(EnviarEstadoToUnity(0.2f));
            //iniciaronAudiosFinales SE REINICIA EN EL UPDATE

            return;
            //}
        }
        else
        {
            string aviso = "Uno de los ESP32 tiene mal la variable -escultura-. Solo puede ser -feliz- o -triste- o -ambas-";
            WS_Client.instance.ConsolePrintln(aviso);
            Debug.LogWarning(aviso);
            return;
        }
    }


    void PrepararFinal(string _escultura)
    {
        if (_escultura == "feliz") felizListoParaFinal = true;
        else if (_escultura == "triste") tristeListoParaFinal = true;

        if (iniciaronAudiosFinales && felizListoParaFinal && tristeListoParaFinal)
        {
            EnviarMensajeComoESP32(_escultura, "EjecutarFinal");
            //Los estados cambian poco antes de terminar el audio
            StartCoroutine(EnviarEstadoToUnity(0.2f));
        }
    }


    void EjecutarFinal(string _escultura)
    {
        if (finalCorrectoFeliz == null || finalDistorsionadoFeliz == null || finalCorrectoTriste == null || finalDistorsionadoTriste == null)
        {
            Debug.LogWarning("Falta la referencia de alguno de los audios finales");
            return;
        }

        Debug.Log("FINAL");
        //NO DEBERIA SER UN STOP, DEBERIA SER UN CROSS FADE
        audioSource.Stop();

        if (estaEscultura == Esculturas.feliz)
        {
            audioSource.clip = finalCorrectoFeliz;
        }
        else if (estaEscultura == Esculturas.triste)
        {
            audioSource.clip = finalCorrectoTriste;
        }

        //ESP32:
        StartCoroutine(EnviarEstadoDeLuces(0f, Esculturas.feliz, Luces.encenderTodas));
        StartCoroutine(EnviarEstadoDeLuces(0f, Esculturas.triste, Luces.encenderTodas));
        //Tras terminar audio
        StartCoroutine(EnviarEstadoDeLuces((duracionAudios*2), Esculturas.triste, Luces.apagar));
        StartCoroutine(EnviarEstadoDeLuces((duracionAudios*2), Esculturas.feliz, Luces.apagar));

        //AQUI SE DEBE ENVIAR LA SENAL AL OTRO TAMBIEN
        audioSource.Play();
        StartCoroutine(ReiniciarExperienciaTrasFinal());
    }
    #endregion


    #region funciones para envio y recepcion de datos
    IEnumerator EnviarEstadoToUnity(float timer)
    {
        yield return new WaitForSeconds(timer);

        // Esto es una guarrada que me dijo chatGPT porque la corrutina estaba tomando los valores recien se ejecutaba y no despues del WaitForSeconds
        int _cancionParte = cancionParte;
        float _tiempoTrasCantar = tiempoTrasCantar;
        bool _inicioDialogo = inicioDialogo;
        bool _iniciaronAudiosFinales = iniciaronAudiosFinales;
        bool _tristeListoParaFinal = tristeListoParaFinal;
        bool _felizListoParaFinal = felizListoParaFinal;
        Esculturas _esculturaInteractuable = esculturaInteractuable;
        Fases _faseActual = faseActual;

        string[] cadena = {
            //int
            _cancionParte.ToString() + separador,
            //float
            _tiempoTrasCantar.ToString() + separador,
            //bool
            _inicioDialogo.ToString() + separador,
            _iniciaronAudiosFinales.ToString() + separador,
            _tristeListoParaFinal.ToString() + separador,
            _felizListoParaFinal.ToString() + separador,
            //Enum Esculturas
            _esculturaInteractuable.ToString() + separador,
            //Enum Fases
            _faseActual.ToString()+ separador,
        };
        string mensaje = string.Join("", cadena);


        if (WS_Client.instance != null)
        {
            WS_Client.instance.Send("Unity(" + WS_Client.instance.nombre + ")" + separador + "ActualizarEstados" + separador + mensaje);
        }
    }

    void RecibirEstadosFromUnity(string nuevosEstados)
    {
        //El [0] es solo el nombre de quien envia, y [1] es el tipo de accion
        string[] mensajeDividido = nuevosEstados.Split(separador);
        //int
        cancionParte = int.Parse(mensajeDividido[2]);
        //float
        tiempoTrasCantar = float.Parse(mensajeDividido[3]);
        //bool
        inicioDialogo = bool.Parse(mensajeDividido[4]);
        iniciaronAudiosFinales = bool.Parse(mensajeDividido[5]);
        tristeListoParaFinal = bool.Parse(mensajeDividido[6]);
        felizListoParaFinal = bool.Parse(mensajeDividido[7]);
        //Enum Esculturas
        esculturaInteractuable = (Esculturas)System.Enum.Parse(typeof(Esculturas), mensajeDividido[8]);
        //Enum Fases
        faseActual = (Fases)System.Enum.Parse(typeof(Fases), mensajeDividido[9]);

    }

    IEnumerator EnviarEstadoDeLuces(float timer, Esculturas _escultura, Luces _estado)
    {
        WS_Client.instance.Send("prueba");

        yield return new WaitForSeconds(timer);
        if (_estado == Luces.parpadear)
        {
            if (_escultura == Esculturas.feliz)
            {
                WS_Client.instance.Send("parpadearFeliz");
            }
            else if (_escultura == Esculturas.triste)
            {
                WS_Client.instance.Send("parpadearTriste");
            }
            else if (_escultura == Esculturas.ambas)
            {
                WS_Client.instance.Send("parpadearFeliz");
                WS_Client.instance.Send("parpadearTriste");
            }
        }
        else if (_estado == Luces.encenderTodas)
        {
            if (_escultura == Esculturas.feliz)
            {
                WS_Client.instance.Send("encenderTodasFeliz");
            }
            else if (_escultura == Esculturas.triste)
            {
                WS_Client.instance.Send("encenderTodasTriste");
            }
            else if (_escultura == Esculturas.ambas)
            {
                WS_Client.instance.Send("encenderTodasFeliz");
                WS_Client.instance.Send("encenderTodasTriste");
            }
        }
        else if (_estado == Luces.apagar)
        {
            if (_escultura == Esculturas.feliz)
            {
                WS_Client.instance.Send("apagarFeliz");
            }
            else if (_escultura == Esculturas.triste)
            {
                WS_Client.instance.Send("apagarTriste");
            }
            else if (_escultura == Esculturas.ambas)
            {
                WS_Client.instance.Send("apagarFeliz");
                WS_Client.instance.Send("apagarTriste");
            }
        }

    }

    void EstadoInicialLuces()
    {
        StartCoroutine(EnviarEstadoDeLuces(0f, Esculturas.feliz, Luces.parpadear));
        StartCoroutine(EnviarEstadoDeLuces(0f, Esculturas.triste, Luces.parpadear));
    }

    void ComportamientoComunLuces(Esculturas esculturaAcariciada)
    {
        if (esculturaAcariciada == Esculturas.feliz)
        {
            //ESP32
            StartCoroutine(EnviarEstadoDeLuces(0f, Esculturas.feliz, Luces.encenderTodas));
            //Tras terminar audio
            StartCoroutine(EnviarEstadoDeLuces(duracionAudios - 0.5f, Esculturas.feliz, Luces.apagar));
            StartCoroutine(EnviarEstadoDeLuces(duracionAudios - 0.5f, Esculturas.triste, Luces.parpadear));
        }
        else if (esculturaAcariciada == Esculturas.feliz)
        {
            //ESP32
            StartCoroutine(EnviarEstadoDeLuces(0f, Esculturas.triste, Luces.encenderTodas));
            //Tras terminar audio
            StartCoroutine(EnviarEstadoDeLuces(duracionAudios - 0.5f, Esculturas.triste, Luces.apagar));
            StartCoroutine(EnviarEstadoDeLuces(duracionAudios - 0.5f, Esculturas.feliz, Luces.parpadear));
        }
    }
    #endregion


    #region Funciones "secundarias" a la logica de la maquina de estados
    void ReiniciarExperiencia()
    {
        tiempoTrasCantar = 0;
        cancionParte = 0;
        inicioDialogo = false;
        esculturaInteractuable = Esculturas.ambas;
        faseActual = Fases.standBy;
        ReiniciarFinal();
    }

    void ReiniciarFinal()
    {
        iniciaronAudiosFinales = false;
        felizListoParaFinal = false;
        tristeListoParaFinal = false;
        StartCoroutine(EnviarEstadoToUnity(0.2f));
    }

    IEnumerator ReiniciarExperienciaTrasFinal()
    {
        yield return new WaitForSeconds(16);
        ReiniciarExperiencia();
        //ESP32:
        StartCoroutine(EnviarEstadoDeLuces(1f, Esculturas.feliz, Luces.parpadear));
        StartCoroutine(EnviarEstadoDeLuces(1f, Esculturas.triste, Luces.parpadear));
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

    IEnumerator CambiarFase(float timer, Fases _nuevaFase)
    {
        yield return new WaitForSeconds(timer);
        faseActual = _nuevaFase;
    }

    void EnviarMensajeComoESP32(string nombreEscultura, string accion)
    {
        if (WS_Client.instance != null)
        {
            WS_Client.instance.Send("Unity(" + WS_Client.instance.nombre + ")" + separador + nombreEscultura + separador + accion);
        }
    }
    #endregion
}

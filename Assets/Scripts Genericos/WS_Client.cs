using UnityEngine.Events;
using WebSocketSharp;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using System;

public class WS_Client : MonoBehaviour
{
    //Documentacion de la libreria "WebSocketSharp": https://github.com/PingmanTools/websocket-sharp/
    WebSocket ws;

    public string nombre;
    public string serverUrl;

    bool conectado;

    [Header("Interfaz")]
    public GameObject boton_conectar;
    public TextMeshProUGUI inputName;
    public TextMeshProUGUI inputIP;


    [Header("Consola")]
    [SerializeField] TextMeshProUGUI consoleText;
    [SerializeField]  int maxLines;
    private List<string> messageList = new List<string>();


    [HideInInspector] public UnityEvent SeConecto;
    [HideInInspector] public UnityEvent<int> RecibeInt;
    [HideInInspector] public UnityEvent<float> RecibeFloat;



    void Awake()
    {
        //Application.targetFrameRate = 60;
        RecibeInt = new UnityEvent<int>();
        RecibeFloat = new UnityEvent<float>();
    }

    private void Start()
    {
        ShowConnectButton();
        if (PlayerPrefs.HasKey("Nombre"))
        {
            nombre = PlayerPrefs.GetString("Nombre");
            ConsolePrintln("Nombre: " +  nombre);
        }
        if (PlayerPrefs.HasKey("IP"))
        {
            serverUrl = PlayerPrefs.GetString("IP");
            ConsolePrintln("IP: " + serverUrl);
        }
        
    }

    void Update()
    {
        ////Hay conexión con el servidor
        //if (ws != null)
        //{
        //    //Evento: Recibe mensaje de servidor (Evento de libreria)
        //    ws.OnMessage += (sender, mensaje) =>
        //    {
        //        Debug.Log("Mensaje recibido: " + mensaje.Data);
        //        if (int.TryParse(mensaje.Data, out int numeroI))
        //        {
        //            RecibeFloat.Invoke(numeroI);
        //        }
        //        if (float.TryParse(mensaje.Data, out float numeroF))
        //        {
        //            RecibeFloat.Invoke(numeroF);
        //        }
        //    };

        //    //Evento: Hubo un error en la conexión (Evento de libreria)
        //    ws.OnError += (sender, mensaje) =>
        //    {
        //        Debug.LogWarning("Error del servidor");
        //    };
        //    //Evento: Se pierde la conexión (Evento de libreria)
        //    ws.OnClose += (sender, e) =>
        //    {
        //        Debug.LogWarning("Se cerro la conexión");
        //        ShowConnectButton();
        //    };
        //    if (!conectado) conectado = true;
        //}


        //if (Input.GetKeyDown("1")) ws.Send("play 1");
        ////if (Input.GetKeyDown("2")) ws.Send("stop 1");


    }


    #region Conexion con servidor
    //Cuando se presiona el boton "Conectar" -----------------------------------------------------------------
    public void ConectToServer()
    {
        if (serverUrl == "ws://:3000" || serverUrl == "")
        {
            Debug.LogWarning("No definiste una IP");
            return;
        }

        ws = new WebSocket(serverUrl);

        // Evento: Se abre la conexión con el servidor
        ws.OnOpen += (sender, mensaje) =>
        {
            conectado = true;
            HideConnectButton();
            Debug.Log("Conexión exitosa");
            ConsolePrintln("-------------------");
            ConsolePrintln("Nombre: " + nombre);
            ConsolePrintln("IP: " + serverUrl);
            ConsolePrintln("Conexión exitosa");
            SeConecto.Invoke(); // Invoca el evento personalizado
            ws.Send("Unity (" + nombre + ")" + ": Se conectó al servidor");
            StartCoroutine(SendPingRoutine());
        };

        // Evento: Error en la conexión
        ws.OnError += (sender, mensaje) =>
        {
            conectado = false;
            ShowConnectButton();
            ConsolePrintln("Error al conectar al servidor: " + mensaje.Message);
            Debug.LogWarning("Error al conectar con el servidor: " + mensaje.Message);
        };

        // Evento: Se cierra la conexión
        ws.OnClose += (sender, mensaje) =>
        {
            conectado = false;
            ShowConnectButton();
            ConsolePrintln("Conexión cerrada");
            Debug.LogWarning("Conexión cerrada");
        };

        ws.Connect();
    }
    //============================================================================================================

    IEnumerator SendPingRoutine()
    {
        while (ws != null && ws.IsAlive)
        {
            bool pingSuccess = ws.Ping();  // Enviar ping
            if (pingSuccess)
            {
                Debug.Log("Ping exitoso");
                ConsolePrintln("Ping, pong exitoso");
            }
            else
            {
                Debug.LogWarning("Ping fallido");
                ConsolePrintln("Ping, pong FALLIDO");
            }

            // Espera 5 segundos antes de enviar otro ping
            yield return new WaitForSeconds(120f);
        }
    }
    #endregion



    #region Interfaz
    public void ReadTextInputIP(string t)
    {
        serverUrl = "ws://" + t + ":3000";
        Debug.Log("La conexión se hará en la IP: " + t);

        PlayerPrefs.SetString("IP", serverUrl);
        PlayerPrefs.Save(); // Forzar el guardado inmediato
    }

    public void ReadTextInputName(string t)
    {
        nombre = t;
        Debug.Log("El nombre de este dispositivo será: " + t);

        PlayerPrefs.SetString("Nombre", nombre);
        PlayerPrefs.Save(); // Forzar el guardado inmediato
    }

    public void DisconnectToServer()
    {
        if (ws != null)
            ws.Close();
    }

    void HideConnectButton()
    {
        boton_conectar.SetActive(false);
    }

    void ShowConnectButton()
    {
        boton_conectar.SetActive(true);
    }

    public void SendEmptyMessage()
    {
        if (ws != null)
        {
            ws.Send("Mensaje de Prueba");
        }
    }

    public void QuitApp()
    {
        Application.Quit();
    }
    #endregion



    //================== Console ===================
    public void ConsolePrint(string message)
    {
        if (messageList.Count >= maxLines) messageList.RemoveAt(0);
        messageList.Add(message);
        consoleText.text = "";
        foreach (string msg in messageList)
        {
            consoleText.text += msg;
        }
    }

    public void ConsolePrintln(string message)
    {
        if (messageList.Count >= maxLines) messageList.RemoveAt(0);
        messageList.Add(message);
        consoleText.text = "";

        DateTime now = DateTime.Now;  // Obtiene la fecha y hora actual del sistema
        string currentHour = now.ToString("HH:mm:ss");

        foreach (string msg in messageList)
        {
            // Agrega cada mensaje en una nueva línea
            consoleText.text += currentHour + ":  " + msg + "\n";
        }
    }
    //===============================================
}

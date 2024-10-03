using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using NativeWebSocket;
using UnityEngine;
using TMPro;
using System;



public class WS_Client : MonoBehaviour
{
    //Documentacion de la libreria "Native WebSocket": https://github.com/endel/NativeWebSocket
    WebSocket websocket;


    [Header("Conexion")]
    public string nombre;
    public string serverUrl;
    float tiempoComprobarConexion;
    bool conectado;


    [Header("Interfaz")]
    public GameObject boton_conectar;
    public TextMeshProUGUI inputName;
    public TextMeshProUGUI inputIP;
    public string MENSAJE;


    [Header("Consola")]
    [SerializeField] TextMeshProUGUI consoleText;
    [SerializeField] int maxLines;
    private List<string> messageList = new List<string>();


    [HideInInspector] public UnityEvent SeConecto;
    [HideInInspector] public UnityEvent<string> OnMessage;



    void Awake()
    {
        Application.runInBackground = true;
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        ShowConnectButton();
        if (PlayerPrefs.HasKey("Nombre"))
        {
            nombre = PlayerPrefs.GetString("Nombre");
            ConsolePrintln("Nombre: " + nombre);
        }
        if (PlayerPrefs.HasKey("IP"))
        {
            serverUrl = PlayerPrefs.GetString("IP");
            ConsolePrintln("IP: " + serverUrl);
        }
    }


    void Update()
    {
        if (isConectionOpen())
        {
            //Mantien la conexión con pings
            SendPing();
        }

#if !UNITY_WEBGL || UNITY_EDITOR
        if (websocket != null)
        {
            websocket.DispatchMessageQueue();
        }
#endif
    }




    #region Conexion con servidor
    public void ConectToServer()
    {
        if (serverUrl == "ws://:3000" || serverUrl == "")
        {
            Debug.LogWarning("No definiste una IP");
            return;
        }

        websocket = new WebSocket(serverUrl);

        // Evento: Se abre la conexión con el servidor
        websocket.OnOpen += () =>
        {
            conectado = true;
            HideConnectButton();
            Debug.Log("Conexión exitosa");
            ConsolePrintln("-------------------");
            ConsolePrintln("Nombre: " + nombre);
            ConsolePrintln("IP: " + serverUrl);
            ConsolePrintln("Conexión exitosa");
            ConsolePrintln("-------------------");

            SeConecto.Invoke(); // Invoca el evento personalizado
            Send("Unity (" + nombre + ")" + ": Se conectó al servidor");
            //StartCoroutine(SendPing());
        };

        //Evento: Recibe mensaje de servidor (recibe bytes)
        websocket.OnMessage += (bytes) =>
        {
            string mensaje = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log("Mensaje recibido: " + mensaje);
            ConsolePrintln(mensaje);
        };

        // Evento: Se cierra la conexión
        websocket.OnClose += (mensaje) =>
        {
            conectado = false;
            ShowConnectButton();
            ConsolePrintln("Conexión cerrada");
            Debug.LogWarning("Conexión cerrada");
            StopAllCoroutines();
        };

        // Evento: Error en la conexión
        websocket.OnError += (mensaje) =>
        {
            conectado = false;
            ShowConnectButton();
            ConsolePrintln("Error al conectar al servidor: " + mensaje);
            Debug.LogWarning("Error al conectar con el servidor: " + mensaje);
        };


        // waiting for messages
        websocket.Connect();
    }

    async void Send(string _mensaje)
    {
        if (isConectionOpen())
        {
            // Sending plain text
            await websocket.SendText(_mensaje);
        }
    }

    private async void OnApplicationQuit()
    {
        await websocket.Close();
    }

    //Mantiene la conexión activa
    void SendPing()
    {
        tiempoComprobarConexion += Time.deltaTime;
        if (tiempoComprobarConexion >= 300)
        {
            Send("Ping");
            tiempoComprobarConexion = 0;
        }

    }

    bool isConectionOpen()
    {
        if (websocket != null)
        {
            if (websocket.State == WebSocketState.Open)
            {
                return true;
            }
        }
        return false;  
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
        if (websocket != null)
            websocket.Close();
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
        if (websocket != null)
        {
            Send("Mensaje de Prueba");
        }
    }

    public void QuitApp()
    {
        Application.Quit();
    }
    #endregion




    #region Consola
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
        consoleText.text = ".";

        // Obtiene la fecha y hora actual del sistema
        DateTime now = DateTime.Now;
        string currentHour = now.ToString("HH:mm:ss");
        messageList[messageList.Count - 1] = currentHour + ":  " + messageList[messageList.Count - 1];

        foreach (string msg in messageList)
        {
            // Agrega cada mensaje en una nueva línea
            consoleText.text += msg + "\n";
        }
    }
    //===============================================
    #endregion
}

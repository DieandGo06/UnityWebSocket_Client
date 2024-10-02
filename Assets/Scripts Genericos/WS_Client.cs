using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using System.Threading;
using WebSocketSharp;
using UnityEngine;
using TMPro;
using System;
using System.Threading.Tasks;



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
    public string MENSAJE;


    [Header("Consola")]
    [SerializeField] TextMeshProUGUI consoleText;
    [SerializeField] int maxLines;
    private List<string> messageList = new List<string>();


    [HideInInspector] public UnityEvent SeConecto;
    [HideInInspector] public UnityEvent<string> OnMessage;

    [HideInInspector] public UnityEvent<int> RecibeInt;
    [HideInInspector] public UnityEvent<float> RecibeFloat;



    void Awake()
    {
        Application.targetFrameRate = 60;
        RecibeInt = new UnityEvent<int>();
        RecibeFloat = new UnityEvent<float>();
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
        StartCoroutine (ConsolePrintAsync("AAAAAA"));
        OnMessage.AddListener(ConsolePrintln);
    }


    void Update()
    {
        ////Si no hay conexion
        if (ws != null)
        {

            //Evento: Recibe mensaje de servidor (Evento de libreria)
            ws.OnMessage += (sender, mensaje) =>
            {
                Debug.Log("Mensaje recibido: " + mensaje.Data);
                MENSAJE = mensaje.Data;
                Debug.Log(MENSAJE);
                StartCoroutine(ConsolePrintAsync("Hola"));
                ConsolePrintln("MENSAJE");
                //Debug.Log(mensaje.RawData);
            };
        }



        //ws.OnError += (sender, mensaje) =>
        //{
        //    Debug.LogWarning("Error del servidor");
        //};
    }



    IEnumerator ConsolePrintAsync(string data)
    {
        Debug.Log("FUNCIONA POR FAVOR 1");
        yield return new WaitForSeconds(0.5f);
        Debug.Log("FUNCIONA POR FAVOR 2");
        ConsolePrintln(data);
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
            ConsolePrintln("-------------------");

            StartCoroutine(SendPingRoutine());
            SeConecto.Invoke(); // Invoca el evento personalizado
            ws.Send("Unity (" + nombre + ")" + ": Se conectó al servidor");
        };

        //Evento: Recibe mensaje de servidor (Evento de libreria)
        ws.OnMessage += (sender, mensaje) =>
        {
            Debug.Log("Mensaje recibido: " + mensaje.Data);
            MENSAJE = mensaje.Data;

            byte[] mensajeBytes = mensaje.RawData; // tu byte array aquí
            string mensajeTexto = System.Text.Encoding.UTF8.GetString(mensajeBytes);
            ConsolePrintln(mensajeTexto);


            //Debug.Log(MENSAJE);
            //StartCoroutine(ConsolePrintAsync("Hola"));
            //ConsolePrintln("MENSAJE");
            //OnMessage.Invoke(MENSAJE);
            //Debug.Log(mensaje.RawData);
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
        while (ws != null)
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

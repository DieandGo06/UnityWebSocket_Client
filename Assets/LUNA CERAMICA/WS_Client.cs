using UnityEngine.Video;
using WebSocketSharp;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WS_Client : MonoBehaviour
{
    WebSocket ws;
    public string nombre;
    public string serverUrl;
    public GameObject boton_conectar;
    public GameObject inputName;
    public GameObject inputIP;
    public AudioSource audio1, audio2, audio3;





    void Start()
    {
        Application.targetFrameRate = 30;
    }

    void Update()
    {
        //Documentacion de la libreria "WebSocketSharp": https://github.com/PingmanTools/websocket-sharp/
        //Hay conexión con el servidor
        if (ws != null)
        {

            //Evento: Recibe mensaje de servidor (Evento de libreria)
            ws.OnMessage += (sender, mensaje) =>
            {
                Debug.Log("Mensaje recibido: " + mensaje.Data);
                
            };

            //Evento: Hubo un error en la conexión (Evento de libreria)
            ws.OnError += (sender, mensaje) =>
            {
                //Debug.LogWarning("No se conectar o recibri data del servidor, prueba reiniciando el servidor");
            };

            //Evento: Se pierde la conexión (Evento de libreria)
            ws.OnClose += (sender, e) =>
            {

            };

            HideUI();


        }
        //No hay conexión con el servidor
        else
        {
            ShowUI();
        }

        if (Input.GetKeyDown("1")) ws.Send("play 1");
        if (Input.GetKeyDown("2")) ws.Send("stop 1");


    }


    //Cuando se presiona el boton "Conectar" -----------------------------------------------------------------
    public void ConectToServer()
    {
        //"on" recibe y "Send" envia
        ws = new WebSocket(serverUrl);
        ws.OnMessage += (sender, e) =>
        {
            //interferenciaValue = float.Parse(e.Data) / 100;
            //Debug.Log("Mensaje recbido de: " + ((WebSocket)sender).Url + " Data: " + e.Data);
        };
        ws.Connect();
        ws.Send(nombre + ("Unity"));
    }

    public void ReadTextInputIP(string t)
    {
        serverUrl = "ws://" + t + ":3000";
        Debug.Log("La conexión se hará en la IP: " + t);
    }

    public void ReadTextInputName(string t)
    {
        nombre = t;
        Debug.Log("El nombre de este dispositivo será: " + t);
    }

    void HideUI()
    {
        boton_conectar.SetActive(false);
        inputName.SetActive(false);
        inputIP.SetActive(false);
    }

    void ShowUI()
    {
        boton_conectar.SetActive(true);
        inputName.SetActive(true);
        inputIP.SetActive(true);
    }
}

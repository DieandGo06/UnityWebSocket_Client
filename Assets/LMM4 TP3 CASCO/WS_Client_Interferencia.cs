using UnityEngine.Video;
using WebSocketSharp;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WS_Client_Interferencia : MonoBehaviour
{
    WebSocket ws;
    public string nombre;
    public string serverUrl;
    public GameObject boton_conectar;
    public GameObject inputName;
    public GameObject inputIP;
    public VideoPlayer video;
    public VideoPlayer interferencia;
    public RawImage lienzoInterferencia;


    public float interferenciaValue; 






    void Start()
    {
        Application.targetFrameRate = 30;
    }

    void Update()
    {
        if (ws != null)
        {
            boton_conectar.SetActive(false);
            inputName.SetActive(false);
            inputIP.SetActive(false);

            video.SetDirectAudioVolume(0, 1-interferenciaValue);
            interferencia.SetDirectAudioVolume(0, interferenciaValue);

            Color transparencia = Color.white;
            transparencia.a = interferenciaValue;
            lienzoInterferencia.color = transparencia;
        }
        else
        {
            boton_conectar.SetActive(true);
            inputName.SetActive(true);
            inputIP.SetActive(true);
        }
    }

    public void ConectToServer()
    {
        //"on" recibe y "Send" envia
        ws = new WebSocket(serverUrl);
        ws.OnMessage += (sender, e) =>
        {
            interferenciaValue = float.Parse(e.Data) / 100;
            //Debug.Log("Mensaje recbido de: " + ((WebSocket)sender).Url + " Data: " + e.Data);
            //Debug.Log("Interferencia: " + e.Data);
            //Debug.Log(interferenciaValue);
        };
        ws.Connect();
        ws.Send(nombre);

        video.Play();
        interferencia.Play();
    }

    public void ReadTextInputIP(string t)
    {
        serverUrl = "ws://" + t + ":3000";
        Debug.Log(t);
    }

    public void ReadTextInputName(string t)
    {
        nombre = t;
        Debug.Log(t);
    }
}

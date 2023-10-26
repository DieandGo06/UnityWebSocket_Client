using UnityEngine.Video;
using WebSocketSharp;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WS_Client_Ceramica : MonoBehaviour
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
        if (ws != null)
        {
            boton_conectar.SetActive(false);
            inputName.SetActive(false);
            inputIP.SetActive(false);
        }
        else
        {
            boton_conectar.SetActive(true);
            inputName.SetActive(true);
            inputIP.SetActive(true);
        }


#if UNITY_STANDALONE
        if (Input.GetKeyDown("1")) ws.Send("play 1");
        if (Input.GetKeyDown("2")) ws.Send("stop 1");

        if (Input.GetKeyDown("3")) ws.Send("play 2");
        if (Input.GetKeyDown("4")) ws.Send("stop 2");

        if (Input.GetKeyDown("5")) ws.Send("play 3");
        if (Input.GetKeyDown("6")) ws.Send("stop 3");
#endif





        if (ws != null)
        {
            ws.OnMessage += (sender, e) =>
            {
                //interferenciaValue = float.Parse(e.Data) / 100;
                //Debug.Log("Mensaje recbido de: " + ((WebSocket)sender).Url + " Data: " + e.Data);

                //#if UNITY_ANDROID
                if (e.Data == "play 1" && !audio1.isPlaying)
                {
                    Debug.Log("play 1");
                    audio1.Play();
                }
                if (e.Data == "play 2" && !audio2.isPlaying)
                {
                    audio2.Play();
                }
                if (e.Data == "play 3" && !audio3.isPlaying)
                {
                    audio3.Play();
                }

                if (e.Data == "stop 1" && audio1.isPlaying)
                {
                    audio1.Stop();
                }
                if (e.Data == "stop 2" && audio2.isPlaying)
                {
                    audio2.Stop();
                }
                if (e.Data == "stop 3" && audio3.isPlaying)
                {
                    audio3.Stop();
                }
                //#endif
            };
        }
            
    }

    public void ConectToServer()
    {
        //"on" recibe y "Send" envia
        ws = new WebSocket(serverUrl);
        ws.OnMessage += (sender, e) =>
        {
            //interferenciaValue = float.Parse(e.Data) / 100;
            //Debug.Log("Mensaje recbido de: " + ((WebSocket)sender).Url + " Data: " + e.Data);

            //#if UNITY_ANDROID
            if (e.Data == "play 1" && !audio1.isPlaying)
            {
                Debug.Log("play 1");
                audio1.Play();
            }
            if (e.Data == "play 2" && !audio2.isPlaying)
            {
                audio2.Play();
            }
            if (e.Data == "play 3" && !audio3.isPlaying)
            {
                audio3.Play();
            }

            if (e.Data == "stop 1" && audio1.isPlaying)
            {
                audio1.Stop();
            }
            if (e.Data == "stop 2" && audio2.isPlaying)
            {
                audio2.Stop();
            }
            if (e.Data == "stop 3" && audio3.isPlaying)
            {
                audio3.Stop();
            }
            //#endif
        };
        ws.Connect();
        ws.Send(nombre);
        //video.Play();
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoInSteamingAssets : MonoBehaviour
{
    [SerializeField]
    public string videoFileName;



    void Awake()
    {
        var videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);
        //videoPlayer.Play();
    }

}

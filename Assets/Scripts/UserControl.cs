using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;
using System.Text.RegularExpressions;

public class UserControl : MonoBehaviour
{
    [SerializeField] string testPath;
    [SerializeField] AudioSource audioSource;
    bool doLoop = true;

    // Start is called before the first frame update
    void Start()
    {
        LoadWav(testPath);
    }

    public void LoadWav(string wavPath)
    {
        // wavPath = @"C:\Users\user\Desktop\私達も恋した幻想郷.wav";

        // パスが空の場合は何もしない
        if (string.IsNullOrEmpty(wavPath))
        {
            Debug.Log("Invalid Filepath Error");
            return;
        }

        // ファイルパスの"を除去
        wavPath = Regex.Replace(wavPath, "\"", "");
        // Debug.Log(wavPath);

        if (!File.Exists(wavPath))
        {
            Debug.Log("File Not Found Error");
            return;
        }

        if (!Path.GetExtension(wavPath).Equals(".wav"))
        {
            Debug.Log("File Extension Error");
            return;
        }
        else
        {
            if (audioSource.clip != null)
            {
                audioSource.Stop();
                // audioSource.clip = null;
            }
            StartCoroutine(LoadWavCoroutine(wavPath));
        }
    }

    IEnumerator LoadWavCoroutine(string wavPath)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + wavPath, AudioType.WAV))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(www.error);
            }
            else
            {
                AudioClip audioClip = DownloadHandlerAudioClip.GetContent(www);
                Debug.Log(audioClip);
                audioSource.clip = audioClip;
                audioSource.Play();
            }
        }
    }
}

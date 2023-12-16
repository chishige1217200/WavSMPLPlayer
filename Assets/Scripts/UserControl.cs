using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor.Timeline;

public class UserControl : MonoBehaviour
{
    [SerializeField] string testPath;
    [SerializeField] AudioSource audioSource;
    [SerializeField] Button playButton;
    [SerializeField] Button pauseButton;
    [SerializeField] Button stopButton;
    [SerializeField] Button loopButton;
    [SerializeField] Slider timeSlider;
    [SerializeField] Slider volumeSlider;
    [SerializeField] Text timeText;
    [SerializeField] Text volumeText;
    [SerializeField] InputField pathField;
    bool isPaused = false;
    bool timeSliderIsDown = false;

    // Start is called before the first frame update
    void Start()
    {
        volumeSlider.value = audioSource.volume * 100f;
        volumeText.text = volumeSlider.value.ToString();
        UpdateLoopColor();
        // LoadWav(testPath);
    }

    // Update is called once per frame
    void Update()
    {
        if (audioSource.clip != null && audioSource.isPlaying)
        {
            UpdateTime();
        }

        if (!audioSource.isPlaying && !timeSliderIsDown && !isPaused)
        {
            StopButton_Click();
        }
    }

    void UpdateTime()
    {
        if (audioSource.clip != null)
        {
            // 全体時間算出
            int a_minutes = (int)(audioSource.clip.length / 60);
            int a_seconds = (int)(audioSource.clip.length % 60);
            // 現在時間算出
            int p_minutes = (int)(audioSource.time / 60);
            int p_seconds = (int)(audioSource.time % 60);

            // 時間表示更新
            string timetext = p_minutes.ToString("D2") + ":" + p_seconds.ToString("D2") + " / " + a_minutes.ToString("D2") + ":" + a_seconds.ToString("D2");
            timeText.text = timetext;

            // スライダー更新
            timeSlider.value = audioSource.time / audioSource.clip.length;
        }
    }

    private void ResetUI()
    {
        UpdatePlayButton();
        UpdateTime();
        timeSlider.value = 0;
    }

    public void PlayButton_Click()
    {
        if (audioSource.clip != null)
        {
            if (audioSource.time == 0)
            {
                audioSource.Play();
                isPaused = false;
                UpdatePlayButton();
                return;
            }

            audioSource.UnPause();
            isPaused = false;
            UpdatePlayButton();
        }
    }

    public void PauseButton_Click()
    {
        if (audioSource.clip != null)
        {
            audioSource.Pause();
            isPaused = true;
            UpdatePlayButton();
        }
    }

    private void UpdatePlayButton()
    {
        if (audioSource.isPlaying)
        {
            playButton.gameObject.SetActive(false);
            pauseButton.gameObject.SetActive(true);
        }
        else
        {
            pauseButton.gameObject.SetActive(false);
            playButton.gameObject.SetActive(true);
        }
    }

    public void StopButton_Click()
    {
        if (audioSource.clip != null)
        {
            audioSource.Stop();
            isPaused = true;
            ResetUI();
        }
    }

    public void LoopButton_Click()
    {
        audioSource.loop = !audioSource.loop;
        UpdateLoopColor();
    }

    private void UpdateLoopColor()
    {
        if (audioSource.loop)
        {
            loopButton.targetGraphic.color = new Color(0.8f, 0.89f, 0.97f);
        }
        else
        {
            loopButton.targetGraphic.color = new Color(1, 1, 1);
        }
    }

    public void SetVolume()
    {
        audioSource.volume = volumeSlider.value / 100f;
        volumeText.text = volumeSlider.value.ToString();
    }

    public void TimeSlider_PointerDown()
    {
        timeSliderIsDown = true;
        if (!isPaused)
        {
            audioSource.Pause();
        }
    }

    public void TimeSlider_PointerUp()
    {
        timeSliderIsDown = false;
        if (!isPaused)
        {
            audioSource.UnPause();
        }
    }

    public void TimeSlider_ValueChanged()
    {
        if (audioSource.clip != null)
        {
            audioSource.time = audioSource.clip.length * timeSlider.value;
        }
    }

    public void PathField_EndEdit()
    {
        LoadWav(pathField.text);
    }

    private void LoadWav(string wavPath)
    {
        pathField.targetGraphic.color = new Color(1, 1, 1);

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
                isPaused = true;
                ResetUI();
                timeText.text = "00:00 / 00:00";
            }
            StartCoroutine(LoadWavCoroutine(wavPath));
        }
    }

    private IEnumerator LoadWavCoroutine(string wavPath)
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
                isPaused = false;
                UpdatePlayButton();
                pathField.targetGraphic.color = new Color(0.8f, 0.89f, 0.97f);
            }
        }
    }
}

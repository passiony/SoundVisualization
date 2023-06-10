using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MicrophoneListener : MonoBehaviour
{
    public GameObject[] obj;

    public bool startMicOnStartup = true;
    public bool stopMicrophoneListener = false;
    public bool startMicrophoneListener = false;
    private bool microphoneListenerOn = false;

    AudioSource src;
    float timeSinceRestart = 0;
    public float threshold = 5;
    private float[] spectrumData = new float[128];

    void Start()
    {
        if (startMicOnStartup)
        {
            RestartMicrophoneListener();
            StartMicrophoneListener();
        }
    }
    void Update()
    {
        if (stopMicrophoneListener)
        {
            StopMicrophoneListener();
        }
        if (startMicrophoneListener)
        {
            StartMicrophoneListener();
        }

        stopMicrophoneListener = false;
        startMicrophoneListener = false;

        //创建Microphone实例
        MicrophoneIntoAudioSource(microphoneListenerOn);
        //处理音频
        ProcessSound();
        //处理波峰
        ShowPeaks();
    }
    //停止录制
    public void StopMicrophoneListener()
    {
        microphoneListenerOn = false;
        src.Stop();
        src.clip = null;

        Microphone.End(null);
    }
    //开始录制
    public void StartMicrophoneListener()
    {
        microphoneListenerOn = true;
        RestartMicrophoneListener();
    }

    //处理声音数据，通过FFT算法
    public void ProcessSound()
    {
        src.GetSpectrumData(spectrumData, 0, FFTWindow.BlackmanHarris);

        for (int i = 0; i < 128; i++)
        {
            var y = spectrumData[i] * 10000;
            y = Mathf.Clamp(y, 0, 10);
            obj[i].gameObject.transform.localScale = new Vector3(0.3f, y, 0.5f); //将可视化的物体和音波相关联
            
            //设定阈值，低于代表环境音，可以忽略
            if (y < threshold)
            {
                spectrumData[i] = 0;
            }
        }
    }

    void ShowPeaks()
    {
        for (int i = 0; i < 128; i++)
        {
            var y = spectrumData[i] * 10000;
            //设定阈值，低于代表环境音，可以忽略
            if (y < threshold)
            {
                spectrumData[i] = 0;
            }
        }
        
        //计算波峰数量
        float total = 0;
        var peaks = FindPeaks.getPeaks(spectrumData);
        for (int i = 0; i < peaks.Length; i++)
        {
            total += spectrumData[peaks[i]];
        }
        obj[127].transform.localScale = new Vector3(0.3f, peaks.Length, 0.5f);
    }

    public void RestartMicrophoneListener()
    {
        src = GetComponent<AudioSource>();
        src.clip = null;
        timeSinceRestart = Time.time;
    }

    void MicrophoneIntoAudioSource(bool MicrophoneListenerOn)
    {
        if (MicrophoneListenerOn)
        {
            if (Time.time - timeSinceRestart > 0.5f && !Microphone.IsRecording(null))
            {
                src.clip = Microphone.Start(null, true, 999, 44100);
                while (!(Microphone.GetPosition(null) > 0)){}
                src.Play(); // Play the audio source
            }
        }
    }
    
    private void OnAudioFilterRead(float[] data, int channels)
    {
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = 0;
        }
    }
}
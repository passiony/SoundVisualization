using System;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MicrophoneListener : MonoBehaviour
{
    private AudioSource m_AudioSouce; //声音播放器
    private float[] spectrumData = new float[512]; //存储音频的fft数据
    private float[] _fragBand = new float[8]; //存储8通达的频率数据

    [SerializeField] private VoiceVisualize m_visualize;

    private void Awake()
    {
        m_AudioSouce = GetComponent<AudioSource>();
        m_AudioSouce.clip = null;
    }

    async void Start()
    {
        while (true)
        {
            StartMicrophoneListener(); //开始录音
            await Task.Delay(TimeSpan.FromSeconds(30));
            StopMicrophoneListener(); //停止录音
            await Task.Delay(TimeSpan.FromMilliseconds(10));
        }
    }

    void Update()
    {
        //处理音频
        ProcessSound();
        //声音可视化
        m_visualize?.SetFraguency(_fragBand);
    }

    //停止录制
    public void StopMicrophoneListener()
    {
        m_AudioSouce.Stop();
        m_AudioSouce.clip = null;

        Microphone.End(null);
    }

    //开始录制
    public void StartMicrophoneListener()
    {
        m_AudioSouce.Stop();
        m_AudioSouce.clip = Microphone.Start(null, true, 360, 44100);
        while (!(Microphone.GetPosition(null) > 0))
        {
        }

        m_AudioSouce.Play(); // Play the audio source
    }

    //处理声音数据，通过FFT算法
    public void ProcessSound()
    {
        m_AudioSouce.GetSpectrumData(spectrumData, 0, FFTWindow.BlackmanHarris);

        int count = 0;
        for (int i = 0; i < 8; i++)
        {
            float average = 0;
            int sampleCount = (int)Mathf.Pow(2, i) * 2;
            if (i == 7)
            {
                sampleCount += 2;
            }

            for (int j = 0; j < sampleCount; j++)
            {
                average += spectrumData[count] * (count + 1);
                count++;
            }

            average /= count;
            _fragBand[i] = average * 1000;
        }
    }

    //声音后处理生命周期
    private void OnAudioFilterRead(float[] data, int channels)
    {
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = 0;
        }
    }
}
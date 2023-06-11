using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

[RequireComponent(typeof(AudioSource))]
public class MicrophoneListener : MonoBehaviour
{
    public bool startMicOnStartup = true;
    public bool stopMicrophoneListener = false;
    public bool startMicrophoneListener = false;
    private bool microphoneListenerOn = false;

    private AudioSource m_AudioSouce;
    private float timeSinceRestart = 0;
    private float[] spectrumData = new float[512];
    private float[] _fragBand = new float[8];

    public GameObject[] obj;
    public int scaleMultiplier = 100;
    public float threhold = 10;
    public int channelCount = 1;
    [FormerlySerializedAs("frames")] public int frameCount = 60;
    public UnityEvent<int[]> FrenquencyEvent;

    private int[] frenquncies = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
    private int frame = 0;

    void Start()
    {
        Application.targetFrameRate = 60;
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
        ShowBox();

        var arr = FindTopTwoIndices(_fragBand, threhold);
        for (int i = 0; i < channelCount; i++)
        {
            if (arr[i] > -1)
                frenquncies[arr[i]]++;
        }

        frame++;
        if (frame >= frameCount)
        {
            frame = 0;
            int[] arr2 = FindTopTwoIndices(frenquncies);
            for (int i = 0; i < frenquncies.Length; i++)
            {
                frenquncies[i] = 0;
            }

            FrenquencyEvent?.Invoke(arr2);
        }
    }

    //停止录制
    public void StopMicrophoneListener()
    {
        microphoneListenerOn = false;
        m_AudioSouce.Stop();
        m_AudioSouce.clip = null;

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
            _fragBand[i] = average * 10;
        }
    }

    void ShowBox()
    {
        for (int i = 0; i < 8; i++)
        {
            var y = _fragBand[i] * scaleMultiplier;
            obj[i].gameObject.transform.localScale = new Vector3(0.3f, y, 0.5f); //将可视化的物体和音波相关联
        }
    }

    int[] FindTopTwoIndices(float[] arr, float threhold)
    {
        int maxIndex = -1;
        int secondMaxIndex = -1;
        float maxValue = int.MinValue;
        float secondMaxValue = int.MinValue;

        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i] > maxValue && arr[i] * scaleMultiplier > threhold)
            {
                secondMaxValue = maxValue;
                secondMaxIndex = maxIndex;
                maxValue = arr[i];
                maxIndex = i;
            }
            else if (arr[i] > secondMaxValue && arr[i] * scaleMultiplier > threhold)
            {
                secondMaxValue = arr[i];
                secondMaxIndex = i;
            }
        }

        return new[] { maxIndex, secondMaxIndex };
    }

    static int[] FindTopTwoIndices(int[] arr)
    {
        int maxIndex = -1;
        int secondMaxIndex = -1;
        int maxValue = int.MinValue;
        int secondMaxValue = int.MinValue;

        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i] > maxValue)
            {
                secondMaxValue = maxValue;
                secondMaxIndex = maxIndex;
                maxValue = arr[i];
                maxIndex = i;
            }
            else if (arr[i] > secondMaxValue)
            {
                secondMaxValue = arr[i];
                secondMaxIndex = i;
            }
        }

        return new[] { maxIndex, secondMaxIndex };
    }

    public void RestartMicrophoneListener()
    {
        m_AudioSouce = GetComponent<AudioSource>();
        m_AudioSouce.clip = null;
        timeSinceRestart = Time.time;
    }

    void MicrophoneIntoAudioSource(bool MicrophoneListenerOn)
    {
        if (MicrophoneListenerOn)
        {
            if (Time.time - timeSinceRestart > 0.5f && !Microphone.IsRecording(null))
            {
                m_AudioSouce.clip = Microphone.Start(null, true, 999, 44100);
                while (!(Microphone.GetPosition(null) > 0))
                {
                }

                m_AudioSouce.Play(); // Play the audio source
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
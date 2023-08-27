using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSourceListener : MonoBehaviour
{
    [SerializeField]  private AudioSource m_AudioSouce; //声音播放器
    [SerializeField] private VoiceVisualize m_visualize;

    private float[] spectrumData = new float[512]; //存储音频的fft数据
    private float[] _fragBand = new float[8]; //存储8通达的频率数据

    void Start()
    {
        Application.targetFrameRate = 60;
        m_AudioSouce.Play();
    }

    void Update()
    {
        //处理音频
        ProcessSound();
        
        //声音可视化
        m_visualize?.SetFraguency(_fragBand);
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
}
using UnityEngine;
public class VoiceVisualize : MonoBehaviour
{
    //可视化缩放强度
    public int m_Scaler = 2;
    //声音可视化游戏对象
    private Transform[] cubes; 
    //存储8通达的频率数据
    private float[] fragBand = new float[8];

    private void Awake()
    {
        cubes = new Transform[8];
        for (int i = 0; i < 8; i++)
        {
            cubes[i] = transform.GetChild(i);
        }
    }

    public void SetFraguency(float[] frags)
    {
        for (int i = 0; i < 8; i++)
        {
            fragBand[i] = frags[i] * m_Scaler;
        }
    }

    void Update()
    {
        //声音可视化
        for (int i = 0; i < 8; i++)
        {
            var y = fragBand[i];
            //将可视化的物体和音波相关联
            cubes[i].localScale = Vector3.Lerp(cubes[i].localScale, new Vector3(1, y, 1), 0.2f); 
        }
    }
}
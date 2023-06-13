using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

[Serializable]
public class AnimalInfo
{
    public int frenquncy;
    public GameObject go;
    public int velocity = 1;
}

public class BirdFly : MonoBehaviour
{
    public AnimalInfo[] m_Animals;

    private MicrophoneListener listener;
    private Dictionary<int, AnimalInfo> m_AnimalDic;
    private int[] lastArgs = new int[]{0,0};

    void Awake()
    {
        listener = gameObject.GetComponent<MicrophoneListener>();
        listener.FrenquencyEvent.AddListener(OnFrenquencyEvent);

        m_AnimalDic = new Dictionary<int, AnimalInfo>();
        foreach (var animal in m_Animals)
        {
            m_AnimalDic.Add(animal.frenquncy, animal);
        }
    }

    // private void OnFrenquencyEvent(int[] args)
    // {
    //     Debug.Log($"OnFrenquencyEvent : {args[0]},{args[1]}");
    //
    //     var arg0 = args[0];
    //     var lastArg0 = lastArgs[0];
    //     
    //     if (m_AnimalDic.ContainsKey(lastArg0) || m_AnimalDic.ContainsKey(arg0))
    //     {
    //         if (arg0 == lastArg0) //长走
    //         {
    //             if (m_AnimalDic.TryGetValue(arg0, out AnimalInfo animal))
    //             {
    //                 var x = animal.go.transform.position.x;
    //                 animal.go.transform.DOMoveX(x + animal.velocity, 1);
    //             }
    //         }
    //         else //短跳
    //         {
    //             if (m_AnimalDic.TryGetValue(arg0, out AnimalInfo animal))
    //             {
    //                 var vector = animal.go.transform.position;
    //                 vector.y += 1;
    //                 animal.go.transform.DOJump(vector,1,1, 1);
    //             }
    //         }
    //     }
    //     
    //     lastArgs[0] = args[0];
    // }
    
    private void OnFrenquencyEvent(int[] args)
    {
        foreach (var pair in m_AnimalDic)
        {
            if (lastArgs.Contains(pair.Key) && args.Contains(pair.Key))//长音
            {
                var animal = pair.Value;
                var x = animal.go.transform.position.x;
                animal.go.transform.DOMoveX(x + animal.velocity, 1);
            }
            else if(args.Contains(pair.Key))//短音
            {
                var animal = pair.Value;
                var vector = animal.go.transform.position;
                vector.y += 1;
                animal.go.transform.DOJump(vector,1,1, 1);
            }
        }
        
        for (int i = 0; i < args.Length; i++)
        {
            lastArgs[i] = args[i];
        }
    }
}
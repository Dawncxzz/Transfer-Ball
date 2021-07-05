using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour
{
    public static HealthController instance { get; private set; }
    public Image mask;
    float eachSize = 150f;

    //调用的时候默认只有这个唯一的生命值组件
    void Awake()
    {
        instance = this;
    }

    void Start()
    {

    }

    //设置mask大小，显示生命值
    public void SetValue(int value)
    {
        mask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value * eachSize);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour
{
    public static HealthController instance { get; private set; }
    public Image mask;
    float eachSize = 150f;

    //���õ�ʱ��Ĭ��ֻ�����Ψһ������ֵ���
    void Awake()
    {
        instance = this;
    }

    void Start()
    {

    }

    //����mask��С����ʾ����ֵ
    public void SetValue(int value)
    {
        mask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value * eachSize);
    }

}

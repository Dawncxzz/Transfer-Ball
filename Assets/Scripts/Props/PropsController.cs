using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropsController : MonoBehaviour
{
    public GameObject props;
    Animator animator;

    //״̬��ʼ��
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        
    }

    //����������ײ���
    public void OnTriggerEnter2D(Collider2D collision) 
    {
        //�ж���ײ���Ƿ���player��Ȼ��ִ��
        PlayerController picker = collision.GetComponent<PlayerController>();
        Debug.Log(collision + ":" + picker);
        if (picker != null)
        {
            picker.PickUp(props);   
            Destroy(gameObject);
        }
    }
}

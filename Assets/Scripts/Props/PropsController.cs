using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropsController : MonoBehaviour
{
    public GameObject props;
    Animator animator;

    //状态初始化
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        
    }

    //触发器的碰撞检测
    public void OnTriggerEnter2D(Collider2D collision) 
    {
        //判断碰撞的是否是player，然后执行
        PlayerController picker = collision.GetComponent<PlayerController>();
        Debug.Log(collision + ":" + picker);
        if (picker != null)
        {
            picker.PickUp(props);   
            Destroy(gameObject);
        }
    }
}

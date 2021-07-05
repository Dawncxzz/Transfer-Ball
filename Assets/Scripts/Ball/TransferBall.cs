using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransferBall : MonoBehaviour
{
    [SerializeField]
    protected float fritionCoe = 0.5f;
    [SerializeField]
    protected GameObject playerPrefab;

    protected bool isLauch;
    protected bool isonGround;
    protected bool preTransfer;

    protected RaycastHit2D canTransfer;
    
    protected Rigidbody2D rigidbody2d;
    protected PlayerController player;
    protected RectTransform playerTransform;
    protected CircleCollider2D ballCollider2d;
    protected BoxCollider2D playerCollider2d;
    protected Animator animator;
    protected RectTransform rectTransform;

    //状态初始化
    public void Start()
    {
        if (isLauch || GetComponentInParent<PlayerController>() != null)
        {
            animator.enabled = false;
            ballCollider2d.isTrigger = false;
        }
    }

    //实例化后调用
    void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        ballCollider2d = GetComponent<CircleCollider2D>();
        animator = GetComponent<Animator>();
        rectTransform = GetComponent<RectTransform>();
    }

    //状态控制函数
    void Update()
    {
        //状态转化
        if (isLauch)
        {
            if (Input.GetKeyDown(KeyCode.Space) || preTransfer)
            {
                Transfer();
            }
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Destroy(gameObject);
            }
        }
        //相对父物体（player）移动
        if (GetComponentInParent<PlayerController>() != null)
        {
            PlayerController parentPlayer = GetComponentInParent<PlayerController>();
            float offset = parentPlayer.lookDirection.x * 0.2f;
            transform.position = parentPlayer.transform.position + new Vector3(offset, -0.25f, 0);
            rigidbody2d.freezeRotation = true;
        }

        
    }
    
    //运动控制函数
    private void FixedUpdate()
    {
        //地面判断，滚动摩擦
        if (isonGround)
        {
            float friction = rigidbody2d.mass * rigidbody2d.gravityScale * fritionCoe / ballCollider2d.radius;
            float direction;
            if (rigidbody2d.velocity.x > 0)
            {
                direction = 1;
            }
            else if (rigidbody2d.velocity.x < 0)
            {
                direction = -1;
            }
            else
            {
                direction = 0;
            }
            rigidbody2d.AddForce(Vector2.right * friction * -direction);
        }
    }

    //发射函数
    public void Lauch (Vector2 direction,float force,PlayerController player)
    {
        rigidbody2d.velocity = direction * force;
        this.player = player;
        playerTransform = player.GetComponent<RectTransform>();
        playerCollider2d = player.GetComponent<BoxCollider2D>();
        isLauch = true;
    }

    //传送函数
    public virtual void Transfer()
    {
        //获取球体半径
        float radius = ballCollider2d.radius;
        //传送检测（当球体位置无法容纳player时）
        canTransfer = Physics2D.Raycast(rigidbody2d.position + Vector2.down * radius * 0.5f,
            Vector2.up,
            playerCollider2d.size.y,
            LayerMask.GetMask("Map"));
        
        if (canTransfer.collider == null)
        {
            //玩家位置传送
            float height = playerCollider2d.size.y;
            Vector2 newPosition = rigidbody2d.position + Vector2.down * Vector2.down * radius * 0.5f
                + Vector2.up * height * 0.5f;
            player.SetisDown(true);
            player.SetisJump(true);
            player.SetProTransfer(true);
            playerTransform.position = newPosition;
            Destroy(gameObject);
        }
    }

    public void setActive(bool isActive) 
    {
        gameObject.SetActive(isActive);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        //地面碰撞检测（使用射线）
        if (isLauch)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position,
                Vector2.down,
                2.0f,
                LayerMask.GetMask("Map"));
            if (hit)
            {
                isonGround = true;
            }
        }
    }
}

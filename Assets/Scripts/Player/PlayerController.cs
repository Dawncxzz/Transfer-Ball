using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed = 6f;
    [SerializeField]
    private float jumpVelocity = 15f;
    [SerializeField]
    private float throwVelocity = 25f;
    [SerializeField]
    private int currentHealth = 3;
    [SerializeField]
    private int maxHealth = 5;
    [SerializeField]
    private GameObject transferBallPrefab;
    [SerializeField]
    private float timeInvincible = 1.0f;

    public int health { get { return currentHealth; } }
    public Vector2 lookDirection;
    private bool isJump;
    private bool isDown;
    private TransferBall[] ballList = new TransferBall[3];
    private int currentBall = 0;
    private bool isInvincible;
    private float invincibleTimer;
    private bool canMove = true; //无敌时间落地判断专用

    private float transferTime = 0.7f;
    private bool proTransfer;

    Rigidbody2D rigidbody2d;
    Animator animator;
    Material material;

    // 参数初始化
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        material = GetComponent<SpriteRenderer>().material;
        //人物一开始向右看
        lookDirection = new Vector2(1, 0);
        animator.SetFloat("LookDirection", lookDirection.x);
        //存储子物体到传送球列表中
        ballList[0] = GetComponentInChildren<TransferBall1>();
    }

    // 人物状态变换专用函数
    void Update()
    {
        //人物方向设置
        animator.SetFloat("Speed", Mathf.Abs(Input.GetAxis("Horizontal") * speed));
        animator.SetFloat("Move X", Input.GetAxis("Horizontal") >= 0 ? 1 : -1);

        //传送后的着色器变化
        if (proTransfer)
        {
            transferTime -= Time.deltaTime;
            if (transferTime > 0)
            {
                material.SetFloat("_Teleportation", 0.7f - transferTime);
            }
            else
            {
                SetProTransfer(false);
                transferTime = 0.7f;
            }
        }

        //人物无敌状态
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
            {
                isInvincible = false;
            }
        }

        //人物站立动画方向设置（各种状态转化为站立状态时执行）
        if (!Mathf.Approximately(Mathf.Abs(Input.GetAxis("Horizontal")) * speed, 0.000f))
        {
            lookDirection = Input.GetAxis("Horizontal") > 0 ? new Vector2(1, 0) : new Vector2(-1, 0);
            animator.SetFloat("LookDirection", lookDirection.x);
        }

        //站立状态转跳跃状态
        if (Input.GetKeyDown(KeyCode.W) && !isJump && !isDown)
        {
            //距离顶部最小跳跃距离检测
            RaycastHit2D leastH = Physics2D.Raycast(rigidbody2d.position + GetComponent<BoxCollider2D>().size * 0.5f,
                Vector2.up,
                0.2f,
                LayerMask.GetMask("Map"));
            if (leastH.collider == null)
            {
                SetisJump(true);
                rigidbody2d.velocity = Vector2.up * jumpVelocity;
                animator.SetTrigger("Jump_up");
            }
        }
        
        //跳跃动画转下降状态
        if (isJump && rigidbody2d.velocity.y <= 0) 
        {
            SetisJump(false);
            SetisDown(true);
            animator.SetTrigger("Jump_down");
        }

        //发射传送球
        if (Input.GetKeyDown(KeyCode.Mouse0)) 
        {
            Vector2 lanuchPosition = rigidbody2d.position + Vector2.down * 0.25f + lookDirection * 0.2f;
            GameObject transferBallObject = Instantiate(transferBallPrefab, lanuchPosition,
                Quaternion.identity);
            TransferBall transferBall = transferBallObject.GetComponent<TransferBall>();
            Vector2 mousePositionOnScreen = Input.mousePosition;
            Vector2 mousePositionInWorld = Camera.main.ScreenToWorldPoint(mousePositionOnScreen);
            transferBall.Lauch((mousePositionInWorld - lanuchPosition).normalized, throwVelocity, GetComponent<PlayerController>());
        }

        //传送球切换
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ChangeBall();
        }
    }

    //人物移动专用函数
    private void FixedUpdate()
    {
        if (canMove)
        {
            rigidbody2d.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, rigidbody2d.velocity.y);
        }
    }

    //碰撞检测函数
    public void OnCollisionEnter2D(Collision2D collision)
    {
        //跳跃状态转为地面状态
        if (isDown) {
            animator.SetFloat("Speed", Mathf.Abs(Input.GetAxis("Horizontal") * speed));
            animator.SetFloat("Move X", Input.GetAxis("Horizontal") >= 0 ? 1 : -1);
            animator.ResetTrigger("Jump_down");
            animator.SetTrigger("OnGround");
            SetisDown(false);
        }
        if (isInvincible)
        {
            canMove = true;
        }
    }

    //捡到传送球时传入到传送球列表中
    public void PickUp(GameObject props) 
    {
        //较蠢的存储方法
        if (props.GetComponent<TransferBall1>() != null && ballList[0] == null)
        {
            GameObject child = Instantiate(props);
            ballList[0] = child.GetComponent<TransferBall1>();
            child.transform.SetParent(gameObject.transform);
            child.SetActive(false);
        }
        if (props.GetComponent<TransferBall2>() != null && ballList[1] == null)
        {
            GameObject child = Instantiate(props);
            ballList[1] = child.GetComponent<TransferBall2>();
            child.transform.SetParent(gameObject.transform);
            child.SetActive(false);
        }
        if (props.GetComponent<TransferBall3>() != null && ballList[2] == null)
        {
            GameObject child = Instantiate(props);
            ballList[2] = child.GetComponent<TransferBall3>();
            child.transform.SetParent(gameObject.transform);
            child.SetActive(false);
        }
        if (props.gameObject.name.Substring(0,6) == "Health") {
            ChangeHealth(1);
        }
    }

    //武器切换函数
    public void ChangeBall() 
    {
        ballList[currentBall].setActive(false);
        for (int i = currentBall; i < ballList.Length; i++) 
        {
            if (ballList[(i + 1) % 3] != null)
            {
                Vector2 lanuchPosition = rigidbody2d.position + Vector2.down * 0.25f + lookDirection * 0.2f;
                ballList[(i + 1) % 3].transform.position = lanuchPosition;
                ballList[(i + 1) % 3].setActive(true);
                transferBallPrefab = ballList[(i + 1) % 3].gameObject;
                currentBall = (i + 1) % 3;
                break;
            }
        }
    }

    //生命值变化函数
    public void ChangeHealth(int healthValue)
    {
        currentHealth += healthValue;
        if (currentHealth  > maxHealth)
        {
            currentHealth = maxHealth;
        }
        if (currentHealth <= 0)
        {
            GameOver();
        }

        if (healthValue < 0)
        {
            rigidbody2d.velocity = new Vector2(-lookDirection.x * 3.0f, 8.0f);
        }
        HealthController.instance.SetValue(currentHealth);
    }

    public void Damage(int healthValue) {
        if (isInvincible)
        {
            return;
        }
        canMove = false;
        isInvincible = true;
        invincibleTimer = timeInvincible;
        ChangeHealth(healthValue);
    }


    //游戏失败函数
    public void GameOver()
    {
        SceneManager.LoadScene(0);
    }

    public void SetisDown(bool isDown)
    {
        this.isDown = isDown;
    }

    public void SetisJump(bool value)
    {
        isJump = value;
    }

    public void SetProTransfer(bool value)
    {
        proTransfer = value;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    
    [SerializeField]
    private float idleTime = 2.0f;
    [SerializeField]
    private float patrolTime = 3.0f;
    [SerializeField]
    private float speed = 3.0f;
    [SerializeField]
    private float deadTime = 2.0f;
    [SerializeField]
    private float xDeadVelocity = 3.0f;

    private bool isRun;
    private float idleTiming;
    private float patrolTiming;
    private bool isDead;

    Rigidbody2D rigidbody2d;
    Animator animator;
    GameObject dyingEnemy;
    BoxCollider2D boxCollider2d;
    RectTransform rectTransform;

    Vector2 lookDirection;

    // ״̬��ʼ��
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider2d = GetComponent<BoxCollider2D>();
        patrolTiming = patrolTime;
        idleTiming = idleTime;
        lookDirection = new Vector2(-1, 0);
    }

    // ״̬ת������
    void Update()
    {
        //����״̬
        if (isDead)
        {
            deadTime -= Time.deltaTime;
            if (deadTime < 0)
            {
                Destroy(dyingEnemy);
            }
        }
        //�˶�״̬
        else
        {
            if (isRun)
            {
                patrolTiming -= Time.deltaTime;
                if (patrolTiming < 0)
                {
                    patrolTiming = patrolTime;
                    isRun = false;
                }
            }
            else
            {
                idleTiming -= Time.deltaTime;
                if (idleTiming < 0)
                {
                    idleTiming = idleTime;
                    lookDirection = -lookDirection;
                    isRun = true;
                }
            }
        }
        
    }

    //�����ƶ�����
    private void FixedUpdate()
    {

        //�˶��ж�
        if (isRun && !isDead)
        {
            rigidbody2d.velocity = new Vector2(lookDirection.x * speed, rigidbody2d.velocity.y);
            animator.SetFloat("Speed", Mathf.Abs(rigidbody2d.velocity.x));
            animator.SetFloat("lookDirection", lookDirection.x);
        }
        if (!isRun && !isDead)
        {
            animator.SetFloat("Speed", Mathf.Abs(rigidbody2d.velocity.x));
            animator.SetFloat("lookDirection", lookDirection.x);
        }
        //�����ж�
        if (isDead)
        {
            animator.SetFloat("Speed", 0.00f);
            transform.position = dyingEnemy.transform.position;
            transform.Rotate(Vector3.forward * 6.0f * lookDirection.x);
        }
    }

    //��ײ����
    public void OnCollisionEnter2D(Collision2D collision)
    {
        //���߼����ײ��Һ��Ƿ��ڵ����Ϸ����Ǿͽ�����������
        RaycastHit2D hitUp = Physics2D.Raycast(rigidbody2d.position + Vector2.up * rectTransform.rect.height * 0.5f + Vector2.left * rectTransform.rect.width * 0.5f,
            Vector2.right,
            rectTransform.rect.width,
            LayerMask.GetMask("Player"));
        if (hitUp && !isDead)
        {
            Die();
        }
    }

    //������ײ����
    public void OnCollisionStay2D(Collision2D collision)
    {

        

        //���߼����ײ��Һ��Ƿ��ڵ������ң��Ǿͽ����˺�����
        RaycastHit2D hitSide = Physics2D.Raycast(rigidbody2d.position + Vector2.left * (boxCollider2d.size.x * 0.5f + 0.5f),
            Vector2.right,
            boxCollider2d.size.x + 1.0f,
            LayerMask.GetMask("Player"));   
        if (hitSide && !isDead)
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player)
            {
                Hit(player);
            }
        }
    }
    //��������
    public void Die() 
    {
        //���õ�ǰ����״̬
        GetComponent<Collider2D>().enabled = false;
        rigidbody2d.freezeRotation = false;
        isDead = true;

        //���ø����壬���ݸ������ƶ�����ʵ����ת
        dyingEnemy = new GameObject("dyingEnemy");
        transform.SetParent(dyingEnemy.transform);
        dyingEnemy.transform.position = transform.position;
        dyingEnemy.AddComponent<Rigidbody2D>();
        dyingEnemy.GetComponent<Rigidbody2D>().gravityScale = rigidbody2d.gravityScale;
        dyingEnemy.GetComponent<Rigidbody2D>().velocity = new Vector2(lookDirection.x * xDeadVelocity, 8.0f);
    }

    //�˺�����
    public void Hit(PlayerController player)
    {
        player.Damage(-1);
    }
}

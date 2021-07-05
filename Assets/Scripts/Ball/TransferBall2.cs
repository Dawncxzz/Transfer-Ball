using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransferBall2 : TransferBall
{

    //重载传送函数，设置初速度为上
    public override void Transfer()
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
            player.GetComponent<Rigidbody2D>().velocity = new Vector2(player.GetComponent<Rigidbody2D>().velocity.x, 16.0f);
            Destroy(gameObject);
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public RectTransform player;
    Vector2 offset;
    void Start()
    {
        offset = transform.position - player.transform.position;
    }

    void Update()
    {
        if (player)
        {
            transform.position = player.transform.position + Vector3.forward * transform.position.z;
        }
    }
}

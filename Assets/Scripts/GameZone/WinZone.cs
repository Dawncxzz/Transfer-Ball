using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinZone : MonoBehaviour
{
    public float fadeTime = 2f;
    GameObject win;
    // Start is called before the first frame update
    void Start()
    {
        win = GameObject.Find("Win");
        win.GetComponent<Image>().color = new Color(255, 255, 255, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            StartCoroutine(Win(win));
        }
    }

    IEnumerator Win(GameObject win)
    {
        float fade = 0;
        while (fade <= fadeTime)
        {
            fade += Time.deltaTime;
            win.GetComponent<Image>().color = new Color(1, 1, 1, fade / fadeTime);
            yield return null;
        }
        Time.timeScale = 0;
    }
}

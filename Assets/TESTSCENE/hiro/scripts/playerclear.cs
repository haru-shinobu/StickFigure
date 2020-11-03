using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerclear : MonoBehaviour
{
    public Vector3 playerpos;
    private GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindWithTag("BoxManager").GetComponent<GameManager>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Clear")
        {
            StartCoroutine(Run());
            gameManager.GameClear();
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator Run()
    {
        //歩行アニメーション
        transform.GetComponent<Box_PlayerController>().Move_Anim(true);
        //Transform PTransform = transform;
        //Vector3 playerpos = PTransform.position;
        transform.position = new Vector3(Mathf.Sin(Time.time) * 5.0f+playerpos.x,  playerpos.y, playerpos.z);
        yield return new WaitForSeconds(3f);
        if (playerpos.y>=15)
        {
            //playerpos.x += 0.1f;
            transform.position = new Vector3(Mathf.Sin(Time.time) *  playerpos.x, 5.0f +playerpos.y, playerpos.z);
            //timer += Time.deltaTime * 0.5f;
            yield return new WaitForEndOfFrame();
            //if (timer >= 1)
        }
        //歩行アニメーション・停止
        transform.GetComponent<Box_PlayerController>().Move_Anim(false);
    }
}

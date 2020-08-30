using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClearCube : MonoBehaviour
{
    public Vector3 playerpos;
    //public GameObject PPos;
    public GameObject hako_Clear;
    //GameObject erea;
    Vector3 Ppos;
    //SpriteRenderer Spr;
    //Vector2 ExtentsHalfPos;
    void Awake()
    {
        playerpos = transform.position;
    }
    private void OnCollisionStay(Collision collision)
    {
        coRoutine();
        Debug.Log("A");
    }

    IEnumerator coRoutine()
    {

        //var Yoko = hako_Clear.GetComponent<Transform>().position.x;
        var Tate = hako_Clear.GetComponent<Transform>().position.y * 0.5;
        //GameObject Tate=GameObject.GetComponent<hako_Clear>();
        //Rigidbody rb = (GameObject.Find("Player")).GetComponent<Rigidbody>();
        Vector3 playerpos = GameObject.Find("Player").transform.position;
        //rb.velocity = Vector3.zero;
        //rb.isKinematic = true;
        //Instantiate(prefab);//クラッカーとか?
        //yield return new WaitForSeconds(1f);
        transform.position = new Vector3(Mathf.Sin(Time.time) * playerpos.x, 10.0f + playerpos.y, playerpos.z);
        yield return new WaitForSeconds(3f);
        //ゴール中央に近づく
        //rb.AddForce(new Vector3(0, 0, 0));
        //Vector3 Startpos = rb.transform.position;
        //Vector3 Endpos = transform.position;
        //Endpos.z = Startpos.z;
        //float timer = 0;
        if (Tate <= transform.position.y)
        {
            transform.position = new Vector3(Mathf.Sin(Time.time) * 5.0f + playerpos.x, playerpos.y, playerpos.z);
            //timer += Time.deltaTime * 0.5f;
            yield return new WaitForEndOfFrame();
            //if (timer >= 1)

        }
        yield return new WaitForSeconds(10f);
        SceneManager.LoadScene("Clear");
    }
}
// Update is called once per frame

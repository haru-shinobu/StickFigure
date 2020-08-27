using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClearCube : MonoBehaviour
{
 
    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Player")
         {
                coRoutine();

         }
    }
    IEnumerator coRoutine()
    {
        Rigidbody rb = (GameObject.Find("Player")).GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;

        //Instantiate(prefab);//クラッカーとか？


        yield return new WaitForSeconds(.1f);
        //ゴール中央に近づく
        rb.AddForce(new Vector3(0, 0, 0));
        Vector3 Startpos = rb.transform.position;
        Vector3 Endpos = transform.position;
        Endpos.z = Startpos.z;
        float timer = 0;
        while (true)
        {
            timer += Time.deltaTime*0.5f;
            yield return new WaitForEndOfFrame();
            rb.transform.position = Vector3.Lerp(Startpos, Endpos, timer);
            if (timer >= 1)
                break;
        }
        yield return new WaitForSeconds(.3f);
        SceneManager.LoadScene("Clear");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

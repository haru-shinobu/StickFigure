using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerclear : MonoBehaviour
{
    public Vector3 playerpos;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.tag == "Clear")
        {
            coRoutine();
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator coRoutine()
    {
        //Transform PTransform = transform;
        //Vector3 playerpos = PTransform.position;
        transform.position = new Vector3(Mathf.Sin(Time.time) * 5.0f+playerpos.x,  + playerpos.y, playerpos.z);
        yield return new WaitForSeconds(3f);
        Debug.Log("B");
        if (playerpos.y>=5)
        {
            //playerpos.x += 0.1f;
            transform.position = new Vector3(Mathf.Sin(Time.time) *  playerpos.x, 5.0f +playerpos.y, playerpos.z);
            //timer += Time.deltaTime * 0.5f;
            yield return new WaitForEndOfFrame();
            //if (timer >= 1)
            Debug.Log("C");
        }
    }
}

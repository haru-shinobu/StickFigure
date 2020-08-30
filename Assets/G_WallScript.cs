using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_WallScript : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Player")
        {
            if (collision.transform.position.x < transform.position.x)
                collision.transform.position -= Vector3.right;
            else
                collision.transform.position += Vector3.right;
        }
    }
}

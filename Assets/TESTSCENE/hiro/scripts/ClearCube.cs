using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClearCube : MonoBehaviour
{
    public GameObject player;
    public GameObject goalText;
    Vector3 Ppos;

    public void Start()
    {
        goalText.gameObject.SetActive(false);
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            coRoutine();
            Debug.Log("A");
        }
    }

    IEnumerator coRoutine()
    { 
        yield return new WaitForSeconds(10f);
        goalText.gameObject.SetActive(true);
        yield return new WaitForSeconds(15f);
        SceneManager.LoadScene("Clear");
    }
}
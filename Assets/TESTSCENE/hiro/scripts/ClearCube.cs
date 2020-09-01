using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClearCube : MonoBehaviour
{
    public GameObject goalText;
    Vector3 Ppos;

    public void Start()
    {
        goalText.gameObject.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(clear());
    }
    //private void OnCollisionStay(Collision collision)
    //{
    //    Debug.Log(collision.gameObject.name);
    //    if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
    //    {
    //        StartCoroutine(clear());
    //    }
    //}

    private IEnumerator clear()
    {
        goalText.gameObject.SetActive(true);
        yield return new WaitForSeconds(10f);
        SceneManager.LoadScene("Clear");
        yield return new WaitForSeconds(15f);
    }
}
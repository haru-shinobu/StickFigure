using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClearCube : MonoBehaviour
{
    public GameObject playerpos;
    public GameObject hako_Clear;
    public GameObject goalText;
    public MeshRenderer tate_mesh;
    Vector3 Ppos;
    void Awake()
    {
        goalText.gameObject.SetActive(false);
    }
    public void Start()
    {
        Transform PTransform = transform;
    }
    public void mesher()
    {
        tate_mesh = transform.GetComponent<MeshRenderer>();
        Vector3 tate = new Vector3(
            Mathf.Abs(tate_mesh.bounds.size.x),
            Mathf.Abs(tate_mesh.bounds.extents.y),
            Mathf.Abs(tate_mesh.bounds.size.z));
        Vector3 tate_Y = transform.position + tate;
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            coRoutine();
            Debug.Log("A");
        }
    }

    IEnumerator coRoutine()
    { 
        //Transform PTransform = transform;
        tate_mesh = transform.GetComponent<MeshRenderer>();
        Vector3 tate = new Vector3(
            Mathf.Abs(tate_mesh.bounds.size.x),
            Mathf.Abs(tate_mesh.bounds.extents.y),
            Mathf.Abs(tate_mesh.bounds.size.z));
        Vector3 tate_Y = transform.position + tate;
        yield return new WaitForSeconds(10);
        goalText.gameObject.SetActive(true);
        yield return new WaitForSeconds(15f);
        SceneManager.LoadScene("Clear");
    }
}
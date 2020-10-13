using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClearCube : MonoBehaviour
{
    //private Vector3 offset;
    public GameObject goalText;
    Vector3 Ppos;
    [SerializeField]
    ParticleSystem Particle;
    [SerializeField]
    ParticleSystem Particle_1;
    [SerializeField]
    ParticleSystem Particle_2;

    public void Start()
    {
        goalText.gameObject.SetActive(false);
        Particle.gameObject.SetActive(false);
        Particle_1.gameObject.SetActive(false);
        Particle_2.gameObject.SetActive(false);
        //offset = transform.position - Particle.transform.position;
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
        //紙吹雪
        //transform.position = Particle.transform.position + offset;
        Particle.gameObject.SetActive(true);
        Particle.Simulate(4.0f, true, false);
        Particle.Play();
        //クラッカー1
        Particle_1.gameObject.SetActive(true);
        Particle_1.Simulate(4.0f, true, false);
        Particle_1.Play();
        //クラッカー2
        Particle_2.gameObject.SetActive(true);
        Particle_2.Simulate(4.0f, true, false);
        Particle_2.Play();
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Clear");
        yield return new WaitForSeconds(5f);
    }
}
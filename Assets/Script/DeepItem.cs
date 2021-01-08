using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeepItem : MonoBehaviour
{
    private GameObject gameManager;
    private GameManager _Manager;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager");
        _Manager = gameManager.GetComponent<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("InTrigger");
        if (other.tag == "PlayerBase")
        {
            Debug.Log("GetItem");
            _Manager.ndCountUp();
            Destroy(this.gameObject);
        }
    }
}

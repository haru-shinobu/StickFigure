using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class roomController : MonoBehaviour
{
    // Start is called before the first frame update
    Camera cam;
    PlayerSqript Player;

    bool m_bCoroutine = true;
    GameObject Prev_wall;
    GameObject[] Walls;
    void Start()
    {
        cam = Camera.main;
        Player = GameObject.Find("Player").GetComponent<PlayerSqript>();
        Walls = GameObject.FindGameObjectsWithTag("Wall");
    }

    
    void Update()
    {
        
    }
    public GameObject WallMovement(GameObject obj)
    {
        return null;
    }

    public void PlayerControllerJudge(bool flag)
    {
        Player.ControllJudge(flag);
    }
    public void SetCamera()
    {

    }

    public void Door()
    {
        if (m_bCoroutine)
        {
            cam.transform.SetParent(Player.transform);
            StartCoroutine(DoorCamera());
        }
    }
    IEnumerator DoorCamera()
    {
        m_bCoroutine = false;
        yield return new WaitForEndOfFrame();
        m_bCoroutine = true;
    }
}

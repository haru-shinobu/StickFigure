using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Transform player;
    Rigidbody rb;

    [SerializeField, Range(1, 5)]
    float Speed = 3;

    bool m_bJump = false;
    bool m_bControll = true;

    bool m_bGimmicFlag = false;

    enum WAY
    {
        RIGHT = -1,
        NORMAL = 0,
        LEFT = 1,
    }
    WAY way = WAY.NORMAL;

    Vector3 MoveAriaLeftTop, MoveAriaRightBottom;

    public RelayWallScript WallScript;
    
    void Start()
    {
        player = this.gameObject.transform;
        rb = player.GetComponent<Rigidbody>();
        m_bJump = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_bControll)
        {
            var leftright = Input.GetAxis("Horizontal");
            if (leftright > 0)
            {
                player.localPosition += Camera.main.transform.right * Speed * 0.01f;
                way = WAY.RIGHT;
            }
            else
                if (leftright < 0)
            {
                player.localPosition -= Camera.main.transform.right * Speed * 0.01f;
                way = WAY.LEFT;
            }

            if (Input.GetButton("Jump"))
            {
                if (!m_bJump)
                {
                    m_bJump = true;
                    rb.AddForce(new Vector2(0, 5), ForceMode.Impulse);
                }
            }
            else
            if (rb.velocity.y == 0) m_bJump = false;
        }
    }

    void SetPlayerMoveLimit()
    {
        MoveAriaLeftTop = WallScript.GetWallAriaLT();
        MoveAriaRightBottom = WallScript.GetWallAriaRB();
    }

    public void ControllJudge(bool flag)
    {
        m_bControll = flag;
    }
    public int Getways()
    {
        return (int)way;
    }
    
}

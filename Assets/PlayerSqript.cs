using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSqript : MonoBehaviour
{
    Transform player;
    bool m_bDoor = false;
    Rigidbody rb;
    [SerializeField, Range(1, 5)]
    float Speed = 3;
    bool m_bJump = false;
    bool m_bControll = true;

    bool PlayerRotateFlag = false;
    string afterwallname;
    roomController roomer;
    void Start()
    {
        player = this.gameObject.transform;
        rb = player.GetComponent<Rigidbody>();
        m_bJump = false;
        roomer = GameObject.Find("roomContoller").GetComponent<roomController>();
    }
    
    void Update()
    {
        if (m_bControll)
        {
            var leftright = Input.GetAxis("Horizontal");
            if (leftright > 0)
                player.localPosition += Camera.main.transform.right * Speed * 0.01f;
            else
                if (leftright < 0)
                player.localPosition -= Camera.main.transform.right * Speed * 0.01f;

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

    public void ControllJudge(bool flag)
    {
        m_bControll = flag;
    }
    /*
    void OnTriggerEnter(Collider other)
    {
        if (m_bControll && !PlayerRotateFlag)//コントロールOK　かつ　回転中でない
        {
            if (other.gameObject.tag == "Wall")//Wall tag
            {
                //現在がそれであるか
                if (other.name != afterwallname)
                {
                    m_bControll = false;
                    player.GetComponent<Rigidbody>().isKinematic = true;

                    roomer.WallMovement(other.gameObject);
                    //***********************************************************
                    var spriterenderer = other.GetComponent<SpriteRenderer>();
                    var sprite_halfX = (spriterenderer.sprite.bounds.extents.x);
                    var diff = other.transform.localPosition - player.localPosition;
                    var axis = Vector3.Cross(player.forward, diff);
                    var angle = Vector3.Angle(player.forward, diff) * (axis.y < 0 ? -1 : 1);
                    if (angle > 0)
                        sprite_halfX = -sprite_halfX;
                    var pos = spriterenderer.transform.TransformPoint(new Vector3(sprite_halfX, 0)) + new Vector3(0, player.transform.localPosition.y);
                
                    GameObject wallobj = other.gameObject;
                    Collider[] targets = Physics.OverlapSphere(transform.position, 1, LayerMask.GetMask("room"));
                    foreach (Collider obj in targets)
                    {
                        if (player.transform.localRotation != obj.transform.localRotation)
                        {
                            wallobj = obj.gameObject;
                            afterwallname = wallobj.name;
                            PlayerRotateFlag = true;
                        }
                    }
                    if (wallobj != other.gameObject)
                        StartCoroutine(Wallpoint(pos, wallobj));
                    else
                    {
                        m_bControll = true;
                        player.GetComponent<Rigidbody>().isKinematic = false;
                    }
                }
            }
        }
    }
    */
    void OnTriggerExit(Collider other)
    {
/*        if (!PlayerRotateFlag)
            if (other.gameObject.tag == "Wall")
                if (other.name == afterwallname)
                    PlayerRotateFlag = true;
                    */
    }
    IEnumerator Wallpoint(Vector3 pos, GameObject obj)//0.5f
    {
        var Ppos = player.localPosition;
        var Prot = player.localRotation;
        var Cpos = Camera.main.transform.localPosition;
        var count = 0f;
        while (count < 1)
        {
            yield return new WaitForEndOfFrame();
            count += Time.deltaTime * 2;
            player.localPosition = Vector3.Lerp(Ppos, pos, count);
            player.localRotation = Quaternion.Lerp(Prot, obj.transform.localRotation, count);
            Camera.main.transform.rotation = Quaternion.Lerp(Prot, obj.transform.localRotation, count);
            Camera.main.transform.localPosition = Vector3.Lerp(Cpos, obj.transform.forward, count);
        }
        player.localPosition = Vector3.Lerp(Ppos, pos, 1);
        player.localRotation = obj.transform.localRotation;
        Camera.main.transform.rotation = obj.transform.localRotation;

        m_bControll = true;
        player.GetComponent<Rigidbody>().isKinematic = false;
    }
}

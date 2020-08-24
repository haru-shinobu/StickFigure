using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RayTest : MonoBehaviour
{
    [SerializeField] private GameObject _tape_;
    public Box_PlayerController BChecker;
    public GameData G_Data;
    public GameObject Text;
    Vector2 Player_verticalhorizontal;
    //public GameObject Player;
    public const float NOTHING = -1;
    public float maxDistance = 30;
    public float distance;
    //public float Point;
    float Sprite_half_DistanceX;
    float Sprite_half_DistanceY;
    SpriteRenderer Sr;
    Vector2 extentsHalfPos;
    Vector3 MoveAriaLeftTop, MoveAriaRightBottom;
    Vector3 LeftTop;
    Vector3 RightBottom;
    


    void Awake()
    {
        Sr = GetComponent<SpriteRenderer>();
        var _sprite = Sr.sprite;
        var _halfX = _sprite.bounds.extents.x;
        var _halfY = _sprite.bounds.extents.y;

        extentsHalfPos = new Vector2(_halfX, _halfY);
        var _vec = new Vector3(-extentsHalfPos.x, extentsHalfPos.y, 0f);
        var _vec2 = new Vector3(extentsHalfPos.x, -extentsHalfPos.y, 0f);
        var LeftLine = Sr.transform.TransformPoint(_vec);
        var RightLine = Sr.transform.TransformPoint(_vec2);
        var sLeftLine = LeftLine;
        var sRightLine = RightLine;
        LeftLine.y = RightLine.y = 0;
        Sprite_half_DistanceX = Vector3.Distance(LeftLine, RightLine) * 1f;
        sLeftLine.x = sRightLine.x = 0;
        Sprite_half_DistanceY = Vector3.Distance(sLeftLine, sRightLine) * 1f;
    }
    void Start()
    {
        //BChecker.Bridge.SetActive(true);
        Text.SetActive(false);
    }
    void Update()
    {
        Vector3 _tape = transform.position;
        BChecker.CheckRedAria();
        if (BChecker.CheckRedAria() == true)
        {
            _tape = transform.position + new Vector3(0, Player_verticalhorizontal.y);
            Vector3 fwd = transform.TransformDirection(0, 0, -10);
            //Rayを飛ばすtransform.TransformDirection(x,y,z);
            //上向き
            if (MoveAriaRightBottom.y + G_Data.RedLine <= transform.position.y)
            {
                fwd = transform.TransformDirection(0, 20, 5);
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    _tape = new Vector3(transform.position.x, MoveAriaLeftTop.y - G_Data.RedLine / 2);
                    BChecker.Bridge.SetActive(true);
                }
                //if (Input.GetKeyDown(KeyCode.Q))
                //{
                    if(BChecker.Bridge.activeInHierarchy==true)
                    {
                        Debug.Log("wataru");
                    }
                //}
            }
            //下向き
            if (MoveAriaLeftTop.y - G_Data.RedLine >= transform.position.y)
            {
                fwd = transform.TransformDirection(0, -20, 5);
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    _tape = new Vector3(transform.position.x, MoveAriaLeftTop.y + G_Data.RedLine / 2);
                }
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    if (_tape_.activeSelf==false)
                    {
                        Debug.Log("wataru");
                    }
                }
            }
            //右向き
            if (MoveAriaLeftTop.x + G_Data.RedLine <= transform.position.x)
            {
                fwd = transform.TransformDirection(20, 0, 5);
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    _tape = transform.position + new Vector3(MoveAriaRightBottom.x - G_Data.RedLine / 2, transform.position.y);
                }
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    if (_tape_.activeSelf==false)
                    {
                        Debug.Log("wataru");
                    }
                }
            }
            //左向き
            if (MoveAriaRightBottom.x - G_Data.RedLine >= transform.position.x)
            {
                fwd = transform.TransformDirection(-20, 0, 5);
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    _tape = new Vector3(MoveAriaRightBottom.x + G_Data.RedLine / 2, transform.position.y);
                }
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    if (_tape_.activeSelf==false)
                    {
                        Debug.Log("wataru");
                    }
                }
            }

            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            RaycastHit Hit;
            if (Physics.Raycast(transform.position, Vector3.down, out Hit)) {
                Debug.Log("Hit.point");
            }
                if (Physics.Raycast(transform.position - transform.forward, fwd, out hit, maxDistance))
            {
                distance = hit.distance;
            }
            else
            {
                distance = NOTHING;
            }
            Debug.DrawRay(transform.position - transform.forward, fwd, Color.red, 5);

            //テキスト消える
            if (distance <= 3)
            {
                Text.SetActive(false);
                //Debug.Log("Delete");
            }
            //テキスト出てくる
            if (distance >= 3)
            {
                Text.SetActive(true);
            }
           }
        //テープ確認
        if (BChecker.Bridge.activeInHierarchy)
        {
            //テープ回収
            if (Input.GetKeyDown(KeyCode.Space))
            {
                BChecker.Bridge.SetActive(false);
            }
        }
      }
   }
//}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    //現在使用していない(後で整理)=============================================
    //Player
    [SerializeField]
    Box_PlayerController player;
    [SerializeField, Range(0.1f, 1), Header("Player一度の移動距離")]
    float _fmoveDistance;
    [SerializeField, Range(0.1f, 10), Header("Player一度の落下速度")]
    float follSpeed;
    //範囲外で回転
    Vector3 RollAriaLT, RollAriaRB;
    //移動制限範囲
    Vector3 MoveAriaLT, MoveAriaRB;
    
    //橋の存在とそのエリア
    bool _bLiveBridge;
    GameObject BridgeObj;
    Vector3 BridgeAriaLT, BridgeAriaRB;

    //現在の箱
    SideColorBoxScript sideBox;
    //箱の登録
    GameObject[] Boxs;
    [SerializeField, Header("回転速度"), Range(0.5f, 2)]
    float ChangeSpeed = 1;
    
    //Prefab
    [SerializeField, Header("橋Prefab")]
    GameObject Bridge;
    
    //UI
    [SerializeField, Header("UI_Canvas")]
    GameObject _Canvas;
    [SerializeField, Header("UI_script")]//橋カウント用
    UIScript _UIScript;
    

    //橋カウント用
    private int nDCount = 5;

    //カメラ
    [SerializeField,Header("カメラマネージャ")]
    CameraManager camM;
    // ↑現在使用していない(後で整理)=============================================
    [SerializeField,Header("アイロン")]
    GameObject Iron;
    [SerializeField,Range(1,3)]
    float i_IronAnimSpeed = 1;

    //ゲームオーバーアニメーション用
    [SerializeField]
    GameOverScript GOsc;

    //GameOver
    bool _bGOflag = false;
    IEnumerator IE_GO;

    //Updateを起動するかON・OFF切り替えするターゲットを登録
    enum Controll_Target
    {
        Player = 0,
    }
    bool[] ControllerActivater;
    GameObject Startbox;
    [SerializeField]
    GameObject arrow;
    SoundManager soundM;
    
    void Awake()
    {
        arrow.SetActive(false);
        ControllerActivater = new bool[System.Enum.GetNames(typeof(Controll_Target)).Length];
        for (int i = 0; i < ControllerActivater.Length; i++)
            ControllerActivater[i] = false;
        
        GameObject[] boxes  = GameObject.FindGameObjectsWithTag("Box");
        Boxs = new GameObject[boxes.Length];
        Boxs = boxes;
        Startbox = transform.GetComponent<StarterScript>().GetStartBox();
        GameObject soundtarget = GameObject.Find("SoundObj");
        if (soundtarget)
            soundM = soundtarget.GetComponent<SoundManager>();
    }

    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Box_PlayerController>();
        _UIScript.ChangeNum(nDCount);
    }

    // Update is called once per frame
    void Update()
    {
        //if (ControllerActivater[(int)Controll_Target.Player])
        //    player.Moving = true;
    }
    //橋カウントチェック
    public bool nDCountCheck()
    {
        return (nDCount > 0) ? true : false;
    }
    //橋カウント・UI
    public void nDCountDown()
    {
        nDCount--;
        if (nDCount >= 0)
            _UIScript.ChangeNum(nDCount);
        if (nDCount == 0)
        {
            if (!GameObject.FindWithTag("Clear").transform.parent.GetComponent<ClearCube>().nDCount_CountEnd)
            {
                IE_GO = GameOver_before();
                StartCoroutine(IE_GO);
            }
        }
    }
    public void CheckBridgeNum()
    {
        if (nDCount <= 0)
        {
            _bGOflag = true;
        }
    }
    public void CheckBridgeGoal()
    {
        if (nDCount <= 0)
        {
            _bGOflag = true;
        }
    }
    //GameOver寸前判定処理をここに。
    IEnumerator GameOver_before()
    {
        while (!_bGOflag)
        {
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForEndOfFrame();
        GOsc.GameOver(this);
    }
    //GameOver処理をここに。
    public void ChangeSceneGameOver()
    {
        SceneManager.LoadScene("GameOver");
    }
    public void GameClear()
    {
        _bGOflag = false;
        if (IE_GO == GameOver_before())
            StopCoroutine(IE_GO);
    }
    public void MakeIron()
    {
        Vector3 campos = Camera.main.transform.position;
        Vector3 startpos = campos + new Vector3(15, 0, 0);
        GameObject ironobj = Instantiate(Iron, startpos, Quaternion.identity);
        IronScript IronSc = ironobj.transform.GetComponent<IronScript>();
        IronSc.arrow = arrow;
        IronSc.Speed = i_IronAnimSpeed;
        IronSc.GSStartPos = startpos;
        IronSc.GSEndPos = Startbox.transform.position - new Vector3(0, 0, Startbox.GetComponent<MeshRenderer>().bounds.extents.z);
        IronSc.MoveOK = true;
        if (soundM)
            soundM.IronSteamSE();
    }
}

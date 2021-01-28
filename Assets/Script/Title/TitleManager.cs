using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


enum TitlePhase : short
{
    TITLEPHASE_INIT = 0x00,
    TITLEPHASE_FADEIN = 0x01,
    TITLEPHASE_RUN = 0x02,
    TITLEPHASE_FADEOUT = 0x03,
    TITLEPHASE_DONE = 0x04
}

enum ButtonSelect : short
{
    BUTTON_GAME = 0x00,
    //BUTTON_RULE = 0x01,
    BUTTON_EXIT = 0x02,
}


public class TitleManager : MonoBehaviour
{
    GameObject SoundObj;

    [SerializeField]
    private TitlePhase m_ePhase;

    [SerializeField]
    private ButtonSelect m_eSelect;

    [SerializeField]
    private Image Game;
    //[SerializeField]
    //private Image Rurle;
    [SerializeField]
    private Image Exit;

    [SerializeField]
    float m_fFadeSpeed;

    public GameObject m_FadeObject;

    //! フェード用フラグ
    FadeManager m_Fade;
    //! 連続入力防止用フラグ
    private bool m_bFlag;

    string sNext;

    // Start is called before the first frame update
    void Start()
    {
        if (m_FadeObject)
            m_Fade = m_FadeObject.GetComponent<FadeManager>();
        m_eSelect = ButtonSelect.BUTTON_GAME;
        m_ePhase = TitlePhase.TITLEPHASE_INIT;
        m_bFlag = false;
        SoundObj = GameObject.Find("SoundObj");
        SoundObj.GetComponent<SoundManager>().BGMState();
    }

    // Update is called once per frame
    void Update()
    {
        bool bFlag = false;
        float horizontal = Input.GetAxis("Horizontal");
        //float vartical = Input.GetAxis("Vertical");

        switch (m_ePhase)
        {
            case TitlePhase.TITLEPHASE_INIT:

                m_ePhase = TitlePhase.TITLEPHASE_FADEIN;
                break;
            case TitlePhase.TITLEPHASE_FADEIN:

                bFlag = m_Fade.isFadeIn(m_fFadeSpeed);
                if (bFlag)
                    m_ePhase = TitlePhase.TITLEPHASE_RUN;
                break;
            case TitlePhase.TITLEPHASE_RUN:

                if (!m_bFlag)
                {
                    switch (m_eSelect)
                    {
                        case ButtonSelect.BUTTON_GAME:

                            //Rurle.GetComponent<TextHilight>().None();
                            Exit.GetComponent<TextHilight>().None();
                            Game.GetComponent<TextHilight>().Flash();
                            if (horizontal != 0)
                            {
                                m_eSelect = ButtonSelect.BUTTON_EXIT;
                                m_bFlag = true;
                            }
                            break;
                        //case ButtonSelect.BUTTON_RULE:

                        //    Game.GetComponent<TextHilight>().None();
                        //    Exit.GetComponent<TextHilight>().None();
                        //    Rurle.GetComponent<TextHilight>().Flash();
                        //    if (Vertical < -0.5f)
                        //    {
                        //        m_eSelect = ButtonSelect.BUTTON_EXIT;
                        //        m_bFlag = true;
                        //    }
                        //    else if (Vertical > 0.5f)
                        //    {
                        //        m_eSelect = ButtonSelect.BUTTON_GAME;
                        //        m_bFlag = true;
                        //    }
                        //    break;
                        case ButtonSelect.BUTTON_EXIT:

                            //Rurle.GetComponent<TextHilight>().None();
                            Game.GetComponent<TextHilight>().None();
                            Exit.GetComponent<TextHilight>().Flash();
                            if (horizontal != 0)
                            {
                                m_eSelect = ButtonSelect.BUTTON_GAME;
                                m_bFlag = true;
                            }
                            break;
                    }
                }
                else
                {
                    if (horizontal == 0)
                        m_bFlag = false;
                }
                if (Input.GetButtonDown("Jump"))
                    m_ePhase = TitlePhase.TITLEPHASE_FADEOUT;

                break;
            case TitlePhase.TITLEPHASE_FADEOUT:

                bFlag = m_Fade.isFadeOut(m_fFadeSpeed);
                if (bFlag)
                    m_ePhase = TitlePhase.TITLEPHASE_DONE;
                break;
            case TitlePhase.TITLEPHASE_DONE:

                switch (m_eSelect)
                {
                    case ButtonSelect.BUTTON_GAME:

                        SceneManager.LoadScene("GameMainScene");
                        break;
                    //case ButtonSelect.BUTTON_RULE:

                    //    SceneManager.LoadScene("RurleScene");
                    //    break;
                    case ButtonSelect.BUTTON_EXIT:

                        Quit();
                        break;
                }
                break;
        }
    }

    public void StringArgFunction(string s)
    {
        //Debug.Log("はいととる");
        sNext = s;
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
    }
}

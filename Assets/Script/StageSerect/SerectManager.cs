using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


enum SERECTPhase : short
{
    SERECTPHASE_INIT = 0x00,
    SERECTPHASE_FADEIN = 0x01,
    SERECTPHASE_RUN = 0x02,
    SERECTPHASE_FADEOUT = 0x03,
    SERECTPHASE_DONE = 0x04
}

enum IMAGESelect : short
{
    IMAGE_STAGE_01 = 0x00,
    IMAGE_STAGE_02 = 0x01,
    IMAGE_STAGE_03 = 0x02,
}

public class SerectManager : MonoBehaviour
{
    GameObject SoundObj;

    [SerializeField]
    private SERECTPhase m_ePhase;

    [SerializeField]
    private IMAGESelect m_eSelect;

    [SerializeField]
    private Image Stage_01;

    [SerializeField]
    private Image Stage_02;

    [SerializeField]
    private Image Stage_03;

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
        {
            m_Fade = m_FadeObject.GetComponent<FadeManager>();
        }
        m_eSelect = IMAGESelect.IMAGE_STAGE_01;
        m_ePhase = SERECTPhase.SERECTPHASE_INIT;
        m_bFlag = false;
        SoundObj = GameObject.Find("SoundObj");
        if(SoundObj)
        {
            SoundObj.GetComponent<SoundManager>().BGMState();
        }
    }

    // Update is called once per frame
    void Update()
    {
        bool bFlag = false;
        float horizontal = Input.GetAxis("Horizontal");

        switch (m_ePhase)
        {
            case SERECTPhase.SERECTPHASE_INIT:

                m_ePhase = SERECTPhase.SERECTPHASE_FADEIN;
                break;
            case SERECTPhase.SERECTPHASE_FADEIN:

                bFlag = m_Fade.isFadeIn(m_fFadeSpeed);
                if (bFlag)
                    m_ePhase = SERECTPhase.SERECTPHASE_RUN;
                break;
            case SERECTPhase.SERECTPHASE_RUN:

                if (!m_bFlag)
                {
                    switch (m_eSelect)
                    {
                        case IMAGESelect.IMAGE_STAGE_01:

                            Stage_01.GetComponent<ChangeMaterial>().Flash();
                            Stage_02.GetComponent<ChangeMaterial>().None();
                            Stage_03.GetComponent<ChangeMaterial>().None();
                            if (horizontal > 0)
                            {
                                m_eSelect = IMAGESelect.IMAGE_STAGE_02;
                                m_bFlag = true;
                            }
                            else if (horizontal < 0)
                            {
                                m_eSelect = IMAGESelect.IMAGE_STAGE_03;
                                m_bFlag = true;
                            }
                            break;

                        case IMAGESelect.IMAGE_STAGE_02:

                            Stage_01.GetComponent<ChangeMaterial>().None();
                            Stage_02.GetComponent<ChangeMaterial>().Flash();
                            Stage_03.GetComponent<ChangeMaterial>().None();
                            if (horizontal > 0)
                            {
                                m_eSelect = IMAGESelect.IMAGE_STAGE_03;
                                m_bFlag = true;
                            }
                            else if (horizontal < 0)
                            {
                                m_eSelect = IMAGESelect.IMAGE_STAGE_01;
                                m_bFlag = true;
                            }
                            break;

                        case IMAGESelect.IMAGE_STAGE_03:

                            Stage_01.GetComponent<ChangeMaterial>().None();
                            Stage_02.GetComponent<ChangeMaterial>().None();
                            Stage_03.GetComponent<ChangeMaterial>().Flash();
                            if (horizontal > 0)
                            {
                                m_eSelect = IMAGESelect.IMAGE_STAGE_01;
                                m_bFlag = true;
                            }
                            else if (horizontal < 0)
                            {
                                m_eSelect = IMAGESelect.IMAGE_STAGE_02;
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
                {
                    m_ePhase = SERECTPhase.SERECTPHASE_FADEOUT;
                }

                break;
            case SERECTPhase.SERECTPHASE_FADEOUT:

                bFlag = m_Fade.isFadeOut(m_fFadeSpeed);
                if (bFlag)
                    m_ePhase = SERECTPhase.SERECTPHASE_DONE;
                break;
            case SERECTPhase.SERECTPHASE_DONE:

                switch (m_eSelect)
                {
                    case IMAGESelect.IMAGE_STAGE_01:

                        SceneManager.LoadScene("Stage01");
                        break;
                    case IMAGESelect.IMAGE_STAGE_02:

                        SceneManager.LoadScene("Stage02");
                        break;
                    case IMAGESelect.IMAGE_STAGE_03:

                        SceneManager.LoadScene("Stage03");
                        break;
                }
                break;
        }
    }

    public void StringArgFunction(string s)
    {
        Debug.Log("はいっとる");
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

    enum ClearPhase : short
    {
        CLEARPHASE_INIT = 0x00,
        CLEARPHASE_FADEIN = 0x01,
        CLEARPHASE_RUN = 0x02,
        CLEARPHASE_FADEOUT = 0x03,
        CLEARPHASE_DONE = 0x04
    }

    enum Click : short
    {
        BUTTON_TITLE = 0x00,
        //BUTTON_RULE = 0x01,
        BUTTON_EXIT = 0x02,
    }

public class ClearFade : MonoBehaviour
    {
        GameObject SoundObj;

        [SerializeField]
        private ClearPhase m_ePhase;

        [SerializeField]
        private ButtonSelect m_eSelect;

        //[SerializeField]
        //private Image Game;
        //[SerializeField]
        //private Image Rurle;
        //[SerializeField]
        //private Image Exit;

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
            m_ePhase = ClearPhase.CLEARPHASE_INIT;
            m_bFlag = false;
            //SoundObj = GameObject.Find("SoundObj");
            //SoundObj.GetComponent<SoundManager>().BGMState();
        }

        // Update is called once per frame
        void Update()
        {
            bool bFlag = false;
            float horizontal = Input.GetAxis("Horizontal");
            //float vartical = Input.GetAxis("Vertical");

            switch (m_ePhase)
            {
                case ClearPhase.CLEARPHASE_INIT:

                    m_ePhase = ClearPhase.CLEARPHASE_FADEIN;
                    break;
                case ClearPhase.CLEARPHASE_FADEIN:

                    bFlag = m_Fade.isFadeIn(m_fFadeSpeed);
                    if (bFlag)
                        m_ePhase = ClearPhase.CLEARPHASE_RUN;
                    break;
                case ClearPhase.CLEARPHASE_RUN:

                    if (!m_bFlag)
                    {
                        switch (m_eSelect)
                        {
                            case ButtonSelect.BUTTON_GAME:

                                //Rurle.GetComponent<TextHilight>().None();
                                //Exit.GetComponent<TextHilight>().None();
                                //Game.GetComponent<TextHilight>().Flash();
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
                                //Game.GetComponent<TextHilight>().None();
                                //Exit.GetComponent<TextHilight>().Flash();
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
                    if (Input.GetMouseButton(0))
                        m_ePhase = ClearPhase.CLEARPHASE_FADEOUT;

                    break;
                case ClearPhase.CLEARPHASE_FADEOUT:

                    bFlag = m_Fade.isFadeOut(m_fFadeSpeed);
                    if (bFlag)
                        m_ePhase = ClearPhase.CLEARPHASE_DONE;
                    break;
                case ClearPhase.CLEARPHASE_DONE:

                    switch (m_eSelect)
                    {
                        case ButtonSelect.BUTTON_GAME:

                            SceneManager.LoadScene("Title");
                            break;
                        //case ButtonSelect.BUTTON_RULE:

                        //    SceneManager.LoadScene("RurleScene");
                        //    break;
                        //case ButtonSelect.BUTTON_EXIT:

                        //    Quit();
                        //    break;
                    }
                    break;
            }
        }

        public void StringArgFunction(string s)
        {
            Debug.Log("はいととる");
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

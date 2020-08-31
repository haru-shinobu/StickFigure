using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    //! 色
    [SerializeField]
    private float r, g, b;

    //! 透明度
    [SerializeField]
    private float m_fAlpha;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Image>().color = new Color(r, g, b, m_fAlpha);
        m_fAlpha = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Image>().color = new Color(r, g, b, m_fAlpha);
    }

    /**
     * @brief   フェードイン
     * @return  フェードが終るとtrueを返す
     */
    public bool isFadeIn(float speed)
    {
        if (m_fAlpha > 0.0f)
            m_fAlpha -= speed;
        else
            return true;

        return false;
    }


    /**
     * @brief   フェードアウト
     * @return  フェードが終るとtrueを返す
     */
    public bool isFadeOut(float speed)
    {
        if (m_fAlpha < 1.0f)
            m_fAlpha += speed;
        else
            return true;

        return false;
    }
}

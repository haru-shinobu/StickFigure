using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHilight : MonoBehaviour
{
    public Image image;
    public float fSpeed;    //! Speed
    float fAlpha;           //! Alpha
    int iPhase;             //! FlashPhase

    void State()
    {
        fAlpha = 0;
        iPhase = 0;
        // 自分を選択状態にする
        //Selectable sel = GetComponent<Selectable>();
        //sel.Select();
    }

    public void update()
    {
        //Button = EventSystem.current.currentSelectedGameObject;

        switch (iPhase)
        {
            case 0:
                if (fAlpha < 1.0f)
                    fAlpha += fSpeed;
                else
                    iPhase = 1;
                break;
            case 1:
                if (fAlpha > 0.0f)
                    fAlpha -= fSpeed;
                else
                    iPhase = 0;
                break;
        }
        if (image)
        {
            image.color = new Color(1, 1, 1, fAlpha);
        }
        //else
        //    image.color = new Color(1, 1, 1, 1);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScript : MonoBehaviour
{
    [SerializeField, Header("ゲームオーバー用キャンバス")]
    Canvas overCanvas;
    GameObject ImageSprite;
    GameManager gm;
    bool Flag = false;
    GameObject MainCam;
    // Start is called before the first frame update
    void Start()
    {
        ImageSprite = overCanvas.transform.GetChild(0).gameObject;
        ImageSprite.SetActive(false);
    }
    
    public void GameOver(GameManager gmsc)
    {
        //橋Destroyしたとき呼ばれるメソッドがあるため、シーン切り替え前に一応破棄できるように。
        GameObject.FindWithTag("Player").GetComponent<Box_PlayerController>().SceneEndBridgeBreak();
        gm = gmsc;
        MainCam = transform.parent.gameObject;
        transform.SetParent(MainCam.transform.parent);
        StartCoroutine(Capture());
        StartCoroutine(GameOverProminence());        
    }

    IEnumerator Capture()
    {
        //ReadPicxelsがこの後でないと使えないので必ず書く
        yield return new WaitForEndOfFrame();

        //スクリーンの大きさのSpriteを作る
        var texture = new Texture2D(Screen.width, Screen.height);

        //スクリーンを取得する
        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        //適応する
        texture.Apply();

        //取得した画像をSpriteに入るように変換する
        byte[] pngdata = texture.EncodeToPNG();
        texture.LoadImage(pngdata);

        //先ほど作ったSpriteに画像をいれる
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

        //Spriteを使用するオブジェクトに指定する
        //     今回はUIのImage
        ImageSprite.GetComponent<Image>().sprite = sprite;

        // サイズ変更
        ImageSprite.GetComponent<RectTransform>().sizeDelta = new Vector2(texture.width, texture.height);

        //imageをアクティブにする
        ImageSprite.gameObject.SetActive(true);
        Flag = true;
    }

    IEnumerator GameOverProminence()
    {
        while (Flag)
        {
            yield return new WaitForEndOfFrame();
        }
        MainCam.SetActive(false);
        //マテリアル動作
        var mat = ImageSprite.GetComponent<Image>().material;
        mat.SetFloat("_Progress", 1.0f);
        float Value = mat.GetFloat("_Progress");
        while(Value > -1.0f)
        {
            yield return new WaitForEndOfFrame();
            Value -= 0.25f * Time.deltaTime;
            mat.SetFloat("_Progress", Value);
            
        }
        yield return new WaitForEndOfFrame();
        //ゲームマネージャのゲームオーバー用シーンチェンジ呼び出し
        gm.ChangeSceneGameOver();
    }
}

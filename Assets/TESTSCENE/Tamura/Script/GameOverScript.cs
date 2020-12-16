using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScript : MonoBehaviour
{
    [SerializeField, Header("ゲームオーバー用キャンバス")]
    Canvas overCanvas;
    [SerializeField, Header("橋カウント用キャンバス")]
    Canvas countCanvas;
    GameObject ImageSprite;
    GameManager gm;
    bool Flag = false;
    GameObject MainCam;
    [SerializeField, Header("UI_ボタン")]
    GameObject[] UIButton;

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
        var UIcountImage1 = countCanvas.transform.GetChild(0);
        var UIcountImage2 = countCanvas.transform.GetChild(1);
        var UIcountImage3 = countCanvas.transform.GetChild(2);
        UIcountImage1.SetParent(overCanvas.transform);
        UIcountImage2.SetParent(UIcountImage1.transform);
        UIcountImage3.SetParent(UIcountImage1.transform);

        
        var UICountImageRectTrans = UIcountImage1.GetComponent<RectTransform>();
        //UIアンカー変更（右上->中央）---------------------
        var parent = UICountImageRectTrans.parent as RectTransform;
        if (parent == null) { Debug.LogError("Parent cannot find."); }
        var diffMin = new Vector2(0.5f,0.5f) - UICountImageRectTrans.anchorMin;
        var diffMax = new Vector2(0.5f,0.5f) - UICountImageRectTrans.anchorMax;
        UICountImageRectTrans.anchorMin = new Vector2(0.5f, 0.5f);
        UICountImageRectTrans.anchorMax = new Vector2(0.5f, 0.5f);
        var diffleft = parent.rect.width * diffMin.x;
        var diffright = parent.rect.width * diffMax.x;
        var diffbottom = parent.rect.height * diffMin.y;
        var difftop = parent.rect.height * diffMax.y;

        UICountImageRectTrans.sizeDelta += new Vector2(diffleft - diffright, diffbottom - difftop);
        var pivot = UICountImageRectTrans.pivot;
        UICountImageRectTrans.anchoredPosition-=new Vector2(
            (diffleft*(1-pivot.x)) + (diffright*pivot.x),
        (diffbottom * (1 - pivot.y)) + (difftop * pivot.y));
        //-------------------------------------------------

        
        //UI位置変化・大きさ変更
        var CountPos = UICountImageRectTrans.position;
        var CountPosHalf = CountPos / 2;
        var UIScale = UICountImageRectTrans.localScale;
        float timer = 0;
        while (timer < 1)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime * 0.25f;
            UICountImageRectTrans.position = Vector3.Lerp(CountPos, CountPosHalf, timer);
            UICountImageRectTrans.localScale = Vector3.Lerp(UIScale, new Vector3(5, 5, 5), timer);
        }
        UICountImageRectTrans.position = CountPosHalf;

        //UI_ボタンを消す(背景キャプチャ焼いたとき、残り続けるため)
        for (int i = 0; i < UIButton.Length; i++)
            UIButton[i].SetActive(false);

        //マテリアル動作
        var mat = ImageSprite.GetComponent<Image>().material;
        mat.SetFloat("_Progress", 1.0f);
        float Value = mat.GetFloat("_Progress");
        //予備
        MainCam.SetActive(false);
        while (Value > -0.8f)
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

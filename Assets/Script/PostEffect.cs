using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostEffect : MonoBehaviour
{
    // アウトラインマテリアル
    public Material _MOutline;

    // 全てのレンダリング後にアウトラインのために呼ばれる関数
    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        //Debug.Log("Lead OnRenderImage");
        Graphics.Blit(src, dest, _MOutline);
    }
}

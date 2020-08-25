using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireLine : MonoBehaviour
{
    // 火オブジェクト
    [SerializeField]
    GameObject _firePrefab;

    // 火の数
    [SerializeField]
    private int _numberOfFire = 0;

    //===========================================================
    // コンストラクタ
    //===========================================================
    void Start()
    {
        if (_firePrefab != null)
        {
            StartCoroutine(CreateFireLine());
        }
    }

    //===========================================================
    // 更新
    //===========================================================
    void Update()
    {
    }

    //===========================================================
    // 火オブジェクト生成
    //===========================================================
    private void CreateFirePrefab(int value)
    {
        // 生成
        GameObject fire = Instantiate(_firePrefab) as GameObject;
        fire.transform.position = new Vector3(transform.position.x + 1.0f * value,
                                              transform.position.y,
                                              transform.position.z);
        // 子として登録
        fire.transform.parent = transform;
    }

    //===========================================================
    // 複数火オブジェクト生成
    //===========================================================
    IEnumerator CreateFireLine()
    {
        for(int i = 0; i < _numberOfFire; i++)
        {
            // 生成
            CreateFirePrefab(i);

            // 0.5秒間停止
            yield return new WaitForSeconds(0.5f);
        }
    }
}

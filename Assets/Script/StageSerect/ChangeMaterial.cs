using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChangeMaterial : MonoBehaviour
{
    [SerializeField] private Material Nolmal_Material = default;
    [SerializeField] private Material Serect_Material;

    bool SerectFlag = false;

    RectTransform h;

    private float Normal = 400.0f;
    private float Serect = 500.0f;

    Material material;

    private float _TexOffsetX = 0;

    [SerializeField] private float _MaxOffset = 7.0f; 

    private void Start()
    {
        h = this.GetComponent<RectTransform>();
        material = this.GetComponent<Image>().material;
    }

    public void None()
    {
        if (material == Serect_Material)
        {
            material.SetTextureOffset("_AddTex", new Vector2(_MaxOffset, 0.5f));
        }
        material = Nolmal_Material;
        material = this.GetComponent<Image>().material;
        h.sizeDelta = new Vector2(Normal, Normal);
        SerectFlag = false;
    }

    public void Flash()
    {
        material = Serect_Material;
        material = this.GetComponent<Image>().material;
        h.sizeDelta = new Vector2(Serect, Serect);
        SerectFlag = true;
    }

    private void Update()
    {
        _TexOffsetX += 0.01f;

        if (SerectFlag)
        {
            if (material == Serect_Material)
            {
                material.SetTextureOffset("_AddTex", new Vector2(_MaxOffset - (_TexOffsetX % _MaxOffset), 0.5f));
            }
        }
    }
}
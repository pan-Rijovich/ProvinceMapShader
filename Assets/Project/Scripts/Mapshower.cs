using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Mapshower : MonoBehaviour
{
    [SerializeField] Texture2D Remap;
    [SerializeField] Color32 targetColor;

    private Color32[] remapArr;
    private Color32[] palleteColorOfsets = new Color32[256 * 256];
    private Texture2D paletteTex;
    private Texture2D remapTex;
    private int width;
    private int height;
    private Color32 prevRemapColor;
    private bool selectAny = false;
    private Texture2D provinceTex;

    void Start()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        var material = GetComponent<Renderer>().material;
        provinceTex = material.GetTexture("_ProvinceTex") as Texture2D;

        var main2remap = new Dictionary<Color32, Color32>();
        remapArr = Remap.GetPixels32();
        //CalcRemap(main2remap);

        width = provinceTex.width;
        height = provinceTex.height;

        InitRemapTexture();
        InitPaletteTexture();
        material.SetTexture("_PaletteTex", paletteTex);
        material.SetTexture("_RemapTex", remapTex);

        stopwatch.Stop();
        UnityEngine.Debug.Log($"remap calc time:{stopwatch.Elapsed}\n");
    }

    public void SelectProvince(Vector2 position)
    {
        int x = (int)Mathf.Floor(position.x * width);
        int y = (int)Mathf.Floor(position.y * height);
        var remapColor = remapArr[x + y * width];
        Color color;

        if (selectAny == false || prevRemapColor.Equals(remapColor) == false)
        {
            if (selectAny) // deselect
            {
                ChangeColor(prevRemapColor, new Color32(255, 255, 255, 0));
            }

            selectAny = true;
            prevRemapColor = remapColor;
            ChangeColor(remapColor, targetColor);
            paletteTex.Apply(false);
        }
    }

    private void CalcRemap(Dictionary<Color32, Color32> main2remap)
    {
        var mainArr = provinceTex.GetPixels32();
        int idx = 0;

        for (int i = 0; i < mainArr.Length; i++)
        {
            var mainColor = mainArr[i];
            if (!main2remap.ContainsKey(mainColor))
            {
                var low = (byte)(idx % 256);
                var high = (byte)(idx / 256);
                main2remap[mainColor] = new Color32(low, high, 0, 255);
                idx++;
                palleteColorOfsets[(high << 8) + low] = mainColor;
            }
            var remapColor = main2remap[mainColor];
            remapArr[i] = remapColor;
        }
    }

    private void ChangeColor(Color32 remapColor, Color32 showColor)
    {
        int xPos = remapColor[0];
        int yPos = remapColor[1];
        paletteTex.SetPixel(xPos, yPos, showColor);
    }

    private void InitPaletteTexture()
    {
        var paletteArr = new Color32[256 * 256];
        for (int i = 0; i < paletteArr.Length; i++)
        {
            paletteArr[i] = new Color32(255, 255, 255, 0);
        }
        paletteTex = new Texture2D(256, 256, TextureFormat.RGBA32, false);
        paletteTex.filterMode = FilterMode.Point;
        paletteTex.SetPixels32(paletteArr);
        paletteTex.Apply(false);
    }

    private void InitRemapTexture()
    {
        remapTex = new Texture2D(width, height, TextureFormat.RG16, false);
        remapTex.filterMode = FilterMode.Point;
        remapTex.SetPixels32(remapArr);
        remapTex.Apply(false);
    }
}

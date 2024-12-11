using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Profiling;

public class Mapshower : MonoBehaviour
{
    //[SerializeField] Texture2D Remap;
    [SerializeField] Color32 targetColor;
    [SerializeField] ComputeShader computeShader;

    private Color32[] remapArr;
    private Color32[] palleteColorOfsets = new Color32[256 * 256];
    private Texture2D paletteTex;
    [SerializeField] private Texture2D remapTex;
    private int width;
    private int height;
    private Color32 prevRemapColor;
    private bool selectAny = false;
    private Texture2D provinceTex;

    void Start()
    {

        var material = GetComponent<Renderer>().material;
        provinceTex = material.GetTexture("_ProvinceTex") as Texture2D;
        
        width = provinceTex.width;
        height = provinceTex.height;

        InitRemapTexture(null);

        CalcRemap();

        InitPaletteTexture();
        material.SetTexture("_PaletteTex", paletteTex);
        material.SetTexture("_RemapTex", remapTex);

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

    private void CalcRemap()
    {
        Profiler.BeginSample("Calc remap");
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        var mainArr = provinceTex.GetRawTextureData();
        byte[] remapByteArr = new byte[(mainArr.Length / 3) * 2];

        int remapIdx = 0;
        int idx = 0;
        var main2remap = new Dictionary<int, short>(4000);

        for (int i = 0; i < mainArr.Length; i += 3)
        {
            int color24 = mainArr[i + 2] << 16 | mainArr[i + 1] << 8 | mainArr[i];
            short remapColor;
            if (!main2remap.ContainsKey(color24))
            {
                var low = (byte)(idx % 256);
                var high = (byte)(idx / 256);
                remapColor = (short)(low | high << 8);
                main2remap[color24] = remapColor;
                idx++;
            }
            else
            {
                remapColor = main2remap[color24];
            }
            remapByteArr[remapIdx++] = (byte)remapColor;
            remapByteArr[remapIdx++] = (byte)(remapColor >> 8);
        }

        var calcTime = stopwatch.Elapsed;
        UnityEngine.Debug.Log($"remap calc time:{calcTime}\n");

        remapTex.LoadRawTextureData(remapByteArr);
        remapTex.Apply(false);

        var fillTime = stopwatch.Elapsed - calcTime;
        UnityEngine.Debug.Log($"fill remapTex time:{fillTime}\n");

        remapArr = new Color32[mainArr.Length / 3];
        for (int i = 0; i < mainArr.Length / 3; i++)
        {
            remapArr[i] = new Color32(remapByteArr[i << 1], remapByteArr[(i << 1) + 1], 0, 255);
        }

        UnityEngine.Debug.Log($"fill remapArr time:{stopwatch.Elapsed - fillTime - calcTime}\n");

        stopwatch.Stop();
        UnityEngine.Debug.Log($"total time:{stopwatch.Elapsed}\n");
        Profiler.EndSample();
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

    private void InitRemapTexture(Color32[] remapArr)
    {
        remapTex = new Texture2D(width, height, TextureFormat.RG16, false);
        remapTex.filterMode = FilterMode.Point;
        if (remapArr != null) 
        {
            remapTex.SetPixels32(remapArr);
            remapTex.Apply(false);
        }
    }
}

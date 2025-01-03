using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Profiling;

public class Mapshower : MonoBehaviour
{
    [SerializeField] Color32 targetColor;

    private Color32[] remapArr;
    private Texture2D paletteTex;
    private Texture2D remapTex;
    private int width;
    private int height;
    private Color32 prevRemapColor;
    private Color32 prevPaletteColor;
    private bool selectAny = false;

    void Start()
    {
        var material = GetComponent<Renderer>().material;
        Texture2D provinceTex = material.GetTexture("_ProvinceTex") as Texture2D;
        
        width = provinceTex.width;
        height = provinceTex.height;

        InitRemapTexture(null);

        CalcRemap(provinceTex);

        InitPaletteTexture();
        material.SetTexture("_PaletteTex", paletteTex);
        material.SetTexture("_RemapTex", remapTex);
    }

    public void SelectProvince(Vector2 position)
    {
        int x = (int)Mathf.Floor(position.x * width);
        int y = (int)Mathf.Floor(position.y * height);
        var remapColor = remapArr[x + y * width];

        if (selectAny == false || prevRemapColor.Equals(remapColor) == false)
        {
            if (selectAny)
            {
                ChangePaletteColor(prevRemapColor, prevPaletteColor);
            }

            selectAny = true;
            prevRemapColor = remapColor;
            prevPaletteColor = GetPalleteColor(remapColor.r,remapColor.g);
            ChangePaletteColor(remapColor, targetColor);
            paletteTex.Apply(false);
        }
    }

    private Color32 GetPalleteColor(byte x, byte y) 
    {
        return paletteTex.GetPixel(x, y);
    }

    private void CalcRemap(Texture2D provinceTex)
    {
        Profiler.BeginSample("Calc remap");
        Stopwatch stopwatch = new();
        stopwatch.Start();

        byte[] provinceByteArr = provinceTex.GetRawTextureData();
        byte[] remapByteArr = new byte[(provinceByteArr.Length / 3) * 2];

        int remapIdx = 0;
        int uniqueProvinceIdx = 0;
        Dictionary<int, short> main2remap = new(4000);

        for (int i = 0; i < provinceByteArr.Length; i += 3)
        {
            int provinceColor24 = provinceByteArr[i + 2] << 16 | provinceByteArr[i + 1] << 8 | provinceByteArr[i];
            short remapColorRG16;
            if (!main2remap.ContainsKey(provinceColor24))
            {
                byte low = (byte)(uniqueProvinceIdx & 0xff); // mod 256
                byte high = (byte)(uniqueProvinceIdx >> 8); // div 256
                remapColorRG16 = (short)(low | high << 8);
                main2remap[provinceColor24] = remapColorRG16;
                uniqueProvinceIdx++;
            }
            else
            {
                remapColorRG16 = main2remap[provinceColor24];
            }
            remapByteArr[remapIdx++] = (byte)remapColorRG16;
            remapByteArr[remapIdx++] = (byte)(remapColorRG16 >> 8);
        }

        var calcTime = stopwatch.Elapsed;
        UnityEngine.Debug.Log($"remap calc time:{calcTime}\n");

        remapTex.LoadRawTextureData(remapByteArr);
        remapTex.Apply(false);

        var fillTime = stopwatch.Elapsed - calcTime;
        UnityEngine.Debug.Log($"fill remapTex time:{fillTime}\n");

        remapArr = new Color32[remapByteArr.Length / 2];
        for (int i = 0; i < remapByteArr.Length / 2; i++)
        {
            remapArr[i] = new Color32(remapByteArr[i << 1], remapByteArr[(i << 1) + 1], 0, 255);
        }

        UnityEngine.Debug.Log($"fill remapArr time:{stopwatch.Elapsed - fillTime - calcTime}\n");

        stopwatch.Stop();
        UnityEngine.Debug.Log($"total time:{stopwatch.Elapsed}\n");
        Profiler.EndSample();
    }

    private void ChangePaletteColor(Color32 remapColor, Color32 showColor)
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

    [ContextMenu("FillRandomColor")]
    private void FillRandomColorPaletteTexture()
    {
        var paletteArr = new Color32[256 * 256];
        var random = new System.Random();

        for (int i = 0; i < paletteArr.Length; i++)
        {
            byte r = (byte)random.Next(256);
            byte g = (byte)random.Next(256);
            byte b = (byte)random.Next(256);
            byte a = 150;
            paletteArr[i] = new Color32(r, g, b, a);
        }

        paletteTex.SetPixels32(paletteArr);
        paletteTex.Apply(false);
    }
}

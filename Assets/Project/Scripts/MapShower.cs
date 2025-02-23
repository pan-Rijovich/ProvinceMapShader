using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Project.Scripts.Configs;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Profiling;
using Debug = UnityEngine.Debug;

public class MapShower
{
    private Color32 _targetColor = new Color32(0, 0, 0, 150);
    private Texture2D _paletteTex;
    private Texture2D _remapTex;
    private Color32 _prevRemapColor;
    private Color32 _prevPaletteColor;
    private bool _isAnySelected = false;

    public MapShower(MapConfig config, Material material)
    {
        _remapTex = config.RemapTexture;
        
        InitPaletteTexture();
        material.SetTexture("_PaletteTex", _paletteTex);
        material.SetTexture("_RemapTex", _remapTex);

        //FillRandomColorPaletteTexture();
    }

    public void SelectProvince(Vector2 uv)
    {
        /*int x = (int)Mathf.Floor(uv.x * _remapTex.width);
        int y = (int)Mathf.Floor(uv.y * _remapTex.height);
        var remapColor = (Color32)_remapTex.GetPixel(x, y);

        if (_isAnySelected == false || _prevRemapColor.Equals(remapColor) == false)
        {
            if (_isAnySelected)
            {
                ChangePaletteColor(_prevRemapColor, _prevPaletteColor);
            }

            _isAnySelected = true;
            _prevRemapColor = remapColor;
            _prevPaletteColor = GetPaletteColor(remapColor.r,remapColor.g);
            ChangePaletteColor(remapColor, _targetColor);
            _paletteTex.Apply(false);
        }*/
    }

    private Color32 GetPaletteColor(byte x, byte y) 
    {
        return _paletteTex.GetPixel(x, y);
    }

    private void ChangePaletteColor(Color32 remapColor, Color32 showColor)
    {
        int xPos = remapColor[0];
        int yPos = remapColor[1];
        _paletteTex.SetPixel(xPos, yPos, showColor);
    }

    private void InitPaletteTexture()
    {
        var paletteArr = new Color32[256 * 256];
        for (int i = 0; i < paletteArr.Length; i++)
        {
            paletteArr[i] = new Color32(255, 255, 255, 0);
        }
        _paletteTex = new Texture2D(256, 256, TextureFormat.RGBA32, false);
        _paletteTex.filterMode = FilterMode.Point;
        _paletteTex.SetPixels32(paletteArr);
        _paletteTex.Apply(false);
    }
    
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

        _paletteTex.SetPixels32(paletteArr);
        _paletteTex.Apply(false);
    }
}

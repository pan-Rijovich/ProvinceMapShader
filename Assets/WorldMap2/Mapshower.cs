using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor.SceneManagement;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Mapshower : MonoBehaviour
{
    int width;
    int height;

    Color32[] remapArr;
    Texture2D paletteTex;

    [NonSerialized] public Color32[] palleteColorOfsets = new Color32[256*256];

    Color32 prevColor;
    bool selectAny = false;

    Texture2D mainTex;

    [SerializeField] private Color32 targetColor;

    // Start is called before the first frame update
    void Start()
    {
        // ������� ����� ������ Stopwatch
        Stopwatch stopwatch = new Stopwatch();

        // ��������� ��������� �������
        stopwatch.Start();

        var material = GetComponent<Renderer>().material;
        mainTex = material.GetTexture("_ProvinceTex") as Texture2D;
        var mainArr = mainTex.GetPixels32();

        width = mainTex.width;
        height = mainTex.height;

        var main2remap = new Dictionary<Color32, Color32>();
        remapArr = new Color32[mainArr.Length];

        CalcRemap(mainArr, main2remap);

        var paletteArr = new Color32[256 * 256];
        for (int i = 0; i < paletteArr.Length; i++)
        {
            paletteArr[i] = new Color32(255, 255, 255, 0);
        }

        var remapTex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        remapTex.filterMode = FilterMode.Point;
        remapTex.SetPixels32(remapArr);
        remapTex.Apply(false);
        material.SetTexture("_RemapTex", remapTex);

        //var file1 = File.OpenWrite(Application.dataPath + "/remapTex.png");
        //file1.Write(remapTex.EncodeToPNG());
        //file1.Close();

        //var Offsets = new Texture2D(256, 256, TextureFormat.RGBA32, false);
        //Offsets.filterMode = FilterMode.Point;
        //Offsets.SetPixels32(palleteColorOfsets);
        //Offsets.Apply(false);
        //var file = File.OpenWrite(Application.dataPath + "/Offsets.png");
        //file.Write(Offsets.EncodeToPNG());
        //file.Close();

        paletteTex = new Texture2D(256, 256, TextureFormat.RGBA32, false);
        paletteTex.filterMode = FilterMode.Point;
        paletteTex.SetPixels32(paletteArr);
        paletteTex.Apply(false);
        material.SetTexture("_PaletteTex", paletteTex);

        stopwatch.Stop();
        UnityEngine.Debug.Log($"remap calc time:{stopwatch.Elapsed}\n");
    }

    void CalcRemap(Color32[] mainArr, Dictionary<Color32, Color32> main2remap)
    {
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

    // Update is called once per frame
    void Update()
    {
        CheckButtons();

        var mousePos = Input.mousePosition;
        var ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo))
        {
            var p = hitInfo.textureCoord;
            int x = (int)Mathf.Floor(p.x * width);
            int y = (int)Mathf.Floor(p.y * height);

            var remapColor = remapArr[x + y * width];

            if (!selectAny || !prevColor.Equals(remapColor))
            {
                if (selectAny)
                {
                    changeColor(prevColor, new Color32(255, 255, 255, 0));
                }
                selectAny = true;
                prevColor = remapColor;
                changeColor(remapColor, targetColor);
                paletteTex.Apply(false);
            }
        }
    }

    private static void CheckButtons()
    {
        if (Input.GetKey(KeyCode.D))
        {
            Camera.main.transform.position += new Vector3(1, 0, 0);
        }
        if (Input.GetKey(KeyCode.A))
        {
            Camera.main.transform.position += new Vector3(-1, 0, 0);
        }
        if (Input.GetKey(KeyCode.W))
        {
            Camera.main.transform.position += new Vector3(0, 1, 0);
        }
        if (Input.GetKey(KeyCode.S))
        {
            Camera.main.transform.position += new Vector3(0, -1, 0);
        }

        var mouse_weel = Input.GetAxis("Mouse ScrollWheel");
        Camera.main.transform.position += new Vector3(0, 0, mouse_weel * 50);
    }

    void changeColor(Color32 remapColor, Color32 showColor){
        int xp = remapColor[0];
        int yp = remapColor[1];

        paletteTex.SetPixel(xp, yp, showColor);
    }
}

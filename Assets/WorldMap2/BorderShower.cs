using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TMPro;
using UnityEngine;

public class BorderShower : MonoBehaviour
{
    Dictionary<long, List<Float2>> borders = new();
    Dictionary<uint, List<long>> provincesBorders = new();
    Dictionary<long, List<Int2>> bordersInt = new();

    uint lastProvince = 0;
    uint gizmosProvince = 0;
    Vector3 meshSize;

    int enumerator = 0;
    Texture2D terrTex;
    Texture2D mainTex;

    [SerializeField] Texture2D palleteOffsets;

    int width;
    int height;

    List<List<Float2>> float2s = new();
    List<List<Float2>> deletedLines = new();
    [Range(0.35f,10f)][SerializeField] float gizmosSize;
    [SerializeField] bool _ChangeOffset;
    [SerializeField] bool _DisplaySinglePoints;
    [SerializeField] bool _DisplayDeletedLines;

    void Start()
    {
        var material = GetComponent<Renderer>().material;
        mainTex = material.GetTexture("_ProvinceTex") as Texture2D;
        var mainArr = mainTex.GetPixels32();

        width = mainTex.width;
        height = mainTex.height;

        terrTex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        var terrArr = new Color32[mainArr.Length];

        FillColorArray(terrArr, new Color32(255, 255, 255, 255));

        terrTex.SetPixels32(terrArr);
        terrTex.Apply(false);
        terrTex.filterMode = FilterMode.Point;
        material.SetTexture("_TerrainTex", terrTex);

        CalculatePoints(mainArr);
        ClearPointsList();

        terrTex.SetPixels32(terrArr);
        terrTex.Apply(false);
        terrTex.filterMode = FilterMode.Point;
        material.SetTexture("_TerrainTex", terrTex);

        foreach (var border in bordersInt) 
        {
            List<Float2> float2 = new();

            foreach (var intPoint in border.Value) 
            {
                Float2 point = new();
                point.position = intPoint.position;
                point.color = intPoint.color;

                point.x = (float)intPoint.x / (width * 2);
                point.y = (float)intPoint.y / (height * 2);
                float2.Add(point);
            }

            borders.Add(border.Key, float2);
        }

        foreach (var province in GetComponent<Mapshower>().palleteColorOfsets)
        {
            uint key = Color32ToUInt(province);

            if (!provincesBorders.TryGetValue(key, out var list))
            {
                list = new List<long>();
                provincesBorders[key] = list;
            }

            foreach (var i in borders.Keys)
            {
                if ((uint)i == key || (uint)(i >> 32) == key) list.Add(i);
            }
        }

        Debug.Log($"total borders:{borders.Count()}");
        foreach (var br in borders.Values)
        {
            if (br.Count < 2)
                Debug.Log($"total points in border:{br.Count}");
            if (br.Count < 2 && br.Count != 0)
            {
                Vector3 position = CalcPosFromUV(br[0], transform.position);
                float2s.Add(br);
            }

        }

        SortPointsIntoLine(width, height, mainArr);

        CalcMeshSize();
    }

    private void SortPointsIntoLine(int width, int height, Color32[] provinceArr)
    {
        float normalizedWidth = 1f / width;
        float normalizedHeight = 1f / height;

        foreach (var float2list in borders.Values.ToList())
        {
            if (float2list.Count < 2)
                continue;
            List<Float2> result = new List<Float2>
            {
                float2list.First()
            };
            float2list.RemoveAt(0);

            float threthholdDistance = Mathf.Sqrt(Mathf.Pow(normalizedWidth, normalizedWidth + normalizedWidth) + Mathf.Pow(normalizedHeight, normalizedHeight + normalizedHeight)) * 0.9f;
            
            FindLine(width, provinceArr, float2list, result, true);
            FindLine(width, provinceArr, float2list, result, false);

            List<List<Float2>> bordersList = new();

            while (float2list.Count > 0) 
            {
                bordersList.Add(new());
                bordersList.Last().Add(float2list.First());
                float2list.RemoveAt(0);
                FindLine(width, provinceArr, float2list, bordersList.Last(), true);
                FindLine(width, provinceArr, float2list, bordersList.Last(), false);

                if (bordersList.Last().Count < 1)
                    break;
            }

            if (float2list.Count > 0) 
            {
                var line = new List<Float2>();
                line.AddRange(float2list);
                deletedLines.Add(line);
                Debug.Log($"Deleted points count:{float2list.Count}");
            }
            float2list.Clear();
            float2list.AddRange(result);

            foreach (var border in bordersList) 
            {
                float2list.AddRange(border);
            }
        }

        static void AddPoint(List<Float2> float2list, List<Float2> result, Float2 point, bool append)
        {
            if(append)
                result.Add(point);
            else
                result.Insert(0, point);

            float2list.Remove(point);
        }

        static void FindLine(int width, Color32[] provinceArr, List<Float2> float2list, List<Float2> result, bool isFirst)
        {
            while (float2list.Count > 0)
            {
                int posUp, posDown, posLeft, posRight, posLeftUp, posRightUp, posLeftDown, posRightDown, posCurrent;

                Float2 currentPoint;

                if (isFirst)
                    currentPoint = result.Last();
                else
                    currentPoint = result.First();

                posUp = currentPoint.position + width;
                posDown = currentPoint.position - width;
                posLeft = currentPoint.position - 1;
                posRight = currentPoint.position + 1;
                posLeftUp = currentPoint.position - 1 + width;
                posRightUp = currentPoint.position + 1 + width;
                posLeftDown = currentPoint.position - 1 - width;
                posRightDown = currentPoint.position + 1 - width;
                posCurrent = currentPoint.position;

                var naibours = float2list.FindAll((x) =>
                {
                    if (x.position == posUp || x.position == posDown ||
                    x.position == posLeft || x.position == posRight ||
                    x.position == posRightDown || x.position == posRightUp ||
                    x.position == posLeftUp || x.position == posLeftDown)
                        return true;

                    return false;
                });

                if (naibours.Count == 0)
                    break;

                var pointsList = naibours.FindAll((x) =>
                {
                    if (x.position == posUp || x.position == posDown || x.position == posLeft || x.position == posRight)
                        return true;
                    return false;
                });

                if (pointsList.Count != 0)
                {
                    pointsList.Sort((a, b) => b.position.CompareTo(a.position));
                    bool added = false;
                    foreach (var point in pointsList)
                    {
                        if (point.position == posRight)
                        {
                            if (posRightUp >= provinceArr.Length || !provinceArr[posRightUp].CompareRGB(provinceArr[posRight]))
                            {
                                AddPoint(float2list, result, point, isFirst);
                                added = true;
                                break;
                            }
                        }
                        else if (point.position == posUp)
                        {
                            if (posRightUp >= provinceArr.Length ||  !provinceArr[posUp].CompareRGB(provinceArr[posRightUp]))
                            {
                                AddPoint(float2list, result, point, isFirst);
                                added = true;
                                break;
                            }
                        }
                        else if (point.position == posLeft)
                        {
                            if (posUp >= provinceArr.Length || !provinceArr[posUp].CompareRGB(provinceArr[posCurrent]))
                            {
                                AddPoint(float2list, result, point, isFirst);
                                added = true;
                                break;
                            }
                        }
                        else if (point.position == posDown)
                        {
                            if (posRight >= provinceArr.Length || !provinceArr[posCurrent].CompareRGB(provinceArr[posRight]))
                            {
                                AddPoint(float2list, result, point, isFirst);
                                added = true;
                                break;
                            }
                        }
                        else
                        {
                            AddPoint(float2list, result, point, isFirst);
                            added = true;
                            break;
                        }
                    }
                    if (added) continue;
                }

                //pointsList = naibours.FindAll((x) =>
                //{
                //    if (x.position == posRightDown || x.position == posRightUp || x.position == posLeftUp || x.position == posLeftDown)
                //        return true;
                //    return false;
                //});

                //if (pointsList.Count != 0)
                //{
                //    pointsList.Sort((a, b) => b.position.CompareTo(a.position));
                //    bool added = false;
                //    foreach (var point in pointsList)
                //    {
                //        if (point.position == posRightDown)
                //        {
                //            if (provinceArr[posCurrent].CompareRGB(provinceArr[posRight]))
                //            {
                //                AddPoint(float2list, result, point, isFirst);
                //                added = true;
                //                break;
                //            }
                //        }
                //        else
                //        {
                //            AddPoint(float2list, result, point, isFirst);
                //            added = true;
                //            break;
                //        }
                //    }
                //    if (added) continue;
                //}

                break;
            }
        }
    }

    private static void FillColorArray(Color32[] texture, Color32 targetColor)
    {
        for (int i = 0; i < texture.Length; i++)
        {
            texture[i] = targetColor;
        }
    }

    private void CalcMeshSize()
    {
        var mesh = GetComponent<MeshFilter>().mesh;

        if (mesh != null)
        {
            Vector3 vector3 = Vector3.zero;
            vector3 += mesh.bounds.size;
            vector3 = new Vector3(vector3.x * transform.localScale.x, vector3.y * transform.localScale.y, vector3.z * transform.localScale.z);
            meshSize = vector3;
        }
    }

    private void CalculatePoints(Color32[] mainArr)
    {
        for (int i = 0; i < mainArr.Length - 1; i++)
        {
            Color32 leftDown = mainArr[i];
            Color32 rightDown = mainArr[i + 1];
            Color32 leftUp = mainArr[i];
            if (i + width < mainArr.Length)
                leftUp = mainArr[i + width];

            if (!leftDown.CompareRGB(leftUp))
            {
                var result = CalculatePosFromIndex(i, width, height);
                result.y = (result.y + 1) * 2;
                result.x = (result.x + 1) * 2;
                result.color = 2;
                result.position = i;
                if (LessThanColor(leftDown, leftUp))
                {
                    
                    AddToDictionary(((long)Color32ToUInt(leftUp) << 32) + Color32ToUInt(leftDown), result, bordersInt);
                    result.position -= 1;
                    result.x -= 2;
                    AddToDictionary(((long)Color32ToUInt(leftUp) << 32) + Color32ToUInt(leftDown), result, bordersInt);
                }
                else
                {
                    AddToDictionary(((long)Color32ToUInt(leftDown) << 32) + Color32ToUInt(leftUp), result, bordersInt);
                    result.position -= 1;
                    result.x -= 2;
                    AddToDictionary(((long)Color32ToUInt(leftDown) << 32) + Color32ToUInt(leftUp), result, bordersInt);
                }
            }

            if (!leftDown.CompareRGB(rightDown))
            {
                var result = CalculatePosFromIndex(i, width, height);
                result.y = (result.y + 1) * 2;
                result.x = (result.x + 1) * 2;
                result.color = 1;
                result.position = i;
                if (LessThanColor(leftDown, rightDown))
                {
                    AddToDictionary(((long)Color32ToUInt(rightDown) << 32) + Color32ToUInt(leftDown), result, bordersInt);
                    result.position -= width;
                    result.y -= 2;
                    AddToDictionary(((long)Color32ToUInt(rightDown) << 32) + Color32ToUInt(leftDown), result, bordersInt);
                }
                else
                {
                    AddToDictionary(((long)Color32ToUInt(leftDown) << 32) + Color32ToUInt(rightDown), result, bordersInt);
                    result.position -= width;
                    result.y -= 2;
                    AddToDictionary(((long)Color32ToUInt(leftDown) << 32) + Color32ToUInt(rightDown), result, bordersInt);
                }
            }
        }
    }

    private void ClearPointsList()
    {
        foreach (var i in bordersInt)
        {
            var temp = i.Value.Distinct().ToList();
            i.Value.Clear();
            i.Value.AddRange(temp);
        }
    }

    private static void AddToDictionary(long key, Int2 value, Dictionary<long, List<Int2>> dictionary)
    {
        if (!dictionary.TryGetValue(key, out var list))
        {
            list = new List<Int2>();
            dictionary[key] = list;
        }

        list.Add(value);
    }

    private static Int2 CalculatePosFromIndex(int index, int width, int height) 
    {
        int y = (index / width);
        int x = (index % width);
        return new Int2(x, y);
    }

    private static Float2 CalculateUVFromIndex(int index, int width, int height) 
    {
        int y = (index / width);
        int x = (index % width);

        Float2 uv = new Float2();
        uv.x = (float)x / width;
        uv.y = (float)y / height;

        return uv;
    }

    private static bool LessThanColor(Color32 a, Color32 b)
    {
        return Color32ToUInt(a) < Color32ToUInt(b);
    }

    private static uint Color32ToUInt(Color32 color)
    {
        return (uint)(color.a << 24 | color.r << 16 | color.g << 8 | color.b);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) 
            enumerator++;
        if (Input.GetKeyDown(KeyCode.Alpha2) && enumerator > 0)
            enumerator--;

        var mousePos = Input.mousePosition;
        var ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo))
        {
            var p = hitInfo.textureCoord;
            int x = (int)Mathf.Floor(p.x * width);
            int y = (int)Mathf.Floor(p.y * height);

            //DrawBorder(mainTex.GetPixel(x, y));

            if(Input.GetKeyDown(KeyCode.Mouse0))
                gizmosProvince = Color32ToUInt(mainTex.GetPixel(x, y));
            //Debug.Log($"gizmos province:{gizmosProvince}");
        }
    }

    private void DrawBorder(Color color)
    {
        foreach (var i in provincesBorders[Color32ToUInt(color)]) 
        {
            foreach (var a in borders[i]) 
            {
                Color32[] terrArr = terrTex.GetPixels32();

                terrArr[a.position] = new Color32(255,0,0,255);
            }
        }

        terrTex.Apply(false);
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {

            foreach (var a in provincesBorders[gizmosProvince])
            {
                //var a = provincesBorders[gizmosProvince][enumerator % provincesBorders[gizmosProvince].Count];
                //Color32 first = new Color32((byte)(a >> 24), (byte)(a >> 16), (byte)(a >> 8), (byte)a);
                //Color32 second = new Color32((byte)(a >> 56), (byte)(a >> 48), (byte)(a >> 40), (byte)(a >> 32));

                //Debug.Log($"Color 1:{first}; Color 2:{second}");

                for (int index = 0; index < borders[a].Count - 1; index++)
                {
                    var i = borders[a][index];
                    //Debug.Log(i);

                    if (i.color == 1)
                        Gizmos.color = Color.red;
                    else if (i.color == 2)
                        Gizmos.color = Color.blue;
                    else
                        Gizmos.color = Color.black;

                    Vector3 gizmosPos = this.transform.position;

                    if (i.color == 1)
                        gizmosPos.z += 0.35f;

                    gizmosPos = CalcPosFromUV(i, gizmosPos);

                    Gizmos.DrawLine(CalcPosFromUV(i, transform.position), CalcPosFromUV(borders[a][index + 1], transform.position));

                    if (i.color == 1)
                        Gizmos.DrawSphere(gizmosPos, 0.35f);
                    else if (i.color == 2)
                        Gizmos.DrawSphere(gizmosPos, 0.15f);

                }
            }
            DisplaySinglePoints();
            DisplayDeletedLines();
        }
    }

    private void DisplayDeletedLines()
    {
        if (_DisplayDeletedLines)
        {
            foreach (var line in deletedLines)
            {
                for (int index = 0; index < line.Count - 1; index++)
                {
                    var i = line[index];
                    //Debug.Log(i);

                    if (i.color == 1)
                        Gizmos.color = Color.red;
                    else if (i.color == 2)
                        Gizmos.color = Color.blue;
                    else
                        Gizmos.color = Color.black;

                    Vector3 gizmosPos = this.transform.position;

                    if (i.color == 1)
                        gizmosPos.z += 0.35f;

                    gizmosPos = CalcPosFromUV(i, gizmosPos);

                    Gizmos.DrawLine(CalcPosFromUV(i, transform.position), CalcPosFromUV(line[index + 1], transform.position));

                    if (i.color == 1)
                        Gizmos.DrawSphere(gizmosPos, 0.35f);
                    else if (i.color == 2)
                        Gizmos.DrawSphere(gizmosPos, 0.15f);
                }
            }
        }
    }

    private void DisplaySinglePoints()
    {
        if (_DisplaySinglePoints)
        {
            foreach (var br in float2s)
            {
                Vector3 gizmosPos = this.transform.position;

                gizmosPos = CalcPosFromUV(br[0], gizmosPos);

                if (br[0].color == 1)
                    Gizmos.DrawSphere(gizmosPos, gizmosSize);
                else if (br[0].color == 2)
                    Gizmos.DrawSphere(gizmosPos, gizmosSize);
            }
        }
    }

    private Vector3 CalcPosFromUV(Float2 i, Vector3 meshCenter)
    {

        meshCenter.x -= meshSize.x / 2;
        meshCenter.y -= meshSize.y / 2;

        meshCenter.x += i.x * meshSize.x;
        meshCenter.y += i.y * meshSize.y;

        if (_ChangeOffset) 
        {
            meshCenter.x += 1;
            meshCenter.y += 1;
        }

        return meshCenter;
    }
}

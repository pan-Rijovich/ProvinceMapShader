using MapBorderRenderer;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class BorderShower : MonoBehaviour
{
    [Range(0.05f, 1f)]
    [SerializeField] private float _gizmosSize;
    [SerializeField] private bool _DisplaySinglePoints;
    [SerializeField] private bool _DisplayDeletedLines;
    [SerializeField] private bool _DrawGizmos;
    [SerializeField] private LineRenderer _lineRendererPrefab;

    private readonly Dictionary<long, List<List<Float2>>> _bordersFloat = new();
    private readonly Dictionary<uint, List<long>> _provincesBorders = new();
    private readonly Dictionary<long, List<Int2>> _bordersInt = new();

    private uint _gizmosProvince = 0;
    private Vector3 _meshSize;

    private int _enumerator = 0;
    private Texture2D _terrTex;
    private Texture2D _provinceTex;

    private int _provinceTexWidth;
    private int _provinceTexHeight;

    private readonly List<List<Float2>> _float2s = new();
    private readonly List<List<Float2>> _deletedLines = new();



    private readonly List<LineRenderer> _lineRenderer = new();
    private Material _material;


    private void Start()
    {
        _material = GetComponent<Renderer>().material;
        _provinceTex = _material.GetTexture("_ProvinceTex") as Texture2D;
        var provinceArr = _provinceTex.GetPixels32();

        _provinceTexWidth = _provinceTex.width;
        _provinceTexHeight = _provinceTex.height;

        //DrawTerrainTextureInWhite(provinceArr);

        CalculatePoints(provinceArr);
        ClearPointsIntList(_bordersInt);
        ConvertToFloat2();

        WriteBordersInPovince();

        Debug.Log($"total borders:{_bordersFloat.Count()}");
        foreach (var br in _bordersFloat) //one point in border debug log
        {
            if (br.Value[0].Count < 2)
                Debug.LogWarning($"Found borders with points count less than 2. Border id:{br.Key}");
        }

        SortPointsIntoLine(_provinceTexWidth, _provinceTexHeight, provinceArr);

        FilterStraightPoints();

        CalcMeshSize();

        foreach (var province in _provincesBorders.Keys)
        {
            DrawLines(province);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            _enumerator++;
        if (Input.GetKeyDown(KeyCode.Alpha2) && _enumerator > 0)
            _enumerator--;

        var mousePos = Input.mousePosition;
        var ray = Camera.main.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            var p = hitInfo.textureCoord;
            int x = (int)Mathf.Floor(p.x * _provinceTexWidth);
            int y = (int)Mathf.Floor(p.y * _provinceTexHeight);

            //DrawBorder(_provinceTex.GetPixel(x, y));

            Color32 color = _provinceTex.GetPixel(x, y);
            var provinceID = color.Color32ToUInt();

            if (Input.GetKeyDown(KeyCode.Mouse0))
                _gizmosProvince = provinceID;
            if (Input.GetKeyDown(KeyCode.Mouse0))
                LineRendererBorderSmooth(provinceID);
            //Debug.Log($"gizmos province:{gizmosProvince}");
        }
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying && _DrawGizmos)
        {
            if (!_provincesBorders.ContainsKey(_gizmosProvince)) return;
            foreach (var a in _provincesBorders[_gizmosProvince])
            {
                foreach (var b in _bordersFloat[a])
                {
                    for (int index = 0; index < b.Count - 1; index++)
                    {
                        var i = b[index];

                        if (i.debugColor == 1)
                            Gizmos.color = Color.red;
                        else if (i.debugColor == 2)
                            Gizmos.color = Color.blue;
                        else
                            Gizmos.color = Color.black;

                        Vector3 gizmosPos = this.transform.position;

                        if (i.debugColor == 1)
                            gizmosPos.z += 0.35f;

                        gizmosPos = CalcPosFromUV(i, gizmosPos);

                        Gizmos.DrawLine(CalcPosFromUV(i, transform.position), CalcPosFromUV(b[index + 1], transform.position));

                        /*                        if (i.debugColor == 1)
                                                    Gizmos.DrawSphere(gizmosPos, 0.35f);
                                                else if (i.debugColor == 2)
                                                    Gizmos.DrawSphere(gizmosPos, 0.15f);*/
                        Gizmos.DrawSphere(gizmosPos, _gizmosSize);
                    }
                }
            }
            DisplaySinglePoints();
            DisplayDeletedLines();
        }
    }

    private void FilterStraightPoints()
    {
        foreach (var br in _bordersFloat.Values)
        {
            foreach (var b in br)
            {
                var border = new List<Float2>();

                for (int i = 1; i < b.Count - 1; i++)
                {
                    int posUp = b[i].positionIndex + _provinceTexWidth;
                    int posDown = b[i].positionIndex - _provinceTexWidth;
                    int posLeft = b[i].positionIndex - 1;
                    int posRight = b[i].positionIndex + 1;
                    var prev = b[i - 1].positionIndex;
                    var next = b[i + 1].positionIndex;
                    bool IsHorisontal = prev == posLeft && next == posRight || prev == posRight && next == posLeft;
                    bool IsVertical = prev == posDown && next == posUp || prev == posUp && next == posDown;
                    bool IsInnerCorner = true;
                    bool ISOuterCorner = prev == posDown && next == posRight || prev == posRight && next == posDown;

                    if (!IsHorisontal && !IsVertical)
                    {
                        border.Add(b[i]);
                    }
                }

                border.Insert(0, b[0]);
                border.Add(b.Last());

                b.Clear();
                b.AddRange(border);
            }
        }
    }

    private void WriteBordersInPovince()
    {
        foreach (var border in _bordersFloat)
        {
            uint province1ID = (uint)border.Key;
            uint province2ID = (uint)(border.Key >> 32);

            AddToListInDictionary(province1ID, border.Key, _provincesBorders);
            AddToListInDictionary(province2ID, border.Key, _provincesBorders);
        }
    }

    private void ConvertToFloat2()
    {
        foreach (var borderInt in _bordersInt)
        {
            List<Float2> borderFloat = new();

            foreach (var intPoint in borderInt.Value)
            {
                Float2 floatPoint = new()
                {
                    positionIndex = intPoint.positionIndex,
                    debugColor = intPoint.debugColor,

                    x = (float)intPoint.x / (_provinceTexWidth * 2), 
                    y = (float)intPoint.y / (_provinceTexHeight * 2)
                };
                borderFloat.Add(floatPoint);
            }
            List<List<Float2>> result = new()
            {
                borderFloat
            };
            _bordersFloat.Add(borderInt.Key, result);
        }
    }

    private void SortPointsIntoLine(int width, int height, Color32[] provinceArr)
    {
        float normalizedWidth = 1f / width;
        float normalizedHeight = 1f / height;

        foreach (var a in _bordersFloat.Values)
        {
            var float2list = a[0];
            if (float2list.Count < 2)
                continue;

            List<List<Float2>> bordersList = new();

            while (float2list.Count > 0) 
            {
                bordersList.Add(new());

                var currentBorder = bordersList.Last();

                currentBorder.Add(float2list.First());
                float2list.RemoveAt(0);
                FindLine(width, provinceArr, float2list, currentBorder, true);
                FindLine(width, provinceArr, float2list, currentBorder, false);

                int posUp = currentBorder.Last().positionIndex + width;
                int posDown = currentBorder.Last().positionIndex - width;
                int posLeft = currentBorder.Last().positionIndex - 1;
                int posRight = currentBorder.Last().positionIndex + 1;

                if (currentBorder.First().positionIndex == posUp ||
                    currentBorder.First().positionIndex == posDown ||
                    currentBorder.First().positionIndex == posLeft ||
                    currentBorder.First().positionIndex == posRight) 
                {
                    currentBorder.Add(currentBorder.First());
                }

                if (currentBorder.Count < 1)
                    break;
            }

            if (float2list.Count > 0) 
            {
                var line = new List<Float2>();
                line.AddRange(float2list);
                _deletedLines.Add(line);
                Debug.Log($"Deleted points count:{float2list.Count}");
            }
            float2list.Clear();

            a.Clear();
            a.AddRange(bordersList);
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

                posUp = currentPoint.positionIndex + width;
                posDown = currentPoint.positionIndex - width;
                posLeft = currentPoint.positionIndex - 1;
                posRight = currentPoint.positionIndex + 1;
                posLeftUp = currentPoint.positionIndex - 1 + width;
                posRightUp = currentPoint.positionIndex + 1 + width;
                posLeftDown = currentPoint.positionIndex - 1 - width;
                posRightDown = currentPoint.positionIndex + 1 - width;
                posCurrent = currentPoint.positionIndex;

                var naibours = float2list.FindAll((x) =>
                {
                    if (x.positionIndex == posUp || x.positionIndex == posDown ||
                    x.positionIndex == posLeft || x.positionIndex == posRight)
                        return true;

                    return false;
                });

                if (naibours.Count == 0)
                    break;

                var pointsList = naibours.FindAll((x) =>
                {
                    if (x.positionIndex == posUp || x.positionIndex == posDown || x.positionIndex == posLeft || x.positionIndex == posRight)
                        return true;
                    return false;
                });

                if (pointsList.Count != 0)
                {
                    pointsList.Sort((a, b) => b.positionIndex.CompareTo(a.positionIndex));
                    bool added = false;
                    foreach (var point in pointsList)
                    {
                        if (point.positionIndex == posRight)
                        {
                            if (posRightUp >= provinceArr.Length || !provinceArr[posRightUp].CompareRGB(provinceArr[posRight]))
                            {
                                AddPoint(float2list, result, point, isFirst);
                                added = true;
                                break;
                            }
                        }
                        else if (point.positionIndex == posUp)
                        {
                            if (posRightUp >= provinceArr.Length ||  !provinceArr[posUp].CompareRGB(provinceArr[posRightUp]))
                            {
                                AddPoint(float2list, result, point, isFirst);
                                added = true;
                                break;
                            }
                        }
                        else if (point.positionIndex == posLeft)
                        {
                            if (posUp >= provinceArr.Length || !provinceArr[posUp].CompareRGB(provinceArr[posCurrent]))
                            {
                                AddPoint(float2list, result, point, isFirst);
                                added = true;
                                break;
                            }
                        }
                        else if (point.positionIndex == posDown)
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

                break;
            }
        }
    }

    //make it static
    private void CalcMeshSize()
    {
        var mesh = GetComponent<MeshFilter>().mesh;

        if (mesh != null)
        {
            Vector3 vector3 = Vector3.zero;
            vector3 += mesh.bounds.size;
            vector3 = new Vector3(vector3.x * transform.localScale.x, vector3.y * transform.localScale.y, vector3.z * transform.localScale.z);
            _meshSize = vector3;
        }
    }

    private void CalculatePoints(Color32[] provinceArr)
    {
        for (int i = 0; i < provinceArr.Length - 1; i++)
        {
            Color32 current = provinceArr[i];
            Color32 right = provinceArr[i + 1];
            Color32 up = provinceArr[i];
            if (i + _provinceTexWidth < provinceArr.Length)
                up = provinceArr[i + _provinceTexWidth];

            if (!current.CompareRGB(up))
            {
                var posInTex = CalculatePosFromIndex(i, _provinceTexWidth, _provinceTexHeight); //province
                posInTex.y = (posInTex.y + 1) * 2; //convert in halfpixel
                posInTex.x = (posInTex.x + 1) * 2;
                posInTex.debugColor = 2;
                posInTex.positionIndex = i;

                long id = GenerateBorderID(current, up);
                AddToListInDictionary(id, posInTex, _bordersInt);
                posInTex.positionIndex -= 1;
                posInTex.x -= 2;
                AddToListInDictionary(id, posInTex, _bordersInt);
            }

            if (!current.CompareRGB(right))
            {
                var posInTex = CalculatePosFromIndex(i, _provinceTexWidth, _provinceTexHeight);
                posInTex.y = (posInTex.y + 1) * 2;
                posInTex.x = (posInTex.x + 1) * 2;
                posInTex.debugColor = 1;
                posInTex.positionIndex = i;

                long id = GenerateBorderID(current, right);

                AddToListInDictionary(id, posInTex, _bordersInt);
                posInTex.positionIndex -= _provinceTexWidth;
                posInTex.y -= 2;
                AddToListInDictionary(id, posInTex, _bordersInt);
            }
        }
    }

    private long GenerateBorderID(Color32 color1, Color32 color2)
    {
        long id;
        if (color1.LessThanColor(color2))
        {
            id = ((long)color2.Color32ToUInt() << 32) + color1.Color32ToUInt();
        }
        else
        {
            id = ((long)color1.Color32ToUInt() << 32) + color2.Color32ToUInt();
        }
        return id;
    }






    private void LineRendererBorder(uint color) 
    {
        if (_lineRenderer.Count == 0)
        {
            DrawLines(color);
        }
        else
        {
            foreach (var line in _lineRenderer) 
            {
                Destroy(line.gameObject);
            }
            _lineRenderer.Clear();
            DrawLines(color);
        }
    }

    private void LineRendererBorderSmooth(uint color)
    {
        if (_lineRenderer.Count == 0)
        {
            DrawLines(color);
        }
        else
        {
            foreach (var line in _lineRenderer)
            {
                Destroy(line.gameObject);
            }
            _lineRenderer.Clear();
            DrawLines(color);
        }
    }

    private void DrawLines(uint color)
    {
        foreach (var a in _provincesBorders[color])
        {
            foreach (var b in _bordersFloat[a])
            {
                _lineRenderer.Add(Instantiate(_lineRendererPrefab));
                Vector3[] points = new Vector3[b.Count];
                for (int i = 0; i < b.Count; i++) 
                {
                    points[i] = CalcPosFromUV(b[i], transform.position) + new Vector3(0, 0, -0.01f);
                }

                var pointCount = points.Length * 2; // + points.Length / 2;
                //_lineRenderer.Last().positionCount = pointCount;
                //_lineRenderer.Last().SetPositions(BSpline.CalculateBSpline(points.ToList(), pointCount).ToArray());
                _lineRenderer.Last().positionCount = points.Length;
                _lineRenderer.Last().SetPositions(points);
            }
        }
    }

    private void DrawBorder(Color32 color)
    {
        foreach (var i in _provincesBorders[color.Color32ToUInt()]) 
        {
            foreach (var a in _bordersFloat[i]) 
            {
                foreach (var b in a)
                {
                    Color32[] terrArr = _terrTex.GetPixels32();

                    terrArr[b.positionIndex] = new Color32(255, 0, 0, 255);
                }
            }
        }

        _terrTex.Apply(false);
    }

    private void DisplayDeletedLines()
    {
        if (_DisplayDeletedLines)
        {
            foreach (var line in _deletedLines)
            {
                for (int index = 0; index < line.Count - 1; index++)
                {
                    var i = line[index];

                    if (i.debugColor == 1)
                        Gizmos.color = Color.red;
                    else if (i.debugColor == 2)
                        Gizmos.color = Color.blue;
                    else
                        Gizmos.color = Color.black;

                    Vector3 gizmosPos = this.transform.position;

                    if (i.debugColor == 1)
                        gizmosPos.z += 0.35f;

                    gizmosPos = CalcPosFromUV(i, gizmosPos);

                    Gizmos.DrawLine(CalcPosFromUV(i, transform.position), CalcPosFromUV(line[index + 1], transform.position));

                    if (i.debugColor == 1)
                        Gizmos.DrawSphere(gizmosPos, 0.35f);
                    else if (i.debugColor == 2)
                        Gizmos.DrawSphere(gizmosPos, 0.15f);
                }
            }
        }
    }

    private void DisplaySinglePoints()
    {
        if (_DisplaySinglePoints)
        {
            foreach (var br in _float2s)
            {
                Vector3 gizmosPos = this.transform.position;

                gizmosPos = CalcPosFromUV(br[0], gizmosPos);

                if (br[0].debugColor == 1)
                    Gizmos.DrawSphere(gizmosPos, _gizmosSize);
                else if (br[0].debugColor == 2)
                    Gizmos.DrawSphere(gizmosPos, _gizmosSize);
            }
        }
    }

    //make it static
    private Vector3 CalcPosFromUV(Float2 i, Vector3 meshCenter)
    {

        meshCenter.x -= _meshSize.x / 2;
        meshCenter.y -= _meshSize.y / 2;

        meshCenter.x += i.x * _meshSize.x;
        meshCenter.y += i.y * _meshSize.y;

        return meshCenter;
    }


    private void DrawTerrainTextureInWhite(Color32[] provinceArr)
    {
        _terrTex = new Texture2D(_provinceTexWidth, _provinceTexHeight, TextureFormat.RGBA32, false);
        var terrArr = new Color32[provinceArr.Length];

        for (int i = 0; i < terrArr.Length; i++)
        {
            terrArr[i] = new Color32(255, 255, 255, 255);
        }

        SetTexture(_terrTex, terrArr, "_TerrainTex");



        _terrTex.SetPixels32(new Color32[provinceArr.Length]);
        _terrTex.Apply(false);
        _terrTex.filterMode = FilterMode.Point;
        _material.SetTexture("_TerrainTex", _terrTex);
    }

















    private void SetTexture(Texture2D texture, Color32[] terrArr, string textureName)
    {
        texture.SetPixels32(terrArr);
        texture.Apply(false);
        texture.filterMode = FilterMode.Point;
        _material.SetTexture(textureName, texture);
    }

    private static void ClearPointsIntList<T, U>(Dictionary<T, List<U>> borders)
    {
        foreach (var i in borders)
        {
            var temp = i.Value.Distinct().ToList();
            i.Value.Clear();
            i.Value.AddRange(temp);
        }
    }

    private static void AddToListInDictionary<T, U>(T key, U value, Dictionary<T, List<U>> dictionary)
    {
        if (!dictionary.TryGetValue(key, out var list))
        {
            list = new List<U>();
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

        Float2 uv = new()
        {
            x = (float)x / width,
            y = (float)y / height
        };

        return uv;
    }
}

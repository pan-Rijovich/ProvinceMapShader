using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class BorderShower : MonoBehaviour
{
    private readonly Dictionary<long, List<List<Float2>>> _borders = new();
    private readonly Dictionary<uint, List<long>> _provincesBorders = new();
    private readonly Dictionary<long, List<Int2>> _bordersInt = new();

    private uint _gizmosProvince = 0;
    private Vector3 _meshSize;

    private int _enumerator = 0;
    private Texture2D _terrTex;
    private Texture2D _mainTex;

    [SerializeField] private Texture2D _palleteOffsets;

    private int _width;
    private int _height;

    private readonly List<List<Float2>> _float2s = new();
    private readonly List<List<Float2>> _deletedLines = new();

    [Range(0.35f,10f)]
    [SerializeField] private float _gizmosSize;

    [SerializeField] private bool _DisplaySinglePoints;
    [SerializeField] private bool _DisplayDeletedLines;

    [SerializeField] private LineRenderer _lineRendererPrefab;

    private readonly List<LineRenderer> _lineRenderer = new();

    private void Start()
    {
        var material = GetComponent<Renderer>().material;
        _mainTex = material.GetTexture("_ProvinceTex") as Texture2D;
        var mainArr = _mainTex.GetPixels32();

        _width = _mainTex.width;
        _height = _mainTex.height;

        _terrTex = new Texture2D(_width, _height, TextureFormat.RGBA32, false);
        var terrArr = new Color32[mainArr.Length];

        FillColorArray(terrArr, new Color32(255, 255, 255, 255));

        SetTexture(_terrTex, material, terrArr, "_TerrainTex");

        CalculatePoints(mainArr);
        ClearPointsIntList(_bordersInt);

        _terrTex.SetPixels32(terrArr);
        _terrTex.Apply(false);
        _terrTex.filterMode = FilterMode.Point;
        material.SetTexture("_TerrainTex", _terrTex);

        ConvertToFloat2();

        foreach (var border in _borders) 
        {
            uint color1 = (uint)border.Key;
            uint color2 = (uint)(border.Key >> 32);

            AddToDictionary(color1, border.Key, _provincesBorders);
            AddToDictionary(color2, border.Key, _provincesBorders);
        }

        Debug.Log($"total borders:{_borders.Count()}");
        foreach (var br in _borders)
        {
            if (br.Value[0].Count < 2)
                Debug.LogWarning($"Found borders with points count less than 2. Border id:{br.Key}");
        }

        SortPointsIntoLine(_width, _height, mainArr);

        CalcMeshSize();

        //foreach (var province in _provincesBorders.Keys) 
        //{
        //    DrawLines(province);
        //}
    }

    private void ConvertToFloat2()
    {
        foreach (var border in _bordersInt)
        {
            List<Float2> float2 = new();

            foreach (var intPoint in border.Value)
            {
                Float2 point = new()
                {
                    position = intPoint.position,
                    color = intPoint.color,

                    x = (float)intPoint.x / (_width * 2),
                    y = (float)intPoint.y / (_height * 2)
                };
                float2.Add(point);
            }
            List<List<Float2>> result = new()
            {
                float2
            };
            _borders.Add(border.Key, result);
        }
    }

    private void SortPointsIntoLine(int width, int height, Color32[] provinceArr)
    {
        float normalizedWidth = 1f / width;
        float normalizedHeight = 1f / height;

        foreach (var a in _borders.Values)
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

                int posUp = currentBorder.Last().position + width;
                int posDown = currentBorder.Last().position - width;
                int posLeft = currentBorder.Last().position - 1;
                int posRight = currentBorder.Last().position + 1;

                if (currentBorder.First().position == posUp ||
                    currentBorder.First().position == posDown ||
                    currentBorder.First().position == posLeft ||
                    currentBorder.First().position == posRight) 
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
                    x.position == posLeft || x.position == posRight)
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

    private void CalculatePoints(Color32[] mainArr)
    {
        for (int i = 0; i < mainArr.Length - 1; i++)
        {
            Color32 leftDown = mainArr[i];
            Color32 rightDown = mainArr[i + 1];
            Color32 leftUp = mainArr[i];
            if (i + _width < mainArr.Length)
                leftUp = mainArr[i + _width];

            if (!leftDown.CompareRGB(leftUp))
            {
                var result = CalculatePosFromIndex(i, _width, _height);
                result.y = (result.y + 1) * 2;
                result.x = (result.x + 1) * 2;
                result.color = 2;
                result.position = i;
                if (LessThanColor(leftDown, leftUp))
                {
                    
                    AddToDictionary(((long)Color32ToUInt(leftUp) << 32) + Color32ToUInt(leftDown), result, _bordersInt);
                    result.position -= 1;
                    result.x -= 2;
                    AddToDictionary(((long)Color32ToUInt(leftUp) << 32) + Color32ToUInt(leftDown), result, _bordersInt);
                }
                else
                {
                    AddToDictionary(((long)Color32ToUInt(leftDown) << 32) + Color32ToUInt(leftUp), result, _bordersInt);
                    result.position -= 1;
                    result.x -= 2;
                    AddToDictionary(((long)Color32ToUInt(leftDown) << 32) + Color32ToUInt(leftUp), result, _bordersInt);
                }
            }

            if (!leftDown.CompareRGB(rightDown))
            {
                var result = CalculatePosFromIndex(i, _width, _height);
                result.y = (result.y + 1) * 2;
                result.x = (result.x + 1) * 2;
                result.color = 1;
                result.position = i;
                if (LessThanColor(leftDown, rightDown))
                {
                    AddToDictionary(((long)Color32ToUInt(rightDown) << 32) + Color32ToUInt(leftDown), result, _bordersInt);
                    result.position -= _width;
                    result.y -= 2;
                    AddToDictionary(((long)Color32ToUInt(rightDown) << 32) + Color32ToUInt(leftDown), result, _bordersInt);
                }
                else
                {
                    AddToDictionary(((long)Color32ToUInt(leftDown) << 32) + Color32ToUInt(rightDown), result, _bordersInt);
                    result.position -= _width;
                    result.y -= 2;
                    AddToDictionary(((long)Color32ToUInt(leftDown) << 32) + Color32ToUInt(rightDown), result, _bordersInt);
                }
            }
        }
    }

    private static void SetTexture(Texture2D texture, Material material, Color32[] terrArr, string textureName)
    {
        texture.SetPixels32(terrArr);
        texture.Apply(false);
        texture.filterMode = FilterMode.Point;
        material.SetTexture(textureName, texture);
    }

    private static void ClearPointsIntList<T,U>(Dictionary<T,List<U>> borders)
    {
        foreach (var i in borders)
        {
            var temp = i.Value.Distinct().ToList();
            i.Value.Clear();
            i.Value.AddRange(temp);
        }
    }

    private static void FillColorArray(Color32[] texture, Color32 targetColor)
    {
        for (int i = 0; i < texture.Length; i++)
        {
            texture[i] = targetColor;
        }
    }

    private static void AddToDictionary<T,U>(T key, U value, Dictionary<T, List<U>> dictionary)
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

    private static bool LessThanColor(Color32 a, Color32 b)
    {
        return Color32ToUInt(a) < Color32ToUInt(b);
    }

    private static uint Color32ToUInt(Color32 color)
    {
        return (uint)(color.a << 24 | color.r << 16 | color.g << 8 | color.b);
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
            int x = (int)Mathf.Floor(p.x * _width);
            int y = (int)Mathf.Floor(p.y * _height);

            //DrawBorder(mainTex.GetPixel(x, y));

            if (Input.GetKeyDown(KeyCode.Mouse0))
                _gizmosProvince = Color32ToUInt(_mainTex.GetPixel(x, y));
            if (Input.GetKeyDown(KeyCode.Mouse0))
                LineRendererBorderSmooth(Color32ToUInt(_mainTex.GetPixel(x, y)));
            //Debug.Log($"gizmos province:{gizmosProvince}");
        }
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
            foreach (var b in _borders[a])
            {
                _lineRenderer.Add(Instantiate(_lineRendererPrefab));
                Vector3[] points = new Vector3[b.Count];
                for (int i = 0; i < b.Count; i++) 
                {
                    points[i] = CalcPosFromUV(b[i], transform.position) + new Vector3(0, 0, -0.01f);
                }

                _lineRenderer.Last().positionCount = b.Count * 2;
                _lineRenderer.Last().SetPositions(BSpline.CalculateBSpline(points.ToList(), b.Count * 2).ToArray());
            }
        }
    }

    private void DrawBorder(Color color)
    {
        foreach (var i in _provincesBorders[Color32ToUInt(color)]) 
        {
            foreach (var a in _borders[i]) 
            {
                foreach (var b in a)
                {
                    Color32[] terrArr = _terrTex.GetPixels32();

                    terrArr[b.position] = new Color32(255, 0, 0, 255);
                }
            }
        }

        _terrTex.Apply(false);
    }

    private void NoGizmos()
    {
        if (Application.isPlaying)
        {
            if (!_provincesBorders.ContainsKey(_gizmosProvince)) return;
            foreach (var a in _provincesBorders[_gizmosProvince])
            {
                foreach (var b in _borders[a])
                {
                    for (int index = 0; index < b.Count - 1; index++)
                    {
                        var i = b[index];

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

                        Gizmos.DrawLine(CalcPosFromUV(i, transform.position), CalcPosFromUV(b[index + 1], transform.position));

                        if (i.color == 1)
                            Gizmos.DrawSphere(gizmosPos, 0.35f);
                        else if (i.color == 2)
                            Gizmos.DrawSphere(gizmosPos, 0.15f);

                    }
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
            foreach (var line in _deletedLines)
            {
                for (int index = 0; index < line.Count - 1; index++)
                {
                    var i = line[index];

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
            foreach (var br in _float2s)
            {
                Vector3 gizmosPos = this.transform.position;

                gizmosPos = CalcPosFromUV(br[0], gizmosPos);

                if (br[0].color == 1)
                    Gizmos.DrawSphere(gizmosPos, _gizmosSize);
                else if (br[0].color == 2)
                    Gizmos.DrawSphere(gizmosPos, _gizmosSize);
            }
        }
    }

    private Vector3 CalcPosFromUV(Float2 i, Vector3 meshCenter)
    {

        meshCenter.x -= _meshSize.x / 2;
        meshCenter.y -= _meshSize.y / 2;

        meshCenter.x += i.x * _meshSize.x;
        meshCenter.y += i.y * _meshSize.y;

        return meshCenter;
    }
}

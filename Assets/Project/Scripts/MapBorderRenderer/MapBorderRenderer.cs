using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MapBorderRenderer
{
    public class MapBorderRenderer : MonoBehaviour
    {
        [Range(0.05f, 1f)]
        [SerializeField] private float _gizmosSize;
        [SerializeField] private bool _DisplaySinglePoints;
        [SerializeField] private bool _DisplayDeletedLines;
        [SerializeField] private bool _DrawGizmos;
        [SerializeField] private LineRenderer _lineRendererPrefab;

        private List<IBorderCreationStep> _steps = new(10);
        private MapBorderData _data;

        private async void Start()
        {
            _data = new(GetComponent<Renderer>().material, GetComponent<MeshFilter>().mesh, transform);

            _steps = new()
            {
                new PointsCreationStep(_data),
                new PointsClearStep(_data),
                new WriteBordersInPovinceStep(_data),
                new SortPointsStep(_data),
                //new FilterStraightPointsStep(_data),
            };

            foreach(var step in _steps)
            {
                await step.Execute();
            }
        }

        private void Update()
        {
            /*var mousePos = Input.mousePosition;
            var ray = Camera.main.ScreenPointToRay(mousePos);
            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                var p = hitInfo.textureCoord;
                int x = (int)Mathf.Floor(p.x * _data._provinceTexWidth);
                int y = (int)Mathf.Floor(p.y * _data._provinceTexHeight);

                //DrawBorder(_provinceTex.GetPixel(x, y));

                Color32 color = _data._provinceTex.GetPixel(x, y);
                var provinceID = color.Color32ToUInt();

                if (Input.GetKeyDown(KeyCode.Mouse0))
                    _data._gizmosProvince = provinceID;
                if (Input.GetKeyDown(KeyCode.Mouse0))
                    LineRendererBorderSmooth(provinceID);
                //Debug.Log($"gizmos province:{gizmosProvince}");
            }*/
        }

        private void OnDrawGizmos()
        {
            /*if (Application.isPlaying && _DrawGizmos)
            {
                if (!_data.ProvincesBorders.ContainsKey(_data._gizmosProvince)) return;
                foreach (var a in _data.ProvincesBorders[_data._gizmosProvince])
                {
                    foreach (var b in _data.BordersFloat[a])
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
                            Gizmos.DrawSphere(gizmosPos, _gizmosSize);
                        }
                    }
                }
                DisplaySinglePoints();
                DisplayDeletedLines();
            }*/
        }

        private void DisplayDeletedLines()
        {
            if (_DisplayDeletedLines)
            {
                foreach (var line in _data.DeletedLines)
                {
                    for (int index = 0; index < line.Count - 1; index++)
                    {
                        var i = line[index];

                        if (i.DebugColor == 1)
                            Gizmos.color = Color.red;
                        else if (i.DebugColor == 2)
                            Gizmos.color = Color.blue;
                        else
                            Gizmos.color = Color.black;

                        Vector3 gizmosPos = this.transform.position;

                        if (i.DebugColor == 1)
                            gizmosPos.z += 0.35f;

                        //gizmosPos = CalcPosFromUV(i, gizmosPos);

                        //Gizmos.DrawLine(CalcPosFromUV(i, transform.position), CalcPosFromUV(line[index + 1], transform.position));

                        if (i.DebugColor == 1)
                            Gizmos.DrawSphere(gizmosPos, 0.35f);
                        else if (i.DebugColor == 2)
                            Gizmos.DrawSphere(gizmosPos, 0.15f);
                    }
                }
            }
        }
    }
}
using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Utils;

namespace MapBorderRenderer.Debugers
{
    public class SortStepDebugger : MonoBehaviour
    {
        [SerializeField] DebugPoint _pointPrefab;
        [SerializeField] Transform _debugPointContainer;
        private MapBorderRenderer _renderer;
        private SortPointsStep _step;
        private MapBorderData _data;
        private Border _border;
        private ActivityPool<DebugPoint> _pool;
        private Vector3 _start;
        
        [ShowInInspector, ReadOnly] private int _subBorderCount = 0;
        [ShowInInspector, ReadOnly] private int _listsInSubBorderCount = 0;
        [ShowInInspector, ReadOnly] private int _shownSubBorderIndex = 0;
        [ShowInInspector, ReadOnly] private int _shownListIndex = 0;
        
        [SerializeField] bool _markStartEndPoint;

        private void Start()
        {
            _renderer = GetComponent<MapBorderRenderer>();
            _renderer.OnInitialized += Init;
        }

        private void Init()
        {
            _renderer.OnInitialized -= Init;
            _step = _renderer._steps[2] as SortPointsStep;
            _data = _renderer._data;
            _start = new Vector3(-_data.MeshSize.x / 2, -_data.MeshSize.y / 2);
            _pool = new ActivityPool<DebugPoint>(_pointPrefab, _debugPointContainer, 1000);
        }

        [Button]
        public void UpdateView()
        {
            GetBorder();
            _pool.HideAll();
            DrawAll();

        }
        public void DrawAll()
        {
            if(_border == null)  return;
            _subBorderCount = _border.SubBorders.Count;
            foreach (var subborder in _border)
            {
                if (_markStartEndPoint)
                {
                    foreach (var list in subborder.SortedPointsLists)
                    {
                        DrawPoint(list.First.Value, Color.green, 0.5f, 1, Color.black);
                        DrawPoint(list.Last.Value, Color.magenta, 0.5f, 1, Color.black);
                    }
                        
                }
                    
                foreach (var list in subborder.SortedPointsLists)
                {
                    var colorStep = 1f / list.Count;
                    var color = 1f;

                    var counter = 0;
                    foreach (var point in list)
                    {
                        counter++;
                        color -= colorStep;
                        DrawPoint(point, new Color(color, 0f, 0f), 0.35f, 15, Color.black, counter.ToString());
                    }
                        
                        
                }
            }
        }

        [Button]
        private void ShowNextSubBorder()
        {
            _shownSubBorderIndex++;
            if(_shownSubBorderIndex >= _subBorderCount) _shownSubBorderIndex -= _subBorderCount;
            ShowSubBorder();
        }
        
        [Button]
        private void ShowPreviousSubBorder()
        {
            _shownSubBorderIndex--;
            if(_shownSubBorderIndex < 0) _shownSubBorderIndex += _subBorderCount;
            ShowSubBorder();
        }

        private void ShowSubBorder()
        {
            _pool.HideAll();
            var subborder = _border.SubBorders[_shownSubBorderIndex];
            if (_markStartEndPoint)
            {
                foreach (var list in subborder.SortedPointsLists)
                {
                    DrawPoint(list.First.Value, Color.green, 0.4f, 1, Color.black);
                    DrawPoint(list.Last.Value, Color.magenta, 0.4f, 1, Color.black);
                }
                        
            }
                    
            foreach (var list in subborder.SortedPointsLists)
            {
                var colorStep = 1f / list.Count;
                var color = 1f;

                var counter = 0;
                foreach (var point in list)
                {
                    counter++;
                    color -= colorStep;
                    DrawPoint(point, new Color(color, 0f, 0f), 0.35f, 15, Color.black, counter.ToString());
                }
            }
        }
        
        [Button]
        private void Clear()
        {
            _pool.HideAll();
        }

        private void DrawPoint(BorderPoint position, Color color, float size, int order, Color textColor = new(), string text = null)
        {
            var point = _pool.Get();
            point.transform.localScale = new Vector3(size, size, size);
            point.transform.position = _start + new Vector3(position.X / 2f, position.Y / 2f, order * -0.001f);
            point.SetColor(color);
            point.SetTextColor(textColor);
            point.SetText(text);
        }

        private void GetBorder()
        {
            var id = _data.GenerateBorderID(_data.Color32ToInt(_renderer._debugProvince), _data.Color32ToInt(_renderer._oldDebugProvince));
            _data.Borders.TryGetValue(id, out _border);
        }
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace MapBorderRenderer
{
    public class BorderPixelsCollectionStep : IBorderCreationStep
    {
        private readonly MapBorderData _data;
        private readonly HashSet<int> _handledPixels = new();
        private readonly Queue<int> _queue = new(200);
        private readonly HashSet<int> _queueContainsChecker = new(200);
        private readonly Color32[] _provinceArr;
        private readonly bool _showExecutionInfo;
        private readonly int _width;
        private readonly int _height;
        private readonly int _length;

        public BorderPixelsCollectionStep(MapBorderData data, bool showExecutionInfo = false)
        {
            _data = data;
            _showExecutionInfo = showExecutionInfo;
            _width = _data.TextureWidth;
            _height = _data.TextureHeight;
            _provinceArr = _data.TextureArr;
            _length = _provinceArr.Length;
        }

        public void DrawGizmos(Color32 provColor)
        {
            if (_data.BorderPixels.ContainsKey(provColor))
            {
                var borderPixels = _data.BorderPixels[provColor];
                
                Vector3 start = new Vector3(-_data.MeshSize.x / 2, -_data.MeshSize.y / 2) + new Vector3(0.5f, 0.5f);
                
                foreach (var border in borderPixels)
                {
                    foreach (var index in border)
                    {
                        Gizmos.color = Color.red;

                        Vector3 pos = start + (Vector3)_data.ConvertIndexToPixelCoordinated(index);
                        Gizmos.DrawSphere(pos, 0.35f);
                    }
                }
            }
        }

        public async Task Execute()
        {
            for (int i = 0; i < _provinceArr.Length - 1; i++)
            {
                if (_handledPixels.Contains(i) == false)
                {
                    Color32 current = _provinceArr[i];
                    if(_data.BorderPixels.ContainsKey(current) == false) _data.BorderPixels.Add(current, new List<HashSet<int>>());
                    var borderPixelsCluster = new HashSet<int>(64);
                    _data.BorderPixels[current].Add(borderPixelsCluster);
                    _queue.Clear();
                    _queue.Enqueue(i);
                    _queueContainsChecker.Clear();
                    _queueContainsChecker.Add(i);

                    while (_queue.Count != 0)
                    {
                        var pixel = _queue.Dequeue();
                        _queueContainsChecker.Remove(pixel);
                        _handledPixels.Add(pixel);
                        
                        var isBorder = false;
                        var upIndex = _data.GetUpIndex(pixel);
                        var leftIndex = _data.GetLeftIndex(pixel);
                        var downIndex = _data.GetDownIndex(pixel);
                        var rightIndex = _data.GetRightIndex(pixel);

                        TryAddCollectNeighbourPixel(pixel, upIndex, ref isBorder, borderPixelsCluster);
                        TryAddCollectNeighbourPixel(pixel, leftIndex, ref isBorder, borderPixelsCluster);
                        TryAddCollectNeighbourPixel(pixel, downIndex, ref isBorder, borderPixelsCluster);
                        TryAddCollectNeighbourPixel(pixel, rightIndex, ref isBorder, borderPixelsCluster);
                        
                        /*переделать в пикселях листы на хешсеты и сделать проверку на наличие в каком-то кластере*/
                    }
                }
            }
            
            if(_showExecutionInfo) Debug.Log(GetExecutionInfo());

            await Task.Yield();
        }

        public string GetExecutionInfo()
        {
            int borderPixelsCount = 0;
            foreach (var border in _data.BorderPixels.Values)
            {
                foreach (var subborder in border)
                {
                    borderPixelsCount += subborder.Count;
                }
            }
            
            var msg = $"{GetType()} handled {_handledPixels.Count} pixels";
            msg += $" and collected {borderPixelsCount} border pixels";
            
            return msg;
        }

        private void TryAddCollectNeighbourPixel(int pixel, int neighbourPixel, ref bool isBorder, HashSet<int> pixels)
        {
            if(_queueContainsChecker.Contains(neighbourPixel)) return;
            
            var colorFrom = _provinceArr[pixel];
            var colorTo = _provinceArr[neighbourPixel];
            
            if (colorFrom.CompareRGB(colorTo))
            {
                if (_handledPixels.Contains(neighbourPixel)) return;
                
                _queue.Enqueue(neighbourPixel);
                _queueContainsChecker.Add(neighbourPixel);
            }
            else
            {
                if (isBorder == false)
                {
                    pixels.Add(pixel);
                    isBorder = true;
                }
            }
        }


    }
}
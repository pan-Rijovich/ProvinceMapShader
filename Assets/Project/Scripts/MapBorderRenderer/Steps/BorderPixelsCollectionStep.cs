using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace MapBorderRenderer
{
    public class BorderPixelsCollectionStep : IBorderCreationStep
    {
        private readonly MapBorderData _data;
        private HashSet<int> _handledPixels;
        private Queue<int> _queue;
        private HashSet<int> _queueContainsChecker;
        private readonly Stopwatch _stopwatch = new();
        private readonly bool _showExecutionInfo;

        private int _tstI;

        public BorderPixelsCollectionStep(MapBorderData data, bool showExecutionInfo = false)
        {
            _data = data;
            _showExecutionInfo = showExecutionInfo;
        }

        public void DrawGizmos(Color32 provColor, Color32 provColor2, int mode)
        {
            if (_data.BorderPixels.ContainsKey(_data.Color32ToInt(provColor)))
            {
                var borderPixels = _data.BorderPixels[_data.Color32ToInt(provColor)];
                
                Vector3 start = new Vector3(-_data.MeshSize.x / 2, -_data.MeshSize.y / 2) + new Vector3(0.5f, 0.5f);
                
                Gizmos.color = Color.magenta;
                Vector3 pos2 = start + (Vector3)_data.ConvertIndexToPixelCoordinated(_tstI);
                Gizmos.DrawSphere(pos2, 0.35f);
                
                var counter = 0;
                foreach (var cluster in borderPixels.Pixels)
                {
                    foreach (var index in cluster)
                    {
                        Gizmos.color = Color.red;

                        Vector3 pos = start + (Vector3)_data.ConvertIndexToPixelCoordinated(index);
                        Gizmos.DrawSphere(pos, 0.25f);
                    }
                    counter++;
                }
            }
        }

        public async Task Execute()
        {
            _handledPixels = new();
            _queue = new(1024);
            _queueContainsChecker = new(1024);
            
            _stopwatch.Restart();
            for (int i = 0; i < _data.TextureArr.Length - 1; i++)
            {
                if (_handledPixels.Contains(i) == false)
                {
                    int current = _data.Color32ToInt(_data.TextureArr[i]);
                    
                    if(_data.BorderPixels.ContainsKey(current) == false) _data.BorderPixels.Add(current, new BorderPixelsCollection(current));
                    var borderPixelsCluster = new HashSet<int>(64);
                    _data.BorderPixels[current].AddCluster(borderPixelsCluster);
                    
                    _queue.Clear();
                    _queue.Enqueue(i);
                    _queueContainsChecker.Clear();
                    _queueContainsChecker.Add(i);

                    while (_queue.Count != 0)
                    {
                        var pixelIndex = _queue.Dequeue();
                        _queueContainsChecker.Remove(pixelIndex);
                        _handledPixels.Add(pixelIndex);
                        
                        
                        _tstI = pixelIndex; // -----------------------------------
                        
                        var isBorder = false;
                        var upIndex = _data.GetUpIndex(pixelIndex);
                        var leftIndex = _data.GetLeftIndex(pixelIndex);
                        var downIndex = _data.GetDownIndex(pixelIndex);
                        var rightIndex = _data.GetRightIndex(pixelIndex);

                        TryAddCollectNeighbourPixel(pixelIndex, upIndex, ref isBorder, borderPixelsCluster);
                        TryAddCollectNeighbourPixel(pixelIndex, leftIndex, ref isBorder, borderPixelsCluster);
                        TryAddCollectNeighbourPixel(pixelIndex, downIndex, ref isBorder, borderPixelsCluster);
                        TryAddCollectNeighbourPixel(pixelIndex, rightIndex, ref isBorder, borderPixelsCluster);
                        
                        
                        await Task.Yield();
                    }
                }
            }
            _stopwatch.Stop();
            
            if(_showExecutionInfo) Debug.Log(GetExecutionInfo());
            
            _handledPixels = null;
            _queue = null;
            _queueContainsChecker = null;

            await Task.Yield();
        }

        public string GetExecutionInfo()
        {
            int borderPixelsCount = 0;
            foreach (var border in _data.BorderPixels.Values)
            {
                foreach (var subborder in border.Pixels)
                {
                    borderPixelsCount += subborder.Count;
                }
            }
            
            var msg = $"{GetType().Name} handled {_handledPixels.Count} pixels";
            msg += $" and collected {borderPixelsCount} border pixels";
            msg += $" for {_data.BorderPixels.Count} colors";
            msg += $" in {_stopwatch.ElapsedMilliseconds} milliseconds";
            
            return msg;
        }

        private void TryAddCollectNeighbourPixel(int pixel, int neighbourPixel, ref bool isBorder, HashSet<int> pixels)
        {
            if(_queueContainsChecker.Contains(neighbourPixel)) return;
            
            var colorFrom = _data.Color32ToInt(_data.TextureArr[pixel]);
            var colorTo = _data.Color32ToInt(_data.TextureArr[neighbourPixel]);
            
            if (colorFrom == colorTo)
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
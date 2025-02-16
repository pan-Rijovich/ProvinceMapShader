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
        private readonly MapBorderGenData _genData;
        private HashSet<int> _handledPixels;
        private Queue<int> _queue;
        private HashSet<int> _queueContainsChecker;
        private readonly Stopwatch _stopwatch = new();
        private readonly bool _showExecutionInfo;

        private int _tstI;

        public BorderPixelsCollectionStep(MapBorderGenData genData, bool showExecutionInfo = false)
        {
            _genData = genData;
            _showExecutionInfo = showExecutionInfo;
        }

        public void DrawGizmos(Color32 provColor, Color32 provColor2, int mode)
        {
            if (_genData.BorderPixels.ContainsKey(provColor.ToInt()))
            {
                var borderPixels = _genData.BorderPixels[provColor.ToInt()];
                
                Vector3 start = new Vector3(-_genData.MapSize.x / 2, -_genData.MapSize.y / 2) + new Vector3(0.5f, 0.5f);

                if (mode != 0)
                {
                    Gizmos.color = Color.magenta;
                    Vector3 pos2 = start + (Vector3)_genData.ConvertIndexToFloatPixelCoordinated(_tstI);
                    Gizmos.DrawSphere(pos2, 0.35f); 
                }
                
                var counter = 0;
                foreach (var cluster in borderPixels.Pixels)
                {
                    foreach (var index in cluster)
                    {
                        Gizmos.color = Color.red;

                        Vector3 pos = start + (Vector3)_genData.ConvertIndexToFloatPixelCoordinated(index);
                        Gizmos.DrawSphere(pos, 0.25f);
                    }
                    counter++;
                }
                
                
                if (mode != 0)
                {               
                    foreach (var index in _handledPixels)
                    {
                        Gizmos.color = Color.blue;

                        Vector3 pos = start + (Vector3)_genData.ConvertIndexToFloatPixelCoordinated(index);
                        Gizmos.DrawSphere(pos, 0.15f);
                    }
                }
            }
        }

        public async Task Execute()
        {
            _handledPixels = new();
            _queue = new(1024);
            _queueContainsChecker = new(1024);
            
            _stopwatch.Restart();
            for (int i = 0; i < _genData.TexPixels.Length - 1; i++)
            {
                if (_handledPixels.Contains(i) == false)
                {
                    int current = _genData.TexPixels[i].ToInt();
                    
                    if(_genData.BorderPixels.ContainsKey(current) == false) _genData.BorderPixels.Add(current, new BorderPixelsCollection(current));
                    var borderPixelsCluster = new HashSet<int>(64);
                    _genData.BorderPixels[current].AddCluster(borderPixelsCluster);
                    
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
                        var upIndex = _genData.GetUpIndex(pixelIndex);
                        var leftIndex = _genData.GetLeftIndex(pixelIndex);
                        var downIndex = _genData.GetDownIndex(pixelIndex);
                        var rightIndex = _genData.GetRightIndex(pixelIndex);

                        TryAddCollectNeighbourPixel(pixelIndex, upIndex, ref isBorder, borderPixelsCluster);
                        TryAddCollectNeighbourPixel(pixelIndex, leftIndex, ref isBorder, borderPixelsCluster);
                        TryAddCollectNeighbourPixel(pixelIndex, downIndex, ref isBorder, borderPixelsCluster);
                        TryAddCollectNeighbourPixel(pixelIndex, rightIndex, ref isBorder, borderPixelsCluster);
                        
                        
                        //await Task.Yield(); // -----------------------------------
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
            foreach (var border in _genData.BorderPixels.Values)
            {
                foreach (var subborder in border.Pixels)
                {
                    borderPixelsCount += subborder.Count;
                }
            }
            
            var msg = $"{GetType().Name} handled {_handledPixels.Count} pixels";
            msg += $" and collected {borderPixelsCount} border pixels";
            msg += $" for {_genData.BorderPixels.Count} colors";
            msg += $" in {_stopwatch.ElapsedMilliseconds} milliseconds";
            
            return msg;
        }

        private void TryAddCollectNeighbourPixel(int pixel, int neighbourPixel, ref bool isBorder, HashSet<int> pixels)
        {
            if(_queueContainsChecker.Contains(neighbourPixel)) return;
            
            var colorFrom = _genData.TexPixels[pixel].ToInt();
            var colorTo = _genData.TexPixels[neighbourPixel].ToInt();
            
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
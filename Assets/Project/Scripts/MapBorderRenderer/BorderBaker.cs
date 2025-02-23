using System;
using System.Collections.Generic;
using System.IO;
using Project.Scripts.MapBorderRenderer;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace Project.Scripts.MapRenderer
{
    public class BorderBaker
    {
        private Camera _camera;
        private ChunkRenderSettingsProvider _settingsProvider;
        private Dictionary<int, RenderTexture> _renderTextures = new();
        private Queue<RenderRequest> _renderQueue = new();
        private bool _isBusy = false;

        public BorderBaker(Camera renderCamera, ChunkRenderSettingsProvider settingsProvider)
        {
            _camera = renderCamera;
            _settingsProvider = settingsProvider;
            InitRenderTextures();
        }

        public void RenderAsync(Vector2 position, Action<Texture2D> onSuccess, float worldSize = 64f)
        {
            var request = new RenderRequest()
            {
                Position = position,
                OnSuccess = onSuccess,
                WorldSize = worldSize
            };
            _renderQueue.Enqueue(request);
            
            if (_isBusy)
            {
                return;
            }

            _isBusy = true;

            RenderNextRequest();
        }
        
        public void Save()
        {
            var counter = 0;
            foreach (var renderTexture in _renderTextures.Values)
            {
                Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
                RenderTexture currentRT = RenderTexture.active;
                RenderTexture.active = renderTexture;

                texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
                texture.Apply();

                RenderTexture.active = currentRT;

                byte[] bytes = texture.EncodeToPNG();

                File.WriteAllBytes(Path.Combine(Application.persistentDataPath, $"Render{counter}.png"), bytes);

                Object.Destroy(texture);
                counter++;
            }
        }

        private void RenderNextRequest()
        {
            if (_renderQueue.Count == 0) return;
            
            var renderRequest = _renderQueue.Dequeue();
            
            int textureSize = _settingsProvider.BorderTextureResolution.CurrentValue;

            var renderTexture = _renderTextures[textureSize];
            _camera.Activate();
            _camera.transform.position = new Vector3(renderRequest.Position.x, renderRequest.Position.y, -10f);
            _camera.orthographicSize = renderRequest.WorldSize;
            _camera.targetTexture = renderTexture;
            _camera.Render();
            _camera.Disactivate(); 
            
            AsyncGPUReadback.Request(renderTexture, 0, TextureFormat.R8, request =>
            {
                if (request.hasError)
                {
                    Debug.LogError("GPU Readback Error!");
                    return;
                }

                Texture2D texture = new Texture2D(textureSize, textureSize, TextureFormat.R8, false);
                texture.filterMode = FilterMode.Bilinear;
                texture.LoadRawTextureData(request.GetData<byte>());
                texture.Apply();
            
                renderRequest.OnSuccess?.Invoke(texture);

                _isBusy = false;
                RenderNextRequest();
            });
        }
        
        private void InitRenderTextures()
        {
            for (int i = 256; i <= 2048; i *= 2)
            {
                var renderTexture = new RenderTexture(i, i, 0);
                renderTexture.enableRandomWrite = true;
                renderTexture.filterMode = FilterMode.Bilinear;
                renderTexture.format = RenderTextureFormat.R8;
                renderTexture.depthStencilFormat = GraphicsFormat.None;
                renderTexture.Create();
                _renderTextures.Add(i, renderTexture);
            }
        }
    }

    public struct RenderRequest
    {
        public Vector2 Position;
        public Action<Texture2D> OnSuccess;
        public float WorldSize;
    }
}
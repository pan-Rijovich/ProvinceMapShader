using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace Project.Scripts.MapRenderer
{
    public class BorderRenderCamera : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        
        private RenderTexture _texture;

        public RenderTexture Render(Vector2 position, float size = 64f)
        {
            _texture = new RenderTexture(512, 512, 0);
            _texture.format = RenderTextureFormat.R8;
            _texture.depthStencilFormat = GraphicsFormat.None;
            _texture.filterMode = FilterMode.Bilinear;
            _texture.Create();
            
            this.Activate();
            transform.position = new Vector3(position.x, position.y, -1f);
            _camera.orthographicSize = size;
            _camera.targetTexture = _texture;
            _camera.Render();
            
            this.Disactivate();
            
            return _texture;
        }
    }
}
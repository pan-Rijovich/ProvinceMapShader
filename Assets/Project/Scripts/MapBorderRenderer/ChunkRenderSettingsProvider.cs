using Assets.Scripts.CameraController;
using R3;

namespace Project.Scripts.MapBorderRenderer
{
    public abstract class ChunkRenderSettingsProvider
    {
        public ReadOnlyReactiveProperty<int> BorderTextureResolution => _borderTextureResolution;
        public ReadOnlyReactiveProperty<float> BorderContrast => _borderContrast;

        protected readonly CameraController _cameraController;
        protected readonly ReactiveProperty<int> _borderTextureResolution = new(1024);
        protected readonly ReactiveProperty<float> _borderContrast = new(1f);

        public ChunkRenderSettingsProvider(CameraController cameraController)
        {
            _cameraController = cameraController;

            _cameraController.Zoom.Subscribe(UpdateSettings);
        }

        protected abstract void UpdateSettings(float zoom);
    }

    public class DefaultChunkRenderSettingsProvider : ChunkRenderSettingsProvider
    {
        public DefaultChunkRenderSettingsProvider(CameraController cameraController) : base(cameraController){}

        protected override void UpdateSettings(float zoom)
        {
            if (zoom > 0.5f)
            {
                _borderTextureResolution.Value = 256;
                _borderContrast.Value = 2.5f;
            }
            else if(zoom > 0.25f)
            {
                _borderTextureResolution.Value = 512;  

                _borderContrast.Value = 1.8f;
            }
            else
            {
                _borderTextureResolution.Value = 1024; 
                _borderContrast.Value = 1.0f;
            }
        }
    }
    
    public class ResolutionTestChunkRenderSettingsProvider : ChunkRenderSettingsProvider
    {
        public ResolutionTestChunkRenderSettingsProvider(CameraController cameraController) : base(cameraController)
        {
            _borderTextureResolution.Value = 1024;
            _borderContrast.Value = 2.5f;
        }

        protected override void UpdateSettings(float zoom){}
    }
}
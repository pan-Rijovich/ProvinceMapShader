using System.Threading.Tasks;
using UnityEngine;

namespace MapBorderRenderer
{
    public interface IBorderCreationStep
    {
        public Task Execute();
        public string GetExecutionInfo();
        public void DrawGizmos(Color32 provColor, Color32 provColor2, int mode);
    }
}
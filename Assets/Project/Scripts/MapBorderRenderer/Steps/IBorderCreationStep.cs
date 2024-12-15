using System.Threading.Tasks;

namespace MapBorderRenderer
{
    public interface IBorderCreationStep
    {
        public Task Execute();
    }
}
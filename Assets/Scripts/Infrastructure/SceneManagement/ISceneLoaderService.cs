using Cysharp.Threading.Tasks;
using System.Threading;

namespace DeminingAcademy.Infrastructure.SceneManagement
{
    public interface ISceneLoaderService
    {
        UniTask LoadSceneAsync(string sceneName, CancellationToken ct = default);
        
        UniTask ReloadCurrentSceneAsync();
    }
}
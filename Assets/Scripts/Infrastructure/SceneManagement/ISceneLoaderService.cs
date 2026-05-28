using Cysharp.Threading.Tasks;

namespace DeminingAcademy.Infrastructure.SceneManagement
{
    public interface ISceneLoaderService
    {
        UniTask LoadSceneAsync(string sceneName);
        
        UniTask ReloadCurrentSceneAsync();
    }
}
using App.Common;
using AppCore.Runtime;
using AppService.Runtime;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Playables;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace App.IceGame
{
    public class StartView : MonoBehaviour, IView
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private PlayableDirector playableDirector;

        public Canvas Canvas => canvas;
        private ModalScreen ModalScreen => ServiceLocator.Get<ModalScreen>();
        
        public static async UniTask<AsyncOperationHandle<GameObject>> LoadAsync()
        {
            var handle = Addressables.LoadAssetAsync<GameObject>("StartView");
            await handle.ToUniTask();
            return handle;
        }
        
        public async UniTask PlayAsync()
        {
            await playableDirector.PlayAsyncSafe(destroyCancellationToken);
        }

        public void Push()
        {
            ModalScreen.Push(this);
        }

        public void Pop()
        {
            ModalScreen.Pop(this);
        }

        public void Open()
        {
            gameObject.SetActive(true);
        }

        public void Close()
        {
            Destroy(gameObject);
        }
    }
}
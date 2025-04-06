using AppCore.Runtime;
using AppService.Runtime;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Playables;

namespace App.Common
{
    public class FadeScreenView : MonoBehaviour, IView
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private CanvasGroup fadeCanvasGroup;
        [SerializeField] private PlayableDirector playableDirector;
        [SerializeField] private PlayableAsset fadeInTimeline;
        [SerializeField] private PlayableAsset fadeOutTimeline;

        private static ModalScreen ModalScreen => ServiceLocator.Get<ModalScreen>();
        public Canvas Canvas => canvas;

        /// <summary>
        /// Addressablesからロード、Instantiateをしてインスタンスを返す
        /// </summary>
        /// <returns></returns>
        public static async UniTask<FadeScreenView> CreateAsync()
        {
            var prefab = await Addressables.InstantiateAsync(nameof(FadeScreenView));
            prefab.SetActive(false);
            return prefab.GetComponentSafe<FadeScreenView>();
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

        /// <summary>
        /// フェードイン
        /// </summary>
        public async UniTask FadeInAsync()
        {
            playableDirector.playableAsset = fadeInTimeline;
            await playableDirector.PlayAsyncSafe(destroyCancellationToken);
        }

        /// <summary>
        /// フェードアウト
        /// </summary>
        public async UniTask FadeOutAsync()
        {
            playableDirector.playableAsset = fadeOutTimeline;
            await playableDirector.PlayAsyncSafe(destroyCancellationToken);
        }

        // 真っ黒にする
        public void Blackout()
        {
            fadeCanvasGroup.alpha = 1f;
        }
    }
}
using AppCore.Runtime;
using AppService.Runtime;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace App.IceGame
{
    public class IceGameView : MonoBehaviour, IView
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private ScoreElementView scoreElementView;
        
        public Canvas Canvas => canvas;
        public ScoreElementView ScoreElementView => scoreElementView;
        private ModalScreen ModalScreen => ServiceLocator.Get<ModalScreen>();
        
        public static async UniTask<IceGameView> CreateAsync()
        {
            var handle = Addressables.InstantiateAsync(nameof(IceGameView));
            await handle.ToUniTask();
            handle.Result.SetActive(false);
            var view = handle.Result.GetComponentSafe<IceGameView>();
            view.Initialize();
            return view;
        }
        
        private void Initialize()
        {
            scoreElementView.Initialize();
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
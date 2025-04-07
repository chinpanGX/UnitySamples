using AppCore.Runtime;
using AppService.Runtime;
using AudioService.Simple;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace App.Title
{
    public class TitleView : MonoBehaviour, IView
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private CustomButton startButton;
        [SerializeField] private CustomButton helpButton;
        [SerializeField] private CustomButton tweetButton;

        public Canvas Canvas => canvas;
        private ViewScreen ViewScreen => ServiceLocator.Get<ViewScreen>();
        private SimpleAudioService AudioService => ServiceLocator.Get<SimpleAudioService>();

        private readonly Subject<Unit> onClickedStart = new();
        private readonly Subject<Unit> onClickedHelp = new();
        private readonly Subject<Unit> onClickedTweet = new();
        public Observable<Unit> OnClickedStart => onClickedStart;
        public Observable<Unit> OnClickedHelp => onClickedHelp;
        public Observable<Unit> OnClickedTweet => onClickedTweet;

        public static async UniTask<TitleView> CreateAsync()
        {
            var prefab = await Addressables.InstantiateAsync(nameof(TitleView));
            prefab.SetActive(false);
            var view = prefab.GetComponentSafe<TitleView>();
            view.Initialize();
            return view;
        }

        private void Initialize()
        {
            startButton.SubscribeToClickAndPlaySe(() => { onClickedStart.OnNext(Unit.Default); },
                new AudioOptions(AudioService, "SE_Ok"), canvasGroup
            );

            helpButton.SubscribeToClickAndPlaySe(() => { onClickedHelp.OnNext(Unit.Default); },
                new AudioOptions(AudioService, "SE_Ok"), canvasGroup
            );
            
            tweetButton.SubscribeToClickAndPlaySe(() => { onClickedTweet.OnNext(Unit.Default); },
                new AudioOptions(AudioService, "SE_Ok"), canvasGroup
            );
        }

        public void Push()
        {
            ViewScreen.Push(this);
        }

        public void Pop()
        {
            ViewScreen.Pop(this);
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
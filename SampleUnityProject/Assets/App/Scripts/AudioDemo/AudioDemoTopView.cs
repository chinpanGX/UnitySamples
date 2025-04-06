#nullable enable
using AppCore.Runtime;
using AppService.Runtime;
using AudioService.Simple;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace App.AudioDemo
{
    public class AudioDemoTopView : MonoBehaviour, IView
    {
        [SerializeField] private Canvas? canvas;
        [SerializeField] private CustomButton? playBgmButton;
        [SerializeField] private CustomButton? stopBgmButton;
        [SerializeField] private CustomButton? openAudioSettingsButton;
        [SerializeField] private CanvasGroup? canvasGroup;
        [SerializeField] private CustomButton? backButton;

        public Canvas? Canvas => canvas;
        private SimpleAudioService AudioService => ServiceLocator.Get<SimpleAudioService>();
        private ViewScreen ViewScreen => ServiceLocator.Get<ViewScreen>();

        private readonly Subject<Unit> onClickedOpenAudioSettings = new();
        private readonly Subject<Unit> onClickedBack = new();
        public Observable<Unit> OnClickedOpenAudioSettings => onClickedOpenAudioSettings;
        public Observable<Unit> OnClickedBack => onClickedBack;

        /// <summary>
        /// Addressablesロードして、AudioDemoTopViewを生成する
        /// </summary>
        /// <returns></returns>
        public static async UniTask<AudioDemoTopView> CreateAsync()
        {
            var go = await Addressables.InstantiateAsync(nameof(AudioDemoTopView));
            go.SetActive(false);
            var view = go.GetComponentSafe<AudioDemoTopView>();
            view.Initialize();
            return view;
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
        
        /// <summary>
        /// 初期化処理
        /// </summary>
        private　void Initialize()
        {
            if (playBgmButton != null)
            {
                // PlayBgmButtonをクリックしたら、BGMを再生する
                playBgmButton.SubscribeToClickAndPlaySe(() => { AudioService.PlayBgm("BGM_Sample"); },
                    new AudioOptions(AudioService, "SE_Ok"), canvasGroup
                );
            }

            if (stopBgmButton != null)
            {
                // StopBgmButtonをクリックしたら、BGMを停止する
                stopBgmButton.SubscribeToClickAndPlaySe(() => { AudioService.StopBgm(); },
                    new AudioOptions(AudioService, "SE_Cancel"), canvasGroup
                );
            }

            if (openAudioSettingsButton != null)
            {
                // OpenAudioSettingsButtonをクリックしたら、onClickedOpenAudioSettingsイベントを発行する
                openAudioSettingsButton.SubscribeToClickAndPlaySe(
                    () => onClickedOpenAudioSettings.OnNext(Unit.Default),
                    new AudioOptions(AudioService, "SE_Ok"),
                    canvasGroup
                );
            }

            if (backButton != null)
            {
                // BackButtonをクリックしたら、onClickedBackイベントを発行する
                backButton.SubscribeToClickAndPlaySe(
                    () => onClickedBack.OnNext(Unit.Default),
                    new AudioOptions(AudioService, "SE_Cancel"),
                    canvasGroup
                );
            }
        }
    }
}
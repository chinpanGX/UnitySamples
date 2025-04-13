using App.Common;
using AppCore.Runtime;
using AppService.Runtime;
using AudioService.Simple;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace App.AudioDemo
{
    public class AudioSettingView : MonoBehaviour, IView
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private CustomSlider masterVolumeSlider;
        [SerializeField] private CustomSlider bgmVolumeSlider;
        [SerializeField] private CustomSlider seVolumeSlider;
        [SerializeField] private CustomButton closeButton;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private ViewPlayableDirector viewPlayableDirector;

        public Canvas Canvas => canvas;
        private ModalScreen ModalScreen => ServiceLocator.Get<ModalScreen>();
        private SimpleAudioService AudioService => ServiceLocator.Get<SimpleAudioService>();

        /// <summary>
        /// Addressablesロードして、AudioSettingViewを生成する
        /// </summary>
        /// <returns></returns>
        public static async UniTask<AudioSettingView> CreateAsync()
        {
            var go = await Addressables.InstantiateAsync("Title/AudioOptionView.prefab");
            var view = go.GetComponentSafe<AudioSettingView>();
            view.gameObject.SetActive(false);
            view.Initialize();
            return view;
        }

        public async UniTask OpenAsync()
        {
            Push();
            Open();
            await viewPlayableDirector.PlayInAsync(destroyCancellationToken);
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
            viewPlayableDirector.PlayInAsync(destroyCancellationToken).Forget();
        }

        public void Close()
        {
            CloseView().Forget();
        }

        private async UniTask CloseView()
        {
            await viewPlayableDirector.PlayOutAsync(destroyCancellationToken);
            Destroy(gameObject);
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        private void Initialize()
        {
            // 保存している音量を取得して、スライダーにセットする
            var volume = AudioService.GetVolumeByPlayerPrefs();
            masterVolumeSlider.SetValueWithNotifySafe(volume.MasterVolume);
            bgmVolumeSlider.SetValueWithNotifySafe(volume.BgmVolume);
            seVolumeSlider.SetValueWithNotifySafe(volume.SeVolume);

            // CloseButtonをクリックしたら、AudioSettingViewを閉じる
            closeButton.SubscribeToClick(() =>
                {
                    AudioService.PlaySe("SE_Cancel");
                    Pop();
                }, canvasGroup
            );

            // Masterのスライダーの値が変更されたら、音量を変更する
            masterVolumeSlider.OnValueChangedObservable.Subscribe(value =>
                {
                    AudioService.ChangeAndWriteMasterVolume(value);
                }
            ).RegisterTo(destroyCancellationToken);

            // BGMのスライダーの値が変更されたら、音量を変更する
            bgmVolumeSlider.OnValueChangedObservable
                .Subscribe(value => { AudioService.ChangeBgmVolume(value); })
                .RegisterTo(destroyCancellationToken);

            // SEのスライダーの値が変更されたら、音量を変更する
            seVolumeSlider.OnValueChangedObservable
                .Subscribe(value => { AudioService.ChangeSeVolume(value); })
                .RegisterTo(destroyCancellationToken);

            // SEのスライダーからフォーカスが外れたとき、変更した音量がわかるようにSEを再生する
            seVolumeSlider.OnPointerUpObservable
                .Subscribe(_ => { AudioService.PlaySe("SE_Ok"); })
                .RegisterTo(destroyCancellationToken);
        }
    }
}
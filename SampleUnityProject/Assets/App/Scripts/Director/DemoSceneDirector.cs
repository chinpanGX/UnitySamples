using App.AudioDemo;
using App.Common;
using App.Title;
using AppCore.Runtime;
using AudioService.Simple;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace App.Director
{
    public class DemoSceneDirector : MonoBehaviour, IDirector
    {
        private TickablePresenter tickablePresenter;

        void Start()
        {
            InitializeAsync().Forget();
        }

        void Update()
        {
            tickablePresenter?.Tick();
        }

        public async Awaitable PushAsync(string key)
        {
            // fadeViewを作成して、フェードインを実行
            var fadeView = await FadeScreenView.CreateAsync();
            fadeView.Push();
            fadeView.Open();
            await fadeView.FadeInAsync();

            // Presenterを切り替える
            await SwitchPresenterAsync(key);

            // フェードアウトを実行し、FadeViewを破棄
            await fadeView.FadeOutAsync();
            fadeView.Pop();
        }

        /// <summary>
        /// 初期化を行う
        /// </summary>
        private async UniTaskVoid InitializeAsync()
        {
            // 画面を真っ暗にする
            var fadeView = await FadeScreenView.CreateAsync();
            fadeView.Push();
            fadeView.Open();
            fadeView.Blackout();

            // オーディオファイルの読み込みを行う
            var audioService = ServiceLocator.Get<SimpleAudioService>();
            await audioService.LoadAllAsyncIfNeed();
            audioService.PlayBgm("BGM_Sample");

            // Presenterのセットアップ
            tickablePresenter = new TickablePresenter();
            await SwitchPresenterAsync("Title");

            // フェードアウトして、画面表示
            await fadeView.FadeOutAsync();
            fadeView.Pop();
        }

        /// <summary>
        /// Presenterの切り替えを行う
        /// </summary>
        /// <param name="key"> 切り替え先のKey </param>
        /// <exception cref="ArgumentException"> Keyの指定が間違っている場合 </exception>
        private async UniTask SwitchPresenterAsync(string key)
        {
            IPresenter request = key switch
            {
                "Title" => new TitlePresenter(this, new TitleModel(), await TitleView.CreateAsync()),
                "AudioDemo" => new AudioDemoPresenter(this, await AudioDemoTopView.CreateAsync()),
                _ => throw new System.ArgumentException($"Unknown Presenter Key : {key}"),
            };
            tickablePresenter.SetRequest(request);
        }
    }
}
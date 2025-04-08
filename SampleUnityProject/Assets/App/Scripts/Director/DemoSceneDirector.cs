using System;
using App.AudioDemo;
using App.Common;
using App.SampleInGame;
using App.SampleInGame.Domain;
using App.SampleInGame.View;
using App.Title;
using AppCore.Runtime;
using AudioService.Simple;
using Cysharp.Threading.Tasks;
using InputSystemWrapper;
using MessagePipe;
using UnityEngine;

namespace App.Director
{
    public class DemoSceneDirector : MonoBehaviour, IDirector
    {
        [SerializeField] private Camera uiCamera;
        
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

            // MessagePipeのビルド
            BuildMessagePipe();

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
        /// MessagePipeのビルドを行う
        /// </summary>
        private void BuildMessagePipe()
        {
            var builder = new BuiltinContainerBuilder();
            builder.AddMessagePipe();
            builder.AddMessageBroker<BallCollisionMessage>();
            var provider = builder.BuildServiceProvider();
            GlobalMessagePipe.SetProvider(provider);
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
                "SampleGame" => await ResolveSampleInGame(),
                _ => throw new ArgumentException($"Unknown Presenter Key : {key}"),
            };
            tickablePresenter.SetRequest(request);
            
        }
        
        /// <summary>
        /// SampleInGamePresenterの依存関係の解決をする。
        /// オブジェクト数が多くなりそうなので、関数化
        /// </summary>
        /// <returns></returns>
        private async UniTask<IPresenter> ResolveSampleInGame()
        {
            var view = await SampleInGameView.CreateAsync(
                GlobalMessagePipe.GetPublisher<BallCollisionMessage>(),
                uiCamera
            );
            var model = new SampleInGameModel(GlobalMessagePipe.GetSubscriber<BallCollisionMessage>());
            var pointerHandler = ServiceLocator.Get<PointerHandler>();
            return new SampleInGamePresenter(this, model, view, pointerHandler);
        }
    }
}
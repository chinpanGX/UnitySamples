using System.Threading;
using App.AudioDemo;
using AppCore.Runtime;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine.AddressableAssets;

namespace App.Title
{
    public class TitlePresenter : IPresenter
    {
        private IDirector Director { get; set; }
        private TitleModel Model { get; set; }
        private TitleView View { get; set; }
        private StateMachine<TitlePresenter> StateMachine { get; set; }
        private readonly CancellationTokenSource cts = new();

        public TitlePresenter(IDirector director, TitleModel model, TitleView view)
        {
            Director = director;
            Model = model;
            View = view;
            StateMachine = new StateMachine<TitlePresenter>(this);
            StateMachine.Change<StateInit>();
        }

        public void Tick()
        {
            StateMachine.Tick();
        }

        public void Dispose()
        {
            View.Pop();
            StateMachine.Dispose();
            Model.Dispose();
        }
        
        private class StateInit : StateMachine<TitlePresenter>.State
        {
            private TitleView view;
            private TitleModel model;

            public override void Begin(TitlePresenter owner)
            {
                // ステートの初期化処理はココ
                view = owner.View;
                model = owner.Model;

                view.OnClickedStart
                    .SubscribeAwait(
                        async (_, token) =>
                        {
                            await owner.Director.PushAsync("InGame").ToUniTask(cancellationToken: token);
                        }, AwaitOperation.Drop
                    ).RegisterTo(owner.cts.Token);
                
                view.OnClickedAudioDemo.SubscribeAwait(
                    async (_, _) =>
                    {
                        var audioSettingsView = await AudioSettingView.CreateAsync();
                        await audioSettingsView.OpenAsync();
                    }, AwaitOperation.Drop
                ).RegisterTo(owner.cts.Token);

                view.OnClickedHelp.Subscribe((_) => OnClickHelp()).RegisterTo(owner.cts.Token);
                
                view.Push();
                view.Open();
            }

            public override void Tick(TitlePresenter owner)
            {
                // ステートの更新処理はココ
            }

            public override void End(TitlePresenter owner)
            {
                // ステートの終了処理はココ
            }

            private void OnClickHelp()
            {
                // ヘルプボタンが押されたときの処理
                model.OpenUrl();
            }
        }
    }
}
using System.Threading;
using App.SampleInGame.View;
using AppCore.Runtime;
using Cysharp.Threading.Tasks;
using InputSystemWrapper;
using R3;
using UnityEngine;

namespace App.SampleInGame
{
    public class SampleInGamePresenter : IPresenter
    {
        private IDirector Director { get; set; }
        private SampleInGameModel Model { get; set; }
        private SampleInGameView View { get; set; }
        private PointerHandler PointerHandler { get; set; }
        private StateMachine<SampleInGamePresenter> StateMachine { get; set; }
        private readonly CancellationTokenSource cts = new();
        
        public SampleInGamePresenter(IDirector director, SampleInGameModel model, SampleInGameView view, PointerHandler pointerHandler)
        {
            Director = director;
            Model = model;
            View = view;
            PointerHandler = pointerHandler;
            StateMachine = new StateMachine<SampleInGamePresenter>(this);
            StateMachine.Change<StateInit>();
        }
        
        public void Tick()
        {
            StateMachine.Tick();
        }
        
        public void Dispose()
        {
            View.Pop();
            View = null;
            Model.Dispose();
            StateMachine.Dispose();
        }
        

        private class StateInit : StateMachine<SampleInGamePresenter>.State
        {
            public override void Begin(SampleInGamePresenter owner)
            {
                owner.Model.TotalScore.Subscribe(totalScore =>
                    {
                        owner.View.SetScore(totalScore);
                    }
                ).RegisterTo(owner.cts.Token);
                
                owner.View.OnClickedBack
                    .SubscribeAwait(
                        async (_, token) =>
                        {
                            owner.PointerHandler.UnregisterOnClickPerformed();
                            await owner.Director.PushAsync("Title").ToUniTask(cancellationToken: token);
                        }, AwaitOperation.Drop
                    ).RegisterTo(owner.cts.Token);
                
                owner.View.Push();
                owner.View.Open();
                
                owner.StateMachine.Change<StateGameState>();
            }
        }

        private class StateGameState : StateMachine<SampleInGamePresenter>.State
        {
            public override void Begin(SampleInGamePresenter owner)
            {
                owner.Model.Initialize();
                owner.PointerHandler.RegisterOnClickPerformed(OnClickPerformed);
                return;

                void OnClickPerformed(Vector3 clickPosition)
                {
                    owner.View.CreateBallAsync(owner.Model.FetchGenerateBallData(), clickPosition).Forget();
                }
            }
            
            public override void Tick(SampleInGamePresenter owner)
            {
                owner.Model.Tick();
            }
        }
    }
}
using System.Threading;
using AppCore.Runtime;

namespace App.IceGame
{
    public class IceGamePresenter : IPresenter
    {
        public IDirector Director { get; }
        public IceGameModel Model { get; }
        public IceGameView View { get; }
        private readonly StateMachine<IceGamePresenter> stateMachine;
        private readonly CancellationTokenSource cts = new();
        private IPresenter presenterImplementation;

        public IceGamePresenter(IDirector director, IceGameModel model, IceGameView view)
        {
            Director = director;
            Model = model;
            View = view;
            stateMachine = new StateMachine<IceGamePresenter>(this);
            stateMachine.Change<StateInit>();
        }
        
        public void Tick()
        {
            stateMachine.Tick();
        }
        
        public void Dispose()
        {
            stateMachine.Dispose();
            View.Pop();
            cts?.Dispose();
            Model?.Dispose();
        }

        private class StateInit : StateMachine<IceGamePresenter>.State
        {
            public override void Begin(IceGamePresenter owner)
            {

                owner.View.Push();
                owner.View.Open();
            }
        }

        private class PreStart : StateMachine<IceGamePresenter>.State
        {
            public override void Begin(IceGamePresenter owner)
            {
                
            }
        }
    }
}
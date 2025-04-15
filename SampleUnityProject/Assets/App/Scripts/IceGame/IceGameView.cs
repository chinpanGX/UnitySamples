using System;
using System.Collections.Generic;
using App.IceGame.Domain;
using AppCore.Runtime;
using AppService.Runtime;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace App.IceGame
{
    public class IceGameView : MonoBehaviour, IView
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private ScoreElementView scoreElementView;
        [SerializeField] private IceLifeElementView iceLifeElementView;
        [SerializeField] private RectTransform spawnPoint;
        [SerializeField] private List<CustomerElementView> customerElementViews;

        public Canvas Canvas => canvas;
        public ScoreElementView ScoreElementView => scoreElementView;

        private ModalScreen ModalScreen => ServiceLocator.Get<ModalScreen>();

        private AsyncOperationHandle<GameObject> iceElementHandle;

        private readonly List<IceElementView> iceElementViews = new();

        public static async UniTask<IceGameView> CreateAsync()
        {
            var handle = Addressables.InstantiateAsync(nameof(IceGameView));
            await handle.ToUniTask();
            handle.Result.SetActive(false);
            var view = handle.Result.GetComponentSafe<IceGameView>();
            view.Initialize();
            return view;
        }

        public void SetReduceLife(int index)
        {
            iceLifeElementView.ReduceLife(index);
        }

        public async UniTask CreateIceElement(IceData data, Action<string> onGiveIce)
        {
            iceElementHandle = await IceElementView.LoadAsync();
            var iceElement = Instantiate(iceElementHandle.Result, spawnPoint);
            iceElement.SetActive(false);
            var comp = iceElement.GetComponent<IceElementView>();
            iceElementViews.Add(comp.Initialize(iceElementHandle, spawnPoint, data, onGiveIce));
        }
        
        public void Tick()
        {
            foreach(var customer in customerElementViews)
            {
                //customer.UpdateOrder();
            }
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
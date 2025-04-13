using System;
using System.Linq;
using App.Common;
using AppCore.Runtime;
using AppService.Runtime;
using AudioService.Simple;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace App.Title
{
    public class TitleView : MonoBehaviour, IView
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private CustomButton startButton;
        [SerializeField] private CustomButton helpButton;
        [SerializeField] private CustomButton audioDemoButton;
        
        private readonly Subject<Unit> onClickedStart = new();
        private readonly Subject<Unit> onClickedHelp = new();
        private readonly Subject<Unit> onClickedAudioDemo = new();
        
        private AsyncOperationHandle<GameObject> popupHandle;
        
        public Observable<Unit> OnClickedStart => onClickedStart;
        public Observable<Unit> OnClickedHelp => onClickedHelp;
        public Observable<Unit> OnClickedAudioDemo => onClickedAudioDemo;
        
        public Canvas Canvas => canvas;
        private ViewScreen ViewScreen => ServiceLocator.Get<ViewScreen>();
        private SimpleAudioService AudioService => ServiceLocator.Get<SimpleAudioService>();

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
            
            audioDemoButton.SubscribeToClickAndPlaySe(() => { onClickedAudioDemo.OnNext(Unit.Default); },
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

        public async UniTask OpenLicensePopup()
        {
            var licenseTextHandle = Addressables.LoadResourceLocationsAsync("License");
            await licenseTextHandle.ToUniTask();
            
            var loadTasks = Enumerable.Select(licenseTextHandle.Result.Select(Addressables.LoadAssetAsync<TextAsset>), 
                asyncOperationHandle => asyncOperationHandle.ToUniTask()).ToList();

            try
            {
                var textResults = await UniTask.WhenAll(loadTasks);

                var body = "";
                foreach (var text in textResults)
                {
                    body += text.text;
                }

                var option = PopupTextOption.CreateOneButtonOption("ライセンス", body, "閉じる");
                popupHandle = await CommonPopupView.InstantiateAsync();
                var popup = popupHandle.Result.GetComponentSafe<CommonPopupView>();
                await popup.ShowOneButton(option);   
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
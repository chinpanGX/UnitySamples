using System;
using AppCore.Runtime;
using AppService.Runtime;
using AudioService.Simple;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace App.Common
{
    public class PopupTextOption
    {
        public readonly string Header;
        public readonly string Content;
        public readonly string NoButtonText;
        public readonly string YesButtonText;
        
        public static PopupTextOption CreateOneButtonOption(string header, string content, string buttonText)
        {
            return new PopupTextOption(header, content, string.Empty, buttonText);
        }
        
        public static PopupTextOption CreateYesNoOption(string header, string content)
        {
            return new PopupTextOption(header, content, "No", "Yes");
        }
        
        public static PopupTextOption CreateOkCancelOption(string header, string content)
        {
            return new PopupTextOption(header, content, "Cancel", "OK");
        }
        
        private PopupTextOption(string header, string content, string noButtonText, string yesButtonText)
        {
            Header = header;
            Content = content;
            NoButtonText = noButtonText;
            YesButtonText = yesButtonText;
        }
    }
    
    public class CommonPopupView : MonoBehaviour, IView
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private CustomText headerText;
        [SerializeField] private CustomText contentText;
        [SerializeField] private CustomButton noButton;
        [SerializeField] private CustomButton yesButton;
        [SerializeField] private ViewPlayableDirector viewPlayableDirector;
        
        public Canvas Canvas => canvas;
        private SimpleAudioService AudioService => ServiceLocator.Get<SimpleAudioService>();
        private ModalScreen ModalScreen => ServiceLocator.Get<ModalScreen>();
        
        public static async UniTask<AsyncOperationHandle<GameObject>> InstantiateAsync()
        {
            var instance = Addressables.InstantiateAsync(nameof(CommonPopupView));
            await instance.ToUniTask();
            instance.Result.gameObject.SetActive(false);
            return instance;
        }
        
        /// <summary>
        /// OneButtonのポップアップを表示する
        /// </summary>
        /// <param name="option"></param>
        /// <description>
        /// Push()とOpen()は自動で行われる
        /// 閉じるまで待機する
        /// </description>
        public async UniTask ShowOneButton(PopupTextOption option)
        {
            ModalScreen.Push(this);
            noButton.gameObject.SetActive(false);
            yesButton.gameObject.SetActive(true);
            yesButton.SetTextSafe(option.YesButtonText);
            var tcs = new UniTaskCompletionSource();
            yesButton.SubscribeToClickAndPlaySe(() => tcs.TrySetResult(), 
                new AudioOptions(AudioService, "SE_Ok"),
                canvasGroup
            );
            headerText.SetTextSafe(option.Header);
            contentText.SetTextSafe(option.Content);
            gameObject.SetActive(true);
            await viewPlayableDirector.PlayInAsync(destroyCancellationToken);
            await tcs.Task;
            await CloseAsync();
        }
        
        /// <summary>
        /// 肯定/否定のポップアップ
        /// </summary>
        /// <param name="option"></param>
        /// <description>
        /// 左ボタンが否定、右ボタンが肯定になる
        /// Push()とOpen()は自動で行われる
        /// 閉じるまで待機する
        /// </description>
        /// <returns> true = yes, false = no </returns>
        public async UniTask<bool> ShowYesNo(PopupTextOption option)
        {
            ModalScreen.Push(this);
            noButton.gameObject.SetActive(true);
            yesButton.gameObject.SetActive(true);
            noButton.SetTextSafe(option.NoButtonText);
            yesButton.SetTextSafe(option.YesButtonText);
            var tcs = new UniTaskCompletionSource<bool>();
            noButton.SubscribeToClickAndPlaySe(() => tcs.TrySetResult(false), 
                new AudioOptions(AudioService, "SE_Cancel"),
                canvasGroup
            );
            yesButton.SubscribeToClickAndPlaySe(() => tcs.TrySetResult(true), 
                new AudioOptions(AudioService, "SE_Ok"),
                canvasGroup
            );
            headerText.SetTextSafe(option.Header);
            contentText.SetTextSafe(option.Content);
            gameObject.SetActive(true);
            await viewPlayableDirector.PlayInAsync(destroyCancellationToken);
            var result = await tcs.Task;
            await CloseAsync();
            return result;
        }
        
        [Obsolete("使用しないでください。")]
        public void Push() { }
        
        [Obsolete("使用しないでください。")]
        public void Pop()
        {
        }
        
        [Obsolete("使用しないでください。")]
        public void Open()
        {
            
        }
        
        [Obsolete("使用しないでください。")]
        public void Close()
        {
            
        }
        
        private async UniTask CloseAsync()
        {
            await viewPlayableDirector.PlayOutAsync(destroyCancellationToken);
            ModalScreen.Pop(this);
            Destroy(gameObject);
        }
    }
}
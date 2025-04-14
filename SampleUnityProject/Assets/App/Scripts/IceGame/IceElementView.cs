using System;
using System.Threading;
using App.IceGame.Domain;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace App.IceGame
{
    public class IceElementView : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField] private Image image;
        public int Index { get; private set; }
        private IceData iceData;
        private AsyncOperationHandle<GameObject> handle;

        public static async UniTask<IceElementView> CreateAsync(Transform parent, int index, IceData data)
        {
            var handle = Addressables.InstantiateAsync("IceElementView", parent);
            await handle.ToUniTask();
            handle.Result.SetActive(false);
            var elementView = handle.Result.GetComponent<IceElementView>();
            elementView.Initialize(index, data, handle);
            return elementView;
        }

        private void Initialize(int index, IceData data, AsyncOperationHandle<GameObject> asyncOperationHandle)
        {
            handle = asyncOperationHandle;
            gameObject.SetActive(true);
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;

            Index = index;
            iceData = data;

            iceData.Life.SubscribeAwait(async (life, ct) =>
                {
                    if (life <= 0)
                    {
                        Destroy(gameObject);
                    }
                    else if (life == 50)
                    {
                        await BlinkAsync(ct).SuppressCancellationThrow();
                    }
                }
            ).RegisterTo(destroyCancellationToken);
        }
        
        private async UniTask BlinkAsync(CancellationToken token)
        {
            var blinkInterval = 0.5f;
            var normalColor = Color.white;
            var blinkColor = Color.clear;

            while (iceData.Life.CurrentValue > 0)
            {
                if (iceData.Life.CurrentValue <= 10)
                {
                    blinkInterval = Mathf.Max(0.1f, blinkInterval - 0.05f); // 徐々に速くする
                }

                image.color = image.color == normalColor ? blinkColor : normalColor;
                var cancel = await UniTask.Delay((int)(blinkInterval * 1000), cancellationToken: token)
                    .SuppressCancellationThrow();

                if (cancel) break;
            }

            image.color = normalColor; // 点滅終了時に色をリセット
        }

        #region ドラッグ&ドロップ

        public void OnBeginDrag(PointerEventData eventData)
        {

        }

        public void OnDrag(PointerEventData eventData)
        {

        }

        public void OnEndDrag(PointerEventData eventData)
        {

        }

        #endregion ドラッグ&ドロップ

        private void OnDestroy()
        {
            if (handle.IsValid())
                handle.Release();
        }
    }
}
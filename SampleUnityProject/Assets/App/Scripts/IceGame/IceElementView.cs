using System;
using System.Collections.Generic;
using System.Threading;
using App.IceGame.Domain;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using ZLinq;

namespace App.IceGame
{
    public class IceElementView : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private RectTransform parentRectTransform;
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private Image image;
        [SerializeField] private List<CustomerElementView> customerElementViews;

        private IceData iceData;
        private Action<string> onGiveIce;

        public static async UniTask<AsyncOperationHandle<GameObject>> LoadAsync()
        {
            var handle = Addressables.LoadAssetAsync<GameObject>("IceElementView");
            await handle.ToUniTask();
            return handle;
        }

        public IceElementView Initialize(AsyncOperationHandle<GameObject> handle, RectTransform parent, IceData data,
            Action<string> onGiveIce)
        {
            var instance = Instantiate(handle.Result, parent);
            instance.SetActive(false);
            gameObject.SetActive(true);
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;

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

            this.onGiveIce = onGiveIce;
            return instance.GetComponent<IceElementView>();
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
            SetPosition(eventData.position);
        }

        public void OnDrag(PointerEventData eventData)
        {
            SetPosition(eventData.position);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            foreach (var customerElementView in customerElementViews
                         .AsValueEnumerable()
                         .Where(customerElementView => Contains(eventData, customerElementView.Area)))
            {
                onGiveIce?.Invoke(customerElementView.OrderUniqueId);
                Destroy(gameObject);
                return;
            }
        }

        #endregion ドラッグ&ドロップ

        private void SetPosition(Vector2 screenPoint)
        {
            if (parentRectTransform == null)
            {
                Debug.LogError("parentRectPosition is null");
                return;
            }

            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform, screenPoint,
                canvas.worldCamera, out Vector2 position
            );
            rectTransform.anchoredPosition = position;
        }

        private static bool Contains(PointerEventData target, RectTransform area)
        {
            var bounds = CalcBounds(area);
            RectTransformUtility.ScreenPointToWorldPointInRectangle(area, target.position, target.pressEventCamera,
                out var worldPos
            );
            worldPos.z = 0f;
            return bounds.Contains(worldPos);
        }

        private static Bounds CalcBounds(RectTransform target)
        {
            var corners = new Vector3[4];
            var min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            target.GetWorldCorners(corners);
            for (var i = 0; i < 4; i++)
            {
                min = Vector3.Min(corners[i], min);
                max = Vector3.Max(corners[i], max);
            }

            max.z = 0f;
            min.z = 0f;

            var bounds = new Bounds(min, Vector3.zero);
            bounds.Encapsulate(max);
            return bounds;
        }
    }
}
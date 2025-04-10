using App.SampleInGame.Domain;
using AppService.Runtime;
using Cysharp.Threading.Tasks;
using MessagePipe;
using R3;
using R3.Triggers;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace App.SampleInGame.Presentation
{
    public class Ball : MonoBehaviour
    {
        private AsyncOperationHandle<GameObject> Handle;
        private IPublisher<BallCollisionMessage> Publisher {get; set;}
        private BallData Data { get; set; }
        
        public static async UniTask CreateAsync(BallData data, IPublisher<BallCollisionMessage> publisher, Transform parent, Vector3 spawnPosition)
        {
            var handle = Addressables.InstantiateAsync(nameof(Ball), parent);
            await handle.ToUniTask();
            var go = handle.Result;
            go.SetActive(false);
            go.GetComponentSafe<RectTransform>().anchoredPosition = new Vector2(spawnPosition.x, spawnPosition.y);
            var ball = go.GetComponentSafe<Ball>();
            ball.Initialize(handle, publisher, data);
        }

        private void Initialize(AsyncOperationHandle<GameObject> handle, IPublisher<BallCollisionMessage> publisher, BallData data)
        {
            Handle = handle;
            Data = data;
            Publisher = publisher;
            gameObject.SetActive(true);
        }

        private void Awake()
        {
            this.OnCollisionEnter2DAsObservable()
                .Where(c => c.gameObject.TryGetComponent<Ball>(out var otherBall) && otherBall != this)
                .Subscribe(_ =>
                {
                    Publisher.Publish(new BallCollisionMessage(Data.Score));
                    Destroy(gameObject);
                })
                .RegisterTo(destroyCancellationToken);
        }
        
        private void OnDestroy()
        {
            if (Handle.IsValid())
            {
                Addressables.Release(Handle);
            }
        }
    }
}
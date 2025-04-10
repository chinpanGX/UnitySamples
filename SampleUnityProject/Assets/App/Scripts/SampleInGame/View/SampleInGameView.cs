using App.SampleInGame.Domain;
using App.SampleInGame.Presentation;
using AppCore.Runtime;
using AppService.Runtime;
using AudioService.Simple;
using Cysharp.Threading.Tasks;
using MessagePipe;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace App.SampleInGame.View
{
    public class SampleInGameView : MonoBehaviour, IView
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private Transform ballParent;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private CustomButton backButton;

        private readonly Subject<Unit> onClickedBack = new();
        public Observable<Unit> OnClickedBack => onClickedBack;

        public Canvas Canvas => canvas;
        private ViewScreen ViewScreen => ServiceLocator.Get<ViewScreen>();
        private IPublisher<BallCollisionMessage> Publisher { get; set; }
        private Camera Camera { get; set; }

        public static async UniTask<SampleInGameView> CreateAsync(IPublisher<BallCollisionMessage> publisher, Camera camera)
        {
            var prefab = await Addressables.InstantiateAsync(nameof(SampleInGameView));
            prefab.SetActive(false);
            var view = prefab.GetComponentSafe<SampleInGameView>();
            view.Initialize(publisher, camera);
            return view;
        }

        private void Initialize(IPublisher<BallCollisionMessage> publisher, Camera uiCamera)
        {
            Publisher = publisher;
            Camera = uiCamera;
            backButton.SubscribeToClickAndPlaySe(() => { onClickedBack.OnNext(Unit.Default); },
                new AudioOptions(ServiceLocator.Get<SimpleAudioService>(), "SE_Ok")
            );
        }

        public async UniTask CreateBallAsync(BallData data, Vector3 clickPosition)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                ballParent as RectTransform, clickPosition, Camera, out var spawnPosition);
            await Ball.CreateAsync(data, Publisher, ballParent, spawnPosition);
        }

        public void SetScore(int score)
        {
            scoreText.text = score.ToString();
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
    }
}
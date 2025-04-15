using App.IceGame.Domain;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace App.IceGame
{
    public class CustomerElementView : MonoBehaviour
    {
        private static readonly string SpriteAtlasPath = "IceGamePackSprite";
        
        [SerializeField] private Image orderIceImage;
        [SerializeField] private RectTransform area;
        
        public RectTransform Area => area;
        public string OrderUniqueId => iceData.UniqueId;
        
        private IceData iceData;
        private AsyncOperationHandle<SpriteAtlas> spriteAtlasHandle;
        
        public void UpdateOrder(IceData data)
        {
            iceData = data;
            LoadSpriteAsync().Forget();
        }
        
        private async UniTaskVoid LoadSpriteAsync()
        {
            spriteAtlasHandle = Addressables.LoadAssetAsync<SpriteAtlas>(SpriteAtlasPath);
            await spriteAtlasHandle.ToUniTask();
            var sprite = spriteAtlasHandle.Result.GetSprite(iceData.GetAssetPath());
            orderIceImage.sprite = sprite;
        }
        
        private void OnDestroy()
        {
            if (spriteAtlasHandle.IsValid())
            {
                spriteAtlasHandle.Release();
            }
        }
    }
}
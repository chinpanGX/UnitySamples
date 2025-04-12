using System.Threading;
using AppService.Runtime;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;

namespace App.Common
{
    public class ViewPlayableDirector : MonoBehaviour
    {
        [SerializeField] private PlayableDirector playableDirector;
        [SerializeField] private PlayableAsset introPlayableAsset;
        [SerializeField] private PlayableAsset outroPlayableAsset;
        
        public async UniTask PlayInAsync(CancellationToken token)
        {
            playableDirector.playableAsset = introPlayableAsset;
            await playableDirector.PlayAsyncSafe(token);
        }
        
        public async UniTask PlayOutAsync(CancellationToken token)
        {
            playableDirector.playableAsset = outroPlayableAsset;
            await playableDirector.PlayAsyncSafe(token);
        }
    }
}
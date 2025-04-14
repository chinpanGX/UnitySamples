using AppService.Runtime;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

namespace App.IceGame
{
    public class ScoreElementView : MonoBehaviour
    {
        [SerializeField] private PlayableDirector playableDirector;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private CustomText addScoreText;
        
        private int totalScore;
        
        public void Initialize()
        {
            totalScore = 0;
            addScoreText.SetTextSafe(string.Empty);
        }
        
        public async UniTask UpdateScore(int score)
        {
            var addScore = score - totalScore;
            addScoreText.SetTextSafe($"+{addScore}");
            await UniTask.WhenAll(playableDirector.PlayAsyncSafe(destroyCancellationToken),
                LMotion.Create(totalScore, score, 0.5f)
                    .BindToText(scoreText).ToUniTask(destroyCancellationToken)
            );
            totalScore = score;
        }
    }
}
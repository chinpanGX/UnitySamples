using UnityEngine;

namespace TweetWithScreenShot
{
    [CreateAssetMenu(fileName = "TweetOptions", menuName = "Create ScriptableObjects/TweetOptions")]
    public class TweetOptions : ScriptableObject
    {
        [SerializeField] private string clientID;
        [SerializeField] private string[] hashTags;
        
        public string ClientID => clientID;
        public string[] HashTags => hashTags;
    }
}
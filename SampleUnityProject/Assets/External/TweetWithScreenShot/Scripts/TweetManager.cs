using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Xml.Linq;
using System;
#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

namespace TweetWithScreenShot
{

    public class TweetManager : MonoBehaviour
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void OpenWindow(string url);
#endif
        [SerializeField] private TweetOptions tweetOptions;
        
        void Start()
        {
            DontDestroyOnLoad(this);
        }
        
        public void Tweet(string text)
        {
            if (string.IsNullOrEmpty(text)) 
                Debug.LogWarning("Tweet text is empty.");
            StartCoroutine(TweetWithScreenShot(text));
        }

        private IEnumerator TweetWithScreenShot(string text)
        {
            yield return new WaitForEndOfFrame();
            var tex = ScreenCapture.CaptureScreenshotAsTexture();

            // imgurへアップロード
            string UploadedURL = "";

            UnityWebRequest www;

            WWWForm wwwForm = new WWWForm();
            wwwForm.AddField("image", Convert.ToBase64String(tex.EncodeToJPG()));
            wwwForm.AddField("type", "base64");

            www = UnityWebRequest.Post("https://api.imgur.com/3/image.xml", wwwForm);

            www.SetRequestHeader("AUTHORIZATION", "Client-ID " + tweetOptions.ClientID);

            yield return www.SendWebRequest();

            if (!string.IsNullOrEmpty(www.error))
            {
                Debug.Log("Error: " + www.error);
            }
            else if (www.responseCode != 200)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Data: " + www.downloadHandler.text);
                XDocument xDoc = XDocument.Parse(www.downloadHandler.text);
                //Twitterカードように拡張子を外す
                string url = xDoc.Element("data").Element("link").Value;
                url = url.Remove(url.Length - 4, 4);
                UploadedURL = url;
            }

            text += " " + UploadedURL;
            var hashTags = tweetOptions.HashTags;
            string hashtags = "&hashtags=";
            if (hashTags.Length > 0)
            {
                hashtags += string.Join (",", hashTags);
            }

            // ツイッター投稿用URL
            string TweetURL = "http://twitter.com/intent/tweet?text=" + text + hashtags;

#if UNITY_WEBGL && !UNITY_EDITOR
            OpenWindow(TweetURL);
#elif UNITY_EDITOR
            System.Diagnostics.Process.Start (TweetURL);
#else
            Application.OpenURL(TweetURL);
#endif
        }
    }
}

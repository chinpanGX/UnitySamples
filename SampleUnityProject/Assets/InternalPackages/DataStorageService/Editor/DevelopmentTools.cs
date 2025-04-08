using UnityEditor;
using UnityEngine;

namespace DataStorageService.Editor
{
    public static class DevelopmentTools
    {
        [MenuItem("DevelopmentTools/セーブデータ/フォルダを開く", priority = 1)]
        public static void OpenExplorer()
        {
            // WindowsEditorの場合は、パスの区切り文字を\\に置き換える
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                var path = $"{Application.persistentDataPath}".Replace("/", "\\");
                System.Diagnostics.Process.Start(path);
            }
            // それ以外の環境（Macとか）は、パスの区切り文字を/のまま
            else
            {
                var path = $"{Application.persistentDataPath}";
                System.Diagnostics.Process.Start(path);
            }
        }
        
        [MenuItem("DevelopmentTools/セーブデータ/削除する", priority = 100)]
        public static void DeleteSaveData()
        {
            var path = $"{Application.persistentDataPath}".Replace("/", "\\");
            if (System.IO.Directory.Exists(path))
            {
                if (EditorUtility.DisplayDialog("確認", 
                        "セーブデータを削除してもよろしいですか？",
                        "Yes", "No"
                    ))
                {
                    System.IO.Directory.Delete(path, true);
                    EditorUtility.DisplayDialog("確認",
                        "セーブデータを削除しました。", "OK"
                    );
                }
            }
            else
            {
                EditorUtility.DisplayDialog("エラー", $"\"{path}\"\nが、見つかりませんでした。", "OK");
            }
        }
    }
}
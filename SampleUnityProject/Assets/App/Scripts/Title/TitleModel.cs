using AppCore.Runtime;
using UnityEngine;

namespace App.Title
{
    public class TitleModel : IModel
    {
        private readonly string Url = "https://github.com/chinpanGX";

        public void Initialize()
        {

        }

        public void Tick()
        {

        }

        public void Dispose()
        {

        }

        public void OpenUrl()
        {
            Application.OpenURL(Url);
        }
    }

}
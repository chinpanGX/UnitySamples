#nullable enable
using System.Threading;
using AppCore.Runtime;
using Cysharp.Threading.Tasks;
using R3;

namespace App.AudioDemo
{
    public class AudioDemoPresenter : IPresenter
    {
        private IDirector Director { get; }
        private AudioDemoTopView View { get; }
        private readonly CancellationTokenSource cts = new();

        public AudioDemoPresenter(IDirector director, AudioDemoTopView view)
        {
            Director = director;
            View = view;

            // AudioSettingのクリックイベントの購読
            View.OnClickedOpenAudioSettings
                // クリックしたら、OpenAudioSettingsを実行する 
                .SubscribeAwait(async (_, _) => { await OpenAudioSettings(); })
                .RegisterTo(cts.Token);

            // Backのクリックイベントの購読
            View.OnClickedBack
                // クリックしたら、Titleに戻る 
                .SubscribeAwait(async (_, _) => await Director.PushAsync("Title"))
                .RegisterTo(cts.Token);

            View.Push();
            View.Open();
        }

        // AudioSettingを開く
        private async UniTask OpenAudioSettings()
        {
            var audioSettingsView = await AudioSettingView.CreateAsync();
            audioSettingsView.Push();
            audioSettingsView.Open();
        }

        public void Tick()
        {

        }

        public void Dispose()
        {
            View.Pop();
            cts.Cancel();
            cts.Dispose();
        }
    }
}
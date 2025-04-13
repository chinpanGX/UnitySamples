using AppCore.Runtime;
using UnityEngine;

namespace App.IceGame
{
    public class IceGameView : MonoBehaviour, IView
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private CanvasGroup canvasGroup;
        
        public Canvas Canvas => canvas;
        private ModalScreen ModalScreen => ServiceLocator.Get<ModalScreen>();
        
        public void Push()
        {
            ModalScreen.Push(this);
        }
        
        public void Pop()
        {
            ModalScreen.Pop(this);
        }
        
        public void Open()
        {
            throw new System.NotImplementedException();
        }
        
        public void Close()
        {
            throw new System.NotImplementedException();
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

namespace App.IceGame
{
    public class IceLifeElementView : MonoBehaviour
    {
        [SerializeField] private List<GameObject> lifeIcons = new ();
        
        public void ReduceLife(int disposedIceCount)
        {
            if (disposedIceCount < 0 || disposedIceCount >= lifeIcons.Count
            )
            {
                return;
            }
            
            lifeIcons[disposedIceCount].SetActive(false);
        }
    }
}
using UnityEngine;
using TS.Gameplay;

namespace TS.Core
{
    public class GameManager : MonoBehaviour
    {
        public GameState State { get; private set; }

        [SerializeField] private Player player;

        private void Awake()
        {
            Application.targetFrameRate = 300;
        }
        
        
    }   
}
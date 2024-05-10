using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GameUI : MonoBehaviour
    {
        public Button endTurnButton;

        void Start()
        {
            endTurnButton.onClick.AddListener(OnEndTurnClicked);
        }

        void OnEndTurnClicked()
        {
            Debug.Log("End Turn Button clicked");
            // Здесь добавьте логику для завершения хода
        }
    }
}
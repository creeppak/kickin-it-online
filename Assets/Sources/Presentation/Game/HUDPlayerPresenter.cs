using TMPro;
using UnityEngine;

namespace KickinIt.Presentation.Match
{
    public class HUDPlayerPresenter : MonoBehaviour
    {
        [SerializeField] private TMP_Text playerName;
        
        public void Set(string userName)
        {
            playerName.text = userName;
        }
    }
}
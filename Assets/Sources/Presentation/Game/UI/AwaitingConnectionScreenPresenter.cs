using KickinIt.Presentation.Screens;
using TMPro;
using UnityEngine;

namespace KickinIt.Presentation.Match
{
    public class AwaitingConnectionScreenPresenter : GameScreenPresenter
    {
        [SerializeField] private TMP_Text _status;
        
        protected override void OnScreenLoaded()
        {
            _status.text = "Awaiting connection...";
        }
    }
}
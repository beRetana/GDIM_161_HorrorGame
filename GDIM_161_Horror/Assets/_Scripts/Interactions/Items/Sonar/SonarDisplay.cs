using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Interactions
{
    public class SonarDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _distanceDisplay;
        [SerializeField] private List<GameObject> _dots;

        private List<Vector2> _objectLocations;
        private float _closestLocation;

        // The size of the sonar screen relative to one unit of its object.
        private const float _SCREEN_SCALE = 0.13f;

        public void LoadInformation(List<Vector2> objectLocations, float closestLocation)
        {
            _objectLocations = objectLocations;
            _closestLocation = closestLocation;
            //Play animation for the sonar scanning
            // Just gonna do it manually for now
            UpdateUI();
        }

        public void UpdateUI()
        {
            DisplayDots(_objectLocations);
            _distanceDisplay.text = _closestLocation.ToString();
        }

        private void DisplayDots(List<Vector2> objectsLocation)
        {
            for (int i = 0; i < _objectLocations.Count; i++)
            {
                _dots[i].transform.localPosition = objectsLocation[i] * _SCREEN_SCALE;
                _dots[i].GetComponent<DotDisplay>().ActivateDot();
            }
        }
    }
}

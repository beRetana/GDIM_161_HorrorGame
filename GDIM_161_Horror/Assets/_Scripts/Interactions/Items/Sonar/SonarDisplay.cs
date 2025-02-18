using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;

namespace Interactions
{
    public class SonarDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _distanceDisplay;
        [SerializeField] private List<GameObject> _dots;

        [Tooltip("Scale From Real World To Sonnar Screen")]
        [SerializeField] [Range(.0f, 1f)] private float _displayScale = 0.1f;

        private List<Vector2> _objectLocations;
        private float _closestLocation;

        public void LoadInformation(List<Vector2> objectLocations, float closestLocation)
        {
            Debug.Log($"Loading Information: {objectLocations}, {closestLocation}");
            _objectLocations = objectLocations;
            _closestLocation = closestLocation;
            //Play animation for the sonar scanning
            // Just gonna do it manually for now
            UpdateUI();
        }

        public void UpdateUI()
        {
            _distanceDisplay.text = _closestLocation.ToString();
            DisplayDots(_objectLocations);
        }

        private void DisplayDots(List<Vector2> objectsLocation)
        {
            Debug.Log($"Displaying Dots");
            for (int i = 0; i < _objectLocations.Count; i++)
            {
                Debug.Log (Vector3.Distance(transform.position, _dots[i].transform.position));
                _dots[i].transform.localScale = objectsLocation[i] * _displayScale;
                _dots[i].GetComponent<DotDisplay>().ActivateDot();
            }
        }
    }
}

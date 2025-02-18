using UnityEngine;
using System.Collections.Generic;

namespace Interactions
{
    public class Sonar : PickableItem
    {
        [Tooltip("Detection Radious for the Sonar")]
        [SerializeField, Range(1f, 50f)] private float _detectionRadius = 20f;
        [SerializeField] private LayerMask _sonarDetectable;
        [SerializeField] private SonarDisplay _sonarDisplay;

        private List<Vector2> _objectsLocation;

        protected override void Start()
        {
            base.Start();
            _objectsLocation = new List<Vector2>();
        }

        private void CollectDetectablesLocation()
        {
            Collider[] listOfObjects = Physics.OverlapSphere(transform.position, _detectionRadius, _sonarDetectable);

            _objectsLocation.Clear();
            foreach (Collider obj in listOfObjects)
            {
                _objectsLocation.Add(new Vector2(obj.transform.position.x, obj.transform.position.z) / _detectionRadius);
            }
        }

        private float GetClosestLocationDistance()
        {
            int firstElement = 0;

            Vector2 closestPosition = _objectsLocation[firstElement];
            Vector2 sonarPosition = new Vector2(transform.position.x, transform.position.z);

            foreach (Vector2 position in _objectsLocation)
            {
                float closestToSonnar = Vector2.Distance(sonarPosition, closestPosition);
                float objToSonnar = Vector2.Distance(sonarPosition, position);

                if ((objToSonnar - closestToSonnar) < 0.1f) closestPosition = position;
            }

            return Vector3.Distance(transform.position, closestPosition);
        }

        private void ScanArea()
        {
            CollectDetectablesLocation();
            Debug.Log($"Sonar Scanning: {_objectsLocation.Count}");
            if (_objectsLocation.Count == 0) return;
            _sonarDisplay.LoadInformation(_objectsLocation, GetClosestLocationDistance());
        }

        public override void UseItem(int playerId)
        {
            base.UseItem(playerId);
            ScanArea();
        }
    }
}

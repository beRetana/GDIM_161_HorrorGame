using UnityEngine;
using System.Collections.Generic;

namespace Interactions
{
    public class Sonar : PickableItem
    {
        [SerializeField] private float _detectionRadius = 20f;
        [SerializeField] private LayerMask _sonnaDetectable;
        [SerializeField] private SonarDisplay _sonarDisplay;

        private HashSet<Transform> _objectsDetected;

        protected override void Start()
        {
            base.Start();
            _objectsDetected = new HashSet<Transform>();
        }

        private void CollectSonarDetectables()
        {
            RaycastHit[] listOfObjects = Physics.SphereCastAll(transform.position, _detectionRadius, Vector3.zero, 0, 1);
            _objectsDetected.Clear();
            foreach (RaycastHit obj in listOfObjects)
            {
                _objectsDetected.Add(obj.transform);
            }
        }

        private void ScanArea()
        {
            CollectSonarDetectables();
            _sonarDisplay.UpdateVisuals(_objectsDetected);
        }

        public override void UseItem(int playerId)
        {
            base.UseItem(playerId);
            ScanArea();
        }
    }
}

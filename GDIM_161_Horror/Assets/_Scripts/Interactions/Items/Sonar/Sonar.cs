using UnityEngine;
using System.Collections.Generic;

namespace Interactions
{
    public class Sonar : PickableItem
    {
        [Tooltip("Detection Radious for the Sonar")]
        [SerializeField, Range(1f, 100f)] private float _detectionRadius = 20f;
        [SerializeField] private LayerMask _sonarDetectable;
        [SerializeField] private SonarDisplay _sonarDisplay;

        private List<Vector2> _objectsRelativeLocation;
        private List<Vector3> _objectsWorldLocation;

        protected override void Start()
        {
            base.Start();
            _objectsRelativeLocation = new List<Vector2>();
            _objectsWorldLocation = new List<Vector3>();
        }

        private void CollectDetectablesLocation(int playerId)
        {
            Collider[] listOfObjects = Physics.OverlapSphere(transform.position, _detectionRadius, _sonarDetectable);
            _objectsRelativeLocation.Clear();
            
            Transform player = PlayerManager.Instance.GetPlayer(playerId).transform;
            Vector3 playerToObject = Vector3.one;

            foreach (Collider obj in listOfObjects)
            {
                _objectsWorldLocation.Add(obj.transform.position);
                playerToObject = obj.transform.position - player.position;
                playerToObject = Quaternion.AngleAxis(player.transform.rotation.eulerAngles.y, -Vector3.up) * playerToObject;
                _objectsRelativeLocation.Add(new Vector2(playerToObject.x, playerToObject.z) / _detectionRadius);
            }
        }

        private float GetClosestLocationDistance(int playerId)
        {
            float closestDistance = _detectionRadius;
            Vector3 player = PlayerManager.Instance.GetPlayer(playerId).transform.position;

            for( int i = 0; i < _objectsWorldLocation.Count-1; i++)
            {
                float closestToSonnar = (_objectsWorldLocation[i] - player).magnitude;
                float nextObject = (_objectsWorldLocation[i + 1] - player).magnitude;

                closestDistance = Mathf.Min(closestDistance, nextObject);
            }

            return Mathf.Round(closestDistance * 100) / 100;
        }

        private void ScanArea(int playerId)
        {
            CollectDetectablesLocation(playerId);
            if (_objectsRelativeLocation.Count == 0) return;
            _sonarDisplay.LoadInformation(_objectsRelativeLocation, GetClosestLocationDistance(playerId));
        }

        public override void UseItem(int playerId)
        {
            base.UseItem(playerId);
            ScanArea(playerId);
        }
    }
}

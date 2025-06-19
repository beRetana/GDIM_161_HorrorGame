using UnityEngine;
using System.Collections.Generic;


namespace Interactions
{
    public class Sonar : NetworkPickableItem
    {
        [Tooltip("Detection Radious for the Sonar")]
        [SerializeField, Range(1f, 100f)] private float _detectionRadius = 20f;
        [SerializeField, Range(0f, 20f)] private float _verticalBuffer = 1f;
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
            _objectsWorldLocation.Clear();

            Transform player = PlayerManager.Instance.GetPlayer(playerId).transform;
            Vector3 playerToObject = Vector3.one;

            foreach (Collider obj in listOfObjects)
            {
                if (Mathf.Abs(obj.transform.position.y - player.position.y) >= _verticalBuffer)
                    continue;

                playerToObject = obj.transform.position - player.position;
                playerToObject = Quaternion.AngleAxis(player.transform.rotation.eulerAngles.y, -Vector3.up) * playerToObject;
                _objectsRelativeLocation.Add(new Vector2(playerToObject.x, playerToObject.z) / _detectionRadius);

                if (obj.TryGetComponent<PlayerObjectController>(out PlayerObjectController playerController))
                    if (playerController.PlayerIdNumber == playerId) continue;
                
                _objectsWorldLocation.Add(obj.transform.position);
                Debugger($"Object Added To Work Location List: {obj.transform.name}");
            }
        }

        private float GetClosestLocationDistance(int playerId)
        {
            if (_objectsWorldLocation.Count == 0) return 0;
            Vector3 player = PlayerManager.Instance.GetPlayer(playerId).transform.position;
            float closestDistance = (_objectsWorldLocation[0] - player).magnitude;

            for ( int i = 0; i < _objectsWorldLocation.Count; i++)
            {
                float distanceToPlayer = (_objectsWorldLocation[i] - player).magnitude;

                closestDistance = Mathf.Min(closestDistance, distanceToPlayer);
            }
            Debugger($"Closest Distance: {closestDistance}");
            return Mathf.Round(closestDistance * 100) / 100;
        }

        private void ScanArea(int playerId)
        {
            CollectDetectablesLocation(playerId);
            if (_objectsRelativeLocation.Count == 0) return;
            _sonarDisplay.LoadInformation(_objectsRelativeLocation, GetClosestLocationDistance(playerId));
            AudioManager.instance.PlayOneShot(FMODEvents.instance.sonarPing, this.transform.position);
        }

        public override void UseItem(int playerId)
        {
            base.UseItem(playerId);
            ScanArea(playerId);
        }
    }
}

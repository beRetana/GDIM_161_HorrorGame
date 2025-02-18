using System.Collections.Generic;
using UnityEngine;

namespace Interactions
{
    public class SonarDisplay : MonoBehaviour
    {
        [SerializeField] 

        public void UpdateVisuals(HashSet<Transform> objectsDetected)
        {

        }

        private float GetClosestObjectDistance(HashSet<Transform> objectsDetected)
        {
            Transform closestObject = null;
            foreach (Transform obj in objectsDetected)
            {
                if (closestObject == null)
                {
                    closestObject = obj;
                    continue;
                }

                float closestToSonnar = Vector3.Distance(transform.position, closestObject.position);
                float objToSonnar = Vector3.Distance(transform.position, obj.position);

                if ((objToSonnar - closestToSonnar) < 0.1f) closestObject = obj;
            }

            return Vector3.Distance(transform.position, closestObject.position);
        }
    }
}

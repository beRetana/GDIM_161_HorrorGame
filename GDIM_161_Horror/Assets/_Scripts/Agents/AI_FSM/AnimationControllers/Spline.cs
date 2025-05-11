using UnityEngine;

namespace AI
{
    public class Spline : MonoBehaviour
    {
        [SerializeField]
        private Transform _start, _middle, _end;

        [SerializeField]
        private bool _showGizmos = true;

        private Vector3 CalculatePosition(float value, Vector3 startPos, Vector3 endPos, Vector3 midPos)
        {
            value = Mathf.Clamp01(value);
            return Vector3.Lerp(Vector3.Lerp(startPos, midPos, value), Vector3.Lerp(midPos, endPos, value), value);
        }

        public Vector3 CalculatePosition(float interpolationAmount)
        {
            return CalculatePosition(interpolationAmount, _start.position, _end.position, _middle.position);
        }

        public Vector3 CalculatePositionStart(float interpolationAmount, Vector3 startPosition)
        {
            return CalculatePosition(interpolationAmount, startPosition, _end.position, _middle.position);
        }

        public Vector3 CalculatePositionEnd(float interpolationAmount, Vector3 endPosition)
        {
            return CalculatePosition(interpolationAmount, _start.position, endPosition, _middle.position);
        }

        public void SetPoints(Vector3 startPoint, Vector3 midPointPosition, Vector3 endPoint)
        {
            if (_start == null || _middle == null || _end == null) return;

            _start.position = startPoint;
            _middle.position = midPointPosition;
            _end.position = endPoint;
        }
        private void OnDrawGizmos()
        {
            if (!_showGizmos || _start == null || _middle == null || _end == null) return;
            
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_start.position, 0.1f);
            Gizmos.DrawSphere(_end.position, 0.1f);
            Gizmos.DrawSphere(_middle.position, 0.1f);
            Gizmos.color = Color.magenta;
            int granularity = 5;
            for (int i = 0; i < granularity; i++)
            {
                Vector3 startPoint = i == 0 ? _start.position : CalculatePosition(i / (float)granularity);
                Vector3 endPoint = i == granularity ? _end.position : CalculatePosition((i + 1) / (float)granularity);
                Gizmos.DrawLine(startPoint, endPoint);
            }
        }

    }
}

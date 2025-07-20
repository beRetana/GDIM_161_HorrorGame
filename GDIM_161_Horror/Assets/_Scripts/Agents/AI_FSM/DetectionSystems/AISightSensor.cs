using System.Collections.Generic;
using UnityEngine;

namespace AI_FSM
{
    [ExecuteInEditMode]

    public class AISightSensor : MonoBehaviour
    {

        [Header("Detection Settings")]
        [SerializeField] private Vector3 _initialPosition;
        [SerializeField] private LayerMask _detectableLayers;
        [SerializeField] private LayerMask _obstructableLayers;
        [SerializeField] private float _range;
        [SerializeField, Range(10, 180)] private float _angle;
        [SerializeField] private float _height;
        [SerializeField, Range(1, 60)] private int _scanFrequency = 30;
        [SerializeField, Range(1,50)] private int _maxDetectables = 50;
        [SerializeField] private bool _enableDebugger;

        public delegate void SightSensor(GameObject target);
        public SightSensor _sightSensor;
        public GameObject TargetDetected { get { return _targetDetected; } }

        private Collider[] _colliders;
        private List<GameObject> _objectsOnSight = new List<GameObject>();
        private GameObject _targetDetected;
        private Vector3 _initialCenterPosition;
        private float _scanInterval;
        private float _scanTimer;
        private float _angleCos;
        private int _count;

        [Header("Debugging Visuals Settings")]
        [SerializeField] private Color _debugMeshColor = Color.green;
        [SerializeField] private Color _debugColorObjectDectected = Color.green;
        [SerializeField] private float _debugSphereSizeDetectedObjects = 0.2f;
        [SerializeField] private int _debugSegments = 10;
        [SerializeField] private bool _enableDebugVisuals;
        private Mesh _debugMesh;

        public float Range { get { return _range; } set { _range = value; } }
        public float Angle { get { return _angle; } 
            set { 
                _angle = value;
                _angleCos = Mathf.Cos(_angle * .5f * Mathf.Deg2Rad);
            } }

        private void Start()
        {
            _sightSensor = (_targetDetected) => { Debbuger($"The object dected value has been updated to {_targetDetected?.name}"); };
            _initialCenterPosition = new Vector3(_initialPosition.x, _initialPosition.y + _height/2, _initialPosition.z);
            _scanInterval = 1/_scanFrequency;
            _colliders = new Collider[_maxDetectables];
            _angleCos = Mathf.Cos(_angle * .5f * Mathf.Deg2Rad);
        }

        void Update(){
            // Timer for frequency of scanning
            _scanTimer -= Time.deltaTime;
            if (_scanTimer <= 0)
            {
                _scanTimer = _scanInterval;
                Scan();
            }
        }

        private void Scan()
        {
            //Scan the view for objects and invoke events if so.

            _count = Physics.OverlapSphereNonAlloc(transform.position + _initialCenterPosition, _range, 
                                                _colliders, _detectableLayers, 
                                                QueryTriggerInteraction.Collide);
            
            bool anythingDetected = false;
            bool sameTarget = false;
            _objectsOnSight.Clear();

            for(int i = 0; i < _count; i++)
            {
                if (!IsInSight(_colliders[i].gameObject)) continue;
                _objectsOnSight.Add(_colliders[i].gameObject);
                anythingDetected = true;

                if (_colliders[i].gameObject != _targetDetected) continue;
                sameTarget = true;
                break;
            }

            if(anythingDetected)
            {
                if (sameTarget) return;

                _targetDetected = ClosestObject(_objectsOnSight);
                //Call Event of object detected.
                _sightSensor?.Invoke(_targetDetected);
            }
            else if (_targetDetected != null)
            {
                _targetDetected = null;
                _sightSensor?.Invoke(_targetDetected);
            }
        }

        private GameObject ClosestObject(in List<GameObject> objectsList)
        {
            GameObject closestObject = null;
            float shortest = (objectsList[0].transform.position - transform.position).sqrMagnitude;

            for (int i = 0; i < objectsList.Count; i++)
            {
                float distance = (objectsList[i].transform.position - transform.position).sqrMagnitude;

                if (distance > shortest) continue;

                closestObject = objectsList[i];
                shortest = distance;
            }

            return closestObject;
        }

        private bool IsInSight(in GameObject target){
            // Check if the object is within angle of view, height and if it's not being blocked by something.
            Vector3 targetDirection = (target.transform.position - transform.position);
            float dotProduct = Vector3.Dot(transform.forward, targetDirection.normalized);

            Debbuger($"The Dot product is {dotProduct} and COS is {_angleCos} and sqrmag is {targetDirection.sqrMagnitude}");

            if (dotProduct >= _angleCos || (dotProduct >= 0 && targetDirection.sqrMagnitude <= 22f))
            {
                Debbuger($"The target {target} is within angle.");
                if ((transform.position.y + _initialPosition.y) <= target.transform.position.y && target.transform.position.y <= (transform.position.y + _initialPosition.y + _height))
                {
                    Debbuger($"The target {target} is within Y range.");
                    return !Physics.Raycast(transform.position + _initialCenterPosition, targetDirection, targetDirection.magnitude, _obstructableLayers);
                }
            }
            return  false;
        }

//__________________________________________________DEBUGGING VISUALS_____________________________________________________________

        private Mesh CreateDebugWedge(){
            // This creates a visual to aid debugging.

            Mesh mesh = new Mesh();

            int numberOfTriangles = (_debugSegments * 4) + 2 + 2;
            int numberVertices = numberOfTriangles * 3;

            Vector3[] vertices = new Vector3[numberVertices];
            int[] triangles = new int[numberVertices];

            Vector3 bottomCenter = Vector3.zero + _initialPosition;
            Vector3 bottomLeft = Quaternion.Euler(0, -_angle * .5f, 0) * Vector3.forward * _range + _initialPosition;
            Vector3 bottomRight = Quaternion.Euler(0, _angle * .5f, 0) * Vector3.forward * _range + _initialPosition;

            Vector3 topCenter = bottomCenter + Vector3.up * _height;
            Vector3 topRight = bottomRight + Vector3.up * _height;
            Vector3 topLeft = bottomLeft + Vector3.up * _height;

            int vert = 0;

            // left side
            vertices[vert++] = bottomCenter;
            vertices[vert++] = bottomLeft;
            vertices[vert++] = topLeft;

            vertices[vert++] = topLeft;
            vertices[vert++] = topCenter;
            vertices[vert++] = bottomCenter;

            // right side
            vertices[vert++] = bottomCenter;
            vertices[vert++] = topCenter;
            vertices[vert++] = topRight;

            vertices[vert++] = topRight;
            vertices[vert++] = bottomRight;
            vertices[vert++] = bottomCenter;

            float currentAngle = -_angle * .5f;
            float deltaAngle = _angle / _debugSegments;
            
            for(int i = 0; i < _debugSegments; ++i){

                bottomLeft = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * _range + _initialPosition;
                bottomRight = Quaternion.Euler(0, currentAngle + deltaAngle, 0) * Vector3.forward * _range + _initialPosition;

                topRight = bottomRight + Vector3.up * _height;
                topLeft = bottomLeft + Vector3.up * _height;

                // far side
                vertices[vert++] = bottomLeft;
                vertices[vert++] = bottomRight;
                vertices[vert++] = topRight;

                vertices[vert++] = topRight;
                vertices[vert++] = topLeft;
                vertices[vert++] = bottomLeft;

                // top
                vertices[vert++] = topCenter;
                vertices[vert++] = topLeft;
                vertices[vert++] = topRight;

                // bottom
                vertices[vert++] = bottomCenter;
                vertices[vert++] = bottomRight;
                vertices[vert++] = bottomLeft;

                currentAngle += deltaAngle;
            }

            for(int i = 0; i < numberVertices; ++i){
                triangles[i] = i;
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();

            return mesh;
        }

        private void OnValidate(){
            _debugMesh = CreateDebugWedge();
            _scanInterval = 1/_scanFrequency;
        }

        private void Debbuger(object log)
        {
            if (_enableDebugger) Debug.Log($"[{this.GetType().Name}] {log}");
        }

        private void OnDrawGizmos(){
            if(_enableDebugVisuals){
                if(_debugMesh){
                    Gizmos.color = _debugMeshColor;
                    Gizmos.DrawMesh(_debugMesh, transform.position, transform.rotation);
                }

                Gizmos.DrawWireSphere(transform.position + _initialCenterPosition, _range);

                for(int i = 0; i < _count; ++i){

                    Gizmos.color = _debugColorObjectDectected;

                    if(_targetDetected != null){
                        Gizmos.DrawSphere(_targetDetected.transform.position, 
                                    _targetDetected.transform.localScale.magnitude * _debugSphereSizeDetectedObjects);
                        break;
                    }
                }
            }
        }
    }
}


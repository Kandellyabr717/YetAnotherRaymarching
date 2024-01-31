using UnityEngine;
using UnityEngine.UIElements;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class RaymarchingRenderer : MonoBehaviour
{
    public RaymarchingObject Object
    {
        get
        {
            _object.Update(_shapeType, _operationType, _transform, _material, _smoothness, _blendStrength);
            return _object;
        }
    }
    public Transform Transform => _transform;
    [SerializeField]
    private Transform _transform;
    public ShapeType ShapeType => _shapeType;
    [SerializeField]
    private ShapeType _shapeType;
    public OperationType OperationType => _operationType;
    [SerializeField]
    private OperationType _operationType;
    [SerializeField]
    private Material _material;
    [SerializeField, Range(0, 1)]
    private float _smoothness;
    [SerializeField, Range(0, 1)]
    private float _blendStrength;
    private RaymarchingObject _object;

    private void Awake()
    {
        _object = new RaymarchingObject();
        _object.Update(_shapeType, _operationType, _transform, _material, _smoothness, _blendStrength);
    }

    private void OnDestroy()
    {
        RenderQueue.RemoveFromRender(this);
    }

    private void OnDisable()
    {
        RenderQueue.RemoveFromRender(this);
    }

    private void OnEnable()
    {
        RenderQueue.AddToRender(this);
    }
}

public struct RaymarchingObject
{
    public const int Size = sizeof(int) * 4 + sizeof(float) * 3 * 3 + sizeof(float) * 5;
    public int ShapeType { get; private set; }
    public int OperationType { get; private set; }
    public Vector3 Position { get; private set; }
    public Vector3 Rotation { get; private set; }
    public Vector3 Scale { get; private set; }
    public int ChildCount { get; private set; }
    public Vector4 Color { get; private set; }
    public float Smoothness { get; private set; }
    public float BlendStrength { get; private set; }

    public void Update(ShapeType shape, OperationType operation, Transform transform,
        Material material, float smoothness, float blendStrength)
    {
        ShapeType = (int)shape;
        OperationType = (int)operation;
        Position = transform.position;
        Rotation = new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z);
        Scale = transform.lossyScale;
        ChildCount = GetChildCount(transform);
        Color = material.color;
        Smoothness = smoothness;
        BlendStrength = blendStrength;

        int GetChildCount(Transform transform)
        {
            var count = 0;
            for(int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).GetComponent<RaymarchingRenderer>())
                {
                    count++;
                }
                count += GetChildCount(transform.GetChild(i));
            }
            return count;
        }
    }
}

public enum ShapeType 
{
    Sphere = 0,
    Cube = 1,
    Torus = 2,
    Prism = 3,
};

public enum OperationType
{
    Unite = 0,
    Cut = 1,
    Mask = 2,
    Blend = 3,
}
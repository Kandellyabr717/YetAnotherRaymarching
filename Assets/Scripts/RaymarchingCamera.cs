using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class RaymarchingCamera : MonoBehaviour
{
    [SerializeField]
    private Transform _light;
    [SerializeField]
    private RaymarcherController _raymarcher;
    private CameraData _cameraData;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        _cameraData.Update(Camera.current);
        var frame = _raymarcher.RenderFrame(source, RenderQueue.Objects,
            _cameraData, _light.rotation * Vector3.forward);
        Graphics.Blit(frame, destination);
    }

    private void Start()
    {
        _cameraData = new CameraData();
    }
}

public struct CameraData
{
    public static int Size = sizeof(float) * 16 * 2 + sizeof(float) * 2 + sizeof(float) * 3;
    public Matrix4x4 ToWorld { get; private set; }
    public Matrix4x4 InverseProjection { get; private set; }
    public Vector3 ProjectionNormal { get; private set; }
    public Vector2 Resolution { get; private set; }

    public void Update(Camera camera)
    {
        ToWorld = camera.cameraToWorldMatrix;
        InverseProjection = camera.projectionMatrix.inverse;
        ProjectionNormal = InverseProjection * new Vector4(0, 0, 0, 1);
        ProjectionNormal = Vector3.Normalize(ToWorld * ProjectionNormal);
        Resolution = new Vector2(camera.pixelWidth, camera.pixelHeight);
    }
}
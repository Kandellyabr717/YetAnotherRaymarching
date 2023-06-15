using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class RaymarcherController : MonoBehaviour
{
    [SerializeField]
    private ComputeShader _shader;
    private RenderTexture _frame;
    private ComputeBuffer _objectBuffer;

    public RenderTexture RenderFrame(RenderTexture source, RaymarchingObject[] objects,
        CameraData cameraData, Vector3 lightDirection)
    {
        if (objects.Length == 0)
        {
            return source;
        }
        UpdateFrameTexture(source);
        _objectBuffer = new ComputeBuffer(objects.Length, RaymarchingObject.Size);
        _objectBuffer.SetData(objects);
        _shader.SetMatrix("CameraToWorld", cameraData.ToWorld);
        _shader.SetMatrix("CameraInverseProjection", cameraData.InverseProjection);
        _shader.SetVector("CameraResolution", cameraData.Resolution);
        _shader.SetVector("CameraProjectionNormal", cameraData.ProjectionNormal);
        _shader.SetVector("LightDirection", lightDirection);
        _shader.SetBuffer(0, "ObjectsBuffer", _objectBuffer);
        _shader.SetInt("ObjectsCount", objects.Length);
        _shader.SetTexture(0, "Source", source);
        _shader.SetTextureFromGlobal(0, "Depth", "_CameraDepthTexture");
        _shader.SetTexture(0, "Frame", _frame);
        _shader.GetKernelThreadGroupSizes(0, out uint x, out uint y, out uint z);
        var treadGroup = new Vector3Int((int)(source.width / x), (int)(source.height / x), 1);
        _shader.Dispatch(0, treadGroup.x, treadGroup.y, treadGroup.z);
        _objectBuffer.Dispose();
        return _frame;
    }

    private void UpdateFrameTexture(RenderTexture source)
    {
        if (_frame == null || _frame.width != source.width || _frame.height != source.height)
        {
            if (_frame != null)
            {
                _frame.Release();
            }
            _frame = new RenderTexture(source.width, source.height, 0,
                RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            _frame.enableRandomWrite = true;
            _frame.Create();
        }
    }
}
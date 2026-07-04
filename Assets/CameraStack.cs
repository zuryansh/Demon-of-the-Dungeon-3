using EditorAttributes;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent (typeof(Camera))]
public class CameraStack : MonoBehaviour
{
    [SerializeField] bool debug;

    [SerializeField] Camera cam;
    [SerializeField]bool isMain;
    private void Start()
    {
        if(cam == null)cam = GetComponent<Camera>();

        GenerateStackIfMain();
    }
    //[Button("Gen Stack")]
    [ContextMenu("Gen Stack")]
    void GenerateStackIfMain()
    {
        isMain = Camera.main == cam;

        if (isMain)
        {
            var baseData = cam.GetUniversalAdditionalCameraData();

            CameraStack[] stack = FindObjectsByType<CameraStack>(FindObjectsSortMode.None);
            foreach (CameraStack cameraStack in stack)
            {
                if(debug)
                Debug.Log(cameraStack.gameObject.name);
                if (!baseData.cameraStack.Contains(cameraStack.cam) && cameraStack!=this)
                {
                    baseData.cameraStack.Add(cameraStack.cam);
                }
            }


        }
    }

}

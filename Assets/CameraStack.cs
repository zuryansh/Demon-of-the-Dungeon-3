using EditorAttributes;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent (typeof(Camera))]
public class CameraStack : MonoBehaviour
{
    Camera MainCam => Camera.main;


    [SerializeField] bool debug;
    [SerializeField] Camera cam;
    [SerializeField] int stackOrder;
    [SerializeField]bool isMain;



    private void Start()
    {
        if(cam == null)cam = GetComponent<Camera>();
        AddToMainStack();
    }


    [Button("Gen Stack")]
    [ContextMenu("Gen Stack")]
    void GenerateStackIfMain()
    {

        if (isMain)
        {
            var mainBaseData = cam.GetUniversalAdditionalCameraData();

            CameraStack[] stack = FindObjectsByType<CameraStack>(FindObjectsSortMode.None);
            foreach (CameraStack cameraStack in stack)
            {
                if (debug)
                    Debug.Log(cameraStack.gameObject.name);
                if (!mainBaseData.cameraStack.Contains(cameraStack.cam) && cameraStack != this)
                {
                    mainBaseData.cameraStack.Add(cameraStack.cam);
                    
                }
            }


        }
    }

    void AddToMainStack()
    {
        if (Camera.main == cam || isMain)
            return;

        var mainCamData = Camera.main.GetUniversalAdditionalCameraData();

        if (!mainCamData.cameraStack.Contains(cam))
        {
            mainCamData.cameraStack.Add(cam);

            // Sort the stack by each camera's CameraStack component.
            mainCamData.cameraStack.Sort((a, b) =>
            {
                CameraStack stackA = a.GetComponent<CameraStack>();
                CameraStack stackB = b.GetComponent<CameraStack>();

                return stackA.stackOrder.CompareTo(stackB.stackOrder);
            });
        }
    }

}

using Unity.Cinemachine;
using UnityEngine;


[RequireComponent(typeof(CinemachineImpulseSource))]
public class CameraShake : Singleton<CameraShake>
{
    [SerializeField] CinemachineImpulseSource source;


    protected override void Awake()
    {
        base.Awake();
        GetComponent<CameraShake>();
    }

    public void Shake(float force)
    {
        source.GenerateImpulseWithForce(force);
    }
}

using UnityEngine;

public class MouseLook : MonoBehaviour
{

    public GameObject myPlayer;
    public bool Locked = false;
    [SerializeField] float offset;
    private void FixedUpdate()
    {
        if (Locked) return;
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;

        difference.Normalize();

        float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, 0f, rotationZ + offset);

        //flipping
        //if (rotationZ < -90 + offset || rotationZ > 90 + offset)
        //{



        //    if (myPlayer.transform.eulerAngles.y == 0)
        //    {


        //        transform.localRotation = Quaternion.Euler(180, 0, -rotationZ);


        //    }
        //    else if (myPlayer.transform.eulerAngles.y == 180)
        //    {


        //        transform.localRotation = Quaternion.Euler(180, 180, -rotationZ);


        //    }

        //}

    }

    public void SetLock(bool locked) { Locked = locked; }

}
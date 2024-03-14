using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CameraSample
{


    public class MySample : MonoBehaviour
    {

        [SerializeField] GameObject role;
        [SerializeField] GameObject cam;

        float distance = 10;
        bool isVertical;
        Vector3 offset;
        void Start()
        {
            offset = cam.transform.position - role.transform.position;
        }

        void Update()
        {

            float dt = Time.deltaTime;

            // 1. 输入控制
            Vector2 moveAxis = GetMoveAxis(cam.transform);
            Vector2 mouseAxis = GetMouseAxis();

            // 2. 角色移动
            role.transform.position += new Vector3(moveAxis.x, 0, moveAxis.y) * dt * 5;
            if (moveAxis != Vector2.zero)
            {
                role.transform.forward = new Vector3(moveAxis.x, 0, moveAxis.y).normalized;
            }

            // 3. 相机旋转(跟随与看向)
            if (isVertical)
            {
                cam.transform.position = offset + role.transform.position;
            }
            else
            {
                mouseAxis *= 10;
                Quaternion xRot = Quaternion.AngleAxis(mouseAxis.x, Vector3.up);
                Quaternion yRot = Quaternion.AngleAxis(-mouseAxis.y, cam.transform.right);
                Quaternion sumRot = xRot * yRot;
                Vector3 dir = (cam.transform.position - role.transform.position).normalized;
                dir = sumRot * dir;
                dir *= distance;
                offset = dir;
                cam.transform.position = dir + role.transform.position;
                var rotation = cam.transform.localRotation;
                Mathf.Clamp(rotation.x, 0, 90);
                cam.transform.rotation = rotation;
            }

            cam.transform.forward = (role.transform.position - cam.transform.position).normalized;
            Debug.Log(cam.transform.position);
        }

        void LateUpdate()
        {

            // 3. 相机跟随(绕角色旋转)
            // cam.transform.position = role.transform.position + new Vector3(0, 3, -distance);

        }

        Vector2 GetMoveAxis(Transform cameraTF)
        {

            Vector2 axis = Vector2.zero;
            axis.x = Input.GetAxisRaw("Horizontal");
            axis.y = Input.GetAxisRaw("Vertical");
            if (axis.y != 0)
            {
                isVertical = true;
            }
            else
            {
                isVertical = false;
            }
            Vector3 forward = cameraTF.forward;
            Vector3 right = cameraTF.right;
            forward.y = 0;
            right.y = 0;
            forward.Normalize(); // x² + y² + z² = 1
            right.Normalize();

            Vector3 moveDir = forward * axis.y + right * axis.x;
            moveDir.Normalize();

            axis.x = moveDir.x;
            axis.y = moveDir.z;

            return axis.normalized;

        }

        Vector2 GetMouseAxis()
        {
            Vector2 axis = Vector2.zero;
            if (Input.GetMouseButton(0))
            {
                axis.x = Input.GetAxis("Mouse X");
                axis.y = Input.GetAxis("Mouse Y");
            }
            return axis;
        }

    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movimiento : MonoBehaviour
{
    public void Move(Vector3 moveDirection,CharacterController controller)
    {
        controller.Move(moveDirection * Time.deltaTime);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipControl : MonoBehaviour
{
    [SerializeField]
    private float thrust;
    [SerializeField]
    private float angularSpeed;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Rotate();
        Acceleration();
    }

    // Управление поворотом корабля
    private void Rotate()
    {
        transform.Rotate(new Vector3(0, Input.GetAxis("Horizontal") * angularSpeed, 0));

        // Вектор скорости всегда расположен по направлению модели
        rb.velocity = transform.forward * rb.velocity.magnitude;
    }

    // Управление ускорением корабля
    private void Acceleration()
    {
        rb.AddForce(Input.GetAxis("Vertical") * transform.forward * thrust);
    }
}

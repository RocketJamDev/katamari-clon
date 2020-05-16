using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    public float rotationSpeed;
    public GameObject katamari;
    public GameObject character;
    public GameObject mainCamera;
    public Animator characterAnimator;

    private Rigidbody rb;
    private GameObject characterModel;

    private void Start()
    {
        rb = katamari.GetComponent<Rigidbody>();
        characterModel = character.transform.GetChild(0).gameObject;
    }

    void FixedUpdate()
    {
        characterAnimator.SetBool("isRunning", false);
        RespondToMovementInput();
        RespondToRotationInput();
    }

    private void RespondToMovementInput()
    {
        if (Input.GetKey(KeyCode.W))
        {
            rb.AddForce(mainCamera.transform.forward, ForceMode.Impulse);
            characterAnimator.SetBool("isRunning", true);
        } 
        else if (Input.GetKey(KeyCode.S))
        {
            rb.AddForce(-mainCamera.transform.forward, ForceMode.Impulse);
            characterAnimator.SetBool("isRunning", true);
        }

        if (Input.GetKey(KeyCode.D))
        {
            rb.AddForce(mainCamera.transform.right, ForceMode.Impulse);
            characterAnimator.SetBool("isRunning", true);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            rb.AddForce(-mainCamera.transform.right, ForceMode.Impulse);
            characterAnimator.SetBool("isRunning", true);
        }
    }

    private void RespondToRotationInput()
    {

        if (Input.GetKey(KeyCode.J))
        {
            character.transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
            characterModel.transform.localRotation = Quaternion.Euler(new Vector3(0, -90, 0));
            characterAnimator.SetBool("isRunning", true);
        }
        else if (Input.GetKey(KeyCode.L))
        {
            character.transform.Rotate(-Vector3.up * rotationSpeed * Time.deltaTime);
            characterModel.transform.localRotation = Quaternion.Euler(new Vector3(0, 90, 0));

            characterAnimator.SetBool("isRunning", true);
        }
        else
        {
            characterModel.transform.localRotation = Quaternion.identity;
        }
    }
}

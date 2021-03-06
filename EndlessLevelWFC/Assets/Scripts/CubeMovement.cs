using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMovement : MonoBehaviour
{
    public float moveSpeed;

    void Start()
    {
        moveSpeed = 5f;
    }

    void Update()
    {
        transform.Translate(moveSpeed * Input.GetAxis("Horizontal") * Time.deltaTime, 0f, moveSpeed * Input.GetAxis("Vertical") * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "LeftChild")
        {
            collision.gameObject.transform.parent.gameObject.transform.parent.gameObject.GetComponent<MapPlacerWfc>().GenerateNeighbour("Left");
            Destroy(gameObject);
        }

        if (collision.gameObject.tag == "RightChild")
        {
            collision.gameObject.transform.parent.gameObject.transform.parent.gameObject.GetComponent<MapPlacerWfc>().GenerateNeighbour("Right");
            Destroy(gameObject);
        }

        if (collision.gameObject.tag == "TopChild")
        {
            collision.gameObject.transform.parent.gameObject.transform.parent.gameObject.GetComponent<MapPlacerWfc>().GenerateNeighbour("Top");
            Destroy(gameObject);

        }

        if (collision.gameObject.tag == "BottomChild")
        {
            collision.gameObject.transform.parent.gameObject.transform.parent.gameObject.GetComponent<MapPlacerWfc>().GenerateNeighbour("Bottom");
            Destroy(gameObject);
        }
    }
}

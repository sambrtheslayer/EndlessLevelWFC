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
        MapPlacerWfc mapGO = collision.gameObject.transform.parent.gameObject.transform.parent.gameObject.GetComponent<MapPlacerWfc>();

        if (collision.gameObject.tag == "LeftChild" && mapGO.haveLeftNeighbour == false)
        {
            mapGO.haveLeftNeighbour = true;
            mapGO.GetTilesForNeighbour("Left");
            //collision.gameObject.transform.parent.gameObject.transform.parent.gameObject.GetComponent<MapPlacerWfc>().GenerateNeighbour("Left");
            //Destroy(gameObject);
        }

        else if (collision.gameObject.tag == "RightChild" && mapGO.haveRightNeighbour == false)
        {
            mapGO.haveRightNeighbour = true;
            mapGO.GetTilesForNeighbour("Right");
            //Destroy(gameObject);
        }

        else if (collision.gameObject.tag == "TopChild" && mapGO.haveTopNeighbour == false)
        {
            mapGO.haveTopNeighbour = true;
            mapGO.GetTilesForNeighbour("Top");
            //Destroy(gameObject);

        }

        else if (collision.gameObject.tag == "BottomChild" && mapGO.haveBottomNeighbour == false)
        {
            mapGO.haveBottomNeighbour = true;
            mapGO.GetTilesForNeighbour("Bottom");
            //Destroy(gameObject);
        }
    }
}

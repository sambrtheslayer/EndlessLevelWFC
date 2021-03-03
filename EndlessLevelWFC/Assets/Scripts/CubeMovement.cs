using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMovement : MonoBehaviour
{
    private VoxelTile[,] Map;
    public float moveSpeed;
    // 0.4 - половина размера текстуры Вокселя
    // 0.8 - шаг сетки
    private float voxelSize = 0.8f;
    private float offsetAnchor = 0.4f;

    void Start()
    {
        moveSpeed = 5f;
        Map = TilePlacerWfc.spawnedTiles;
    }

    void Update()
    {
        transform.Translate(moveSpeed * Input.GetAxis("Horizontal") * Time.deltaTime, 0f, moveSpeed * Input.GetAxis("Vertical") * Time.deltaTime);
        int cubeX = (int)((transform.position.x - offsetAnchor) / voxelSize);
        int cubeZ = (int)((transform.position.z - offsetAnchor) / voxelSize);
        Debug.Log(cubeX + " " + cubeZ);
        
        //CheckSideMap(cubeX, cubeZ);
    }

    private void CheckSideMap(int cubeX, int cubeZ)
    {
        if (cubeX == 0)
        {
            Debug.Log("Left side");
            
        }

        else if (cubeX == 7)
        {
            Debug.Log("Right side");
        }

        else if (cubeZ == 0)
        {
            Debug.Log("Back side");
        }

        else if (cubeZ == 7)
        {
            Debug.Log("Forward side");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "MapEnd")
        {
            print("end of map!");
        }
    }
}

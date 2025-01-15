using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HelperUtilities 
{
    /// <summary>
    /// Empty string debug check
    /// </summary>
    public static bool ValidateCheckEmptyString(Object thisObject,string fieldName,string stringToCheck)
    {
        if (stringToCheck == "")
        {
            Debug.Log(fieldName + "is empty and must contain a value in object " + thisObject.name.ToString());
            return true;
        }
        return false;
    }

    public static bool ValidateCheckNullValue(Object thisObject,string fileName,Object objectToCheck)
    {
        if (objectToCheck == null)
        {
            Debug.Log(fileName + " is null and must contain a value in object " + thisObject.name.ToString());
            return true;
        }
        return false;
    }
    public static bool ValidateCheckEnumerableValues(Object thisObject,string fieldName, IEnumerable enumerableObjectToCheck)
    {
        bool error = false;
        int count = 0;

        if (enumerableObjectToCheck == null)
        {
            Debug.Log(fieldName + " is null in object " + thisObject.name.ToString());
            return true;
        }
        foreach(var item in enumerableObjectToCheck)
        {
            if (item == null)
            {
                Debug.Log(fieldName + " has null values in object " + thisObject.name.ToString());
                error = true;
            }
            else
            {
                count++;
            }
        }

        if (count == 0)
        {
            Debug.Log(fieldName + " has no values in object " + thisObject.name.ToString());
            error = true;
        }
        return error;
    }


    public static bool ValidateCheckPositiveValue(Object thisObject,string fileName,int valueToCheck,bool isZeroAllowed)
    {
        bool error = false;
        if (isZeroAllowed)
        {
            if (valueToCheck < 0)
            {
                Debug.Log(fileName + " must contain a positive value or zero in object " + thisObject.name.ToString());
                error = true;
            }

        }
        else
        {
            if (valueToCheck <= 0)
            {
                Debug.Log(fileName + " must contain a positive value in object " + thisObject.name.ToString());
                error = true;
            }
        }
        return error;
    }

    public static Vector3 GetSwapnPositionNearestToPlayer(Vector3 playerPosition)
    {
        Room currentRoom = GameManager.Instance.GetCurrentRoom();

        Grid grid = currentRoom.instantiatedRoom.grid;

        
        Vector3 nearestSwapnPosition = grid.CellToWorld((Vector3Int)currentRoom.spawnPositionArrary[0]);

        foreach(Vector2Int posGrid in currentRoom.spawnPositionArrary)
        {
            Vector3 spwanPositionToWorld = grid.CellToWorld((Vector3Int)posGrid);
            if (Vector3.Distance(playerPosition, nearestSwapnPosition) > Vector2.Distance(playerPosition, spwanPositionToWorld))
            {
                nearestSwapnPosition = spwanPositionToWorld;
            }
        }
        return nearestSwapnPosition;
    }
}

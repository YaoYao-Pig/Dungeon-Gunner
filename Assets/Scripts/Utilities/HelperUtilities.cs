using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HelperUtilities 
{
    public static Camera mainCamera;
    public static Vector3 GetMouseWorldPosition()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.x = Mathf.Clamp(mouseScreenPosition.x, 0, Screen.width);
        mouseScreenPosition.y = Mathf.Clamp(mouseScreenPosition.y, 0, Screen.height);
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);
        mousePosition.z = 0f;

        return mousePosition;

    }

    public static float GetAngleFromVector(Vector3 vector)
    {
        float radius = Mathf.Atan2(vector.y, vector.x);

        float degrees = radius * Mathf.Rad2Deg;
        return degrees;
    }

    public static AimDirection GetAimDirection(float angleDegrees)
    {
        AimDirection aimDirection;

        if (angleDegrees >= 22f && angleDegrees <= 67f)
        {
            aimDirection = AimDirection.UpRight;
        }
        else if (angleDegrees > 67f && angleDegrees <= 112f)
        {
            aimDirection = AimDirection.Up;
        }
        else if (angleDegrees > 112f && angleDegrees <= 158f)
        {
            aimDirection = AimDirection.UpLeft;
        }
        else if ((angleDegrees > 158f && angleDegrees <= 180f)||(angleDegrees>-180f&&angleDegrees<=-135f))
        {
            aimDirection = AimDirection.Left;
        }
        else if (angleDegrees > -135f && angleDegrees <= -45f)
        {
            aimDirection = AimDirection.Down;
        }
        else if ((angleDegrees > -45f && angleDegrees <= 0f)||(angleDegrees>0f&&angleDegrees<22f))
        {
            aimDirection = AimDirection.Right;
        }
        else
        {
            aimDirection = AimDirection.Right;
        }
        return aimDirection;
    }
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

    public static bool ValidateCheckPositiveValue(Object thisObject, string fileName, float valueToCheck, bool isZeroAllowed)
    {
        bool error = false;
        if (isZeroAllowed)
        {
            if (valueToCheck < 0f)
            {
                Debug.Log(fileName + " must contain a positive value or zero in object " + thisObject.name.ToString());
                error = true;
            }

        }
        else
        {
            if (valueToCheck <= 0f)
            {
                Debug.Log(fileName + " must contain a positive value in object " + thisObject.name.ToString());
                error = true;
            }
        }
        return error;
    }



    public static bool ValidateCheckPositiveValueRange(Object thisObject, string fieldNameMiniimum, float valueToCheckMinimum, string fieldNameMaximum,
        float valueToCheckMaximum,bool isZeroAllowed)
    {
        bool error = false;

            if (valueToCheckMinimum > valueToCheckMaximum)
            {
                Debug.Log(fieldNameMiniimum + " must be less than or equal to " + fieldNameMaximum+" in object " + thisObject.name.ToString());
                error = true;
            }

        if (ValidateCheckPositiveValue(thisObject, fieldNameMiniimum, valueToCheckMinimum, isZeroAllowed)) error = true;
        if (ValidateCheckPositiveValue(thisObject, fieldNameMaximum, valueToCheckMaximum, isZeroAllowed)) error = true;

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

    

    public static Vector3 GetDirectionVectorFromAngle(float angle)
    {
        return new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle), 0f);
    } 

    public static float LinearToDecibels(int linear)
    {
        float linearScaleRange = 20f;
        return Mathf.Log10((float)linear / linearScaleRange) * 20f;
    }

    public static void CameraWorldPositionBounds(out Vector2Int cameraWorldPositionLowerBounds,out Vector2Int cameraWorldPositionUpperBounds,
        Camera camera)
    {
        Vector3 worldPositionViewportBottomLeft = camera.ViewportToWorldPoint(new Vector3(0f, 0f, 0f));
        Vector3 worldPositionViewportTopLeft = camera.ViewportToWorldPoint(new Vector3(1f, 1f, 0f));
        cameraWorldPositionLowerBounds = new Vector2Int((int)worldPositionViewportBottomLeft.x, (int)worldPositionViewportBottomLeft.y);
        cameraWorldPositionUpperBounds = new Vector2Int((int)worldPositionViewportTopLeft.x, (int)worldPositionViewportTopLeft.y);
    }


}

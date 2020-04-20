using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class GameManager : MonoBehaviour
{
    public ARRaycastManager RaycastManager;
    public ARPlaneManager PlaneManager;
    public GameObject Game;
    public Door Door;
    // Update is called once per frame
    void Update()
    {

        if (Door.DoorPlaced)
        {
            return;
        }
        
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            var hits = TryRaycast(TrackableType.PlaneWithinPolygon, Input.GetTouch(0).position);
            if (hits.Count > 0)
            {
                var plane = PlaneManager.GetPlane(hits[0].trackableId);
                if (plane.alignment != PlaneAlignment.Vertical)
                    return;
                PlaceDoor(hits[0].pose.position, hits[0].pose.rotation);
            }
        }
    }

    private List<ARRaycastHit> TryRaycast(TrackableType trackableTypes, Vector2 screenPoint)
      {
        var results = new List<ARRaycastHit>();
        RaycastManager.Raycast(screenPoint, results, trackableTypes);
        return results;
      }

    private void PlaceDoor(Vector3 position, Quaternion rotation)
    {
        Game.transform.position = position;
        Game.transform.rotation = rotation;
        Door.Init();
        Door.DoorPlaced = true;
    }
}

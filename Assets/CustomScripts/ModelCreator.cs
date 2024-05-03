using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.UI.GridLayoutGroup;
using static UnityEngine.UI.Image;

public class TouchTriggeredHouseCreation : MonoBehaviour
{
    public Button scanButton;
    public GameObject houseTarget;
    public GameObject[] allTargets;
    public List<GameObject> cornerTargets = new List<GameObject>();
    public List<GameObject> openingTargets = new List<GameObject>();
    public List<GameObject> innerTargets = new List<GameObject>();
    public List<GameObject> furnitureTargets = new List<GameObject>();
    public GameObject wallPrefab;
    public GameObject plusPrefab;
    public GameObject tPrefab;
    public Material floorMaterial;
    public float floorOpacity = 0.5f;
    public float distanceThreshold = 0.5f;

    void Update()
    {
        scanButton.onClick.AddListener(CreateHouse);
    }

    void CreateHouse()
    {
        if (AreTargetsTracked())
        {
            foreach (Transform child in houseTarget.transform)
            {
                Destroy(child.gameObject);
            }
            CreateBase();
            CreateWalls();
            CreateOpeningWalls();
            ConnectInnerWalls();
            CreateFurniture();
            CreateFloor();
        }
        ResetTracking();
    }

    void ResetTracking()
    {
        TrackerManager.Instance.GetTracker<ObjectTracker>().Stop();
        ObjectTracker tracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
        tracker.GetTargetFinder<ImageTargetFinder>().ClearTrackables(true);
        tracker.Start();
    }

    void UpdateTargetList()
    {
        cornerTargets.Clear();
        innerTargets.Clear();
        openingTargets.Clear();
        furnitureTargets.Clear();
        foreach (GameObject target in allTargets)
        {
            if (target.GetComponent<TrackableBehaviour>().CurrentStatus == TrackableBehaviour.Status.TRACKED || target.GetComponent<TrackableBehaviour>().CurrentStatus == TrackableBehaviour.Status.EXTENDED_TRACKED || target.GetComponent<TrackableBehaviour>().CurrentStatus == TrackableBehaviour.Status.NO_POSE || target.GetComponent<TrackableBehaviour>().CurrentStatus == TrackableBehaviour.Status.DETECTED)
            {
                if (target.CompareTag("CornerTarget"))
                    cornerTargets.Add(target);
                else if (target.CompareTag("InnerTarget"))
                    innerTargets.Add(target);
                else if (target.CompareTag("OpeningTarget"))
                    openingTargets.Add(target);
                else if (target.CompareTag("FurnitureTarget"))
                    furnitureTargets.Add(target);
            }
        }
    }

    bool AreTargetsTracked()
    {
        int trackedCornerTargets = cornerTargets.Count(target => (target.GetComponent<TrackableBehaviour>().CurrentStatus == TrackableBehaviour.Status.TRACKED || target.GetComponent<TrackableBehaviour>().CurrentStatus == TrackableBehaviour.Status.EXTENDED_TRACKED || target.GetComponent<TrackableBehaviour>().CurrentStatus == TrackableBehaviour.Status.NO_POSE || target.GetComponent<TrackableBehaviour>().CurrentStatus == TrackableBehaviour.Status.DETECTED));
        if (trackedCornerTargets < 4 || trackedCornerTargets % 2 != 0)
        {
            Debug.Log("Either less than 4 corner targets being tracked or not a multiple of 2.");
            return false;
        }
        return true;
    }

    void CreateBase()
    {
        Vector3 origin = Vector3.zero;
        foreach (GameObject target in cornerTargets)
        {
            origin += target.transform.position;
        }
        origin /= cornerTargets.Count;
        Vector3[] relativePositions = new Vector3[cornerTargets.Count];
        for (int i = 0; i < cornerTargets.Count; i++)
        {
            relativePositions[i] = cornerTargets[i].transform.position - origin;
        }
        for (int i = 0; i < cornerTargets.Count; i++)
        {
            GameObject childCopy = Instantiate(cornerTargets[i], houseTarget.transform);
            childCopy.transform.localPosition = relativePositions[i];
            childCopy.transform.localPosition = new Vector3(childCopy.transform.localPosition.x, 0f, childCopy.transform.localPosition.z);
            Vector3 currentRotation = childCopy.transform.rotation.eulerAngles;
            childCopy.transform.rotation = Quaternion.Euler(0f, currentRotation.y, 0f);
        }
    }

    void CreateWalls()
    {
        for (int i = 0; i < cornerTargets.Count; i++)
        {
            GameObject currentTarget = cornerTargets[i];

            for (int j = i + 1; j < cornerTargets.Count; j++)
            {
                GameObject nextTarget = cornerTargets[j];
                float distanceX = Mathf.Abs(nextTarget.transform.position.x - currentTarget.transform.position.x);
                float distanceZ = Mathf.Abs(nextTarget.transform.position.z - currentTarget.transform.position.z);
                if (distanceX <= distanceThreshold || distanceZ <= distanceThreshold)
                {
                    Vector3 centerPosition = (currentTarget.transform.position + nextTarget.transform.position) / 2f;
                    Vector3 origin = Vector3.zero;
                    foreach (GameObject target in cornerTargets)
                    {
                        origin += target.transform.position;
                    }
                    origin /= cornerTargets.Count;
                    Vector3 relativeWallPosition = centerPosition - origin;
                    if (!AreTargetsBetweenCorners(currentTarget, nextTarget) && !WallExistsAtPosition(relativeWallPosition))
                    {
                        string wallName = "Wall (" + currentTarget.name + nextTarget.name + ")";
                        GameObject wall = Instantiate(wallPrefab, houseTarget.transform);
                        wall.name = wallName;
                        wall.transform.localPosition = relativeWallPosition;
                        Vector3 scale = new Vector3(0.2487f, 0.2487f, 0.2487f);
                        if (distanceX <= distanceThreshold)
                        {
                            float scaleFactor = (distanceZ - 1f) / 4.5f;
                            scale.x = scaleFactor;
                            wall.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
                        }
                        else if (distanceZ <= distanceThreshold)
                        {
                            float scaleFactor = (distanceX - 1f) / 4.5f;
                            scale.x = scaleFactor;
                            wall.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                        }
                        wall.transform.localScale = scale;
                        wall.transform.parent = houseTarget.transform;
                    }
                }
            }
        }
    }

    bool WallExistsAtPosition(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, 0.1f);
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.CompareTag("Wall"))
            {
                return true;
            }
        }
        return false;
    }

    void CreateOpeningWalls()
    {
        for (int i = 0; i < cornerTargets.Count; i++)
        {
            GameObject corner1 = cornerTargets[i];
            for (int j = i + 1; j < cornerTargets.Count; j++)
            {
                GameObject corner2 = cornerTargets[j];
                float distanceX = Mathf.Abs(corner2.transform.position.x - corner1.transform.position.x);
                float distanceZ = Mathf.Abs(corner2.transform.position.z - corner1.transform.position.z);
                if (corner1 != corner2 && (distanceX <= distanceThreshold || distanceZ <= distanceThreshold))
                {
                    List<GameObject> targetsBetweenCorners = GetTargetsBetweenCorners(corner1, corner2);
                    if (targetsBetweenCorners.Count > 0)
                    {
                        foreach (GameObject target in targetsBetweenCorners)
                        {
                            Vector3 targetPosition = target.transform.position;
                            Vector3 origin = Vector3.zero;
                            foreach (GameObject target2 in cornerTargets)
                            {
                                origin += target2.transform.position;
                            }
                            origin /= cornerTargets.Count;
                            Vector3 relativePosition = targetPosition - origin;

                            if (target.CompareTag("InnerTarget"))
                            {
                                CreateInnerTargets(relativePosition);
                            }
                            else
                            {
                                bool withinThresholdX = Mathf.Abs(corner2.transform.position.x - corner1.transform.position.x) <= distanceThreshold;
                                bool withinThresholdZ = Mathf.Abs(corner2.transform.position.z - corner1.transform.position.z) <= distanceThreshold;
                                string openingName = "Opening (" + corner1.name + target.name + corner2.name + ")";
                                GameObject opening = Instantiate(target, houseTarget.transform);
                                opening.name = openingName;
                                opening.transform.localPosition = relativePosition;
                                opening.transform.parent = houseTarget.transform;
                                if (withinThresholdX)
                                {
                                    opening.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
                                }
                                if (withinThresholdZ)
                                {
                                    opening.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                                }
                            }
                        }
                        CreateBetweenWalls(corner1, targetsBetweenCorners[0]);
                        for (int k = 0; k < targetsBetweenCorners.Count - 1; k++)
                        {
                            CreateBetweenWalls(targetsBetweenCorners[k], targetsBetweenCorners[k + 1]);
                        }
                        CreateBetweenWalls(targetsBetweenCorners[targetsBetweenCorners.Count - 1], corner2);
                    }
                }
            }
        }
    }

    List<GameObject> GetTargetsBetweenCorners(GameObject corner1, GameObject corner2)
    {
        List<GameObject> targetsBetweenCorners = new List<GameObject>();
        foreach (GameObject target in openingTargets.Concat(innerTargets))
        {
            if (IsBetweenCorners(target.transform.position, corner1.transform.position, corner2.transform.position))
            {
                targetsBetweenCorners.Add(target);
            }
        }
        return targetsBetweenCorners;
    }

    bool AreTargetsBetweenCorners(GameObject corner1, GameObject corner2)
    {
        foreach (GameObject target in openingTargets.Concat(innerTargets))
        {
            if (IsBetweenCorners(target.transform.position, corner1.transform.position, corner2.transform.position))
            {
                return true;
            }
        }
        return false;
    }

    bool IsBetweenCorners(Vector3 point, Vector3 corner1, Vector3 corner2)
    {
        float minX = Mathf.Min(corner1.x, corner2.x);
        float maxX = Mathf.Max(corner1.x, corner2.x);
        float minZ = Mathf.Min(corner1.z, corner2.z);
        float maxZ = Mathf.Max(corner1.z, corner2.z);
        return point.x >= minX && point.x <= maxX && point.z >= minZ && point.z <= maxZ;
    }


    void CreateBetweenWalls(GameObject corner1, GameObject corner2)
    {
        Vector3 wallPosition = (corner1.transform.position + corner2.transform.position) / 2f;
        Vector3 origin = Vector3.zero;
        foreach (GameObject target in cornerTargets)
        {
            origin += target.transform.position;
        }
        origin /= cornerTargets.Count;
        Vector3 relativeWallPosition = wallPosition - origin;
        float distanceX = Mathf.Abs(corner2.transform.position.x - corner1.transform.position.x);
        float distanceZ = Mathf.Abs(corner2.transform.position.z - corner1.transform.position.z);
        if (!WallExistsAtPosition(relativeWallPosition))
        {
            string wallName = "Wall (" + corner1.name + corner2.name + ")";
            GameObject wall = Instantiate(wallPrefab, houseTarget.transform);
            wall.name = wallName;
            wall.transform.localPosition = relativeWallPosition;
            Vector3 scale = new Vector3(0.2487f, 0.2487f, 0.2487f);
            if (distanceX <= distanceThreshold)
            {
                float scaleFactor = distanceX;
                scale.x = scaleFactor;
                wall.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
            }
            else if (distanceZ <= distanceThreshold)
            {
                float scaleFactor = distanceZ;
                scale.x = scaleFactor;
                wall.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
            wall.transform.localScale = scale;
            wall.transform.parent = houseTarget.transform;
        }
    }

    void ConnectInnerWalls()
    {
        foreach (GameObject inner1 in innerTargets)
        {
            foreach (GameObject inner2 in innerTargets)
            {
                float distanceX = Mathf.Abs(inner2.transform.position.x - inner1.transform.position.x);
                float distanceZ = Mathf.Abs(inner2.transform.position.z - inner1.transform.position.z);
                Vector3 wallPosition = (inner1.transform.position + inner2.transform.position) / 2f;
                Vector3 origin = Vector3.zero;
                foreach (GameObject target in cornerTargets)
                {
                    origin += target.transform.position;
                }
                origin /= cornerTargets.Count;
                Vector3 relativeWallPosition = wallPosition - origin;
                if (inner1 != inner2 && (distanceX <= distanceThreshold || distanceZ <= distanceThreshold))
                {
                    if (!WallExistsAtPosition(relativeWallPosition))
                    {
                        CreateBetweenWalls(inner1, inner2);
                    }
                }
            }
        }
    }

    void CreateInnerTargets(Vector3 replacementPosition)
    {
        foreach (GameObject inner in innerTargets)
        {
            Vector3 origin = Vector3.zero;
            foreach (GameObject target in cornerTargets)
            {
                origin += target.transform.position;
            }
            origin /= cornerTargets.Count;
            Vector3 relativePosition = replacementPosition - origin;
            Vector3 innerPos = inner.transform.position;
            string innerName = "Inner (" + inner.name + ")";
            bool hasLeft = false;
            bool hasRight = false;
            bool hasAbove = false;
            bool hasBelow = false;
            foreach (GameObject target in openingTargets.Concat(innerTargets).Concat(cornerTargets))
            {
                float distanceX = Mathf.Abs(target.transform.position.x - inner.transform.position.x);
                float distanceZ = Mathf.Abs(target.transform.position.z - inner.transform.position.z);
                if (target != inner)
                {
                    if (target.transform.position.x < innerPos.x && distanceX <= distanceThreshold)
                    {
                        hasLeft = true;
                    }
                    if (target.transform.position.x > innerPos.x && distanceX <= distanceThreshold)
                    {
                        hasRight = true;
                    }
                    if (target.transform.position.z < innerPos.z && distanceZ <= distanceThreshold)
                    {
                        hasBelow = true;
                    }
                    if (target.transform.position.z > innerPos.z && distanceZ <= distanceThreshold)
                    {
                        hasAbove = true;
                    }
                }
            }
            Debug.Log("Inner: " + hasLeft + hasRight + hasBelow + hasAbove);
            if (hasLeft && hasRight && hasAbove && hasBelow)
            {
                GameObject innerModel = Instantiate(plusPrefab, houseTarget.transform);
                innerModel.name = innerName;
                innerModel.transform.localPosition = relativePosition;
                innerModel.transform.parent = houseTarget.transform;
                innerModel.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
            else if (hasAbove && hasRight && hasBelow)
            {
                GameObject innerModel = Instantiate(tPrefab, houseTarget.transform);
                innerModel.name = innerName;
                innerModel.transform.localPosition = relativePosition;
                innerModel.transform.parent = houseTarget.transform;
                innerModel.transform.rotation = Quaternion.Euler(0f, 270f, 0f);
            }
            else if (hasLeft && hasAbove && hasRight)
            {
                GameObject innerModel = Instantiate(tPrefab, houseTarget.transform);
                innerModel.name = innerName;
                innerModel.transform.localPosition = relativePosition;
                innerModel.transform.parent = houseTarget.transform;
                innerModel.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            }
            else if (hasLeft && hasBelow && hasRight)
            {
                GameObject innerModel = Instantiate(tPrefab, houseTarget.transform);
                innerModel.name = innerName;
                innerModel.transform.localPosition = relativePosition;
                innerModel.transform.parent = houseTarget.transform;
                innerModel.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
            else if (hasLeft && hasAbove && hasBelow)
            {
                GameObject innerModel = Instantiate(tPrefab, houseTarget.transform);
                innerModel.name = innerName;
                innerModel.transform.localPosition = relativePosition;
                innerModel.transform.parent = houseTarget.transform;
                innerModel.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
            }
            else if (hasAbove && hasBelow)
            {
                GameObject innerModel = Instantiate(wallPrefab, houseTarget.transform);
                innerModel.name = innerName;
                innerModel.transform.localPosition = relativePosition;
                innerModel.transform.parent = houseTarget.transform;
                innerModel.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
            }
            else if (hasLeft && hasRight)
            {
                GameObject innerModel = Instantiate(wallPrefab, houseTarget.transform);
                innerModel.name = innerName;
                innerModel.transform.localPosition = relativePosition;
                innerModel.transform.parent = houseTarget.transform;
                innerModel.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
        }
    }

    void CreateFurniture()
    {
        foreach (GameObject furniture in furnitureTargets)
        {
            if (furniture.GetComponent<TrackableBehaviour>().CurrentStatus == TrackableBehaviour.Status.TRACKED || furniture.GetComponent<TrackableBehaviour>().CurrentStatus == TrackableBehaviour.Status.EXTENDED_TRACKED || furniture.GetComponent<TrackableBehaviour>().CurrentStatus == TrackableBehaviour.Status.NO_POSE || furniture.GetComponent<TrackableBehaviour>().CurrentStatus == TrackableBehaviour.Status.DETECTED)
            {
                Vector3 furniturePosition = furniture.transform.position;
                Vector3 origin = Vector3.zero;
                foreach (GameObject corner in cornerTargets)
                {
                    origin += corner.transform.position;
                }
                origin /= cornerTargets.Count;
                Vector3 relativePosition = furniturePosition - origin;
                if (IsWithinHouseBounds(furniturePosition))
                {
                    string furnitureName = "Furniture (" + furniture.name + ")";
                    GameObject placedFurniture = Instantiate(furniture, houseTarget.transform);
                    placedFurniture.name = furnitureName;
                    placedFurniture.transform.localPosition = relativePosition;
                    placedFurniture.transform.parent = houseTarget.transform;
                    placedFurniture.transform.rotation = houseTarget.transform.rotation;
                }
                else
                {
                    Debug.LogWarning("Furniture " + furniture.name + " is not within the bounds of the house.");
                }
            }
            else
            {
                Debug.LogWarning("Furniture " + furniture.name + " is not found or not being tracked.");
            }
        }
    }

    bool IsWithinHouseBounds(Vector3 position)
    {
        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minZ = float.MaxValue;
        float maxZ = float.MinValue;
        foreach (GameObject corner in cornerTargets)
        {
            Vector3 cornerPosition = corner.transform.position;
            if (cornerPosition.x < minX)
                minX = cornerPosition.x;
            if (cornerPosition.x > maxX)
                maxX = cornerPosition.x;
            if (cornerPosition.z < minZ)
                minZ = cornerPosition.z;
            if (cornerPosition.z > maxZ)
                maxZ = cornerPosition.z;
        }
        if (position.x >= minX && position.x <= maxX && position.z >= minZ && position.z <= maxZ)
            return true;
        else
            return false;
    }

    void CreateFloor()
    {
        List<Vector3> boundaryPoints = new List<Vector3>();
        foreach (GameObject corner in cornerTargets)
        {
            boundaryPoints.Add(corner.transform.position);
        }
        Vector3 origin = Vector3.zero;
        foreach (Vector3 point in boundaryPoints)
        {
            origin += point;
        }
        origin /= boundaryPoints.Count;
        GameObject floorObject = new GameObject("Floor");
        floorObject.transform.SetParent(houseTarget.transform);
        floorObject.transform.localPosition = Vector3.zero;
        List<Vector3> localVertices = new List<Vector3>();
        foreach (Vector3 point in boundaryPoints)
        {
            localVertices.Add(point - origin);
        }
        Mesh mesh = new Mesh();
        Vector3[] vertices = localVertices.ToArray();
        int[] triangles = Triangulate(vertices);
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
        }
        mesh.uv = uvs;
        MeshRenderer meshRenderer = floorObject.AddComponent<MeshRenderer>();
        meshRenderer.material = new Material(floorMaterial);
        meshRenderer.material.color = new Color(meshRenderer.material.color.r, meshRenderer.material.color.g, meshRenderer.material.color.b, floorOpacity);
        floorObject.AddComponent<MeshFilter>().mesh = mesh;
        floorObject.AddComponent<MeshCollider>().sharedMesh = mesh;
    }

    int[] Triangulate(Vector3[] vertices)
    {
        List<int> triangles = new List<int>();
        if (vertices.Length < 3)
        {
            Debug.LogWarning("Cannot triangulate less than 3 vertices.");
            return triangles.ToArray();
        }
        for (int i = 1; i < vertices.Length - 1; i++)
        {
            triangles.Add(0);
            triangles.Add(i);
            triangles.Add(i + 1);
        }
        return triangles.ToArray();
    }
}
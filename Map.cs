using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public float maxX;
    public float maxY;
    public float maxHeight;
    public Flight[] flights;
    public GameObject flightArc;
    public float lineAccuracy;
    List<KeyValuePair<Flight, GameObject>> flightsWithObjects;
 
    // Start is called before the first frame update
    void Start()
    {
        //DrawFlightTrajectory(flightArc, maxHeight, .5f, lineAccuracy);
        flightsWithObjects = new List<KeyValuePair<Flight, GameObject>>();
        foreach(Flight flight in flights) {
            GameObject newFlightArc = Instantiate(flightArc, gameObject.transform);
            flightsWithObjects.Add(new KeyValuePair<Flight, GameObject>(flight, newFlightArc));
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (KeyValuePair<Flight, GameObject> flightArch in flightsWithObjects) {
           //TODO if the flight is greater than start time and less than end time
           if(true) {
               float[] departurePositionWorld = TranslateToWorldUnits(flightArch.Key.takeOffPosition[0], flightArch.Key.takeOffPosition[1]);
               float[] arrivalPositionWorld = TranslateToWorldUnits(flightArch.Key.landingPosition[0], flightArch.Key.landingPosition[1]);
               DrawFlightTrajectory(flightArch.Value, .5f, new Vector3(departurePositionWorld[0], 0, departurePositionWorld[1]), new Vector3(arrivalPositionWorld[0], 0, arrivalPositionWorld[1]));
           }
        }
    }
    
    float[] TranslateToWorldUnits(float latitude, float longitude) {
        float newX = MapValue(180, -180, maxX, -maxX, latitude);
        float newY = MapValue(90, -90, maxY, -maxY, longitude);
        float[] result = {newX, newY};
        return result;
    }
    
    float MapValue(float oldMax, float oldMin, float newMax, float newMin, float value) {
        float oldRange = oldMax - oldMin;
        float newRange = newMax - newMin;
        float newValue = (((value - oldMin) * newRange) / oldRange) + newMin;
        return newValue;
    }
    
    void DrawFlightTrajectory(GameObject flightArc, float percentTime, Vector3 startingPos, Vector3 endingPos) {
        LineRenderer lineRenderer = flightArc.GetComponent<LineRenderer>();
        lineRenderer.positionCount = (int) lineAccuracy + 1;
        lineRenderer.startWidth = 0f;
        lineRenderer.endWidth = .1f;
        for (float i = 0; i <= lineAccuracy; i++) {
            float xToAdd = (endingPos.x - startingPos.x) * (i / lineAccuracy);
            float x = startingPos.x + xToAdd;
            float zToAdd = (endingPos.z - startingPos.z) * (i / lineAccuracy);
            float z = startingPos.z + zToAdd;
            float midPointX = ((endingPos.x - startingPos.x) / 2f) + startingPos.x;
            float midPointZ = ((endingPos.z - startingPos.z) / 2f) + startingPos.z;
            Vector3 midPoint = new Vector3(midPointX, 0f, midPointZ);
            float totalDistance = Vector3.Distance(startingPos, endingPos);
            float currentDistance = Vector3.Distance(startingPos, new Vector3(x, 0f, z));
            float midPointDistance = Vector3.Distance(midPoint, startingPos);
            float y = -1f * ( (4f * maxHeight) /  Mathf.Pow(totalDistance, 2f)) * Mathf.Pow(currentDistance - midPointDistance, 2f) + maxHeight;
            Vector3 pointOnLine = new Vector3(x, y, z);
            lineRenderer.SetPosition((int) i, pointOnLine);
        }
        float alpha = 1.0f;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.red, percentTime), new GradientColorKey(Color.black, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
        );
        gradient.mode = GradientMode.Fixed;
        lineRenderer.colorGradient = gradient;
        //make the sphere follow the line
        Transform plane = flightArc.transform.GetChild(0);
        plane.transform.localPosition = lineRenderer.GetPosition((int) Mathf.Ceil(percentTime * lineAccuracy));
    }
}

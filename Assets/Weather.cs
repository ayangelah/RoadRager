using System.Collections;
//using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Weather : MonoBehaviour
{
    //game objects and components
    [SerializeField] ParticleSystem rain;
    [SerializeField] ParticleSystem snow;
    [SerializeField] GameObject background;
    [SerializeField] Transform otherCar;
    [SerializeField] Scrolling scrollingscript;
    [SerializeField] float speed;
    [SerializeField] float acceleration;
    [SerializeField] bool truckActive;
    const float topspeed = 1;
    const float truckspeed = 0.2f;
    float newspeed;

    //UI
    [SerializeField] Text speedometer;
    [SerializeField] Text weatherConditions;
    [SerializeField] Text timeOfDay;
    [SerializeField] Text roadConditions;
    [SerializeField] Slider ragemeter;

    //colors
    Color DAY_COLOR;
    Color NIGHT_COLOR;
    Color TWILIGHT_COLOR;

    protected int precipitationType;
    protected bool precipitationChanging;
    SpriteRenderer bgSpriteRenderer;

    protected bool timeChanging;
    protected int timeIterator;

    protected bool rageCalculating;
    /*
    const int V1;
    const int V2;
    const int V3;*/
    const int V5 = 2;
    /*
    const int V9;
    const int V10;*/

    const int V13_PRECIP_CLEAR = 0;
    const int V13_PRECIP_RAIN = 1;
    const int V13_PRECIP_SNOW = 2;

    const int V18_ROAD_DRY = 0;
    const int V18_ROAD_WET = 1;
    const int V18_ROAD_VISIBLETRACKS = 2;
    const int V18_ROAD_SNOW = 3;

    const int V19_SKY_DAYLIGHT = 0;
    const int V19_SKY_NIGHT = 1;
    const int V19_SKY_TWILIGHT = 2;

    const int WEATHER_CYCLE_LOOP_TIME = 10;
    const int DAY_CYCLE_LOOP_TIME = 7;
    const int RAGE_CALCULATION_TIME = 10;

    [SerializeField] private int[] currentconditions = new int[20];

    // Start is called before the first frame update
    void Start()
    {
        bgSpriteRenderer = background.GetComponent<SpriteRenderer>();
        for (int i = 0; i < 20; i++)
        {
            currentconditions[i] = 0;
        }

        /*
        currentconditions[1] = V1;
        currentconditions[2] = V2;
        currentconditions[3] = V3;
        currentconditions[5] = V5;
        currentconditions[9] = V9;
        currentconditions[10] = V10;*/

        currentconditions[13] = V13_PRECIP_CLEAR;
        //isPrecipitating = false;
        precipitationType = 0;
        precipitationChanging = false;

        speedometer.text = "Speedometer: " + (speed * 160).ToString() + " kph";
        currentconditions[18] = V18_ROAD_DRY;
        currentconditions[19] = V19_SKY_DAYLIGHT;
        timeIterator = 1;
        DAY_COLOR = new Color(0.6f, 0.85f, 1f, 1f);
        NIGHT_COLOR = new Color(0.2f, 0.5f, 0.7f, 1f);
        TWILIGHT_COLOR = new Color(0.06f, 0.17f, 0.25f, 1f);
        bgSpriteRenderer.color = DAY_COLOR;
        ragemeter.value = 0.5f;
        rageCalculating = false;
    }

    // Update is called once per frame
    void Update()
    {
        //ragemeter.value = 0; //UPDATE

        //speed stuff
        if (Input.GetKey(KeyCode.D) && (speed < topspeed)) {
            newspeed = speed + acceleration;
            speed = newspeed;
            scrollingscript.speed = speed;
            if (truckActive && otherCar.position.x > 7)
            {
                otherCar.position += Vector3.left * speed * truckspeed;
            }
            speedometer.text = "Speedometer: " + ((speed % 1f) * 160).ToString("F2") + " kph";
        }

        if (Input.GetKey(KeyCode.A) && (speed > 0))
        {
            newspeed = speed - acceleration;
            speed = newspeed;
            if (speed < 0)
                speed = 0;
            scrollingscript.speed = speed;
            if (truckActive && otherCar.position.x < 35)
            {
                otherCar.position += Vector3.right * speed * truckspeed;
            }
            speedometer.text = "Speedometer: " + ((speed % 1f) * 160).ToString("F2") + " kph";
        }

        //precipitation
        if (!precipitationChanging) {
            StartCoroutine(ChangePrecipitation());
        }

        if (!timeChanging) {
            StartCoroutine(ChangeTime());
        }

        if (!rageCalculating) {
            StartCoroutine(CalculateRage());
        }

        //precipitation conditions change particle system
        if (currentconditions[13] == V13_PRECIP_CLEAR) {
            weatherConditions.text = "weather conditions: Clear";
            roadConditions.text = "road conditions: Dry";
            currentconditions[18] = V18_ROAD_DRY;
            if (rain.isPlaying)
                rain.Stop();
            if (snow.isPlaying)
                snow.Stop();
        } else if (currentconditions[13] == V13_PRECIP_RAIN) {
            weatherConditions.text = "weather conditions: Raining";
            roadConditions.text = "road conditions: Wet";
            currentconditions[18] = V18_ROAD_WET;
            if (!rain.isPlaying)
                rain.Play();
            if (snow.isPlaying)
                snow.Stop();
        } else {
            weatherConditions.text = "weather conditions: Snowing";
            roadConditions.text = "road conditions: Snow";
            currentconditions[18] = V18_ROAD_SNOW;
            if (rain.isPlaying)
                rain.Stop();
            if (!snow.isPlaying)
                snow.Play();
        }

        //precipitation condtiions change road conditions
        //daylight changes background color
        if (currentconditions[19] == V19_SKY_DAYLIGHT) {
            bgSpriteRenderer.color = DAY_COLOR;
            timeOfDay.text = "time of day: Daytime";
        } else if (currentconditions[19] == V19_SKY_NIGHT) {
            bgSpriteRenderer.color = NIGHT_COLOR;
            timeOfDay.text = "time of day: Nighttime";
        } else if (currentconditions[19] == V19_SKY_TWILIGHT) {
            bgSpriteRenderer.color = TWILIGHT_COLOR;
            timeOfDay.text = "time of day: Twilight";
        }
    }

    IEnumerator ChangePrecipitation()
    {
        precipitationChanging = true;
        yield return new WaitForSeconds(WEATHER_CYCLE_LOOP_TIME);
        currentconditions[13] = Random.Range(0, 3);
        precipitationChanging = false;
    }

    IEnumerator ChangeTime() {
        timeChanging = true;
        yield return new WaitForSeconds(DAY_CYCLE_LOOP_TIME);

        if (currentconditions[19] == V19_SKY_DAYLIGHT) {
            timeIterator = 1;
        } else if (currentconditions[19] == V19_SKY_TWILIGHT) {
            timeIterator = -1;
        }

        currentconditions[19] = currentconditions[19] + timeIterator;
        timeChanging = false;
    }

    IEnumerator CalculateRage() {
        rageCalculating = true;
        // Get the argument values here

        int V2_arg = 900; // Constant 900 cm
        int V3_arg = 1; // Constant driving on lane 1 (left lane)
        int V4_arg = (int) ((speed % 1f) * 160);
        int V5_arg = 2500; // Constant 2500 kg
        int V6_arg = 2; // Constant 2 axle vehicle
        int V8_arg = (int) (0.4 * 160); // Constant 80kph of preceding vehicle
        int V9_arg = 4020; // Constant 10000 kg of preceding vehicle
        int V10_arg = 727; // Constant 1000 cm of preceding vehicle
        int V11_arg = (int) (otherCar.position.x * (5 * V2_arg) / (V4_arg * 0.277778));
        int V12_arg = 0;
        if (currentconditions[13] == V13_PRECIP_CLEAR)
        {
            V12_arg = 10;
        }
        else if (currentconditions[13] == V13_PRECIP_RAIN)
        {
            V12_arg = 4;
        }
        else
        {
            V12_arg = -9;
        }

        string V13_arg = ""; // "clear", "rain", "snow"
        if (currentconditions[13] == V13_PRECIP_CLEAR) {
            V13_arg = "clear";
        } else if (currentconditions[13] == V13_PRECIP_RAIN) {
            V13_arg = "rain";
        } else{
            V13_arg = "snow";
        }

        int V15_arg = 0;
        if (currentconditions[19] == V19_SKY_DAYLIGHT)
        {
            V15_arg = Random.Range(75, 95);
        }
        else if (currentconditions[19] == V19_SKY_NIGHT)
        {
            V15_arg = Random.Range(50, 71);
        }
        else
        {
            V15_arg = Random.Range(60, 75);
        }

        int V17_arg = 0; // "clear", "rain", "snow"
        if (currentconditions[13] == V13_PRECIP_CLEAR)
        {
            V17_arg = Random.Range(0, 9);
        }
        else if (currentconditions[13] == V13_PRECIP_RAIN)
        {
            V17_arg = Random.Range(5,15);
        }
        else
        {
            V17_arg = Random.Range(9, 15);
        }

        string V18_arg = ""; // "Dry", "Wet", "Snow covered"
        if (currentconditions[18] == V18_ROAD_DRY)
        {
            V18_arg = "Dry";
        }
        else if (currentconditions[18] == V18_ROAD_WET)
        {
            V18_arg = "Wet";
        }
        else // Snow covered or Visible Tracks
        {
            V18_arg = "Snow covered";
        }

        string V19_arg = ""; // "daylight", "twilight", "night"
        if (currentconditions[19] == V19_SKY_DAYLIGHT) {
            V19_arg = "daylight";
        } else if (currentconditions[19] == V19_SKY_NIGHT) {
            V19_arg = "night";
        } else {
            V19_arg = "twilight";
        }

        /*yield return new WaitForSeconds(RAGE_CALCULATION_TIME);

        ProcessStartInfo psi = new ProcessStartInfo();
        psi.UseShellExecute = false;
        psi.RedirectStandardOutput = true;
        //p.StartInfo.CreateNoWindow = false;

        psi.FileName = "./Assets/test";
        psi.Arguments = "./Assets/test";
        Process p = Process.Start(psi);
        string strOutput = p.StandardOutput.ReadToEnd();
        p.WaitForExit();

        UnityEngine.Debug.Log(strOutput);
        UnityEngine.Debug.Log("test");
        UnityEngine.Debug.Log(Directory.GetCurrentDirectory());
        timeOfDay.text = strOutput;*/

        UnityEngine.Debug.Log("Finished parsing arguments");

        string url = "https://us-central1-braided-torch-379704.cloudfunctions.net/drivan";
        string json = "{\"V3\":" + V3_arg + ",\"V4\":" + V4_arg + ",\"V8\":" + V8_arg + ",\"V9\":" + V9_arg + ",\"V10\":" + V10_arg + ",\"V11\":" + V11_arg + ",\"V18\":\"" + V18_arg + "\",\"V12\":" + V12_arg + ",\"V13\":\"" + V13_arg + "\",\"V15\":" + V15_arg + ",\"V17\":" + V17_arg + ",\"V19\":\"" + V19_arg + "\",\"V2\":" + V2_arg + ",\"V5\":" + V5_arg + ",\"V6\":" + V6_arg + "}";
        //string json = "{\"V3\":2,\"V4\":69,\"V8\":68,\"V9\":3244,\"V10\":710,\"V11\":1,\"V18\":\"Dry\",\"V12\":9,\"V13\":\"snow\",\"V15\":91,\"V17\":8,\"V19\":\"night\",\"V2\":2001,\"V5\":27812,\"V6\":6}";
        UnityEngine.Debug.Log(json);

        var uwr = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "application/json");

        //Send the request then wait here until it returns
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError)
        {
            UnityEngine.Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            UnityEngine.Debug.Log(uwr.downloadHandler.text);
            string response = uwr.downloadHandler.text;
            response = response.Substring(1, response.Length - 2);
            string[] responseData = response.Split(' ');

            UnityEngine.Debug.Log("Received: " + responseData[0] + " and " + responseData[1]);
            ragemeter.value = float.Parse(responseData[0]);
        }

        rageCalculating = false;
    }
}

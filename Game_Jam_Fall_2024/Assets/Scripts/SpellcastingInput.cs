using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpellcastingInput : MonoBehaviour
{
    [SerializeField] private Camera main_camera;
    [SerializeField] private List<Vector2> points; //all tracked points
    [SerializeField] private List<Vector2> corners; //corner points
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject greenSquare;
    [SerializeField] private GameObject redSquare;
    [SerializeField] private GameObject particlePrefab;
    [SerializeField] private string dirSequence; //0 - 7, up = 0, cw around
    private Vector2 lastPos; //stores last mousePos to ensure only new mousePos are gathered
    private bool flag; //tracks whether rune is still being drawn
    [SerializeField] private float angleThreshold = 140f; //degrees, working value of 140
    [SerializeField] private float minDistance = 20.0f; //min distance between while finding corners, working value of 20
    [SerializeField] private float postMinDistance = 40.0f; //after corners have been found, removes corners closer than this distance, working value of 40 - not necessary?
    private Dictionary<string, string> map = new Dictionary<string, string>(); //dirSequence, rune

    [SerializeField] private float clickTime = 0f;
    [SerializeField] private float clickThreshold = 0.2f;

    [SerializeField] private Attack attackScript;

    public AudioSource completeSpell;

    // Start is called before the first frame update
    void Start()
    {
        points = new List<Vector2>();
        corners = new List<Vector2>();
        flag = true;
        dirSequence = "";
        map.Add("53", "Fire"); //add runes like this until i find a better way
        map.Add("35", "Ice");
        map.Add("31", "Lightning");

        attackScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Attack>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0) && flag) //collects tracked points
        {
            clickTime += Time.deltaTime;
            Vector2 mousePos = Input.mousePosition;
            if (mousePos != lastPos)
            {
                points.Add(mousePos);
                lastPos = mousePos;
                GameObject tempSquare = Instantiate(greenSquare, canvas.transform);
                if (particlePrefab != null)
                {
                    // Get the position where the spell is drawn
                    Vector3 spellPosition = main_camera.ScreenToWorldPoint(mousePos);
                    spellPosition.z = 0; // Ensure the z position is 0 for 2D

                    // Instantiate the particle system at the spell position
                    GameObject particle = Instantiate(particlePrefab, spellPosition, Quaternion.identity, canvas.transform);
                    var main = particle.GetComponent<ParticleSystem>().main;
                    main.startColor = new Color(1.0f, 0.84f, 0.0f); // Gold color

                    // Optionally, add some randomness to the particle positions
                    ParticleSystem particleSystem = particle.GetComponent<ParticleSystem>();
                    ParticleSystem.ShapeModule shape = particleSystem.shape;
                    shape.shapeType = ParticleSystemShapeType.Sphere;
                    shape.radius = 0.5f; // Adjust the radius as needed
                }
                tempSquare.GetComponent<RectTransform>().position = new Vector3(main_camera.ScreenToWorldPoint(mousePos).x, main_camera.ScreenToWorldPoint(mousePos).y, 0);
            }
        }
        if (Input.GetKeyUp(KeyCode.Mouse0) && flag)
        {
            if(clickTime >= clickThreshold && points.Count > 2)
            {
                flag = false;
                GetCorners(points);
            }
            else{
                clickTime = 0f;
                ClearDrawing();
            }
        }
        if (Input.GetKeyDown(KeyCode.Space)) //resets scene using space bar
        {
            ClearDrawing();
        }

        //camera movement
        // if (Input.GetKeyDown(KeyCode.W))
        // {
        //     main_camera.transform.position = new Vector3(main_camera.transform.position.x, main_camera.transform.position.y + 1, main_camera.transform.position.z);
        // }
        // if (Input.GetKeyDown(KeyCode.A))
        // {
        //     main_camera.transform.position = new Vector3(main_camera.transform.position.x - 1, main_camera.transform.position.y, main_camera.transform.position.z);
        // }
        // if (Input.GetKeyDown(KeyCode.S))
        // {
        //     main_camera.transform.position = new Vector3(main_camera.transform.position.x, main_camera.transform.position.y - 1, main_camera.transform.position.z);
        // }
        // if (Input.GetKeyDown(KeyCode.D))
        // {
        //     main_camera.transform.position = new Vector3(main_camera.transform.position.x + 1, main_camera.transform.position.y, main_camera.transform.position.z);
        // }
    }

    //takes vector of tracked points and converts them to a vector of corner points for GetDirection()
    void GetCorners(List<Vector2> points)
    {
        Vector2 point1 = points[0];
        Vector2 point2 = points[1];
        Vector2 point3 = points[2];
        int i = 2;
        corners.Add(point1); //always adds first point
        while (true)
        {
            if (Vector2.Distance(point2, point1) > minDistance) //first length must be long enough
            {
                if (Vector2.Distance(point3, point2) > minDistance) //second length must be long enough
                {
                    float angle = Vector2.Angle(point1 - point2, point3 - point2); //degrees, 0 <= angle <= 180;
                    if (angle <= angleThreshold && angle >= 180 - angleThreshold) //if angle is not straight line
                    {
                        corners.Add(point2); //add middle point as corner
                        point1 = point2; //use middle point as new first point, and third point as new middle point
                        point2 = point3;
                        i++;
                        if (i >= points.Count) //use next point as new third point if possible, otherwise loop is done
                        {
                            break;
                        }
                        point3 = points[i];
                    }
                    else //angle is straight line
                    {
                        point2 = point3; //use third point as new middle, dont change first point
                        i++;
                        if (i >= points.Count)
                        {
                            break;
                        }
                        point3 = points[i];
                    }
                }
                else //second distance isnt long enough, use new third point
                {
                    i++;
                    if (i >= points.Count)
                    {
                        break;
                    }
                    point3 = points[i];
                }
            }
            else //first distance isnt long enough, update second and third points
            {
                point2 = point3;
                i++;
                if (i >= points.Count)
                {
                    break;
                }
                point3 = points[i];
            }
        }
        corners.Add(points[points.Count - 1]); //always add final point
        for (int j = 0; j < corners.Count - 1; j++)
        {
            if (Vector2.Distance(corners[j], corners[j + 1]) < postMinDistance) //if 2 corners are too close together after collecting them all, delete one
            {
                corners.Remove(corners[j]);
            }
        }
        /* //uncomment for red squares at corners
        for (int k = 0; k < corners.Count; k++)
        {
            GameObject tempSquare = Instantiate(redSquare, canvas.transform);
            tempSquare.GetComponent<RectTransform>().position = new Vector3(main_camera.ScreenToWorldPoint(corners[k]).x, main_camera.ScreenToWorldPoint(corners[k]).y, 0);
        }
        */
        GetDirections(corners);
    }

    //takes a vector of corner points and turns them into a series of directions for GetRune()
    void GetDirections(List<Vector2> corners)
    {
        for (int i = 0; i < corners.Count - 1; ++i)
        {
            Vector2 point1 = corners[i];
            Vector2 point2 = corners[i + 1];
            float angle = Mathf.Atan2(point2.y - point1.y, point2.x - point1.x) * Mathf.Rad2Deg; //returns -180 degrees to 180 degrees
            //Debug.Log("angle: " + angle);
            if (angle >= 157.5)
            {
                dirSequence += "6"; //left
            }
            else if (angle >= 112.5)
            {
                dirSequence += "7"; //up-left
            }
            else if (angle >= 77.5)
            {
                dirSequence += "0"; //up
            }
            else if (angle >= 22.5)
            {
                dirSequence += "1"; //up-right
            }
            else if (angle >= -22.5)
            {
                dirSequence += "2"; //right
            }
            else if (angle >= -77.5)
            {
                dirSequence += "3"; //down-right
            }
            else if (angle >= -112.5)
            {
                dirSequence += "4"; //down
            }
            else if (angle >= -157.5)
            {
                dirSequence += "5"; //down-left
            }
            else
            {
                dirSequence += "6"; //left
            }
        }
        //Debug.Log(dirSequence);
        GetRune(dirSequence);
    }

    //takes series of directions and matches it to known rune sequences
    void GetRune(string dirSequence)
    {
        if (map.ContainsKey(dirSequence)) //rune exists, cast spell here
        {
            completeSpell.Play();
            attackScript.AddSpell(map[dirSequence]);
            if (particlePrefab != null)
            {
                Vector2 mousePos = Input.mousePosition;
                GameObject particle = Instantiate(particlePrefab, new Vector3(main_camera.ScreenToWorldPoint(mousePos).x, main_camera.ScreenToWorldPoint(mousePos).y, 0), Quaternion.identity);
                var main = particle.GetComponent<ParticleSystem>().main;
                if (map[dirSequence] == "Fire")
                {
                    main.startColor = Color.red;
                }
                else if (map[dirSequence] == "Ice")
                {
                    main.startColor = Color.cyan;
                }
                else if (map[dirSequence] == "Lightning")
                {
                    main.startColor = Color.blue;
                }
            }
        }
        else //if no match, reverses the sequence and tries again
        {
            string reverseSequence = ReverseSequence(dirSequence);
            if (map.ContainsKey(reverseSequence)) //rune exists, cast spell here
            {
                completeSpell.Play();
                attackScript.AddSpell(map[reverseSequence]);
            }
            else //if no match again, rune doesn't exist
            {
                Debug.Log("invalid shape");
            }
        }
        Invoke("ClearDrawing", 1.0f);
    }

    //reverses series of directions by flipping both individual directions and their order
    string ReverseSequence(string dirSequence)
    {
        string output = "";
        for (int i = dirSequence.Length - 1; i >= 0; i--)
        {
            if (dirSequence[i] == '0')
            {
                output += "4";
            }
            else if (dirSequence[i] == '1')
            {
                output += "5";
            }
            else if (dirSequence[i] == '2')
            {
                output += "6";
            }
            else if (dirSequence[i] == '3')
            {
                output += "7";
            }
            else if (dirSequence[i] == '4')
            {
                output += "0";
            }
            else if (dirSequence[i] == '5')
            {
                output += "1";
            }
            else if (dirSequence[i] == '6')
            {
                output += "2";
            }
            else
            {
                output += "3";
            }
        }
        return output;
    }

    void ClearDrawing()
    {
        flag = true;
        points = new List<Vector2>();
        corners = new List<Vector2>();
        var squares = GameObject.FindGameObjectsWithTag("square");
        foreach (var i in squares)
        {
            Destroy(i);
        }
        dirSequence = "";
    }
}

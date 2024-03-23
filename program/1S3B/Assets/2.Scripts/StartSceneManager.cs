using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSceneManager : MonoBehaviour
{
    public GameObject back1_1;
    public GameObject back1_2;
    public GameObject back1_3;

    public GameObject back2_1;
    public GameObject back2_2;
    public GameObject back2_3;

    public GameObject back3_1;
    public GameObject back3_2;
    public GameObject back3_3;

    private float speed1;
    private float speed2;
    private float speed3;

    private void Start()
    {
        speed1 = 0.5f;
        speed2 = 1;
        speed3 = 1.5f;
    }

    private void Update()
    {

        back1_1.transform.position -= new Vector3(speed1 * Time.deltaTime, 0, 0);
        back1_2.transform.position -= new Vector3(speed1 * Time.deltaTime, 0, 0);
        back1_3.transform.position -= new Vector3(speed1 * Time.deltaTime, 0, 0);
        back2_1.transform.position -= new Vector3(speed2 * Time.deltaTime, 0, 0);
        back2_2.transform.position -= new Vector3(speed2 * Time.deltaTime, 0, 0);
        back2_3.transform.position -= new Vector3(speed2 * Time.deltaTime, 0, 0);
        back3_1.transform.position -= new Vector3(speed3 * Time.deltaTime, 0, 0);
        back3_2.transform.position -= new Vector3(speed3 * Time.deltaTime, 0, 0);
        back3_3.transform.position -= new Vector3(speed3 * Time.deltaTime, 0, 0);

    }

}

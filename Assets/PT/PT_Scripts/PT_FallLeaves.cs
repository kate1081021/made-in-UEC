using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PT_FallLeaves : MonoBehaviour
{
    // Update is called once per frame
    [SerializeField] private GameObject leaf;
    [SerializeField] private List<GameObject> leaves;
    [SerializeField] private float falltime;
    [SerializeField] public float obstacle_power;

    void Awake()
    {
        falltime = Random.Range(0.5f, 1f);
    }
    void Update()
    {
        falltime -= Time.deltaTime * Time.timeScale;
        if (falltime < 0)
        {
            falltime = Random.Range(0.5f, 0.7f);
            Vector3 sp = new Vector3(transform.position.x + Random.Range(-5f, 5f), transform.position.y + Random.Range(-1f, 1f), Random.Range(-0.1f, 1f));
            leaves.Add(Instantiate(leaf, sp, Quaternion.identity));
        }
        for (int i = 0; i < leaves.Count;)
        {
            GameObject g = leaves[i];
            Vector3 tp = g.transform.position;
            g.transform.position = new Vector3(tp.x + 5 * obstacle_power * Time.deltaTime * Time.timeScale, tp.y - 1.5f * Time.deltaTime * Time.timeScale, tp.z);
            if (g.transform.position.y < -3.5f)
            {
                leaves.Remove(g);
                Destroy(g);
                continue;
            }
            i++;
        }
    }
}

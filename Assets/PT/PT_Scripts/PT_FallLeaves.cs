using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class PT_FallLeaves : MonoBehaviour
{
    // Update is called once per frame
    [SerializeField] private GameObject leaf;
    [SerializeField] private List<GameObject> leaves;
    [SerializeField] private List<float> leaves_random_index;
    [SerializeField] private float falltime;
    [SerializeField] public float obstacle_power;
    [SerializeField] private float time;

    void Awake()
    {
        falltime = Random.Range(0.5f, 1f);
        time = 0f;
    }
    void Update()
    {
        falltime -= Time.deltaTime * Time.timeScale;
        time += Time.deltaTime * Time.timeScale;
        if (falltime < 0)
        {
            falltime = Random.Range(0.5f, 0.7f);
            Vector3 sp = new Vector3(transform.position.x + Random.Range(-5f, 5f), transform.position.y + Random.Range(-1f, 1f), Random.Range(-0.1f, 1f));
            Quaternion sr = Quaternion.Euler(0, 0, Random.Range(-5f, 5f));
            leaves.Add(Instantiate(leaf, sp, sr));
            leaves_random_index.Add(Random.Range(0f, 100f));
        }
        for (int i = 0; i < leaves.Count;)
        {
            GameObject g = leaves[i];
            float offset = leaves_random_index[i];

            // サイズも変える、反転も
            float flip = (offset > 50f) ? -1f : 1f;
            g.transform.localScale = new Vector3( flip * (offset/5000f + 0.035f), offset/5000f + 0.035f, 0.05f);

            // 下方向
            Vector3 nextPos = g.transform.position;
            nextPos.y -= 1.5f * Time.deltaTime * Time.timeScale;
            nextPos.x += 5 * obstacle_power * Time.deltaTime * Time.timeScale;

            // 揺れ
            // Vector3 tp = g.transform.position;
            float wave = Mathf.Cos(time * 2.0f + offset) * Time.deltaTime * Time.timeScale;

            // 座標更新
            nextPos.x += wave;
            // g.transform.position = new Vector3(tp.x + 5 * obstacle_power * Time.deltaTime * Time.timeScale + Mathf.Sin(2 * Mathf.PI * f * time) * Time.deltaTime * Time.timeScale, tp.y - 1.5f * Time.deltaTime * Time.timeScale, tp.z);
            g.transform.position = nextPos;

            // 回転
            Quaternion nextRot = g.transform.rotation;
            nextRot.z += wave;
            g.transform.rotation = nextRot;

            if (g.transform.position.y < -3.5f)
            {
                leaves.Remove(g);
                leaves_random_index.Remove(offset);
                Destroy(g);
                continue;
            }
            i++;
        }
    }
}

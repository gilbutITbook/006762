using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    public Transform areaTopLeft;
    public Transform areaBottomRight;
    public GameObject firePrefab;
    public float minTime = 1f;
    public float maxTime = 2f;

    IEnumerator Attack()
    {
        // 이동 계산.
        Vector3 from_position = transform.position;
        Vector3 to_position = new Vector3(
            Random.Range(areaTopLeft.position.x, areaBottomRight.position.x),
            Random.Range(areaBottomRight.position.y, areaTopLeft.position.y),
            transform.position.z);
        float start_time = Time.time;
        float move_time = Random.Range(minTime, maxTime);

        // 이동 처리.
        while (true)
        {
            float t = (Time.time - start_time) / move_time;
            transform.position = Vector3.Lerp(from_position, to_position, t);
            if (t >= 1f)
                break;
            yield return null;
        }

        // 도착 후 공격.
        Instantiate(firePrefab, transform.position, Quaternion.identity);

        // 다음 이동과 공격 시작.
        StartCoroutine(Attack());
    }

    void Start()
    {
        StartCoroutine(Attack());
    }
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PuzzleGenerator : MonoBehaviour {

    public GameObject testUnit;

    void Start()
    {
        //float x = testUnit.GetComponent<SpriteRenderer>().bounds.size.x;
        //float pixel = testUnit.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit;
        //float width = x * pixel;
        StartCoroutine(shrinkUnit(this.gameObject));
         
    }

    IEnumerator shrinkUnit(GameObject unit)
    {
        yield return new WaitForSeconds(1f);
        Vector3 desVec = new Vector3(unit.transform.position.x + 0.4f, unit.transform.position.y + 0.4f, 0);
        //while (unit.transform.localScale != Vector3.zero)
        //{
        //    unit.transform.localScale = Vector3.MoveTowards(unit.transform.localScale, Vector3.zero, 0.175f);
        //    unit.transform.position = Vector3.MoveTowards(unit.transform.position, desVec, 0.06f);
        //    yield return new WaitForEndOfFrame();
        //}
        //while (true)
        //{
        //    unit.transform.position = Vector3.MoveTowards(unit.transform.position, new Vector3(10, 10, 10), 1f);
        //    yield return new WaitForEndOfFrame();
        //}
    }
}

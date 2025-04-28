using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightning : MonoBehaviour
{
        public LineRenderer line;
        public GameObject target;
        public Vector2 start;
        private Collider2D[] targets;
        private bool onPrograss;
        private float delay;
        private float maxDelay = 0.3f;

    void Update()
    {
        if(onPrograss)
        {
            if(delay >= maxDelay)
            {
                onPrograss = false;
                delay = 0;
                LineReset();
            }else{
                delay += Time.deltaTime;
            }
        }
    }
    public void Init(LineRenderer line)
        {
            targets = new Collider2D[5];
            this.line = line;
        }
        private Coroutine sutDownCoroutine;
        public void SetTarget(Vector2 start,GameObject target)
        {
            onPrograss = true;
            delay = 0;
            this.start = start;
            this.target = target;

            line.gameObject.SetActive(true);
            line.SetPosition(0,start);
            line.SetPosition(1,target.transform.position);
            
            // if(sutDownCoroutine != null) StopCoroutine(sutDownCoroutine);
            // sutDownCoroutine = StartCoroutine(SutDownCo());
            
            //Detect overlap Circle
            
        }

        // IEnumerator SutDownCo()
        // {
        //     float percent = 0;
        //     Vector2 end = line.GetPosition(1);
        //     while(percent <1)
        //     {
        //         percent += Time.deltaTime * 3;
        //         var position = Vector2.Lerp(start,end,percent);
        //         line.SetPosition(1,position);
        //         yield return null;
        //     }

        //     LineReset();
        // }

        private void LineReset()
        {
            line.gameObject.SetActive(false);
        }
}

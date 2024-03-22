using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHookTest : MonoBehaviour
{
    public LineRenderer line;
    public Transform hook;
    private Vector2 _mouseDir;
    private bool isHook;
    private bool isLineMax;
    public bool isAttach;
    
    // Start is called before the first frame update
    void Start()
    {
        line.positionCount = 2;
        line.endWidth = line.startWidth = 0.05f;
        line.SetPosition(0, transform.position);
        line.SetPosition(1, hook.position);
        line.useWorldSpace = true;
        hook.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        line.SetPosition(0, transform.position);
        line.SetPosition(1, hook.position);

        if (Input.GetMouseButtonDown(0) && !isAttach)
        {
            hook.position = transform.position;
            _mouseDir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            isHook = true;
            isLineMax = false;
            hook.gameObject.SetActive(true);
        }

        if (isHook && !isLineMax && !isAttach)
        {
            hook.Translate(_mouseDir.normalized * Time.deltaTime * 15);

            if (Vector2.Distance(transform.position, hook.position) > 5)
            {
                isLineMax = true;
            }
        }
        else if (isHook && isLineMax && !isAttach)
        {
            hook.position = Vector2.MoveTowards(hook.position, transform.position, Time.deltaTime * 15);
            if (Vector2.Distance(transform.position, hook.position) < 0.1f)
            {
                isHook = false;
                isLineMax = false;
                hook.gameObject.SetActive(false);
            }
        }
        else if (isAttach)
        {
            if (Input.GetMouseButtonDown(0))
            {
                isAttach = false;
                isHook = false;
                isLineMax = false;
                hook.GetComponent<HookTest>()._joint2D.enabled = false;
                hook.gameObject.SetActive(false);
            }
        }
    }
}

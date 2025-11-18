using System.Collections;
using UnityEngine;

public class HydraulicPress_Net : ActivatableObject_Net_Entity
{

    protected override void Active()
    {
        PressOn();
    }
    protected override void Deactive()
    {
        PressRelease();
    }


    #region  Press

    #region Steam Ani

    [Header("Steam Ani")]
    [SerializeField] Animator _UPStem;
    [SerializeField] Animator _DownStem;
    public void IsAnimationPlaying(string animationName)
    {
        AnimatorStateInfo stateInfo1 = _UPStem.GetCurrentAnimatorStateInfo(0);
        AnimatorStateInfo stateInfo2 = _DownStem.GetCurrentAnimatorStateInfo(0);

        //Steam Sound

        //Steam Sound

        if (!stateInfo1.IsName(animationName))
        {
            _UPStem.Play(animationName);
        }
        if (!stateInfo2.IsName(animationName))
        {
            _DownStem.Play(animationName);
        }

    }
    #endregion

    [Header("Light")]
    [SerializeField] SpriteRenderer lightSpriteRenderer;
    [SerializeField] Sprite greenSprite;
    [SerializeField] Sprite redSprite;
    [SerializeField] Material _GreenLightMat;
    [SerializeField] Material _RedLightMat;

    [Header("Press")]
    [SerializeField] Transform rayPoint;
    [SerializeField] float rayDistance;
    [SerializeField] Transform pressTr;
    [SerializeField] BoxCollider2D pressCol;
    [SerializeField] float pressSpeed = 1.5f;
    private float minPressLength = -1.85f;
    private float maxPressLength = 2.5f;
    private Vector2 minColOffset = new Vector2(-0.2f, 0);
    private Vector2 minColSize = new Vector2(0, 0.6f);
    private Vector2 maxColOffset = new Vector2(-2.4f, 0);
    private Vector2 maxColSize = new Vector2(4.4f, 0.6f);
    private Coroutine pressOnCoroutine;
    private Coroutine pressReleaseCoroutine;

    private RaycastHit2D hit;
    [SerializeField] LayerMask layerMask;

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(rayPoint.position, pressTr.right * rayDistance);
    }
#endif
    private IEnumerator PressOnCo()
    {

        lightSpriteRenderer.sprite = greenSprite;
        lightSpriteRenderer.material = _GreenLightMat;

        float x = pressTr.localPosition.x;
        float percent = (x - minPressLength) / (maxPressLength - minPressLength);

        //Sound Start
        if (soundCoroutine != null) StopCoroutine(soundCoroutine);        
        soundCoroutine = StartCoroutine(ActiveEffectSound());
        //Sound Start

        while (percent < 1)
        {
            hit = Physics2D.Raycast(rayPoint.position, pressTr.right, rayDistance, layerMask);
            if (hit.collider == null)
            {
                x += Time.deltaTime * pressSpeed;
                pressTr.localPosition = new Vector3(x, 0, 0);

                percent = (x - minPressLength) / (maxPressLength - minPressLength);
                pressCol.offset = Vector2.Lerp(minColOffset, maxColOffset, percent);
                pressCol.size = Vector2.Lerp(minColSize, maxColSize, percent);
            }
            else
            {
                //Stop Press
                pressOnCoroutine = null;
                    //Sound End
                isSoundPlaying = false;
                    //Sound End
                yield break;
                //Stop Press
            }
            yield return null;
        }
        //Sound End
        isSoundPlaying = false;
        //Sound End
        pressTr.localPosition = new Vector3(maxPressLength, 0, 0);
        pressCol.offset = maxColOffset;
        pressCol.size = maxColSize;

        pressOnCoroutine = null;
    }

    private IEnumerator PressReleaseCo()
    {
        float x = pressTr.localPosition.x;
        lightSpriteRenderer.sprite = redSprite;
        lightSpriteRenderer.material = _RedLightMat;
        float percent = (x - minPressLength) / (maxPressLength - minPressLength);

        //Sound Start
        if (soundCoroutine != null) StopCoroutine(soundCoroutine);
        soundCoroutine = StartCoroutine(ActiveEffectSound());
        //Sound Start
        while (x > minPressLength)
        {
            pressTr.localPosition = new Vector3(x, 0, 0);
            x -= Time.deltaTime * pressSpeed;

            percent = (x - minPressLength) / (maxPressLength - minPressLength);

            pressCol.offset = Vector2.Lerp(minColOffset, maxColOffset, percent);
            pressCol.size = Vector2.Lerp(minColSize, maxColSize, percent);

            yield return null;
        }
        pressTr.localPosition = new Vector3(minPressLength, 0, 0);
        pressCol.offset = minColOffset;
        pressCol.size = minColSize;

        //Sound End
        isSoundPlaying = false;
        //Sound End

        pressReleaseCoroutine = null;
        

    }
    #region  Clean
    public override void Clean_Value()
    {
        StopAllCoroutines();
        lightSpriteRenderer.sprite = redSprite;
        lightSpriteRenderer.material = _RedLightMat;
        
        pressTr.localPosition = new Vector3(minPressLength, 0, 0);
        pressCol.offset = minColOffset;
        pressCol.size = minColSize;
        isSoundPlaying = false;
        pressReleaseCoroutine = null;
    }

    #endregion
    private void PressOn()
    {
        IsAnimationPlaying("Steam");

        if (pressReleaseCoroutine != null)
        {
            StopCoroutine(pressReleaseCoroutine);
            pressOnCoroutine = null;
        }
        pressOnCoroutine = StartCoroutine(PressOnCo());
    }
    private void PressRelease()
    {

        IsAnimationPlaying("Steam");
        if (pressOnCoroutine != null)
        {
            StopCoroutine(pressOnCoroutine);
            pressOnCoroutine = null;
        }
        pressReleaseCoroutine = StartCoroutine(PressReleaseCo());
    }

    #endregion

    private bool isSoundPlaying;
    AudioSourceController audioSourceController;
    private Coroutine soundCoroutine;
    IEnumerator ActiveEffectSound()
    {
        isSoundPlaying = true;
        float elis = 0;

        Managers.Sound.PlaySound3D(GlobalText.HYDRAULICPRESS_STEAM,transform.position);

        if (audioSourceController == null)
        {
            audioSourceController = Managers.Sound.PlaySound3D(GlobalText.HYDRAULICPRESS_LOOP, transform.position, 0, true);
        }
        else
        {
            audioSourceController.ClipChange(Managers.Sound.GetAudioClip(GlobalText.HYDRAULICPRESS_LOOP), true);
        }

        var source = audioSourceController.GetAudioSource();

        while (elis < 1)
        {
            elis += Time.deltaTime;
            elis = Mathf.Clamp01(elis);
            source.volume = elis;
            yield return null;
        }

        yield return new WaitUntil(() => !isSoundPlaying);

        var clip = Managers.Sound.GetAudioClip(GlobalText.HYDRAULICPRESS_END);
        audioSourceController.ClipChange(clip, false);
        audioSourceController = null;
        soundCoroutine = null;
    }
    
}

using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ShadowCasterSetting : MonoBehaviour
{

#region Reflection field
    static readonly FieldInfo meshField = typeof(ShadowCaster2D).GetField("m_Mesh", BindingFlags.NonPublic | BindingFlags.Instance);
	static readonly FieldInfo shapePathField = typeof(ShadowCaster2D).GetField("m_ShapePath", BindingFlags.NonPublic | BindingFlags.Instance);
	static readonly FieldInfo shapePathHashField = typeof(ShadowCaster2D).GetField("m_ShapePathHash", BindingFlags.NonPublic | BindingFlags.Instance);
    static readonly FieldInfo sortingLayersField = typeof(ShadowCaster2D).GetField("m_ApplyToSortingLayers", BindingFlags.NonPublic | BindingFlags.Instance);
	static readonly MethodInfo generateShadowMeshMethod = typeof(ShadowCaster2D)
									.Assembly
									.GetType("UnityEngine.Rendering.Universal.ShadowUtility")
									.GetMethod("GenerateShadowMesh", BindingFlags.Public | BindingFlags.Static);
#endregion

    [SerializeField] ShadowCaster2D shadowCaster2D;

    // public void Setting()
    // {
    //     shadowCaster2D = GetComponent<ShadowCaster2D>();
    // }


    // private void Start(){
    
    //     //Test
    //    shadowCaster2D = GetComponent<ShadowCaster2D>();
    // }
#region Get,Set
   public ShadowCasterStruct GetShadowCasterStruct()
   {
    ShadowCasterStruct data = new ShadowCasterStruct(transform.position,shadowCaster2D.selfShadows,GetSortingLayers(),shadowCaster2D.shapePath);
    return data;
   }

    private int[] GetSortingLayers()
    {
        return sortingLayersField.GetValue(shadowCaster2D) as int[];
    }

   public void SetShadowCasterData(ShadowCasterStruct data)
   {
        shadowCaster2D.selfShadows = data.selfShadows;
        SetSortingLayer(data.sortingLayers);

        shapePathField.SetValue(shadowCaster2D, data.vertices); //ShadowCaster2D가 사용할 모양 경로(Shape Path)를 testPath 값으로 설정
        shapePathHashField.SetValue(shadowCaster2D, Random.Range(int.MinValue, int.MaxValue)); //해쉬값 설정
		meshField.SetValue(shadowCaster2D, new Mesh()); //기존 메쉬를 덮어쓰고 새롭게 생성한 빈 Mesh 객체를 ShadowCaster2D에 설정
        generateShadowMeshMethod.Invoke(shadowCaster2D,
		new object[] { meshField.GetValue(shadowCaster2D), shapePathField.GetValue(shadowCaster2D) });
   }
   private void SetSortingLayer(int[] layers)
   {
        // var renderer = GetComponent<Renderer>();
        sortingLayersField.SetValue(shadowCaster2D,layers);
   }
#endregion
}


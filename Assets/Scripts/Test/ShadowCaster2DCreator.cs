using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;



#if UNITY_EDITOR

[RequireComponent(typeof(CompositeCollider2D))]
public class ShadowCaster2DCreator : MonoBehaviour
{
	[SerializeField]
	private bool selfShadows = true;

	private CompositeCollider2D tilemapCollider;

	static readonly FieldInfo meshField = typeof(ShadowCaster2D).GetField("m_Mesh", BindingFlags.NonPublic | BindingFlags.Instance);
	static readonly FieldInfo shapePathField = typeof(ShadowCaster2D).GetField("m_ShapePath", BindingFlags.NonPublic | BindingFlags.Instance);
	static readonly FieldInfo shapePathHashField = typeof(ShadowCaster2D).GetField("m_ShapePathHash", BindingFlags.NonPublic | BindingFlags.Instance);
	static readonly MethodInfo generateShadowMeshMethod = typeof(ShadowCaster2D)
									.Assembly
									.GetType("UnityEngine.Rendering.Universal.ShadowUtility")
									.GetMethod("GenerateShadowMesh", BindingFlags.Public | BindingFlags.Static);

	public void Create()
	{
		DestroyOldShadowCasters();
		tilemapCollider = GetComponent<CompositeCollider2D>();

		for (int i = 0; i < tilemapCollider.pathCount; i++)
		{
			Vector2[] pathVertices = new Vector2[tilemapCollider.GetPathPointCount(i)];
			tilemapCollider.GetPath(i, pathVertices);
			GameObject shadowCaster = new GameObject("shadow_caster_" + i);
			shadowCaster.transform.parent = gameObject.transform;
			ShadowCaster2D shadowCasterComponent = shadowCaster.AddComponent<ShadowCaster2D>();
			shadowCasterComponent.selfShadows = this.selfShadows;

			Vector3[] testPath = new Vector3[pathVertices.Length];
			for (int j = 0; j < pathVertices.Length; j++)
			{
				testPath[j] = pathVertices[j];
			}

			shapePathField.SetValue(shadowCasterComponent, testPath); //ShadowCaster2D가 사용할 모양 경로(Shape Path)를 testPath 값으로 설정
            shapePathHashField.SetValue(shadowCasterComponent, Random.Range(int.MinValue, int.MaxValue)); //해쉬값 설정
			meshField.SetValue(shadowCasterComponent, new Mesh()); //기존 메쉬를 덮어쓰고 새롭게 생성한 빈 Mesh 객체를 ShadowCaster2D에 설정
            generateShadowMeshMethod.Invoke(shadowCasterComponent,
			new object[] { meshField.GetValue(shadowCasterComponent), shapePathField.GetValue(shadowCasterComponent) });
		}
	}
	public void DestroyOldShadowCasters()
	{

		var tempList = transform.Cast<Transform>().ToList();
		foreach (var child in tempList)
		{
			DestroyImmediate(child.gameObject);
		}
	}
}

[CustomEditor(typeof(ShadowCaster2DCreator))]
public class ShadowCaster2DTileMapEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Create"))
		{
			var creator = (ShadowCaster2DCreator)target;
			creator.Create();
		}

		if (GUILayout.Button("Remove Shadows"))
		{
			var creator = (ShadowCaster2DCreator)target;
			creator.DestroyOldShadowCasters();
		}
		EditorGUILayout.EndHorizontal();
	}

}

#endif














































































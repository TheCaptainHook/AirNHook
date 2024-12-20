
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TestTextEffect : MonoBehaviour
{
  [SerializeField] TextMeshProUGUI main_Text;

    public AnimationCurve curve;
    CharInfoField[] charInfos;

    float glitchIntensity = 5f; // 흔들리는 강도
    float glitchFrequency = 0.1f; // 흔들림 업데이트 주기
    int characterChangeCount = 4; // 문자가 변하는 횟수
    float characterChangeInterval = 0.2f; // 문자 변환 간격


    private void Start(){

        main_Text.ForceMeshUpdate();

        charInfos = new CharInfoField[main_Text.text.Length];
        for(int i = 0; i< main_Text.text.Length;i++){
            charInfos[i] = new CharInfoField(main_Text.textInfo,main_Text.textInfo.characterInfo[i]);
        }

        // charInfo = main_Text.textInfo.characterInfo[1];
        // vertexIndex = charInfo.vertexIndex;
        // materialIndex = charInfo.materialReferenceIndex;
        // vertices = main_Text.textInfo.meshInfo[materialIndex].vertices;
        
        // originalVertices = vertices.Clone() as Vector3[];
        // charCenter = (vertices[vertexIndex] + vertices[vertexIndex + 2]) / 2;

        StartCoroutine(EffectCo(charInfos));
    }


    private void Update(){
        
        // OnEffect(charInfos);
        
    }

    private void OnEffect(CharInfoField[] charInfos){

        for(int i =0; i< charInfos.Length;i++){
            CharInfoField charInfoField = charInfos[i];

            for(int j = 0; j<4;j++){
                Vector3 offset = new Vector3(
                    Random.Range(-glitchIntensity, glitchIntensity),
                    Random.Range(-glitchIntensity, glitchIntensity),
                    0);
                charInfoField.vertices[charInfoField.vertexIndex + j] = charInfoField.originalVertices[charInfoField.vertexIndex + j] + offset;

            }

            var meshInfo = main_Text.textInfo.meshInfo[charInfoField.materialIndex];
            meshInfo.mesh.vertices = meshInfo.vertices;
            main_Text.UpdateGeometry(meshInfo.mesh,charInfoField.materialIndex);    

        }
        // for (int i = 0; i < main_Text.textInfo.meshInfo.Length; i++)
        // {
        //         var meshInfo = main_Text.textInfo.meshInfo[i];
        //         meshInfo.mesh.vertices = meshInfo.vertices;
        //         main_Text.UpdateGeometry(meshInfo.mesh,i);
        // }
            
    }


    IEnumerator EffectCo(CharInfoField[] charInfos){
        while(true){
            int num = Random.Range(0,charInfos.Length);
            yield return CharEffectCo(charInfos[num]);
            // for(int i =0; i< charInfos.Length;i++){
            //     CharInfoField charInfoField = charInfos[i];
            //     if(charInfoField.charInfo.isVisible)
            //     yield return CharEffectCo(charInfoField);

            // }

            
        }
    }

    float duration = 1;
    IEnumerator CharEffectCo(CharInfoField charInfoField){
        float percent =0;

        while(percent < .5f){
            percent += Time.deltaTime;

            for(int j = 0; j<4;j++){
                Vector3 offset = new Vector3(
                    Random.Range(-glitchIntensity, glitchIntensity),
                    Random.Range(-glitchIntensity, glitchIntensity),
                    0);
                charInfoField.vertices[charInfoField.vertexIndex + j] = charInfoField.originalVertices[charInfoField.vertexIndex + j] + offset;

            }

            var meshInfo = main_Text.textInfo.meshInfo[charInfoField.materialIndex];
            meshInfo.mesh.vertices = meshInfo.vertices;
            main_Text.UpdateGeometry(meshInfo.mesh,charInfoField.materialIndex);  
           
            yield return null;
        }

        for(int j = 0; j<4;j++) 
            charInfoField.vertices[charInfoField.vertexIndex + j] = charInfoField.originalVertices[charInfoField.vertexIndex + j];

    }
         

        
}
    

 public class CharInfoField{

        public TMP_TextInfo tmp;
        public TMP_CharacterInfo charInfo;
        public Vector3[] vertices;
        public Vector3[] originalVertices;
        public int vertexIndex;
        public int materialIndex;
        public Vector3 charCenter;
        public CharInfoField(TMP_TextInfo tmp,TMP_CharacterInfo charInfo){
            this.tmp = tmp;
            this.charInfo = charInfo;
            vertexIndex = charInfo.vertexIndex;
            materialIndex = charInfo.materialReferenceIndex;

            vertices = tmp.meshInfo[materialIndex].vertices;
            originalVertices = (Vector3[])vertices.Clone();
            charCenter = (vertices[vertexIndex] + vertices[vertexIndex + 2]) / 2;

        }
}

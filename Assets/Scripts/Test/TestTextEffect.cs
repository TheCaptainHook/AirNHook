
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class TestTextEffect : MonoBehaviour
{
  [SerializeField] TextMeshProUGUI main_Text;

    CharInfoField[] charInfos;
    float glitchIntensity = 2f; // 흔들리는 강도


    private void Start(){

        main_Text.ForceMeshUpdate();

        charInfos = new CharInfoField[main_Text.text.Length];
        for(int i = 0; i< main_Text.text.Length;i++){
            charInfos[i] = new CharInfoField(main_Text.textInfo,main_Text.textInfo.characterInfo[i]);
        }


        StartCoroutine(EffectCo(charInfos));
    }



    IEnumerator EffectCo(CharInfoField[] charInfos){
        while(true){
            int num = Random.Range(0,charInfos.Length);
            yield return CharEffectCo();

            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator CharEffectCo(){
        float percent =0;

        char[] cached = main_Text.text.ToCharArray();
        float changeCharPercent = 0;

        int[] c= new int[2];
        for (int i = 0; i < 2; i++) 
        {
            int ran = Random.Range(0, main_Text.text.Length);
            c[i] = ran;
        }

        while (percent < 1f){
            percent += Time.deltaTime;
            changeCharPercent += Time.deltaTime;

            for(int i = 0; i < 2; i++)
            {
                CharInfoField charInfoField = charInfos[c[i]];
               
                if (changeCharPercent > 0.3f)
                {
                    cached[charInfoField.charIndex] = (char)Random.Range(33, 126);
                    main_Text.SetText(cached);
                }

                //float size = 1 - percent;
                for (int j = 0; j < 4; j++)
                {
                    Vector3 offset = new Vector3(
                        Random.Range(-glitchIntensity, glitchIntensity),
                        Random.Range(-glitchIntensity, glitchIntensity),
                        0);
                    charInfoField.vertices[charInfoField.vertexIndex + j] = charInfoField.originalVertices[charInfoField.vertexIndex + j] + offset;

                }

                var meshInfo = main_Text.textInfo.meshInfo[charInfoField.materialIndex];
                meshInfo.mesh.vertices = meshInfo.vertices;
                main_Text.UpdateGeometry(meshInfo.mesh, charInfoField.materialIndex);

            }

            if (changeCharPercent > 0.3f) changeCharPercent = 0;



            yield return null;
        }


        for(int i = 0; i < c.Length; i++)
        {
            CharInfoField charInfoField = charInfos[c[i]];
            cached[charInfoField.charIndex] = charInfoField.orgChar;
            main_Text.SetText(cached);

            for (int j = 0; j < 4; j++)
                charInfoField.vertices[charInfoField.vertexIndex + j] = charInfoField.originalVertices[charInfoField.vertexIndex + j];
        }

    }
         
}
    

// public class CharInfoField{

//        public TMP_TextInfo tmp;
//        public TMP_CharacterInfo charInfo;
//        public Vector3[] vertices;
//        public Vector3[] originalVertices;
//        public int vertexIndex;
//        public int materialIndex;
//        public Vector3 charCenter;

//        public char orgChar;
//        public int charIndex;

//        public CharInfoField(TMP_TextInfo tmp,TMP_CharacterInfo charInfo){
//            this.tmp = tmp;
//            this.charInfo = charInfo;
//            vertexIndex = charInfo.vertexIndex;
//            materialIndex = charInfo.materialReferenceIndex;

//            vertices = tmp.meshInfo[materialIndex].vertices;
//            originalVertices = (Vector3[])vertices.Clone();
//            charCenter = (vertices[vertexIndex] + vertices[vertexIndex + 2]) / 2;

//            orgChar = charInfo.character;
//            charIndex = charInfo.index;

//        }
//}

using System.Collections;
using System.Collections.Generic;
using GoogleSheet.Type;
using UnityEngine;

public class UGS_Type : MonoBehaviour
{
    [Type(typeof(Vector2), new string[] { "Vector2", "vector2", "Vec2" })]
    public class Vector2Type : IType
    {
        public object DefaultValue => Vector2.zero;
        /// <summary>
        /// value는 스프레드 시트에 적혀있는 값
        /// </summary> 
        public object Read(string value)
        {
            // value : [1,2,3] 
            var values = ReadUtil.GetBracketValueToArray(value);
            float x = float.Parse(values[0]);
            float y = float.Parse(values[1]);
            return new Vector2(x, y);
        }

        /// <summary>
        /// value write to google sheet
        /// </summary> 
        public string Write(object value)
        {
            Vector2 v = (Vector2)value;
            return $"[{v.x},{v.y}]";
        }
    }


    [Type(typeof(Vector2Int), new string[] { "Vector2Int", "vector2Int", "Vec2Int" })]
    public class Vector2IntType : IType
    {
        public object DefaultValue => Vector2Int.zero;
        /// <summary>
        /// value는 스프레드 시트에 적혀있는 값
        /// </summary> 
        public object Read(string value)
        {
            // value : [1,2,3] 
            var values = ReadUtil.GetBracketValueToArray(value);
            int x = int.Parse(values[0]);
            int y = int.Parse(values[1]);
            return new Vector2Int(x, y);
        }

        /// <summary>
        /// value write to google sheet
        /// </summary> 
        public string Write(object value)
        {
            Vector2Int v = (Vector2Int)value;
            return $"[{v.x},{v.y}]";
        }
    }




}

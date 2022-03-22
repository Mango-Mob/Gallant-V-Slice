using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "characterData", menuName = "Game Data/Character Data", order = 1)]
public class CharacterData : ScriptableObject
{
    public string m_name;
    public Sprite[] m_characterBody;
    public Sprite[] m_characterFace;
    public Vector2[] m_faceAnchor;


    public void DrawToGUI(Vector2 position, int bodyID, int faceID, float scale)
    {
        if (bodyID < m_characterBody.Length && faceID < m_characterFace.Length)
        {
            Texture body = m_characterBody[bodyID].texture;
            Texture face = m_characterFace[faceID].texture;

            Vector2 anchor = Vector2.zero;
            if (bodyID < m_faceAnchor.Length)
                anchor = new Vector2(m_faceAnchor[bodyID].x, m_faceAnchor[bodyID].y);

            GUILayout.Space(body.height);
            GUILayout.BeginArea(new Rect(position.x, position.y, body.width, body.height));
            GUI.DrawTexture(new Rect(0, 0, body.width * scale, body.height * scale), body);
            GUI.DrawTexture(new Rect(anchor.x, anchor.y, face.width * scale, face.height * scale), face);
            GUILayout.EndArea();
        }
    }

    public void DrawToGUI(Rect rect, int bodyID, int faceID)
    {
        if (bodyID < m_characterBody.Length && faceID < m_characterFace.Length)
        {
            Texture body = m_characterBody[bodyID].texture;
            Texture face = m_characterFace[faceID].texture;

            Vector2 anchor = Vector2.zero;
            if (bodyID < m_faceAnchor.Length)
                anchor = new Vector2(m_faceAnchor[bodyID].x, m_faceAnchor[bodyID].y);

            float scale = Mathf.Min(rect.width / body.width, rect.height / body.height);
            GUILayout.Space(rect.height * scale);
            GUILayout.BeginArea(rect);
            GUI.DrawTexture(new Rect(0, 0, body.width * scale, body.height * scale), body);
            GUI.DrawTexture(new Rect(anchor.x, anchor.y, face.width * scale, face.height * scale), face);
            GUILayout.EndArea();
        }
    }
}
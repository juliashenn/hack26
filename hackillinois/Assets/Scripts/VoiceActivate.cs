using TMPro;
using UnityEngine;

public class VoiceActivate : MonoBehaviour
{
    [SerializeField] private TMP_Text commentText;

    void Awake()
    {
        if (commentText == null)
        {
            // Go up to parent, then find the path to TMP
            Transform parent = transform.parent.parent;
            if (parent != null)
            {
                Transform t = parent.Find("Panel/Scroll View/Viewport/Transcription");
                if (t != null)
                    commentText = t.GetComponent<TMP_Text>();
                else
                    Debug.LogError("Cannot find CommentTMP under ChildA/ChildB!");
            }
            else
            {
                Debug.LogError("No parent found for this GameObject!");
            }
        }
    }
    public void startListening()
    {
        //Debug.Log($"StartListening called with: {commentText.name} | instanceID: {commentText.GetInstanceID()}");
        VoiceCommentManager.Instance.StartListening(commentText);
    }


}

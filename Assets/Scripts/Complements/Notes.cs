using UnityEngine;

namespace UtilsComplements
{
    [DisallowMultipleComponent]
    public class Notes : MonoBehaviour
    {
        [SerializeField, TextArea(5, 10)] private string _notes = "[Write some notes of interest]";
    }
}

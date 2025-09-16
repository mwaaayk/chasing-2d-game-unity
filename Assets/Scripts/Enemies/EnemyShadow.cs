namespace Driball.Enemies
{
    using UnityEngine;

    public class EnemyShadow : MonoBehaviour
    {
        [SerializeField] private Vector3 offset = new Vector3(0, -0.2f, 0);

        public Transform Caster { get; set; }

        private void Update()
        {
            if (Caster.gameObject.activeSelf)
            {
                transform.position = Caster.transform.position + offset;
                transform.rotation = Caster.transform.rotation;
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}
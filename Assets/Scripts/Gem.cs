namespace Driball
{
    using UnityEngine;

    public class Gem : MonoBehaviour
    {
        [SerializeField] private GameObject collectVFX;

        public int Points;

        public void Collect()
        {
            GameObject vfx = Instantiate(collectVFX);
            vfx.transform.position = transform.position;

            GameEvents.GemCollected(Points);
            gameObject.SetActive(false);
        }
    }
}
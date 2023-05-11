using UnityEngine;
using SimplesDev.TarzanSimulator.Managers;

namespace SimplesDev.TarzanSimulator.Components
{
    public class Blackhole : MonoBehaviour
    {
        GameManager gm;
        private void Start()
        {
            gm = GameManager.Instance;
        }

        private void FixedUpdate()
        {
            this.transform.position = new Vector3(gm.character.position.x,this.transform.position.y,this.transform.position.z);
        }
    }
}
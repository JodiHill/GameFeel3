using UnityEngine;

public class ParticlePlayer : MonoBehaviour
{
    public ParticleSystem ps;
    public Collider2D col;

    public void PlayParticle()
    {
        ps.Play();
        Destroy(col);
        Destroy(this);
    }
}

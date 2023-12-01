using UnityEngine;

public class ParticlePlayer : MonoBehaviour
{
    public ParticleSystem ps;
    public Collider2D col;

    public void PlayParticle()
    {
        ps.Play();
        AudioSource source = GetComponent<AudioSource>();
        if (source != null)
        {
            source.Play();
        }
        Destroy(col);
        Destroy(this);
    }
}

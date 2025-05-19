using UnityEngine;


public class Symbol : MonoBehaviour
{
    public string symbolType;
    public float winMultiplier;
    private Slot slot;
    private bool _flag = true;
    public float score;
    private AudioSource _audioSource;
    public float speed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        slot = GetComponentInParent<Slot>();
    }

    // Update is called once per frame
    private void Update()
    {
        //Debug.Log(this.transform.parent);
        transform.position += Vector3.down * (speed * Time.deltaTime);
        if (transform.position.y <= 1 && _flag)
        {
            slot.Next();
            _audioSource.Play();
            _flag = false;
        }
        else if (transform.position.y < -3.75f)
        {
            Destroy(this.gameObject);
        }
    }
}

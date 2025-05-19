using System.Collections;
using UnityEngine;


public class Slot : MonoBehaviour
{
    [SerializeField] private GameObject[] symbols;
    [SerializeField] private float[] scores;
    private AudioSource _audioSource;
    private int _num;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _num = Random.Range(0, symbols.Length);
        _audioSource = GetComponent<AudioSource>();
        Instantiate(symbols[_num], this.transform, false);
    }

    public void StartSpinning()
    {
        // Удаляем старые символы
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        // Создаём первый символ
        _num = Random.Range(0, symbols.Length);
        Instantiate(symbols[_num], this.transform, false);
    }
    public void Next()
    {
        _num = Random.Range(0, symbols.Length);
        Instantiate(symbols[_num], this.transform,  false);
    }

    public IEnumerator Stopper(Transform centerChild)
    {
        yield return new WaitUntil(() => centerChild.position.y <= 0);
        _audioSource.Play();
        for (int i = 0; i < this.transform.childCount; i++)
        {
            //Debug.Log(this.transform.GetChild(i).position);
            this.transform.GetChild(i).GetComponent<Symbol>().speed = 0;
        }
    }
    public float Stop()
    {
        Transform centerChild = this.transform.GetChild(transform.childCount - 1);
        float score = centerChild.GetComponent<Symbol>().score;
        StartCoroutine(Stopper(centerChild));
        return score;
    }
}

using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public class IARound : MonoBehaviour
{
    [Header("Movment Options")]
    public float speed;
    [Header("Round Options")]
    public GameObject startPosition;
    public GameObject endPosition;
    [Header("Animation Options")]
    public string idelAnimation = "Enemy_Idle";
    public string runAnimation = "Enemy_Runing";
    public string jumplAnimation = "Enemy_Jump";
    [HideInInspector]
    public Vector3 startPositionv3;
    public Vector3 endPositionv3;
    public bool movingToEnd;
    private Vector3 targetPosition;
    
    // Child Sprite Var
    private string spriteChildName = "Sprite";
    private SpriteRenderer SpriteRenderer;
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        startPositionv3 = transform.position;
        endPositionv3 = endPosition.transform.position;
        movingToEnd = true;
        SpriteRenderer = gameObject.transform.Find(spriteChildName).GetComponent<SpriteRenderer>();
        animator = gameObject.transform.Find(spriteChildName).GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Movment();
    }
    public void Movment()
    {
        targetPosition = (movingToEnd) ? endPositionv3 : startPositionv3;
        if (movingToEnd)
        {
            SpriteRenderer.flipX = false;
        }
        else
        {
            SpriteRenderer.flipX = true;
        }
        //
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        if (transform.position == targetPosition)
            movingToEnd = !movingToEnd;
        SpriteAnimate();
    }
    private void SpriteAnimate()
    {

            animator.Play(runAnimation);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().TakeDamge(1);
            Debug.Log("trigger!");
        }
        
    }

}

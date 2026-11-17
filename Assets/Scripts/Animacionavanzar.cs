using UnityEngine;

public class Animacionavanzar : MonoBehaviour
{
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // Detecta si se mantiene apretada la W o la Flecha Arriba
        bool apretandoAdelante = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);

        // Activa o desactiva la animación en el Animator
        anim.SetBool("Avanzando", apretandoAdelante);
    }
}

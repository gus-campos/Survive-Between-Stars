    using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;

public class MineBuilding : MonoBehaviour {

    // Para armazenar script mãe
    private Mine mine;
    // Componente animator
    private Animator animator;

    // Start is called before the first frame update
    void Start() {

        // Armazenar script do objeto mãe
        mine = transform.parent.gameObject.GetComponentInParent<Mine>();
        // Capturar animator
        animator = GetComponent<Animator>();

    }
    
    void Update() {

        // Se acabar a animação, chamar função do script mãe
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f) { mine.EndBuilding(); }
    }
}

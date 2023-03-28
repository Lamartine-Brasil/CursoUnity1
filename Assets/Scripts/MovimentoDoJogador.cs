using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimentoDoJogador : MonoBehaviour
{
    [Header("Referências")]
    private Rigidbody2D oRigidbody2D;
    private Animator oAnimator;

    [Header("Movimento Horizontal")]
    public float velocidadeDoJogador = 9f;
    public bool indoParaDireita;

    [Header("Pulo")]
    public bool estaNoChao;
    public float alturaDoPulo = 20f;
    public float tamanhoDoRaioDeVerificacao = 0.5f;
    public Transform verificadorDeChao;
    public LayerMask layerDoChao;

    [Header("Wall Jump")]
    public bool estaNaParede;
    public bool estaPulandoNaParede;
    public Transform verificadorDeParede;
    public float forcaXDoWallJump = 12f;
    public float forcaYDoWallJump = 10f;
    public float tempoDeWallJump = 0.1f;

    [Header("Desabilitar controle Temporariamente")]
    private bool desativarControle = false;
    public float tempoDeDesativarControle = 0.1f;

    void Awake()
    {
        oRigidbody2D = GetComponent<Rigidbody2D>();
        oAnimator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MovimentarJogador();
        Pular();
        WallJump();
    }

    private void MovimentarJogador()
    {
        if (desativarControle == false)
        {
            float movimentoHorizontal = Input.GetAxis("Horizontal");

            oRigidbody2D.velocity = new Vector2(movimentoHorizontal * velocidadeDoJogador, oRigidbody2D.velocity.y);

            if(movimentoHorizontal > 0)
            {
                transform.localScale = new Vector3(1f, 1f, 1f);
                indoParaDireita = true;
            }
            else if(movimentoHorizontal < 0)
            {
                transform.localScale = new Vector3(-1f, 1f, 1f);
                indoParaDireita = false;
            }

            if(movimentoHorizontal == 0 && estaNoChao == true)
            {
                oAnimator.Play("jogador-idle");
            }
            else if(movimentoHorizontal != 0 && estaNoChao == true &&estaNaParede == false)
            {
                oAnimator.Play("jogador-andando");
            }
        }
    }

    private void Pular()
    {
        estaNoChao = Physics2D.OverlapCircle(verificadorDeChao.position, tamanhoDoRaioDeVerificacao, layerDoChao);

        if(Input.GetButtonDown("Jump") && estaNoChao)
        {
            oRigidbody2D.AddForce(new Vector2(0f, alturaDoPulo), ForceMode2D.Impulse);
        }

        if(estaNoChao == false && estaNaParede == false)
        {
            oAnimator.Play("jogador-pulando");
        }
    }

    private void WallJump()
    {
        estaNaParede = Physics2D.OverlapCircle(verificadorDeParede.position, tamanhoDoRaioDeVerificacao, layerDoChao);

        if(estaNaParede == true && estaNoChao == false && Input.GetAxis("Horizontal") != 0)
        {
            oAnimator.Play("jogador-deslizando-na-parede");
        }

        if(Input.GetButtonDown("Jump") && estaNaParede == true && estaNoChao == false)
        {
            estaPulandoNaParede = true;
        }

        if(estaPulandoNaParede == true)
        {
            if(indoParaDireita == true)
            {
                oRigidbody2D.velocity = new Vector2(-forcaXDoWallJump, forcaYDoWallJump);
            }
            else
            {
                oRigidbody2D.velocity = new Vector2(forcaXDoWallJump, forcaYDoWallJump);
            }

            Invoke(nameof(DeixarEstarPulandoNaParedeComoFalso), tempoDeWallJump);
            Invoke(nameof(travarControle), tempoDeWallJump);
            Invoke(nameof(destravarControle), tempoDeWallJump + tempoDeDesativarControle);
        }
        
    }

    private void DeixarEstarPulandoNaParedeComoFalso()
    {
        estaPulandoNaParede = false;
    }

    private void travarControle()
    {
        desativarControle = true;
    }
    private void destravarControle()
    {
        desativarControle = false;
    }
}

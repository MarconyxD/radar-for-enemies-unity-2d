using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;
using UnityEngine;

public class Enemy_Radar : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject iconPosition;
    private GameObject icon;
    private Renderer rd;
    private Vector3 localIconScreen;

    // Start is called before the first frame update
    void Start()
    {
        rd = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        ScreenIcon();
    }

    private void ScreenIcon()
    {/*
        // Confere dist ncia para definir qual icone utilizar: o de longe ou o de perto
        if (Vector3.Distance(transform.position, player.transform.position) > 17f && Vector3.Distance(transform.position, player.transform.position) <= 25f)
        {
            //iconePosicao.GetComponent<Image>().sprite = iconePosicaoLonge.GetComponent<Image>().sprite;
        }
        if (Vector3.Distance(transform.position, player.transform.position) <= 17f)
        {
            //iconePosicao.GetComponent<Image>().sprite = iconePosicaoPerto.GetComponent<Image>().sprite;
        }*/

        if (rd.isVisible == false) // Verifica se o monstro n o foi renderizado
        {
            if (icon == null && Vector3.Distance(transform.position, player.transform.position) <= 20f) // Verifica se o icone est  ativo e se o monstro est  pr ximo
            {
                icon = Instantiate(iconPosition, new Vector3(0, 0, 0), Quaternion.identity);
                icon.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, false);
            }

            if (icon != null && Vector3.Distance(transform.position, player.transform.position) > 20f)
            {
                Destroy(icon);
            }

            Vector2 direction = player.transform.position - transform.position; // Direção do monstro para o jogador
            int layer_mask = LayerMask.GetMask("CamBox"); // Define a layer do raycast
            RaycastHit2D ray = Physics2D.Raycast(transform.position, direction, 50f, layer_mask); // N o esquecer se setar a layer

            if (ray.collider != null) // Verifica se o raycast encontrou o colisor
            {
                if (ray.collider.tag == "MainCamera" && icon != null) // Verifica a tag do objeto que possui o colisor
                {
                    localIconScreen = Camera.main.WorldToScreenPoint(ray.point); // Converte a coordenada do mundo para coordenada da tela
                    icon.transform.position = localIconScreen; // Define posi  o do icone
                }
            }
        }
        else
        {
            if (icon != null) // Se o monstro   renderizado, desliga o icone
            {
                Destroy(icon);
            }
        }
    }

}

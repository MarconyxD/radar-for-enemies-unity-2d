# English version
[Versão em Português](#Versão-em-Português)

# Creating a radar for on-screen enemies in Unity in a 2D game

The version of Unity used in this project was 2022.3.4f1. The project was created as a 2D project.

In some types of games, it becomes interesting to apply radar mechanics to identify the position and approach of enemies. This becomes a useful tool for the user's decision-making, as they can prepare themselves in some way before the enemy even appears on their screen.

## Proximity icon

To begin, you need to add two GameObjects to the scene: one to represent the player and one to represent the enemy. Also, add a Sprite Renderer to each of the objects, so that it can be rendered on the screen. Use any image to represent the two characters. In my case, I used two black squares. It is important that the characters are rendered because we will use the identification of this rendering on the screen as a condition to identify whether the enemy is being displayed or not.

![01](https://github.com/MarconyxD/radar-for-enemies-unity-2d/assets/71736128/fce9308b-8602-463a-8ab3-5fc1b91d3f25)

Note that I kept one of the objects in the camera area and the other I kept outside.

Then, create a script and name it whatever you want. In this script we will implement the logic for displaying the radar. First, it is necessary to create the variables to identify the player object, the icon (which we will create an Image on Canvas), the Renderer of the object itself and a Vector3 to save the position of the icon on the screen. We will use [SerializeField] so that we can define the parameters through the Inspector but keep them private.

```
[SerializeField] private GameObject player;
[SerializeField] private GameObject iconPosition;
private Renderer rd;
private Vector3 localIconScreen;
```

In the Start() method we want, when starting the application, to define that the Renderer to be used as rd is the one belonging to the object that has the script, so we declare it.

```
void Start()
{
    rd = GetComponent<Renderer>();
}
```

Next, we need to create a method to display the radar icon on the screen, and it must be updated all the time, so this method must be called in Update(). I will name this method ScreenIcon().

```
void Update()
{
    ScreenIcon();
}
```

The ScreenIcon() method will be of type void and its access modifier may be of type private. In it, as this script will be added to the enemy, we start by checking that the enemy is not being rendered on the screen, identifying if rd.isVisible is equal to false.

```
private void ScreenIcon()
{ 
    if (rd.isVisible == false){}
}
```

When checking that the enemy is not being displayed, you must check whether the icon is already being displayed or not. As this method is called in Update() every time, we must realize that its call is constant, so it is interesting to always check the current state of each element. Therefore, it is checked whether the icon is already active and whether the distance between the player and the enemy is less than a certain value. In this case, I put this distance as 20. To determine the distance, Vector3.Distance() is used, which determines the distance between two points based on their locations. In this case, the first point is the enemy's own location and the second point is the player's location. First, check if the icon is NOT active and if the distance IS LESS THAN OR EQUAL TO 20, to activate the icon, if the result is positive. Then, check if the icon IS active and if the distance is greater than 20, to deactivate it, as the enemy is too far away.

```
if (iconPosition.activeSelf == false && Vector3.Distance(transform.position, player.transform.position) <= 20f)
{
    iconPosition.SetActive(true);
}

if (iconPosition.activeSelf == true && Vector3.Distance(transform.position, player.transform.position) > 20f)
{
    iconPosition.SetActive(false);
}
```

This way, we will be able to activate the icon when the enemy approaches and deactivate it when the player or the enemy move away.

However, we still need to add an else to disable the icon when the enemy is being rendered, in other words, it is on the screen. So if the enemy is on the screen being rendered and the icon is active, the icon should be disabled.

```
else
{
    if (iconPosition.activeSelf == true)
    {
        iconPosition.SetActive(false);
    }
}
```

The complete code for the Screen Icon method so far is as follows:

```
private void ScreenIcon()
{
    if (rd.isVisible == false)
    {
        if (iconPosition.activeSelf == false && Vector3.Distance(transform.position, player.transform.position) <= 20f)
        {
            iconPosition.SetActive(true);
        }

        if (iconPosition.activeSelf == true && Vector3.Distance(transform.position, player.transform.position) > 20f)
        {
            iconPosition.SetActive(false);
        }

    else
    {
        if (iconPosition.activeSelf == true
        {
            iconPosition.SetActive(false);
        }
    }
}
```

Don't forget to save your script after making all changes, otherwise the new lines of code will not be executed in Unity!

The next step is to create the Image of the icon in the scene. Add a UI -> Image to the scene. Use an image to represent the icon. In this case, I will use a red circle.

![02](https://github.com/MarconyxD/radar-for-enemies-unity-2d/assets/71736128/aa0f046b-2944-4741-a533-953c18bf077d)

After doing this, disable Image. We don't want the icon to be displayed at the start of the application. Thus, the objects we will have on the scene will be these:

![03](https://github.com/MarconyxD/radar-for-enemies-unity-2d/assets/71736128/e9542f02-5b33-4a36-918c-0e155d24aaf4)

The Canvas and EventSystem are created automatically when creating the Image. I changed the name of my Image to Icon.

Now the next step is to add the script we created as an enemy component and set the remaining parameters in the Inspector.

![04](https://github.com/MarconyxD/radar-for-enemies-unity-2d/assets/71736128/db1db608-572b-41bd-b122-0968a18ca69f)

For the player parameter, drag the scene's player GameObject. To iconPosition, drag the Image with name icon that we just created.

Now, we can test. To perform the test, ensure that the enemy is not appearing in either the Editor or the Player. It cannot be rendered anywhere on the screen. Then press Play and change the enemy's position just with your Transform. Make it approach the player, move beyond the limit defined in the script and appear on the screen, to watch the icon disappear and appear.

https://github.com/MarconyxD/radar-for-enemies-unity-2d/assets/71736128/cb7a2c6a-7c9e-4fed-bc27-e7dc1636f74b

But isn't it interesting that the icon appears in the middle of the screen, right? So, the next step is to make it appear only in the corner of the screen and in the direction from which the enemy is coming.

## Direction and proximity icon

We will continue using the same script, but we will add new lines of code.

Let's add more functions to the if of the ScreenIcon method. After checking whether the icon is active or not, we need to create three new parameters: one for the direction the enemy is in relation to the player, one for a layer_mask that we will use to identify the camera area and another for a Raycast2D that will be used to identify the limits of the camera where the icon will be created.

```
Vector2 direction = player.transform.position - transform.position;
int layer_mask = LayerMask.GetMask("CamBox");
RaycastHit2D ray = Physics2D.Raycast(transform.position, direction, 50f, layer_mask);
```

The direction is calculated by the difference between the player's position and the enemy's position. The layer mask is defined by the layer mask that already exists with the name shown in parentheses, which in this case is “CamBox”. We haven't created this layer mask yet. We will do it next. Raycast2D is like a line that always leaves an origin, points in a direction, has a defined length and belongs to a layer mask. In this case, we defined its origin as the enemy's position, the direction as the previously calculated one, in other words, it will point towards the player, the size was defined as 50, which is an arbitrary value that can be changed, if you want, and the layer mask is the one we defined previously, being “CamBox”.

First, we check if the Raycast is colliding with something. At this point, it won't collide with anything since we haven't added the collider yet. But we want it to collide, so if so, it will check if the collider belongs to an object with the MainCamera tag. If applicable, the point where the collision occurred will be converted to the display screen coordinate and saved in the localIconScreen parameter, which we declared at the beginning but had not yet used. We then update the icon position so that it is equal to that of localIconScreen, in other words, equal to the collision point of the Raycast with the camera collider.

```
if (ray.collider != null)
{
    if (ray.collider.tag == "MainCamera")
    {
        localIconScreen = Camera.main.WorldToScreenPoint(ray.point);
        iconPosition.transform.position = localIconScreen;
    }
}
```

This way, we have the code completely finished. The complete code can be seen below:

```
using UnityEngine;

public class Enemy_Radar : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject iconPosition;
    private Renderer rd;
    private Vector3 localIconScreen;

    void Start()
    {
        rd = GetComponent<Renderer>();
    }

    void Update()
    {
        ScreenIcon();
    }

    private void ScreenIcon()
    {
        if (rd.isVisible == false)
        {
            if (iconPosition.activeSelf == false && Vector3.Distance(transform.position, player.transform.position) <= 20f) 
            {
                iconPosition.SetActive(true);
            }

            if (iconPosition.activeSelf == true && Vector3.Distance(transform.position, player.transform.position) > 20f)
            {
                iconPosition.SetActive(false);
            }

            Vector2 direction = player.transform.position - transform.position; 
            int layer_mask = LayerMask.GetMask("CamBox");             RaycastHit2D ray = Physics2D.Raycast(transform.position, direction, 50f, layer_mask); 

            if (ray.collider != null)
            {
                if (ray.collider.tag == "MainCamera")
                {
                    localIconScreen = Camera.main.WorldToScreenPoint(ray.point);
                    iconPosition.transform.position = localIconScreen;
                }
            }
        }
        else
        {
            if (iconPosition.activeSelf == true)
            {
                iconPosition.SetActive(false);
            }
        }
    }

}
```

Don't forget to save your script after making all changes, otherwise the new lines of code will not be executed in Unity!

Now, we must make some configurations in Unity. First, in MainCamera, change the Layer that is in Default by creating a new one with the name CamBox and selecting it. Next, you need to add a 2D Box Collider to the camera. Add this collider and define where you want the edges of it to be. It is interesting that they are set back a little from the screen so that the icon can be fully displayed. The green lines define the collision area and the white lines define the display area.

![05](https://github.com/MarconyxD/radar-for-enemies-unity-2d/assets/71736128/4cfd05b7-54e5-45bc-bb11-5cbd2a15640b)

Finally, it is necessary to define that this collider does not interact with the other colliders in the scene because, when we have a complete scene with several elements, it is not interesting for the camera to collide with them.

To do this, just go to Edit -> Project Setting -> Physics 2D -> Layer Collision Matrix and uncheck the CamBox interaction with Default. Thus, there will be no interaction between the camera collider and the other colliders that are in the Default mask.

![06](https://github.com/MarconyxD/radar-for-enemies-unity-2d/assets/71736128/f51db9c8-0897-490a-859e-3f920a3229e5)

So, just test to check the result.

https://github.com/MarconyxD/radar-for-enemies-unity-2d/assets/71736128/6b622f00-8e35-4505-8602-1b413596f1d3

If you want a more precise identification, it is interesting to check the position of the pivot and reference points of the enemy and the icon as well, so that they are correctly centered.

## Icon for various enemies

The way we did it, the icon only works for a specific enemy. If we want each enemy to have a radar icon, one option is to create an image on Canvas for each of them. This is problematic and tiring, as we can have dozens of enemies in our game, or even infinite enemies, spawning all the time.

To solve this problem, we will use Prefabs. Prefabs are objects that we create in our environment, define their characteristics, components and parameters and save them as “standard objects”. Thus, we can replicate this object infinitely many times, whenever necessary, and destroy it when we no longer need it, making it easier to produce the same object in the scene. For example, if we have a cannon that fires several projectiles and they explode when they come into contact with something, we can produce a Prefab of this projectile and, whenever the cannon fires, we instantiate a new projectile, creating it in the scene, and destroy it like this to touch something. We repeat this process infinitely many times, so that we will always have just one projectile in the scene, preventing us from having dozens or hundreds of the same object. Prefabs serve both to organize the scene and to reduce processing.

So how do we create a Prefab? Just drag the object to your Project folders. This way, we will have the Prefab ready.

So let's start by making the icon enabled in the scene and turning it into Prefab. Then, we will delete the icon on the screen and update the iconPosition parameter in the enemy components, placing the Prefab we created as its value.

https://github.com/MarconyxD/radar-for-enemies-unity-2d/assets/71736128/ba3741ad-476e-4186-9bfc-8bdcccfae0fe

Now, we need to make some changes to the code. The first is to add a new parameter at the beginning of the code. This parameter will represent the Prefab GameObject that will be instantiated.

```
private GameObject icon;
```

Next, we need to replace the if that checks if the icon is active and the if that checks if the icon is disabled. First, we will now check whether the icon is instantiated or not, rather than activated. Then, instead of activating it, let's instantiate it. When instantiating it, we are creating it in the scene. And instead of deactivating it, we're going to destroy it, to prevent it from taking up space in the scene unnecessarily. We will use null to check if the icon object we created is empty or if it has an object defined to it. This will be enough to determine whether the icon exists or not.

```
if (icon == null && Vector3.Distance(transform.position, player.transform.position) <= 20f)
{
    icon = Instantiate(iconPosition, new Vector3(0, 0, 0), Quaternion.identity);
    icon.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, false);
}

if (icon != null && Vector3.Distance(transform.position, player.transform.position) > 20f)
{
    Destroy(icon);
}
```

We define that the new GameObject icon will receive the instance of the icon that will be created. To instantiate it, we first define the object we want to be instantiated, which in this case is the iconPosition object. This object has the Prefab we created, as we just did. Then, we define its position as the origin and its rotation as the rotation that the object already has. In the next line we define that it will be the child of an object that is in the scene and that has the “Canvas” tag. UI objects must be children of Canvas, so we need to define this. To do this, let's go back to Unity and create a tag called “Canvas” and place it on the Canvas object.

![07](https://github.com/MarconyxD/radar-for-enemies-unity-2d/assets/71736128/2018a38d-d2f0-4386-a2c2-dbf7495d1e06)

Continuing with the script, we need to add a new condition to the if that checks the tag of the object that the Raycast is colliding with. We need it to check that icon is not null, because if we do any action to an object that is null, we will have an error in our game. So, to avoid the error, we perform this check. The actions after the condition remain the same.

```
if (ray.collider.tag == "MainCamera" && icon != null)
{
    localIconScreen = Camera.main.WorldToScreenPoint(ray.point);
    icon.transform.position = localIconScreen; 
}
```

Finally, at the end, we have the else that serves to identify whether the enemy has been rendered. We want if the icon exists, it will be destroyed when the enemy is rendered.

```
else
{
    if (icon != null)
    {
        Destroy(icon);
    }
}
```

Thus, the new complete code will be:

```
using UnityEngine;

public class Enemy_Radar : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject iconPosition;
    private GameObject icon;
    private Renderer rd;
    private Vector3 localIconScreen;

    void Start()
    {
        rd = GetComponent<Renderer>();
    }
    void Update()
    {
        ScreenIcon();
    }

    private void ScreenIcon()
    {
        if (rd.isVisible == false) 
        {
            if (icon == null && Vector3.Distance(transform.position, player.transform.position) <= 20f) 
            {
                icon = Instantiate(iconPosition, new Vector3(0, 0, 0), Quaternion.identity);
                icon.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, false);
            }

            if (icon != null && Vector3.Distance(transform.position, player.transform.position) > 20f)
            {
                Destroy(icon);
            }

            Vector2 direction = player.transform.position - transform.position; 
            int layer_mask = LayerMask.GetMask("CamBox"); 
            RaycastHit2D ray = Physics2D.Raycast(transform.position, direction, 50f, layer_mask); 

            if (ray.collider != null)
            {
                if (ray.collider.tag == "MainCamera" && icon != null)
                {
                    localIconScreen = Camera.main.WorldToScreenPoint(ray.point);
                    icon.transform.position = localIconScreen;
                }
            }
        }
        else
        {
            if (icon != null)
            {
                Destroy(icon);
            }
        }
    }

}
```

We can test with one enemy and add several enemies to make sure everything is working correctly.

https://github.com/MarconyxD/radar-for-enemies-unity-2d/assets/71736128/a4d6e2e5-a279-442f-87d2-04831c1a375f


# Versão em Português
[English version](#English-version)

# Criando um radar para inimigos na tela na Unity em um jogo 2D

A versão da Unity utilizada neste projeto foi a 2022.3.4f1. O projeto foi criado como sendo um projeto 2D.

Em alguns tipos de jogos se torna interessante a aplicação da mecânica de radar para a identificação da posição e aproximação dos inimigos. Esta se torna uma ferramenta útil para a tomada de decisões do usuário, uma vez que o mesmo possa se preparar de alguma forma antes mesmo do inimigo aparecer em sua tela.

## Ícone de proximidade

Para começar, é necessário adicionar dois GameObject à cena, um para representar o jogador e outro para representar o inimigo. Adicione também um Sprite Renderer a cada um dos objetos, para que o mesmo possa ser renderizado na tela. Utilize qualquer imagem para a representação dos dois personagens. No meu caso, utilizei dois quadrados na cor preta. É importante que os personagens sejam renderizados, pois iremos utilizar a identificação desta renderização na tela como condição para identificar se o inimigo está sendo exibido ou não.

![01](https://github.com/MarconyxD/radar-for-enemies-unity-2d/assets/71736128/fce9308b-8602-463a-8ab3-5fc1b91d3f25)

Observe que mantive um dos objetos na área da câmera e o outro não.

Em seguida, crie um script e nomeie-o da forma que desejar. Neste script iremos implementar a lógica para a exibição do radar. Primeiro, é necessário criar as variáveis para identificar o objeto do jogador, o ícone (que iremos criar uma Imagem no Canvas), o Renderer do próprio objeto e um Vector3 para salvarmos a posição do ícone na tela. Utilizaremos [SerializeField] para que possamos definir os parâmetros pelo Inspector mas mantendo-os como private.

```
[SerializeField] private GameObject player;
[SerializeField] private GameObject iconPosition;
private Renderer rd;
private Vector3 localIconScreen;
```

No método Start() queremos que, ao iniciar a aplicação, seja definido que o Renderer nomeado como rd é pertencente ao objeto que possuir o script, então, realizamos sua declaração.

```
void Start()
{
    rd = GetComponent<Renderer>();
}
```

A seguir, precisamos criar um método para exibir o ícone do radar na tela, e ele deve ser atualizado a todo o momento, então este método deve ser chamado em Update(). Irei dar o nome de ScreenIcon() para este método.

```
void Update()
{
    ScreenIcon();
}
```

O método ScreenIcon() será do tipo void e seu modificador de acesso pode ser do tipo private. Nele, como este script será adicionado ao inimigo, começamos verificando se o inimigo não está sendo renderizado na tela, identificando se rd.isVisible é igual a false.

```
private void ScreenIcon()
{ 
    if (rd.isVisible == false){}
}
```

Ao verificar que o inimigo não está sendo exibido, deve-se verificar se o ícone já está sendo exibido ou não. Como este método é chamado no Update() a cada momento, devemos levar em consideração sua chamada constante, por isso é interessante sempre verificar o atual estado de cada elemento. Portanto, verifica-se se o ícone já está ativo e se a distância entre o jogador e o inimigo é menor que determinado valor. Neste caso, coloquei esta distância como sendo 20. Para determinar a distância utiliza-se Vector3.Distance(), que determina a distância entre dois pontos com base em suas localizações. Neste caso, o primeiro ponto é a localização do próprio inimigo e o segundo ponto é a localização do jogador. Primeiro, verifica-se se o ícone NÃO está ativo e se a distância É MENOR OU IGUAL a 20, para ativar o ícone, em caso positivo. Depois, verifica-se se o ícone ESTÁ ativo e se a distância é maior que 20, para desativá-lo, pois o inimigo estar muito longe.

```
if (iconPosition.activeSelf == false && Vector3.Distance(transform.position, player.transform.position) <= 20f)
{
    iconPosition.SetActive(true);
}

if (iconPosition.activeSelf == true && Vector3.Distance(transform.position, player.transform.position) > 20f)
{
    iconPosition.SetActive(false);
}
```

Assim, conseguimos ativar o ícone quando o inimigo se aproximar e desativá-lo quando o jogador e/ou o inimigo se afastarem.

Porém, ainda precisamos adicionar um else para desativar o ícone quando o inimimo estiver sendo renderizado, ou seja, estiver na tela. Então, se o inimigo estiver na tela sendo renderizado e o ícone estiver ativo, o ícone deve ser desativado.

```
else
{
    if (iconPosition.activeSelf == true)
    {
        iconPosition.SetActive(false);
    }
}
```

O código completo do método Screen Icon, até agora, é o seguinte:

```
private void ScreenIcon()
{
    if (rd.isVisible == false)
    {
        if (iconPosition.activeSelf == false && Vector3.Distance(transform.position, player.transform.position) <= 20f)
        {
            iconPosition.SetActive(true);
        }

        if (iconPosition.activeSelf == true && Vector3.Distance(transform.position, player.transform.position) > 20f)
        {
            iconPosition.SetActive(false);
        }

    else
    {
        if (iconPosition.activeSelf == true
        {
            iconPosition.SetActive(false);
        }
    }
}
```

Não se esqueça de salvar seu script após realizar todas as alterações, senão as novas linhas de código não serão executadas na Unity!

O próximo passo é criar a Image do ícone na cena. Adicione um UI -> Image à cena. Utilize alguma imagem para representar o ícone. Neste caso, irei utilizar um círculo vermelho. 

![02](https://github.com/MarconyxD/radar-for-enemies-unity-2d/assets/71736128/aa0f046b-2944-4741-a533-953c18bf077d)

Após fazer isso, desabilite a Image, para que o jogo seja iniciado sem o ícone aparecer. Assim, os objetos que teremos em cena serão estes:

![03](https://github.com/MarconyxD/radar-for-enemies-unity-2d/assets/71736128/e9542f02-5b33-4a36-918c-0e155d24aaf4)

O Canvas e o EventSystem são criados automaticamente ao criar a Image. Alterei o nome da minha Image para Icon.

Agora, o próximo passo é adicionar o script que criamos como componente do inimigo e definir os parâmetros restantes no Inspector.

![04](https://github.com/MarconyxD/radar-for-enemies-unity-2d/assets/71736128/db1db608-572b-41bd-b122-0968a18ca69f)

Para o parâmetro player, arraste o GameObject do player da cena. Para iconPosition, arraste a Image com nome icon que acabamos de criar.

Agora, podemos testar. Para realizar o teste, garanta que o inimigo não esteja aparecendo nem no Editor e nem no Player. Ele não pode ser renderizado em nenhuma parte da tela. Em seguida, dê Play e altere a posição do inimigo apenas pelo seu Transform. Faça-o se aproximar do jogador, se distanciar além do limite definido no script e aparecer na tela, para observar o ícone sumindo e aparecendo.

https://github.com/MarconyxD/radar-for-enemies-unity-2d/assets/71736128/cb7a2c6a-7c9e-4fed-bc27-e7dc1636f74b

Mas não é interessante que o ícone apareça no meio da tela, correto? Então, o próximo passo é fazermos com que ele apareça apenas no canto da tela e na direção de onde o inimigo estiver vindo.

## Direction and proximity icon

Iremos continuar utilizando o mesmo script, mas iremos adicionar novas linhas de código.

Vamos adicionar mais funções ao if do método ScreenIcon. Após as duas verificações do ícone estar ou não estar ativo, precisamos criar três novos parâmetros: um para a direção que o inimigo está com relação ao jogador, um para uma layer_mask que iremos utilizar para identificar a área da câmera e outro para um Raycast2D que será utilizado para identificar os limites da câmera onde o ícone será criado.

```
Vector2 direction = player.transform.position - transform.position;
int layer_mask = LayerMask.GetMask("CamBox");
RaycastHit2D ray = Physics2D.Raycast(transform.position, direction, 50f, layer_mask);
```

A direção é calculada pela diferença entre a posição do jogador e a posição do inimigo. A layer mask é definida pela layer mask que já existir com o nome apresentado entre parênteses, que no caso é “CamBox”. Ainda não criamos esta layer mask. Iremos fazer isto logo em seguida. Já o Raycast2D é como uma linha que sempre sai de uma origem, aponta em uma direção, tem um comprimento definido e pertence a uma layer mask. Neste caso, definimos sua origem como sendo o inimigo, a direção sendo a calculada anteriormente, ou seja, ela irá apontar para o jogador, o tamanho foi definido como sendo 50, que é um valor arbitrário que pode ser alterado, caso desejado, e a layer mask é a que definimos anteriormente, sendo a “CamBox”.

Primeiros, verificamos se o Raycast está colidindo com algo. Neste momento, ele não irá colidir com nada, pois ainda não adicionamos o colisor. Mas queremos que ele colida, então, em caso afirmativo, ele irá verificar se o colisor pertence a um objeto com a tag MainCamera. Caso pertencer, o ponto onde a colisão aconteceu será convertido para a coordenada da tela de exibição e salvo no parâmetro localIconScreen, que declaramos lá no começo mas ainda não tínhamos utilizado. Em seguida, atualizamos a posição do ícone para que seja igual à de localIconScreen, ou seja, igual ao ponto de colisão do Raycast com o colisor da câmera.

```
if (ray.collider != null)
{
    if (ray.collider.tag == "MainCamera")
    {
        localIconScreen = Camera.main.WorldToScreenPoint(ray.point);
        iconPosition.transform.position = localIconScreen;
    }
}
```

Desta forma, temos o código completamente finalizado. O código completo pode ser visto abaixo:

```
using UnityEngine;

public class Enemy_Radar : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject iconPosition;
    private Renderer rd;
    private Vector3 localIconScreen;

    void Start()
    {
        rd = GetComponent<Renderer>();
    }

    void Update()
    {
        ScreenIcon();
    }

    private void ScreenIcon()
    {
        if (rd.isVisible == false)
        {
            if (iconPosition.activeSelf == false && Vector3.Distance(transform.position, player.transform.position) <= 20f) 
            {
                iconPosition.SetActive(true);
            }

            if (iconPosition.activeSelf == true && Vector3.Distance(transform.position, player.transform.position) > 20f)
            {
                iconPosition.SetActive(false);
            }

            Vector2 direction = player.transform.position - transform.position; 
            int layer_mask = LayerMask.GetMask("CamBox");             RaycastHit2D ray = Physics2D.Raycast(transform.position, direction, 50f, layer_mask); 

            if (ray.collider != null)
            {
                if (ray.collider.tag == "MainCamera")
                {
                    localIconScreen = Camera.main.WorldToScreenPoint(ray.point);
                    iconPosition.transform.position = localIconScreen;
                }
            }
        }
        else
        {
            if (iconPosition.activeSelf == true)
            {
                iconPosition.SetActive(false);
            }
        }
    }

}
```

Não se esqueça de salvar seu script após realizar todas as alterações, senão as novas linhas de código não serão executadas na Unity!

Agora, devemos fazer algumas configurações na Unity. Primeiro, na MainCamera, altere a Layer que está em Default criando uma nova com o nome CamBox e selecione-a. Em seguida, é necessário adicionar um Box Collider 2D à câmera. Adicione este colisor e defina onde quer que as bordas dele estejam. É interessante que as bordas estejam um pouco recuadas com relação à tela para que o ícone possa ser totalmente exibido. As linhas verdes definem a área de colisão e as brancas definem a área de exibição.

![05](https://github.com/MarconyxD/radar-for-enemies-unity-2d/assets/71736128/4cfd05b7-54e5-45bc-bb11-5cbd2a15640b)

Por fim, é necessário definir que este colisor não atue com os demais colisores do cenário pois, ao termos uma cena completa com vários elementos, não é interessante que a câmera choque com eles.

Para isto, basta ir em Edit -> Project Setting -> Physics 2D -> Layer Collision Matrix e desmarcar a interação da CamBox com o Default. Assim, não haverá interação entre o colisor da câmera e os outros colisores que estiverem na mask Default.

![06](https://github.com/MarconyxD/radar-for-enemies-unity-2d/assets/71736128/f51db9c8-0897-490a-859e-3f920a3229e5)

Assim, basta testar para verificar o resultado.

https://github.com/MarconyxD/radar-for-enemies-unity-2d/assets/71736128/6b622f00-8e35-4505-8602-1b413596f1d3

Caso desejar uma identificação mais precisa, é interessante conferir a posição dos pontos de pivot e referência do inimigo e do ícone também, para que fiquem corretamente centralizados.

## Ícone para vários inimigos

Da forma que fizemos, o ícone funciona apenas para um inimigo específico. Se quisermos que cada inimigo possua um ícone de radar, uma opção é criar uma imagem no Canvas para cada um deles. Isso é algo problemático e cansativo, pois podemos ter dezenas de inimigos em nosso jogo, ou até mesmo inimigos infinitos, spawnando a todo o momento.

Para resolver este problema, iremos utilizar de Prefabs. Prefabs são objetos que criamos em nossa cena, definimos suas características, componentes e parâmetros e salvamos eles como “objetos padrão”. Assim, podemos replicar este objeto infinita vezes, sempre que necessário, e destruí-lo quando não precisarmos mais, facilitando a produção de um mesmo objeto na cena. Por exemplo, se temos um canhão que dispara várias projéteis e eles explodem quando entram em contato com algo, podemos produzir um Prefab deste projétil e, sempre que o canhão disparar, instanciamos um novo projétil, criando-o na cena, e o destruímos assim que tocar em algo. Repetimos este processo infinitas vezes, de forma que teremos sempre apenas um projétil em cena, evitando que tenhamos dezenas ou centenas de um mesmo objeto. Os Prefabs servem tanto para organização da cena como para redução de processamento.

Então, como criamos um Prefab? Basta arrastar o objeto para suas pastas de Project. Assim, teremos o Prefab pronto.

Então, vamos começar habilitando o ícone presente na cena e transformando-o em Prefab. Depois, vamos apagar o ícone na tela e atualizar nos componentes do inimigo o parâmetro iconPosition, colocando o Prefab que criamos como sendo seu valor.

https://github.com/MarconyxD/radar-for-enemies-unity-2d/assets/71736128/ba3741ad-476e-4186-9bfc-8bdcccfae0fe

Agora, precisamos realizar algumas alterações no código. A primeira delas é adicionar um novo parâmetro no começo do código. Este parâmetro irá representar o GameObject do Prefab que será instanciado.

```
private GameObject icon;
```

Em seguida, precisamos substituir o if que verifica se o ícone está ativo e o if que verifica se o ícone está desativado. Primeiro, que agora iremos verificar se o ícone está instanciado ou não, ao invés de ativado. Em seguida, ao invés de ativá-lo, vamos instanciá-lo. Ao instanciá-lo, estamos o criando na cena. E ao invés de desativá-lo, vamos destruí-lo, para evitar que ocupe espaço na cena sem ser necessário. Iremos utilizar null para verificar se o objeto icon que criamos está vazio ou se possui algum objeto definido a ele. Isso será o bastante para determinar se o ícone existe ou não.

```
if (icon == null && Vector3.Distance(transform.position, player.transform.position) <= 20f)
{
    icon = Instantiate(iconPosition, new Vector3(0, 0, 0), Quaternion.identity);
    icon.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, false);
}

if (icon != null && Vector3.Distance(transform.position, player.transform.position) > 20f)
{
    Destroy(icon);
}
```

Definimos que o novo GameObject icon irá receber a instância do ícone que será criado. Para instanciá-lo, definimos primeiro o objeto que queremos que seja instanciado, que no caso, é o objeto iconPosition. Este objeto possui o Prefab que criamos, conforme acabamos de fazer. Em seguida, definimos sua posição como sendo a origem e sua rotação como sendo a rotação que o objeto já possuir. Na linha seguinte definimos que ele será filho de um objeto que estiver em cena e que possuir a tag “Canvas”. Objetos de UI devem ser filhos do Canvas, então, precisamos definir isso. Para tal, vamos voltar à Unity e criar uma tag chamada “Canvas” e coloca-la no objeto Canvas.

![07](https://github.com/MarconyxD/radar-for-enemies-unity-2d/assets/71736128/2018a38d-d2f0-4386-a2c2-dbf7495d1e06)

Continuando com o script, precisamos adicionar uma nova condição ao if que verifica a tag do objeto ao qual o Raycast está colidindo. Precisamos que ele verifique se icon não é null, pois, se fizermos qualquer ação a um objeto que é null, teremos um erro no nosso jogo. Então, para evitar o erro, realizamos esta verificação. As ações após a condição continuam as mesmas.

```
if (ray.collider.tag == "MainCamera" && icon != null)
{
    localIconScreen = Camera.main.WorldToScreenPoint(ray.point);
    icon.transform.position = localIconScreen; 
}
```

Por fim, ao final, temos o else que serve para identificar se o inimigo foi renderizado. Queremos que, caso o ícone existir, ele seja destruído quando o inimigo for renderizado.

```
else
{
    if (icon != null)
    {
        Destroy(icon);
    }
}
```

Assim, o novo código completo será:

```
using UnityEngine;

public class Enemy_Radar : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject iconPosition;
    private GameObject icon;
    private Renderer rd;
    private Vector3 localIconScreen;

    void Start()
    {
        rd = GetComponent<Renderer>();
    }
    void Update()
    {
        ScreenIcon();
    }

    private void ScreenIcon()
    {
        if (rd.isVisible == false) 
        {
            if (icon == null && Vector3.Distance(transform.position, player.transform.position) <= 20f) 
            {
                icon = Instantiate(iconPosition, new Vector3(0, 0, 0), Quaternion.identity);
                icon.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, false);
            }

            if (icon != null && Vector3.Distance(transform.position, player.transform.position) > 20f)
            {
                Destroy(icon);
            }

            Vector2 direction = player.transform.position - transform.position; 
            int layer_mask = LayerMask.GetMask("CamBox"); 
            RaycastHit2D ray = Physics2D.Raycast(transform.position, direction, 50f, layer_mask); 

            if (ray.collider != null)
            {
                if (ray.collider.tag == "MainCamera" && icon != null)
                {
                    localIconScreen = Camera.main.WorldToScreenPoint(ray.point);
                    icon.transform.position = localIconScreen;
                }
            }
        }
        else
        {
            if (icon != null)
            {
                Destroy(icon);
            }
        }
    }

}
```

Podemos testar com um inimigo e adicionar diversos inimigos, duplicando o original, para observar que está tudo funcionando corretamente.

https://github.com/MarconyxD/radar-for-enemies-unity-2d/assets/71736128/a4d6e2e5-a279-442f-87d2-04831c1a375f

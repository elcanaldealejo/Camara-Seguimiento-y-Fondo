using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Camera2D : MonoBehaviour
{

[Header ("Settings")]
[SerializeField] private bool horizontalFollow = true;
[SerializeField] private bool verticalFollow = true;

[Header("Horizontal")]
[SerializeField] [Range( 0 , 1 )] private float horizontalInfluence = 1f;//Rango de seguimiento en la horizontal
[SerializeField] private float horizontalOffset = 0f; // Distancia de persecucion dentro de la camara en X
[SerializeField] private float horizontalSmoothness = 3f;// suavidad con que alcanza al objetivo a seguir en X
[SerializeField] private float minPOSx= 0f; //Posicion MINIMA en X
[SerializeField] private float maxPOSx= 150f; //Posicion MAXIMA en X
[SerializeField] private float maxPOSy= 4f;//Posicion MINIMA en Y

[Header("Vertical")]
[SerializeField] [Range(0 , 1)] private float verticalInfluence = 1f;//Rango de seguimiento en la Vertical
[SerializeField] private float verticalOffset = 0f;// Distancia de persecucion dentro de la camara en Y
[SerializeField] private float verticalSmoothness = 3f;// suavidad con que alcanza al objetivo a seguir en Y

private GameObject Target;
public Vector3 TargetPosition {get; set;}
public Vector3 CameraTargetPosition {get; set;}

private float _targetHorizontalSmoothFollow;
private float _targetVerticalSmoothFollow;

//Podemos crear el numero que se desee de fondos y ajustarles una velocidad movimiento
//al tiempo que el juegador se desplaza
public Transform FondoLejos, FondoMedio, FondoCerca;
private float lastXPos;

void Start(){
    Target = GameObject.FindWithTag("Player");//El nombre de Tag que tenga nuestro personaje a seguir
    lastXPos = transform.position.x;
}
private void Update(){    
    MoveCamera();
    float amountToMoveX = transform.position.x - lastXPos;
    //El fondo lejos no deja al jugador, esto se traduce que no se deja atras.
    FondoLejos.position = FondoLejos.position + new Vector3(amountToMoveX,0f,0f);
    //El fondo medio se desplazará mientras el jugador siga avanzando, pero MUY lento.
    FondoMedio.position += new Vector3(amountToMoveX * .9f, 0f,0f);
    //Los fondos cercanos deben desplazarse más rapido pero no tanto como el jugador.
    FondoCerca.position += new Vector3(amountToMoveX * .98f, 0f,0f);
    
    //Otros fondos adicionales que esten en el entorno del jugador(Muy cerca) durante la escena 
    //...se desplazarán a la misma velocidad que se mueva el jugador.
    
    lastXPos = transform.position.x;
   

}
private void MoveCamera(){
    if(Target == null){
        return;
    }
    TargetPosition = GetTargetPosition(Target);
    
    CameraTargetPosition = new Vector3(TargetPosition.x, TargetPosition.y, 0f); 
    // seguir el axis elegido
    float xPos = horizontalFollow ? CameraTargetPosition.x : transform.localPosition.x;
    float yPos = verticalFollow ? CameraTargetPosition.y : transform.localPosition.y;
    // se establece
    CameraTargetPosition += new Vector3(horizontalFollow ? horizontalOffset : 0f, verticalFollow ? verticalOffset : 0f, 0f );

    // Establecer valor del suavizado
    _targetHorizontalSmoothFollow = Mathf.Lerp(_targetHorizontalSmoothFollow, CameraTargetPosition.x, horizontalSmoothness * Time.deltaTime);
    _targetVerticalSmoothFollow = Mathf.Lerp(_targetVerticalSmoothFollow, CameraTargetPosition.y, verticalSmoothness * Time.deltaTime);
 
    //Obtener dirección hacia la posición del jugador
    float xDirection = _targetHorizontalSmoothFollow - transform.localPosition.x;
    float yDirection = _targetVerticalSmoothFollow - transform.localPosition.y;
    Vector3 deltaDireccion = new Vector3(xDirection, yDirection, 0f);
    // Nueva posicion
    Vector3 newCameraPosition = transform.localPosition + deltaDireccion;
    if(newCameraPosition.y>maxPOSy)
        newCameraPosition = new Vector3(newCameraPosition.x,maxPOSy,newCameraPosition.z);
    if(newCameraPosition.x<minPOSx)
        newCameraPosition = new Vector3(minPOSx,newCameraPosition.y,newCameraPosition.z);
    if(newCameraPosition.x>maxPOSx)
        newCameraPosition = new Vector3(maxPOSx,newCameraPosition.y,newCameraPosition.z);    
   
    //Aplicar nueva posicion
    transform.localPosition = new Vector3(newCameraPosition.x, newCameraPosition.y, transform.localPosition.z);

}
private Vector3 GetTargetPosition(GameObject player){
    float xPos = 0f;
    float yPos = 0f;
    xPos += (player.transform.position.x + horizontalOffset) * horizontalInfluence;
    yPos += (player.transform.position.y + verticalOffset) * verticalInfluence;
    Vector3 positionTarget = new Vector3(xPos, yPos, transform.position.z);
    return positionTarget;
}

private void CenterOnTarget(GameObject player){
    
    Target = player;
    Vector3 targetPos = GetTargetPosition(Target);
    _targetHorizontalSmoothFollow = targetPos.x;
    _targetVerticalSmoothFollow = targetPos.y;
    transform.localPosition = targetPos;
}

private void StopFollow(GameObject player){
Target = null;
}


private void StartFollowing(GameObject player){
    Target = player;
    CenterOnTarget(Target);
}

private void OnDrawGizmos(){
    Gizmos.color = Color.red;
    Vector3 camPosition = new Vector3 (transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + 2f);
    Gizmos.DrawWireSphere(camPosition, 0.5f);
}

private void OnEnable(){
   
    
}
private void OnDisable(){
  
}

}

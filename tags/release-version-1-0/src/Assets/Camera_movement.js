#pragma strict

function Start () {

}
var speed = 30;
function Update ()  {
    var x = Input.GetAxis("Horizontal") * Time.deltaTime * speed;
    var y = Input.GetAxis("Vertical") * Time.deltaTime * speed;
    transform.Translate(x, y, 0);
    
}
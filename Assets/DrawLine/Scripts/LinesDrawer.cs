using UnityEngine;

public class LinesDrawer : MonoBehaviour {

	public GameObject linePrefab;
	public LayerMask cantDrawOverLayer;

	[Space ( 30f )]
	public Gradient lineColor;
	public float linePointsMinDistance;
	public float lineWidth;
	public CameraControl cameraControl;
	Line currentLine;
	public bool Enable = false,UsePhysics=false;
	Camera cam;
	public RectTransform rectTransform;

	void Start ( ) {
		cam = Camera.main;
    }

	public void SelfUpdate ( ) {
        if (Enable)
		{
			if ( Input.GetMouseButtonDown ( 0 ) )
				BeginDraw ( );

		
		}
        if (currentLine != null)
        {
            Draw();
        }

        if (Input.GetMouseButtonUp(0))
        {
            EndDraw();
            cameraControl.followPosition = false;
        }
    }

	// Begin Draw ----------------------------------------------
	public void BeginDraw ( ) {
		currentLine = Instantiate ( linePrefab, this.transform ).GetComponent <Line> ( );
		currentLine.EnablePhysics = UsePhysics;
		//Set line properties
		currentLine.UsePhysics ( false );
		currentLine.SetLineColor ( lineColor );
		currentLine.SetPointsMinDistance ( linePointsMinDistance );
		currentLine.SetLineWidth ( lineWidth );

	}
	public void AddPoint(Vector2 point)
	{
		currentLine.AddPoint(point);
	}
	// Draw ----------------------------------------------------
	void Draw ( ) {
		Vector2 mousePosition = cam.ScreenToWorldPoint ( Input.mousePosition );

		//Check if mousePos hits any collider with layer "CantDrawOver", if true cut the line by calling EndDraw( )
		RaycastHit2D hit = Physics2D.CircleCast ( mousePosition, lineWidth / 3f, Vector2.zero, 1f, cantDrawOverLayer );
		if ( hit )
		{
            EndDraw();
        }
		else
		{
            

			if (Enable)
			{
                currentLine.AddPoint(mousePosition + new Vector2(1, -2f));
                cameraControl.FollowPosition(mousePosition);
            }
            
        }
	}
	// End Draw ------------------------------------------------
	public void EndDraw ( ) {
		if ( currentLine != null ) {
			if ( currentLine.pointsCount < 2||!Enable ) {
				//If line has one point
				Destroy ( currentLine.gameObject );
			} else {
				//Add the line to "CantDrawOver" layer
				//currentLine.gameObject.layer = cantDrawOverLayerIndex;

				//Activate Physics on the line
				currentLine.UsePhysics ( UsePhysics );

				currentLine = null;
			}
		}
	}
}

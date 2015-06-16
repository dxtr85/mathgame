/**
*
*
*	MouseGesture
*	
*	@notice		Mouse Gesture Recognizer
*	@author		Didier Brun
*	@version	1.0
* 	@date		2007-05-17
* 	@link		http://www.bytearray.org/?p=91
* 
* 
*	Original author :
*	-----------------
*	Didier Brun aka Foxy
*	webmaster@foxaweb.com
*	http://www.foxaweb.com
*
* 	AUTHOR ******************************************************************************
* 
*	authorName : 	Didier Brun - www.foxaweb.com
* 	contribution : 	the original class
* 	date :			2007-01-18
* 
* 	VISIT www.byteArray.org
* 
* 
*   Initial adjustment to Unity3D in C# - Jakub Mitoraj
*
*
*	LICENSE ******************************************************************************
* 
* 	This class is under RECIPROCAL PUBLIC LICENSE.
* 	http://www.opensource.org/licenses/rpl.php
* 
* 	Please, keep this header and the list of all authors
* 
* 
*
*/

using UnityEngine;
using System.Collections;
using System.Timers;

public class MouseController : MonoBehaviour {

	public bool gestureCapturingEnabled = true;		// By default you can capture gestures
	public bool drawingEnabled = true;
	public Texture2D textureForGesture;

	//public GUIText capturedValue;
	public static uint DEFAULT_NB_SECTORS=8; 		// Number of sectors
	public static float DEFAULT_TIME_STEP =0.008F;	// Capture interval in sec
	public static uint DEFAULT_PRECISION =20;	    // Precision of catpure in pixels
	public static uint DEFAULT_FIABILITY =10;		// Default fiability level (30)

	private bool isCapturing = false;				// Updates when LMB down or up and capturing enabled
	private string gestureCaptured = "";

	private ArrayList moves = new ArrayList();		// Mouse gestures
	private Vector3 lastPoint;						// Last mouse point
	private int mouseZone;     						// Mouse zone
	private uint captureDepth;			    		// Current capture depth 
	private Object rect;			     			// Rectangle zone
	private ArrayList points = new ArrayList();		// Mouse points 
	private float timeSinceLastMoveAddition;

	//protected Timer timer;					    // Timer
	protected float sectorRad;					    // Angle of one sector		
	protected ArrayList anglesMap;					// Angles map 

	protected struct GestureStruct
	{
		public string GestureName;
		public int GestureSize;
		public int[] GestureMoves;
	};
	private static ArrayList gestures = new ArrayList();				// Gestures to match
	private GestureStruct currentGesture;
	private DigitsController digCtrl;

	/**
		*	Add a gesture
		*/
	public void addGesture(string o, string gesture){
		GestureStruct g = new GestureStruct ();
		g.GestureName = o;
		g.GestureSize = gesture.Length;
		g.GestureMoves = new int[g.GestureSize];

		for (int i=0; i<g.GestureSize ;i++){
			g.GestureMoves[i] = (gesture[i]=='.' ? -1 : (int)char.GetNumericValue(gesture,i));
		}

	    gestures.Add(g);
	}

	// ------------------------------------------------
	//
	// ---o private methods
	//
	// ------------------------------------------------
	
	/**
	*	Initialization
	*/

		void Start () {
		lastPoint = new Vector3();
		//capturedValue = GameObject.FindGameObjectWithTag (Tags.text).GetComponent<GUIText>();
		digCtrl = GameObject.FindGameObjectWithTag (Tags.gameController).GetComponent<DigitsController> ();
		timeSinceLastMoveAddition = 0.0F;
		// Build the angles map
		buildAnglesMap();

		// Gesture Spots
		gestures.Clear ();

		// Creating a list of gestures to match associated with their names
		addDefaultGestures();

//		textureForGesture = new Texture2D (1, 1);
//		textureForGesture.SetPixel (0, 0, Color.yellow);
//		textureForGesture.Apply ();

	}
	
	/**
	*	Build the angles map
	*/
	protected void buildAnglesMap(){
		
		// Angle of one sector
		sectorRad=Mathf.PI*2/DEFAULT_NB_SECTORS;
		
		// map containing sectors no from 0 to PI*2
		anglesMap= new ArrayList();
		
		// the precision is Math.PI*2/100
		float step = Mathf.PI*2/100;
		
		// memorize sectors
		int sector = 0;
		for (float i =-sectorRad/2;i<Mathf.PI*2-sectorRad/2;i+=step){
			sector = (int)Mathf.Floor((i+sectorRad/2)/sectorRad);
			anglesMap.Add(sector);
		}
	}

	protected void addDefaultGestures(){
//		addGesture("0","32107654");
		addGesture("0","07654321");
		addGesture("1","126");
		addGesture("1","16");
		addGesture("2","2107650");
//		addGesture("2","21076540");
//		addGesture("2","107650");
//		addGesture("2","107650");
		addGesture("3","107654076543");
		addGesture("3","07654076543");
//		addGesture("4","5026");
		addGesture("4","506");
		addGesture("5","461076543");
		addGesture("5","46076543");
		addGesture("6","4567012345");
		addGesture("6","456701234");
//		addGesture("6","567012345");
//		addGesture("6","56701234");
//		addGesture("6","34567012345");
//		addGesture("6","3456701234");
		addGesture("7","056");
		addGesture("8","4567076543210123");
		addGesture("9","0123456701");
		addGesture("9","12345670");
		addGesture("9","54321076543");
//		addGesture("BACKSPACE","34543");
		addGesture("BACKSPACE","444");
		addGesture("-","00000");		//Minus sign
//		addGesture("-","70107");		//Minus sign
		addGesture("DOT",".");
		addGesture("r","6210");
//		addGesture("+","036");
//		addGesture("(","567");
//		addGesture(")","765");

	}

	/**
	*	Add a move 
	*/
	protected void addMove(int dx, int dy){
		float angle =Mathf.Atan2(dy,dx)+sectorRad/2;
		if (angle<0)angle+=Mathf.PI*2;
		int no=(int)Mathf.Floor(angle/(Mathf.PI*2)*100);
		moves.Add(anglesMap[no]);

		//Debug.Log ((object)anglesMap[no]);
		//Debug.Log("Dupa po katach");
	}

	/**
	 *  Convert moves into fixed size structure 
	 */

	protected void convertMoves(){

		//Removing consecutive repetitions in moves for faster comparison calculation
		moves.TrimToSize ();
		ArrayList trimmedMoves = new ArrayList();
		int lastMove = -2; 

		if (moves.Capacity > 50) {		//Testowanie
			for (int move = 4; move < moves.Capacity; move++) {
				if ((int)moves [move] != lastMove) {
					lastMove = (int)moves [move];
					trimmedMoves.Add (lastMove);
				}
			}
		} else {
			trimmedMoves = moves;
		}

		currentGesture = new GestureStruct();
		currentGesture.GestureName="unknown";
		currentGesture.GestureSize=trimmedMoves.Count;//moves.Count;
		currentGesture.GestureMoves = new int[currentGesture.GestureSize];

		int i=0;
		IEnumerator e = trimmedMoves.GetEnumerator();
		//IEnumerator e = moves.GetEnumerator();
		while (e.MoveNext())
		{
			//Debug.Log(e.Current);
			currentGesture.GestureMoves[i] = (int)e.Current;
			i++;
		}

		//Logging
		string tempCur = "";
		foreach (int g in currentGesture.GestureMoves)
						tempCur += g;
		//Debug.Log ("Truncated gesture: "+tempCur);
	}

	/**
	*	Match the gesture
	*/
	protected void matchGesture(){
		uint bestCost = 1000000;
//		int nbGestures = gestures.Count;
		uint cost;
		string bestGesture=null;

		//IDictionaryEnumerator denum = gestures.GetEnumerator();
		//DictionaryEntry dentry;

		convertMoves ();

		foreach(GestureStruct g in gestures )
		{
			//dentry = (DictionaryEntry) denum.Current;
					
			//gest=(Hashtable)dentry.Value;
			//DictionaryEntry gentry = (DictionaryEntry)gest.GetEnumerator().Current;
			//int sizeA = gest.Length;
			//int[] gestArray = new int[gest.Count];
			//gestArray = (int[])gest.ToArray(typeof(int));

			//int[] movesArray = new int[moves.Count];
			//movesArray = (int[])moves.ToArray(typeof(int));

			cost=costLeven(g,currentGesture);
			//Debug.Log("Gesture "+g.GestureName+" cost "+cost);

			if (cost<=DEFAULT_FIABILITY){
		
			if (cost<bestCost){
				bestCost    = cost;
				bestGesture = g.GestureName;
				}
			}
		}

		if (bestGesture != null) {
			//Debug.Log(bestGesture);
			//Debug.Log(bestCost);
			gestureCaptured = bestGesture;
		}

	}
	
	/**
	*	dif angle
	*/
	protected uint difAngle(int a,int b){
		uint dif = (uint)Mathf.Abs(a-b);
		if (dif>(DEFAULT_NB_SECTORS/2)) dif = (uint)DEFAULT_NB_SECTORS-dif;
		return dif;
	}
	
	/**
	*	return a filled 2D table
	*   protected function fill2DTable(w:uint,h:uint,f:*):Array{
	*/
	protected uint[,] fill2DTable(int w,int h, uint f){
		uint[,] o = new uint[w,h];
		for (int x=0;x<w;x++){
			for (int y=0;y<h;y++) 
				o[x,y]=f;
		}
		return o;
	}
	
	/**
	*	cost Levenshtein
	*   protected function costLeven(a:Array,b:Array):uint{
	*/
	protected uint costLeven(GestureStruct g,GestureStruct currentGest){

		// point
		if (g.GestureMoves[0]==-1){
			return (uint)(currentGest.GestureSize==0 ? 0 : 100000);
		}
		
		// precalc difangles
		uint[,] d = fill2DTable(g.GestureSize+1,currentGest.GestureSize+1,0);
		uint[,] w = (uint[,])d.Clone();
		
		for (int x=1;x<=g.GestureSize;x++){
			for (int y = 1;y<currentGest.GestureSize;y++){
				d[x,y]=difAngle(g.GestureMoves[x-1],currentGest.GestureMoves[y-1]);
			}
		}
		
		// max cost
		for (int y=1;y<=currentGest.GestureSize;y++)w[0,y]=100000;
		for (int x=1;x<=g.GestureSize;x++)w[x,0]=100000;
		w[0,0]=0;
		
		// levensthein application
		uint cost = 0;
		uint pa;
		uint pb;
		uint pc;

		int ex = 0;
		int ey = 0;

		for (ex=1;ex<=g.GestureSize;ex++){
			for (ey=1;ey<currentGest.GestureSize;ey++){
				cost=(uint)d[ex,ey];
				pa=(uint)w[ex-1,ey]+cost;
				pb=(uint)w[ex,ey-1]+cost;
				pc=(uint)w[ex-1,ey-1]+cost;
				w[ex,ey]=(uint)Mathf.Min(Mathf.Min(pa,pb),pc);
			}
		}

		return w[ex-1,ey-1];
	}

	public void ResetGestureCapturing(){
		if (isCapturing) matchGesture();
		isCapturing = false;
		gestureCapturingEnabled = false;
		drawingEnabled = false;
		ResetMovesAndPointsArray ();
	}

	void ResetMovesAndPointsArray(){
		moves.Clear ();
		moves.TrimToSize ();
		points.Clear();
		points.TrimToSize ();
	}

	void UpdateGestureCapturingModeStatus(){
		if (Input.GetMouseButtonDown (0) && gestureCapturingEnabled) {
			drawingEnabled = true;
			isCapturing = true;
			timeSinceLastMoveAddition = 0.0F;
			lastPoint = Input.mousePosition;
			//capturedValue.text = "Capturing...";
		}
		if ((Input.GetMouseButtonUp(0)  && gestureCapturingEnabled)) {
			if (isCapturing){
				isCapturing = false;
				matchGesture();
			}
				string tempLogResult ="";


			//Logging
			for(int i=0;i<moves.Count;i++){
				tempLogResult+=moves[i].ToString();
			}
			//Debug.Log(tempLogResult);

			//capturedValue.text = gestureCaptured;
			digCtrl.addSymbol(gestureCaptured);
			ResetMovesAndPointsArray();
		}
		//isCapturing = Input.GetMouseButton (0) && gestureCapturingEnabled; //I don't know if this function is being called from proper location
			//So in case it is not, I update isCapturing with last expression for cases when mouse button up or down might have been omitted
	}

	void CaptureMousePosition(){
	if (isCapturing) {
			timeSinceLastMoveAddition+=Time.deltaTime;
			if (timeSinceLastMoveAddition >= DEFAULT_TIME_STEP) {
				timeSinceLastMoveAddition = 0.0F;
				// calcul dif 
				int msx = (int)Input.mousePosition.x;
				int msy = (int)Input.mousePosition.y;
		
				int difx = (int)(msx - (int)lastPoint.x);
				int dify = (int)(msy - (int)lastPoint.y);
				float sqDist = difx * difx + dify * dify;
				float sqPrec = DEFAULT_PRECISION * DEFAULT_PRECISION;
			
				if (sqDist > sqPrec) {
					//Debug.Log(Input.mousePosition);
					lastPoint = Input.mousePosition;
					points.Add (lastPoint);
					addMove (difx, dify);
				}
			}
		}
	}

	void DrawGesture(){
		float x, y;

		if (isCapturing && drawingEnabled) {
			for(int i = 0;i<points.Count;i++){
				x = ((Vector3)points[i]).x;
				y = ((Vector3)points[i]).y;

				GUI.DrawTexture( new Rect(x,Screen.height-y,25,25),	textureForGesture);
			}
				
		}
	}
	// Update is called once per frame
	void FixedUpdate () {
	}
	void Update () {
		UpdateGestureCapturingModeStatus ();
		CaptureMousePosition ();
	}

	void OnGUI() {
		DrawGesture ();
	}
}

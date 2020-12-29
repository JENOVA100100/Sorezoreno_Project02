/************************************************************************************
Copyright : Copyright (c) Facebook Technologies, LLC and its affiliates. All rights reserved.

Licensed under the Oculus Utilities SDK License Version 1.31 (the "License"); you may not use
the Utilities SDK except in compliance with the License, which is provided at the time of installation
or download, or which otherwise accompanies this software in either electronic or hard copy form.
You may obtain a copy of the License at https://developer.oculus.com/licenses/utilities-1.31

Unless required by applicable law or agreed to in writing, the Utilities SDK distributed
under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
ANY KIND, either express or implied. See the License for the specific language governing
permissions and limitations under the License.
************************************************************************************/

using System;
using UnityEngine;
using System.Collections; // Scene移動の為に必要か？と思いとりあえず入れました(04/12/2020)
using UnityEngine.SceneManagement; //Sceneの為に必要なので追加しました。(04/12/2020)

/// <summary>
/// Controls the player's movement in virtual reality.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class OVRPlayerController : MonoBehaviour
{
	/// <summary>
	/// The rate acceleration during movement.
	/// </summary>
	public float Acceleration = 0.1f;

	/// <summary>
	/// The rate of damping on movement.
	/// </summary>
	public float Damping = 0.3f;

	/// <summary>
	/// The rate of additional damping when moving sideways or backwards.
	/// </summary>
	public float BackAndSideDampen = 0.5f;

	/// <summary>
	/// The force applied to the character when jumping.
	/// </summary>
	public float JumpForce = 0.3f;

	/// <summary>
	/// The rate of rotation when using a gamepad.
	/// </summary>
	public float RotationAmount = 1.5f;

	/// <summary>
	/// The rate of rotation when using the keyboard.
	/// </summary>
	public float RotationRatchet = 45.0f;

	/// <summary>
	/// The player will rotate in fixed steps if Snap Rotation is enabled.
	/// </summary>
	[Tooltip("The player will rotate in fixed steps if Snap Rotation is enabled.")]
	public bool SnapRotation = true;

	/// <summary>
	/// How many fixed speeds to use with linear movement? 0=linear control
	/// </summary>
	[Tooltip("How many fixed speeds to use with linear movement? 0=linear control")]
	public int FixedSpeedSteps;

	/// <summary>
	/// If true, reset the initial yaw of the player controller when the Hmd pose is recentered.
	/// </summary>
	public bool HmdResetsY = true;

	/// <summary>
	/// If true, tracking data from a child OVRCameraRig will update the direction of movement.
	/// </summary>
	public bool HmdRotatesY = true;

	/// <summary>
	/// Modifies the strength of gravity.
	/// </summary>
	public float GravityModifier = 0.379f;

	/// <summary>
	/// If true, each OVRPlayerController will use the player's physical height.
	/// </summary>
	public bool useProfileData = true;

	/// <summary>
	/// The CameraHeight is the actual height of the HMD and can be used to adjust the height of the character controller, which will affect the
	/// ability of the character to move into areas with a low ceiling.
	/// </summary>
	[NonSerialized]
	public float CameraHeight;

	/// <summary>
	/// This event is raised after the character controller is moved. This is used by the OVRAvatarLocomotion script to keep the avatar transform synchronized
	/// with the OVRPlayerController.
	/// </summary>
	public event Action<Transform> TransformUpdated;

	/// <summary>
	/// This bool is set to true whenever the player controller has been teleported. It is reset after every frame. Some systems, such as
	/// CharacterCameraConstraint, test this boolean in order to disable logic that moves the character controller immediately
	/// following the teleport.
	/// </summary>
	[NonSerialized] // This doesn't need to be visible in the inspector.
	public bool Teleported;

	/// <summary>
	/// This event is raised immediately after the camera transform has been updated, but before movement is updated.
	/// </summary>
	public event Action CameraUpdated;

	/// <summary>
	/// This event is raised right before the character controller is actually moved in order to provide other systems the opportunity to
	/// move the character controller in response to things other than user input, such as movement of the HMD. See CharacterCameraConstraint.cs
	/// for an example of this.
	/// </summary>
	public event Action PreCharacterMove;

	/// <summary>
	/// When true, user input will be applied to linear movement. Set this to false whenever the player controller needs to ignore input for
	/// linear movement.
	/// </summary>
	public bool EnableLinearMovement = true;

	/// <summary>
	/// When true, user input will be applied to rotation. Set this to false whenever the player controller needs to ignore input for rotation.
	/// </summary>
	public bool EnableRotation = true;

	/// <summary>
	/// Rotation defaults to secondary thumbstick. You can allow either here. Note that this won't behave well if EnableLinearMovement is true.
	/// </summary>
	public bool RotationEitherThumbstick = false;

	protected CharacterController Controller = null;
	protected OVRCameraRig CameraRig = null;

	private float MoveScale = 1.0f;
	private Vector3 MoveThrottle = Vector3.zero;
	private float FallSpeed = 0.0f;
	private OVRPose? InitialPose;
	public float InitialYRotation { get; private set; }
	private float MoveScaleMultiplier = 1.0f;
	private float RotationScaleMultiplier = 1.0f;
	private bool SkipMouseRotation = true; // It is rare to want to use mouse movement in VR, so ignore the mouse by default.
	private bool HaltUpdateMovement = false;
	private bool prevHatLeft = false;
	private bool prevHatRight = false;
	private float SimulationRate = 60f;
	private float buttonRotation = 0f;
	private bool ReadyToSnapTurn; // Set to true when a snap turn has occurred, code requires one frame of centered thumbstick to enable another snap turn.
	private bool playerControllerEnabled = false;

    //public string beforeScene; //シーンは今のところ切れ変わらないからコメントアウト(29/12/2020)string型の変数beforeSceneを宣言　Sceneの移行のために入れました。(04/12/2020) ⇐publicにしてみる？
    //string WormMove; //シーンは今のところ切れ変わらないからコメントアウト(29/12/2020)。シーンが切れ変わったらこれが反転する。(06/12/2020)⇐publicにしてみる？
    //string ChickenMove; //シーンは今のところ切れ変わらないからコメントアウト(29/12/2020)。シーンが切れ変わったらこれが反転する。(06/12/2020)⇐publicにしてみる？

    GameObject umweltmanager;　//class　GameObject　の変数を宣言。(29/12/2020)シーン切れ変わりの為にumweltmanagerを参照する為に作りました。(08/12/2020)
    UmweltManager manager; //class　UmweltManager　の変数を宣言。(29/12/2020)
    public string HowMove; //class string型のpublic変数 HowMoveを宣言。(29/12/2020)

    void Start()
	{
		// Add eye-depth as a camera offset from the player controller
		var p = CameraRig.transform.localPosition;
		p.z = OVRManager.profile.eyeDepth;
		CameraRig.transform.localPosition = p;

        umweltmanager = GameObject.Find("umweltmanager"); //オブジェクト名からゲームオブジェクトumweltmanagerを探し変数umweltmanagerに代入。(29/12/2020)
        manager = umweltmanager.GetComponent<UmweltManager>(); //変数umweltmanagerからclass UmweltManagerを取得し、変数managerに代入。(29/12/2020)

        //beforeScene = "Test01_wormmovearound";  //void OnActiveSceneChangedは現段階ではシーン変化させないので、使用しない。よってコメントアウトする(29/12/2020)。起動時のシーン名（とりあえずTest01_wormmovearound)。　Sceneの移行のために入れました。(04/12/2020)

        //SceneManager.activeSceneChanged += OnActiveSceneChanged;  //void OnActiveSceneChangedは現段階ではシーン変化させないので、使用しない。よってコメントアウトする(29/12/2020)。☆☆これは使う？(https://qiita.com/Maru60014236/items/6c1250ba98c178eac664)

    }

    //以下の void OnActiveSceneChangedは現段階ではシーン変化させないので、使用しない。よってコメントアウトする。(29/12/2020)
    /*
    void OnActiveSceneChanged(Scene prevScene, Scene nextScene)     //シーンが変化したら実行される？関数。(06/12//2020) 
    {
        gameobject.GetComponent<UmweltManager>();  //☆シーン切れ変わりの為にchangesceneのSceneMove.csを参照する為に作りました。念の為にここに書いているだけで、これから実装です。(08/12/2020) SceneMoveを消してUmweltManagerにするのでいまはコメントアウト2020/12/25

        //Test01_wormmovearoundからTest01_chickenmovearoundへと変化
        if (beforeScene == "Test01_wormmovearound" && nextScene.name == "Test01_chickenmovearound")
        {
            WormMove = false;
            ChickenMove = true;
        }   
        
        //Test01_chickenmovearoundからTest01_wormmovearoundへと変化
        if(beforeScene == "Test01_chickenmovearound" && nextScene.name == "Test01_wormmovearound")
        {
            WormMove = true;
            ChickenMove = false;
        } 


        beforeScene = nextScene.name;
    }
    */

    void Awake()
	{
		Controller = gameObject.GetComponent<CharacterController>();

		if (Controller == null)
			Debug.LogWarning("OVRPlayerController: No CharacterController attached.");

		// We use OVRCameraRig to set rotations to cameras,
		// and to be influenced by rotation
		OVRCameraRig[] CameraRigs = gameObject.GetComponentsInChildren<OVRCameraRig>();

		if (CameraRigs.Length == 0)
			Debug.LogWarning("OVRPlayerController: No OVRCameraRig attached.");
		else if (CameraRigs.Length > 1)
			Debug.LogWarning("OVRPlayerController: More then 1 OVRCameraRig attached.");
		else
			CameraRig = CameraRigs[0];

		InitialYRotation = transform.rotation.eulerAngles.y;
	}

	void OnEnable()
	{
	}

	void OnDisable()
	{
		if (playerControllerEnabled)
		{
			OVRManager.display.RecenteredPose -= ResetOrientation;

			if (CameraRig != null)
			{
				CameraRig.UpdatedAnchors -= UpdateTransform;
			}
			playerControllerEnabled = false;
		}
	}

	void Update()
	{

        //gameobject.GetComponent<UmweltManager>(); //void OnActiveSceneChangedは現段階ではシーン変化させないので、使用しない。よってコメントアウトする(29/12/2020)。SceneMoveを消してUmweltManagerにするのでいまはコメントアウト2020/12/25
        HowMove = manager.MainUmwelt;　//manager変数に格納された、class UmweltManagerのMainUmweltに格納されているstring型の文字列を代入する。

        if (!playerControllerEnabled)
		{
			if (OVRManager.OVRManagerinitialized)
			{
				OVRManager.display.RecenteredPose += ResetOrientation;

				if (CameraRig != null)
				{
					CameraRig.UpdatedAnchors += UpdateTransform;
				}
				playerControllerEnabled = true;
			}
			else
				return;
		}
		//Use keys to ratchet rotation
		if (Input.GetKeyDown(KeyCode.Q))
			buttonRotation -= RotationRatchet;

		if (Input.GetKeyDown(KeyCode.E))
			buttonRotation += RotationRatchet;
	}

	protected virtual void UpdateController()
	{
		if (useProfileData)
		{
			if (InitialPose == null)
			{
				// Save the initial pose so it can be recovered if useProfileData
				// is turned off later.
				InitialPose = new OVRPose()
				{
					position = CameraRig.transform.localPosition,
					orientation = CameraRig.transform.localRotation
				};
			}

			var p = CameraRig.transform.localPosition;
			if (OVRManager.instance.trackingOriginType == OVRManager.TrackingOrigin.EyeLevel)
			{
				p.y = OVRManager.profile.eyeHeight - (0.5f * Controller.height) + Controller.center.y;
			}
			else if (OVRManager.instance.trackingOriginType == OVRManager.TrackingOrigin.FloorLevel)
			{
				p.y = -(0.5f * Controller.height) + Controller.center.y;
			}
			CameraRig.transform.localPosition = p;
		}
		else if (InitialPose != null)
		{
			// Return to the initial pose if useProfileData was turned off at runtime
			CameraRig.transform.localPosition = InitialPose.Value.position;
			CameraRig.transform.localRotation = InitialPose.Value.orientation;
			InitialPose = null;
		}

		CameraHeight = CameraRig.centerEyeAnchor.localPosition.y;

		if (CameraUpdated != null)
		{
			CameraUpdated();
		}

		UpdateMovement();

		Vector3 moveDirection = Vector3.zero;

		float motorDamp = (1.0f + (Damping * SimulationRate * Time.deltaTime));

		MoveThrottle.x /= motorDamp;
		MoveThrottle.y = (MoveThrottle.y > 0.0f) ? (MoveThrottle.y / motorDamp) : MoveThrottle.y;
		MoveThrottle.z /= motorDamp;

		moveDirection += MoveThrottle * SimulationRate * Time.deltaTime;

		// Gravity
		if (Controller.isGrounded && FallSpeed <= 0)
			FallSpeed = ((Physics.gravity.y * (GravityModifier * 0.002f)));
		else
			FallSpeed += ((Physics.gravity.y * (GravityModifier * 0.002f)) * SimulationRate * Time.deltaTime);

		moveDirection.y += FallSpeed * SimulationRate * Time.deltaTime;


		if (Controller.isGrounded && MoveThrottle.y <= transform.lossyScale.y * 0.001f)
		{
			// Offset correction for uneven ground
			float bumpUpOffset = Mathf.Max(Controller.stepOffset, new Vector3(moveDirection.x, 0, moveDirection.z).magnitude);
			moveDirection -= bumpUpOffset * Vector3.up;
		}

		if (PreCharacterMove != null)
		{
			PreCharacterMove();
			Teleported = false;
		}

		Vector3 predictedXZ = Vector3.Scale((Controller.transform.localPosition + moveDirection), new Vector3(1, 0, 1));

		// Move contoller
		Controller.Move(moveDirection);
		Vector3 actualXZ = Vector3.Scale(Controller.transform.localPosition, new Vector3(1, 0, 1));

		if (predictedXZ != actualXZ)
			MoveThrottle += (actualXZ - predictedXZ) / (SimulationRate * Time.deltaTime);
	}


    float countup = 0.0f; // 以下のミミズの移動の際の振動に使う変数
    // bool vibrationr = null;　 使わないかも
    //speedによって接触を判断するやり方は、ばらつきが出すぎるので判断できないとします。⇐他の速さの出し方があるならばそれを後回しにしてやってみましょう。
    //Vector3 latestPos; //フレーム最後の座標を取得したい
    //Vector3 speed;　//現在の移動速度
    //float splength; // 移動ベクトルの長さ　spped.magnitude;

    public virtual void UpdateMovement()
	{
		    if (HaltUpdateMovement)
			    return;

		    if (EnableLinearMovement)
		    {
			    bool moveForward = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
			    bool moveLeft = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
			    bool moveRight = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
			    bool moveBack = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);

			    bool dpad_move = false;

			    if (OVRInput.Get(OVRInput.Button.DpadUp))
			    {
				    moveForward = true;
				    dpad_move = true;

			    }

			    if (OVRInput.Get(OVRInput.Button.DpadDown))
			    {
				    moveBack = true;
				    dpad_move = true;
			    }

			    MoveScale = 1.0f;

			    if ((moveForward && moveLeft) || (moveForward && moveRight) ||
				    (moveBack && moveLeft) || (moveBack && moveRight))
				    MoveScale = 0.70710678f;        //ここは斜め移動かな？

			    // No positional movement if we are in the air
			    if (!Controller.isGrounded)
				    MoveScale = 0.0f;

			    MoveScale *= SimulationRate * Time.deltaTime;       //ここは速さ？

			    // Compute this for key movement
			    float moveInfluence = Acceleration * 0.1f * MoveScale * MoveScaleMultiplier;

			    // Run!
			    if (dpad_move || Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
				    moveInfluence *= 2.0f;

			    Quaternion ort = transform.rotation;
			    Vector3 ortEuler = ort.eulerAngles;
			    ortEuler.z = ortEuler.x = 0f;
			    ort = Quaternion.Euler(ortEuler);

			    if (moveForward)
				    MoveThrottle += ort * (transform.lossyScale.z * moveInfluence * Vector3.forward);
			    if (moveBack)
				    MoveThrottle += ort * (transform.lossyScale.z * moveInfluence * BackAndSideDampen * Vector3.back);
			    if (moveLeft)
				    MoveThrottle += ort * (transform.lossyScale.x * moveInfluence * BackAndSideDampen * Vector3.left);
			    if (moveRight)
				    MoveThrottle += ort * (transform.lossyScale.x * moveInfluence * BackAndSideDampen * Vector3.right);



			    moveInfluence = Acceleration * 0.1f * MoveScale * MoveScaleMultiplier;

    #if !UNITY_ANDROID // LeftTrigger not avail on Android game pad
			    moveInfluence *= 1.0f + OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger);
    #endif

			    Vector2 primaryAxis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);      //移動はここか？？

			    // If speed quantization is enabled, adjust the input to the number of fixed speed steps.
			    if (FixedSpeedSteps > 0)
			    {
				    primaryAxis.y = Mathf.Round(primaryAxis.y * FixedSpeedSteps) / FixedSpeedSteps;
				    primaryAxis.x = Mathf.Round(primaryAxis.x * FixedSpeedSteps) / FixedSpeedSteps;
			    }


                if (HowMove == "worm")    //ここで、UmweltManagerに格納されているstring型の文字列で移動方法を制御する。(29/12/2020)これで、シーンが"Test01_wormmove"の時にのみ実装される移動方法としている(06/12/2020)
                {
                    float movehmd = CameraRig.centerEyeAnchor.localPosition.y;//これでHMDの高さを取っている（床からの高さ）
                    float movecontrollerx = CameraRig.rightHandAnchor.localPosition.x - CameraRig.leftHandAnchor.localPosition.x; // これは高さと違い絶対値を取る必要があるので、if文で両側から抑え込む
                    //float movecontrollery = CameraRig.rightHandAnchor.localPosition.y - CameraRig.leftHandAnchor.localPosition.y; // これは高さと違い絶対値を取る必要があるので、if文で両側から抑え込む　とりあえずxしか使わない（処理が重いし）
                    //float movecontrollerz = CameraRig.rightHandAnchor.localPosition.z - CameraRig.leftHandAnchor.localPosition.z; // これは高さと違い絶対値を取る必要があるので、if文で両側から抑え込む　とりあえずxしか使わない（処理が重いし）
                    //float moveabs = Math.Abs(move);

                    //Debug.Log(movecontrollerx);
                    // transform.localPosition.y - CameraRig.transform.localPosition.y (y座標取れる？)
           
                    if ( movehmd < 0.5f & movehmd > 0.15f) //HMDの高さが0.15ｍ～0.5ｍの時実行   movecontrollerx < 0.12 & movecontrollerx > -0.05を入れると処理落ちしちゃう
                    {
                        if (movecontrollerx < 0.12 & movecontrollerx > -0.05) //新たなif をつくったらなんとか動いたが重い
                        {
                            //transform.localPosition = transform.localPosition - Camera.main.transform.forward * Time.deltaTime * 0.4f; //代わりにこれを入れてみた！⇐出来た！！　*0.3f⇒*0.4fに変更
                            //speedによって接触を判断するやり方は、ばらつきが出すぎるので判断できないとします。⇐他の速さの出し方があるならばそれを後回しにしてやってみましょう。
                            //speed = ((transform.localPosition - latestPos) / Time.deltaTime);   //移動速度　重くなる？
                            //speed.y = 0.0f;
                            //splength = speed.magnitude;　//移動速度の各ベクトルの二乗の和のルート
                            //Debug.Log(splength);  //なぜ表示されない？


                            countup += Time.deltaTime;
                            // このやり方では、splengthにばらつきがありすぎて、上手くいかないので、物体に接触した時のみ止まり、それ以外は皆さんには視覚情報で判断してもらうかな？if(splength > 3000.0f) {　//もし、移動してるならばという意味　
                                if (countup < 1.5f & countup >= 0.0f) //0～1.0　⇒　0～1.5秒に変更
                                {
                                    transform.localPosition = transform.localPosition - Camera.main.transform.forward * Time.deltaTime * 0.4f; //代わりにこれを入れてみた！⇐出来た！！　*0.3f⇒*0.4fに変更 そしてL411から移動
                                    OVRInput.SetControllerVibration(0.2f, 0.3f, OVRInput.Controller.RTouch);//右をバイブレーション
                                    OVRInput.SetControllerVibration(0.2f, 0.3f, OVRInput.Controller.LTouch);//左をバイブレーション
                                }
                                else if (countup < 3.0f & countup >= 1.5f) //1.0～2.0　⇒　1.5～3.0秒に変更
                                {
                                    OVRInput.SetControllerVibration(0.0f, 0.0f, OVRInput.Controller.LTouch);
                                    OVRInput.SetControllerVibration(0.0f, 0.0f, OVRInput.Controller.RTouch);
                                }
                                else
                                {
                                    countup = 0.0f; //初期化
                                    OVRInput.SetControllerVibration(0.0f, 0.0f, OVRInput.Controller.RTouch);
                                    OVRInput.SetControllerVibration(0.0f, 0.0f, OVRInput.Controller.LTouch);
                                }
                       
                    
                   
                        }
                    }
                }


                if(HowMove == "chicken") //ここで、UmweltManagerに格納されているstring型の文字列で移動方法を制御する。(29/12/2020)これで、シーンが"Test01_chickenmove"の時にのみ実装される移動方法としている(06/12/2020)
            {
                    
			        if (primaryAxis.y > 0.0f)
				        MoveThrottle += ort * (primaryAxis.y * transform.lossyScale.z * moveInfluence * Vector3.forward);

			        if (primaryAxis.y < 0.0f)
				        MoveThrottle += ort * (Mathf.Abs(primaryAxis.y) * transform.lossyScale.z * moveInfluence *
									           BackAndSideDampen * Vector3.back);

			        if (primaryAxis.x < 0.0f)
				        MoveThrottle += ort * (Mathf.Abs(primaryAxis.x) * transform.lossyScale.x * moveInfluence *
									           BackAndSideDampen * Vector3.left);

			        if (primaryAxis.x > 0.0f)
				        MoveThrottle += ort * (primaryAxis.x * transform.lossyScale.x * moveInfluence * BackAndSideDampen *
									           Vector3.right);
                }    
                    //ここまでが動きか？（1 コメントアウトしてみようか！）
            }


                    // これが回転か？コメントアウトしてみよう↓　⇐なんかコメントアウトしたら、処理落ち？したぞ？？
                if(HowMove == "chicken") //ここで、UmweltManagerに格納されているstring型の文字列で移動方法を制御する。(29/12/2020)これで、シーンが"Test01_chickenmove"の時にのみ実装される移動方法としている(06/12/2020)
        { 
                    if (EnableRotation)　　
		            {
			            Vector3 euler = transform.rotation.eulerAngles;
			            float rotateInfluence = SimulationRate * Time.deltaTime * RotationAmount * RotationScaleMultiplier;

			            bool curHatLeft = OVRInput.Get(OVRInput.Button.PrimaryShoulder);

			            if (curHatLeft && !prevHatLeft)
				            euler.y -= RotationRatchet;

			            prevHatLeft = curHatLeft;

			            bool curHatRight = OVRInput.Get(OVRInput.Button.SecondaryShoulder);

			            if (curHatRight && !prevHatRight)
				            euler.y += RotationRatchet;

			            prevHatRight = curHatRight;

			            euler.y += buttonRotation;
			            buttonRotation = 0f;


    #if !UNITY_ANDROID || UNITY_EDITOR
			            if (!SkipMouseRotation)
				            euler.y += Input.GetAxis("Mouse X") * rotateInfluence * 3.25f;
    #endif

			            if (SnapRotation)
			            {
				            if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickLeft) ||
					            (RotationEitherThumbstick && OVRInput.Get(OVRInput.Button.PrimaryThumbstickLeft)))
				            {
					            if (ReadyToSnapTurn)
					            {
						            euler.y -= RotationRatchet;
						            ReadyToSnapTurn = false;
					            }
				            }
				            else if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickRight) ||
					            (RotationEitherThumbstick && OVRInput.Get(OVRInput.Button.PrimaryThumbstickRight)))
				            {
					            if (ReadyToSnapTurn)
					            {
						            euler.y += RotationRatchet;
						            ReadyToSnapTurn = false;
					            }
				            }
				            else
				            {
					            ReadyToSnapTurn = true;
				            }
			            }
			            else
			            {
				            Vector2 secondaryAxis = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
				            if (RotationEitherThumbstick)
				            {
					            Vector2 altSecondaryAxis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
					            if (secondaryAxis.sqrMagnitude < altSecondaryAxis.sqrMagnitude)
					            {
						            secondaryAxis = altSecondaryAxis;
					            }
				            }
				            euler.y += secondaryAxis.x * rotateInfluence;
			            }

			            transform.rotation = Quaternion.Euler(euler);
		            }　//ここまでが回転か？　
                }
    }


    /// <summary>
    /// Invoked by OVRCameraRig's UpdatedAnchors callback. Allows the Hmd rotation to update the facing direction of the player.
    /// </summary>
    public void UpdateTransform(OVRCameraRig rig)
	{
		Transform root = CameraRig.trackingSpace;
		Transform centerEye = CameraRig.centerEyeAnchor;

		if (HmdRotatesY && !Teleported)
		{
			Vector3 prevPos = root.position;
			Quaternion prevRot = root.rotation;

			transform.rotation = Quaternion.Euler(0.0f, centerEye.rotation.eulerAngles.y, 0.0f);

			root.position = prevPos;
			root.rotation = prevRot;
		}

		UpdateController();
		if (TransformUpdated != null)
		{
			TransformUpdated(root);
		}
	}

	/// <summary>
	/// Jump! Must be enabled manually.
	/// </summary>
	public bool Jump()
	{
		if (!Controller.isGrounded)
			return false;

		MoveThrottle += new Vector3(0, transform.lossyScale.y * JumpForce, 0);

		return true;
	}

	/// <summary>
	/// Stop this instance.
	/// </summary>
	public void Stop()
	{
		Controller.Move(Vector3.zero);
		MoveThrottle = Vector3.zero;
		FallSpeed = 0.0f;
	}

	/// <summary>
	/// Gets the move scale multiplier.
	/// </summary>
	/// <param name="moveScaleMultiplier">Move scale multiplier.</param>
	public void GetMoveScaleMultiplier(ref float moveScaleMultiplier)
	{
		moveScaleMultiplier = MoveScaleMultiplier;
	}

	/// <summary>
	/// Sets the move scale multiplier.
	/// </summary>
	/// <param name="moveScaleMultiplier">Move scale multiplier.</param>
	public void SetMoveScaleMultiplier(float moveScaleMultiplier)
	{
		MoveScaleMultiplier = moveScaleMultiplier;
	}

	/// <summary>
	/// Gets the rotation scale multiplier.
	/// </summary>
	/// <param name="rotationScaleMultiplier">Rotation scale multiplier.</param>
	public void GetRotationScaleMultiplier(ref float rotationScaleMultiplier)
	{
		rotationScaleMultiplier = RotationScaleMultiplier;
	}

	/// <summary>
	/// Sets the rotation scale multiplier.
	/// </summary>
	/// <param name="rotationScaleMultiplier">Rotation scale multiplier.</param>
	public void SetRotationScaleMultiplier(float rotationScaleMultiplier)
	{
		RotationScaleMultiplier = rotationScaleMultiplier;
	}

	/// <summary>
	/// Gets the allow mouse rotation.
	/// </summary>
	/// <param name="skipMouseRotation">Allow mouse rotation.</param>
	public void GetSkipMouseRotation(ref bool skipMouseRotation)
	{
		skipMouseRotation = SkipMouseRotation;
	}

	/// <summary>
	/// Sets the allow mouse rotation.
	/// </summary>
	/// <param name="skipMouseRotation">If set to <c>true</c> allow mouse rotation.</param>
	public void SetSkipMouseRotation(bool skipMouseRotation)
	{
		SkipMouseRotation = skipMouseRotation;
	}

	/// <summary>
	/// Gets the halt update movement.
	/// </summary>
	/// <param name="haltUpdateMovement">Halt update movement.</param>
	public void GetHaltUpdateMovement(ref bool haltUpdateMovement)
	{
		haltUpdateMovement = HaltUpdateMovement;
	}

	/// <summary>
	/// Sets the halt update movement.
	/// </summary>
	/// <param name="haltUpdateMovement">If set to <c>true</c> halt update movement.</param>
	public void SetHaltUpdateMovement(bool haltUpdateMovement)
	{
		HaltUpdateMovement = haltUpdateMovement;
	}

	/// <summary>
	/// Resets the player look rotation when the device orientation is reset.
	/// </summary>
	public void ResetOrientation()
	{
		if (HmdResetsY && !HmdRotatesY)
		{
			Vector3 euler = transform.rotation.eulerAngles;
			euler.y = InitialYRotation;
			transform.rotation = Quaternion.Euler(euler);
		}
	}
}

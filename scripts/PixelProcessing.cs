//TO-DO: (V)red rectangle
//       (V)normalized heatmap image on takePhoto() - red-0 to green 100 show map range, show accuracy percent - + optionally line
//  (markus)zoom camera
//       (*)crop back and forth
//       (V)trackbar hashes
//       (V)fix camera change
//       (V)image with Markus
//       ( )fix dot flicker
//          sync instabutton - iOS save
//      (V)remove ui on face detection
//      (V)face rect feedback circle in the middle
//markus - on face detection - remember last state;


using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;
using System.Text;

#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Runtime.InteropServices;
using System.IO;
using UnityEngine.Networking;
using System.Reflection;

#if UNITY_5_3 || UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif
//using OpenCVForUnity;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.ObjdetectModule;
using OpenCVForUnity.UnityUtils;

//[DllImport ("__Internal")]
//private static string intero n pSelector(); 
namespace AeStatix
{
    /// <summary>
    //AeStatix - real time image analysis and feedback
    /// </summary>
    public class PixelProcessing : MonoBehaviour
    {
        // costume center object
        public class Centers
        {
            public int name { get; set; }
            public Point point { get; set; }
            public Centers(int Name, Point Point)
            {
                name = Name; //TO-DO: match int var to names of channels
                point = Point;
            }
        }
        //frame processing
        //show mats
        [Header("debug")]
        public bool development = false;
        [Range(0, 360)]
        public float hueBar = 360;
        [Range(0, 360)]
        public float minHue = 0;

        [Range(0, 255)]
        public int intAlpha = 0;
        [Range(0, 255)]
        public int intBeta = 0;
        [Range(0, 255)]
        public int intGamma = 0;
        //      [Range(0,255)]
        //      public int intBeta =0;
        //      public bool flipflop = false;
        [Header("Analysis")]
        [SerializeField]
        public bool showCalcMats = false;
        int frameCount = 0;
        [SerializeField]
        [Range(0.016f, 5f)]
        float secondsBtwProcessing = 0.52f;

        [SerializeField]
        [Range(0.01f, 1f)]
        float resizeFactor = 0.502f;
        //      [SerializeField]
        //      [Range(1f,5f)]
        float exaggerateData = 1.92f;
        //      float exaggerateData = 1f;
        [SerializeField]
        [Range(0, 10)]
        int exaggerateDataFace = 4;
        Size resizeSize;
        Size PyrSize;
        //      [SerializeField]
        //      [Range(0.8f,1f)]
        float speed = 0.85f;
        bool centersFlag = false;
        [Space(10)]




        //centers
        float x, y, z;
        List<Centers> centersObj = new List<Centers>();
        List<Centers> displayCenters = new List<Centers>();
        List<Centers> currentCenters = new List<Centers>();
        Centers averageCenter;
        // temp center point
        Point point;
        //moments array
        List<Moments> moments = new List<Moments>();

        //draw
        Scalar white = new Scalar(255, 255, 255);
        Scalar red = new Scalar(200, 50, 50, 255);
        Scalar green = new Scalar(50, 250, 50, 255);
        Scalar crossColor = new Scalar(170, 170, 170, 255);
        Scalar crossColorGreen = new Scalar(170, 250, 170, 255);
        Scalar guideColor = new Scalar(50, 250, 50, 255);
        Scalar UIgreen = new Scalar(168, 221, 168, 255);
        Scalar blue = new Scalar(50, 50, 250, 255);
        //      Scalar averageColor = new Scalar(123,123,204,255);
        Scalar averageColor = new Scalar(79, 44, 162, 255);
        Point crossPoint;


        //edge
        [Header("Edge")]
        [SerializeField]
        bool edgeBias = false;
        [SerializeField]
        [Range(0, 1)]
        float edgeWeight = 0.719f;
        //      [SerializeField]
        //      [Range(0,30)]
        int blurSize = 5;
        //      [SerializeField]
        //      [Range(0,100)]
        int edgeGamma = 0;
        //      [SerializeField]
        bool thresh = true;
        //      [SerializeField]
        //      [Range(0,255)]
        int edgeThreshold = 100;
        [SerializeField]
        [Range(0, 255)]
        int cannyThreshold = 100;



        //location bias
        [Header("Location")]
        [SerializeField]
        bool loactionBias = false;
        [SerializeField]
        [Range(0f, 1f)]
        float locationWeight = 0.2f;
        [SerializeField]
        [Range(0f, 1f)]
        float rationOfScreen = 0.4f;
        [SerializeField]
        bool drawRect = true;

        UnityEngine.Rect unityRect;
        [Space(10)]

        //color coeficientes
        [Header("Color")]
        //weighted average
        [SerializeField]
        bool weightedAverage = true;
        public bool showDots = true;

        [SerializeField]
        [Range(0.01f, 2f)]
        float redCoeficiente = 1f;
        [SerializeField]
        [Range(0.01f, 2f)]
        float greenCoeficiente = 0.8f;
        [SerializeField]
        [Range(0.01f, 2f)]
        float blueCoeficiente = 1.2f;
        [Space(10)]

        //snap to center
        [SerializeField]
        bool snapToCenter = true;
        [SerializeField]
        bool snapToCenterShowRect = true;
        [SerializeField]
        [Range(1, 300)]
        int snapToCenterSize = 126;
        [SerializeField]
        [Range(1, 300)]
        int snapToCenterSizeFace = 80;
        OpenCVForUnity.CoreModule.Rect snapToCenterRect;

        //take photo
        //static int pauseFrames = 12;
        static int pauseFrames = 0;
        int photoStartFrame = (0 - (pauseFrames + 1));
        bool photoTaken = false;
        //trackbar
        [SerializeField]
        bool showTrackBar = true;

        float precentageToCenter = 0.1f;
        [SerializeField]
        [Range(0, 150)]
        int triHight = 50;
        int[] polyVertexCountTrack = new int[3];
        int[] polyVertexCountBar = new int[3];
        Scalar trackColor = new Scalar(0, 0, 0, 255);
        //background blue: // Scalar barColor = new Scalar(82,137,206,255);
        Scalar barColor = new Scalar(255, 255, 255, 255);
        int nContours = 3;
        Point zeroPoint = new Point(0, 0);

        List<MatOfPoint> triangleTrack;
        List<MatOfPoint> triangleBar;

        MatOfPoint trackPoints;
        MatOfPoint barPoints;
        Point[] barPointsArray;
        Point centerPoint;
        Point pointForTrackBarDiff;
        float totalDistance;
        float innitialTotalDistance;
        float trackbarDiffFloat;
        float frameWidth;
        float frameHeight;
        Point middleOfTheFramePoint;
        Text accuracyText;
        Slider accuracySlider;

        //logic frame count
        bool frameProcessingInit = false;

        //ui cross
        [SerializeField]
        bool cross = true;
        [SerializeField]
        bool guide = true;
        [SerializeField]
        bool centerCross = true;

        //build prefrences
        [SerializeField]
        bool webglBuild = false;
        [SerializeField]
        bool iosBuild = true;
        [SerializeField]
        bool actualIosBuild = false;

        //file upload
        [SerializeField]
        Texture2D fileUpload = null;
        bool fileUploadFlag = false;
        bool afterImageEncoding = false;
        int assetWidth;
        int assetHeight;
        bool fileuploadFirstItiration = true;
        Mat photoDataCopy;
        Mat photoCopy;
        Mat photoHeatmapCopy;
        byte[] payload1;
        byte[] payload2;
        byte[] payload3;
        MatOfByte photoBuffer;
        MatOfByte photoDataBuffer;
        MatOfByte photoHeatmapBuffer;
        string downloadImageUrl;
        string downloadImageDataUrl;
        string downloadImageHeatmapUrl;
        [Space(10)]

        //faceDetection
        [SerializeField]
        bool faceDetection = false;
        CascadeClassifier cascade;
        MatOfRect faces;
        OpenCVForUnity.CoreModule.Rect[] rects;
        List<int> displayFacePoints = new List<int>();
        List<int> currentFacePoints = new List<int>();
        bool facesFlag = false;
        int lastFaceFrame = 0;
        [SerializeField]
        [Range(1, 20)]
        int numberOfFramesWithNoFace = 10;
        OpenCVForUnity.CoreModule.Range horiRange;
        OpenCVForUnity.CoreModule.Range vertRange;
        int[] intMaxDetections = new int[1];
        MatOfInt maxDetections;
        bool flippedForPhoto = false;
        int faceMiddleX = 0;
        int faceMiddleY = 0;
        bool trackbarFace = false;

        Color faceBackgroundColorRed = new Color(55, 10, 0, 200);
        Color faceBackgroundColorGray = new Color(182, 182, 182, 255);
        Scalar faceSubmatColor = new Scalar(0, 0, 0, 80);
        Size maxFaceSize;
        int rectsY;
        int rectsWidth;
        bool greenRectFeedback = false;
        bool cameraToggle = false;
        int cameraIndexUI = 0;
        Toggle faceToogleUi;
        Toggle crossToogleUi;
        Toggle centerCrossToogleUi;
        Text disclaimerUi;
        Text disclaimerUiChild;
        Image disclaimerBackground;
        Toggle heatmapToggle;


        [Space(10)]

        //heatmap
        [SerializeField]
        public bool simpleHeat = false;
        [SerializeField]
        bool heatmap = true;
        [SerializeField]
        Size blurKernalSize = new Size(15, 15);
        [SerializeField]
        [Range(-10, 50)]
        int int1 = 0;
        [SerializeField]
        [Range(-10, 50)]
        int int2 = 0;
        bool heatmapFlag = false;
        public int conversionInt = 0;
        List<Mat> heatChannels;
        //      GameObject hsvUI ;
        //      bool hsvUIBool = false;

        //insta button
        float[] distanceToEdge = new float[4];
        int minDistanceToEdge;
        bool wasAtPhoto = false;
        bool downloadingPhoto = false;
        bool insta = false;
        //file service
        public static string url;
        //      public 

        //crop
        OpenCVForUnity.CoreModule.Range colsRange;
        OpenCVForUnity.CoreModule.Range rowsRange;
        float deltaRows;
        float deltaCols;
        float resizedValueCols;
        float resizedValueRows;
        bool initialUpload = false;

        private float lastClick = 0;
        private float waitTime = 1.0f; //wait time befor reacting
        private float downTime; //internal time from when the key is pressed
        private bool isHandled = false;

        Button takePhotoButton;
        Button cropButton;
        Button cropRectButton;
        Button confirmButton;


        bool allowCrop = false;
        bool cropRect = false;
        bool isMousePressed = false;
        bool firstOnCrop = false;
        bool leftIsDown = false;
        bool afterFirstCropMoseUp = false;
        Point firstMouseDown;
        Point secondPoint;

        //      Touch firstTouch;
        Touch secondTouch;
        //      Touch[] touchesArray;

        //audio
        AudioSource audio;


        //green rect
        bool harmonyRectFeedback = false;
        /////////////////////////////////

        /// <summary>
        /// Set this to specify the name of the device to use.
        /// </summary>
        string requestedDeviceName = null;

        /// <summary>
        /// Set the requested width of the camera device.
        /// </summary>
        //int requestedWidth = 1534;
        int requestedWidth = 768;

        /// <summary>
        /// Set the requested height of the camera device.
        /// </summary>
        //int requestedHeight = 1050;
        int requestedHeight = 540;

        /// <summary>
        /// Set the requested to using the front camera.
        /// </summary>
        bool requestedIsFrontFacing = false;

        /// <summary>
        /// The webcam texture.
        /// </summary>
        WebCamTexture webCamTexture;

        /// <summary>
        /// The webcam device.
        /// </summary>
        WebCamDevice webCamDevice;

        // MATS:

        /// <summary>
        /// The rgba mat.
        /// </summary>
        Mat rgbaMat;

        /// The rgb mat.
        Mat rgbMat;
        Mat rgbCloneMat;
        Mat rgbReferenceMat;
        //gray mat
        Mat grayMat;
        //gray mat
        Mat grayFaceMat;

        //faceMat
        Mat faceMat;
        //to copy to resize mat
        Mat faceRefMat;

        //resize mat
        Mat resizeMat;

        //heatmap dialte
        Mat kernelMat;
        Mat erodeKernelMat;

        //resize mat
        Mat heatmapMat;
        Mat heatmapMatPower;

        //resize mat
        Mat locationMat;
        OpenCVForUnity.CoreModule.Rect sub;

        //white mat
        Mat whiteMat;

        //black mat
        Mat blackMat;
        //resize mat
        Mat GUImat;

        //file upload mat
        Mat fileUploadMat;
        Mat fileUploadResizedMat;
        byte[] fileUploadData;

        //submat
        Mat submat;
        Mat zeroMat;
        //submat
        Mat faceSubmat;

        //copy mat GUI
        Mat copyMat;

        //photo mat
        Mat photoMat;

        //photo border mat
        Mat photoWhiteMat;

        //pyramid mat for heatmap
        Mat pyrMat;
        Mat pyrMatRGB;
        //cascade rotate mat
        Mat rotateMat;
        //channels List
        List<Mat> channels;

        //center Points list
        List<Point> centerPoints;

        /// <summary>
        /// The colors.
        /// </summary>
        Color32[] colors;
        Color32[] fileColors;

        /// <summary>
        /// The texture.
        /// </summary>
        Texture2D texture;

        /// <summary>
        /// file texture.
        /// </summary>
        Texture2D fileTexture;

        //GUI texture
        Texture2D GUItexture;

        /// <summary>
        /// Indicates whether this instance is waiting for initialization to complete.
        /// </summary>
        bool isInitWaiting = false;

        /// <summary>
        /// Indicates whether this instance has been initialized.
        /// </summary>
        bool hasInitDone = false;

        // Use this for initialization
        void Start()
        {
            //call webGL "on start" messeges to user
            if (webglBuild)
            {
                Application.ExternalCall("onStartFromUnity");
                Debug.Log("OnStart called from unity");
            }
            //ui reset n switches
            loactionBias = true;
            edgeBias = true;
            weightedAverage = true;
            showDots = true;
            cross = false;
            centerCross = true;
            guide = true;
            faceDetection = false;
            heatmap = false;
            //          hsvUI = GameObject.FindGameObjectWithTag ("hsv");
            if (iosBuild)
            {
                //face ui
                faceToogleUi = GameObject.FindGameObjectWithTag("facedetection").GetComponent<Toggle>();
            }
            //          heatmapToggle = GameObject.FindGameObjectWithTag ("heatmap-toggle").GetComponent<Toggle>();
            crossToogleUi = GameObject.FindGameObjectWithTag("cross").GetComponent<Toggle>();
            centerCrossToogleUi = GameObject.FindGameObjectWithTag("centerCross").GetComponent<Toggle>();
            disclaimerUi = GameObject.FindGameObjectWithTag("heatmap-disclaimer").GetComponent<Text>();
            disclaimerUiChild = GameObject.FindGameObjectWithTag("heatmap-child").GetComponent<Text>();
            disclaimerBackground = GameObject.FindGameObjectWithTag("heatmap-background").GetComponent<Image>();
            disclaimerBackground.enabled = false;
            disclaimerUi.enabled = false;
            disclaimerUiChild.enabled = false;

            cropButton = GameObject.FindGameObjectWithTag("crop-button").GetComponent<Button>();
            takePhotoButton = GameObject.FindGameObjectWithTag("takePhoto-button").GetComponent<Button>();
            cropRectButton = GameObject.FindGameObjectWithTag("crop-rect").GetComponent<Button>();
            cropButton.gameObject.SetActive(false);
            cropRectButton.gameObject.SetActive(false);

            confirmButton = GameObject.FindGameObjectWithTag("confirm-button").GetComponent<Button>();
            confirmButton.gameObject.SetActive(false);

            initialUpload = true;
            audio = GetComponent<AudioSource>();

            if (centerCross)
            {
                //              centerCross = true; 
                centerCrossToogleUi.isOn = true;
                centerCross = true;
            }

            //trackbar
            accuracySlider = GameObject.FindGameObjectWithTag("accuracy").GetComponent<Slider>();


            //build settings
            if (iosBuild)
            {
                requestedWidth = 1534;
                requestedHeight = 1050;
            }
            if (webglBuild)
            {
                requestedWidth = 640;
                requestedHeight = 480;
            }

            //webGL load cascade

#if UNITY_WEBGL && !UNITY_EDITOR
            Stack<IEnumerator> coroutines = new Stack<IEnumerator> ();

            var getFilePath_Coroutine = Utils.getFilePathAsync("lbpcascade_frontalface_improved.xml", 
            //var getFilePath_Coroutine = Utils.getFilePathAsync("lbpcascade_frontalface.xml", 

            (result) => {
            coroutines.Clear ();

            cascade = new CascadeClassifier ();
            cascade.load(result);
            Initialize ();
            }, 
            (result, progress) => {
            Debug.Log ("getFilePathAsync() progress : " + result + " " + Mathf.CeilToInt (progress * 100) + "%");
            });
            coroutines.Push (getFilePath_Coroutine);
            StartCoroutine (getFilePath_Coroutine);
#else
            cascade = new CascadeClassifier();
            //          cascade = new CascadeClassifier (Utils.getFilePath ("lbpcascade_frontalface.xml"));
            cascade = new CascadeClassifier(Utils.getFilePath("lbpcascade_frontalface_improved.xml"));
            //cascade.load (Utils.getFilePath ("haarcascade_frontalface_alt.xml"));

            //            if (cascade.empty ()) {
            //                Debug.LogError ("cascade file is not loaded.Please copy from “OpenCVForUnity/StreamingAssets/” to “Assets/StreamingAssets/” folder. ");
            //            }
            Initialize();
#endif

            // init before face detection
            //Initialize ();
        }

        /// <summary>
        /// Initialize of web cam texture.
        /// </summary>
        private void Initialize()
        {
            if (isInitWaiting)
                return;

            StartCoroutine(_Initialize());
        }

        /// <summary>
        /// Initialize of webcam texture.
        /// </summary>
        /// <param name="deviceName">Device name.</param>
        /// <param name="requestedWidth">Requested width.</param>
        /// <param name="requestedHeight">Requested height.</param>
        /// <param name="requestedIsFrontFacing">If set to <c>true</c> requested to using the front camera.</param>
        private void Initialize(string deviceName, int requestedWidth, int requestedHeight, bool requestedIsFrontFacing)
        {
            if (isInitWaiting)
                return;

            this.requestedDeviceName = deviceName;
            this.requestedWidth = requestedWidth;
            this.requestedHeight = requestedHeight;
            this.requestedIsFrontFacing = requestedIsFrontFacing;

            StartCoroutine(_Initialize());
        }

        /// <summary>
        /// Initialize of webcam texture by coroutine.
        /// </summary>
        private IEnumerator _Initialize()
        {
            if (hasInitDone)
                Dispose();

            isInitWaiting = true;

            if (!String.IsNullOrEmpty(requestedDeviceName))
            {
                Debug.Log("deviceName is " + requestedDeviceName);
                webCamTexture = new WebCamTexture(requestedDeviceName, requestedWidth, requestedHeight);

            }
            else
            {
                //Debug.Log ("deviceName is null");

                //TEST IOS device
                // Checks how many and which cameras are available on the device
                //              for (int cameraIndex = 0; cameraIndex < WebCamTexture.devices.Length; cameraIndex++) {
                //                  if (WebCamTexture.devices [cameraIndex].isFrontFacing == requestedIsFrontFacing) {
                //                      //Debug.Log (cameraIndex + " name " + WebCamTexture.devices [cameraIndex].name + " isFrontFacing " + WebCamTexture.devices [cameraIndex].isFrontFacing);
                //                      webCamDevice = WebCamTexture.devices [cameraIndex];
                //                      webCamTexture = new WebCamTexture (webCamDevice.name, requestedWidth, requestedHeight);
                //                      break;
                //                  }
                //              }
                webCamDevice = WebCamTexture.devices[cameraIndexUI];
                webCamTexture = new WebCamTexture(webCamDevice.name, requestedWidth, requestedHeight);

            }

            if (webCamTexture == null)
            {
                if (WebCamTexture.devices.Length > 0)
                {
                    Debug.Log("<unity> number of cameras: " + WebCamTexture.devices.Length);
                    webCamDevice = WebCamTexture.devices[0];
                    webCamTexture = new WebCamTexture(webCamDevice.name, requestedWidth, requestedHeight);
                }
                else
                {
                    webCamTexture = new WebCamTexture(requestedWidth, requestedHeight);
                }
            }

            Debug.Log("<unity> in camera initiation\n" +
                "<unity> webCamTexture.videoRotationAngle: " + webCamTexture.videoRotationAngle + "\n" +
                "<unity> name" + webCamTexture.deviceName + "\n" +
                "<unity> webCamTexture.videoVerticallyMirrored: " + webCamTexture.videoVerticallyMirrored + "\n" +
                "<unity> webCamTexture.wrapMode: " + webCamTexture.wrapMode);

            // Starts the camera.
            webCamTexture.Play();

            while (true)
            {
                // If you want to use webcamTexture.width and webcamTexture.height on iOS, you have to wait until webcamTexture.didUpdateThisFrame == 1, otherwise these two values will be equal to 16. (http://forum.unity3d.com/threads/webcamtexture-and-error-0x0502.123922/).
#if UNITY_IOS && !UNITY_EDITOR && (UNITY_4_6_3 || UNITY_4_6_4 || UNITY_5_0_0 || UNITY_5_0_1)
                if (webCamTexture.width > 16 && webCamTexture.height > 16) {
#else
                if (webCamTexture.didUpdateThisFrame && webCamTexture.width > 100)
                {
#if UNITY_IOS && !UNITY_EDITOR && UNITY_5_2
                while (webCamTexture.width <= 16) {
                webCamTexture.GetPixels32 ();
                yield return new WaitForEndOfFrame ();
                } 
#endif
#endif
                    webCamTexture.requestedFPS = 60;
                    Debug.Log("<unity> Camera: (" + webCamTexture.width + "px," + webCamTexture.height + "px) " + webCamTexture.requestedFPS + "fps");
                    //Debug.Log ("videoRotationAngle " + webCamTexture.videoRotationAngle + " videoVerticallyMirrored " + webCamTexture.videoVerticallyMirrored + " isFrongFacing " + webCamDevice.isFrontFacing);

                    //                  Debug.Log("<unity> bfr OnInitiated(), camera texture width" + webCamTexture.width );

                    isInitWaiting = false;
                    hasInitDone = true;

                    OnInited();

                    break;
                }
                else
                {
                    //                  Debug.Log("<unity> yield, camera texture width: " + webCamTexture.width );
                    yield return 0;
                }
            }
        }

        /// <summary>
        /// Releases all resource.
        /// </summary>
        private void Dispose()
        {
            isInitWaiting = false;
            hasInitDone = false;

            //ui reset
            loactionBias = false;
            edgeBias = false;
            if (cameraIndexUI == 0)
            {
                faceDetection = false;
                frameCount = 0;
            }
            else
            {
                frameCount = 15;
            }
            heatmap = false;
            //for camera fix
            //          channels = null;
            //          heatChannels = null;
            //          averageCenter = null;
            //          middleOfTheFramePoint = null;
            //          displayCenters = null;
            //          faces = null
            //
            //individualColorCoeficients = false;

            if (webCamTexture != null)
            {
                webCamTexture.Stop();
                webCamTexture = null;
            }
            if (rgbaMat != null)
            {
                rgbaMat.Dispose();
                rgbaMat = null;
            }
            if (rgbMat != null)
            {
                rgbMat.Dispose();
                rgbMat = null;
            }
            if (rgbCloneMat != null)
            {
                rgbCloneMat.Dispose();
                rgbCloneMat = null;
            }
            if (rgbReferenceMat != null)
            {
                rgbReferenceMat.Dispose();
                rgbReferenceMat = null;
            }
            if (fileUploadMat != null)
            {
                fileUploadMat.Dispose();
                fileUploadMat = null;
            }

            if (fileUploadResizedMat != null)
            {
                fileUploadResizedMat.Dispose();
                fileUploadResizedMat = null;
            }
            if (grayMat != null)
            {
                grayMat.Dispose();
                grayMat = null;
            }
            if (grayFaceMat != null)
            {
                grayFaceMat.Dispose();
                grayFaceMat = null;
            }
            if (faceMat != null)
            {
                faceMat.Dispose();
                faceMat = null;
            }
            if (faceRefMat != null)
            {
                faceRefMat.Dispose();
                faceRefMat = null;
            }
            if (whiteMat != null)
            {
                whiteMat.Dispose();
                whiteMat = null;
            }
            if (blackMat != null)
            {
                blackMat.Dispose();
                blackMat = null;
            }
            if (locationMat != null)
            {
                locationMat.Dispose();
                locationMat = null;
            }
            if (resizeMat != null)
            {
                resizeMat.Dispose();
                resizeMat = null;
            }
            if (kernelMat != null)
            {
                kernelMat.Dispose();
                kernelMat = null;
            }
            if (erodeKernelMat != null)
            {
                erodeKernelMat.Dispose();
                erodeKernelMat = null;
            }
            if (heatmapMat != null)
            {
                heatmapMat.Dispose();
                heatmapMat = null;
            }
            if (zeroMat != null)
            {
                zeroMat.Dispose();
                zeroMat = null;
            }
            if (heatmapMatPower != null)
            {
                heatmapMatPower.Dispose();
                heatmapMatPower = null;
            }
            if (submat != null)
            {
                submat.Dispose();
                submat = null;
            }
            if (faceSubmat != null)
            {
                faceSubmat.Dispose();
                faceSubmat = null;
            }
            if (copyMat != null)
            {
                copyMat.Dispose();
                copyMat = null;
            }
            if (GUImat != null)
            {
                GUImat.Dispose();
                GUImat = null;
            }
            if (photoMat != null)
            {
                photoMat.Dispose();
                photoMat = null;
            }
            if (photoWhiteMat != null)
            {
                photoWhiteMat.Dispose();
                photoWhiteMat = null;
            }
            if (rotateMat != null)
            {
                rotateMat.Dispose();
                rotateMat = null;
            }
            if (pyrMat != null)
            {
                pyrMat.Dispose();
                pyrMat = null;
            }
            if (pyrMatRGB != null)
            {
                pyrMatRGB.Dispose();
                pyrMatRGB = null;
            }
        }

        ////////////
        /// MAIN INITIATION (after getting the WebcamTexture)
        ///////////

        private void OnInited()
        {
            //call "on almostDone" to browser
            if (webglBuild)
            {
                Application.ExternalCall("onAlmostDoneFromUnity");
                Debug.Log("onAlmostDoneFromUnity called from unity");
            }
            //texture initiation
            if (colors == null || colors.Length != webCamTexture.width * webCamTexture.height)
                colors = new Color32[webCamTexture.width * webCamTexture.height];
            if (texture == null || texture.width != webCamTexture.width || texture.height != webCamTexture.height)
                texture = new Texture2D(webCamTexture.width, webCamTexture.height, TextureFormat.RGBA32, false);

            //mats sizes initiation
            rgbaMat = new Mat(webCamTexture.height, webCamTexture.width, CvType.CV_8UC4);
            rgbMat = new Mat(webCamTexture.height, webCamTexture.width, CvType.CV_8UC3);
            rgbCloneMat = rgbMat.clone();
            rgbReferenceMat = rgbMat.clone();
            frameWidth = rgbMat.width();
            frameHeight = rgbMat.height();
            resizeSize = new Size((int)Math.Round(webCamTexture.width * resizeFactor), (int)Math.Round(webCamTexture.height * resizeFactor));
            PyrSize = new Size((int)Math.Round(webCamTexture.width * resizeFactor / 1.3), (int)Math.Round(webCamTexture.height * resizeFactor / 1.3));
            pyrMat = new Mat(PyrSize, CvType.CV_8UC3);
            pyrMatRGB = new Mat(PyrSize, CvType.CV_8UC3);
            resizeMat = new Mat(resizeSize, CvType.CV_8UC3);
            rotateMat = new Mat((int)resizeSize.width, (int)resizeSize.height, CvType.CV_8UC3);
            //          heatmapMat = OpenCVForUnity.Mat.zeros (webCamTexture.height, webCamTexture.width, CvType.CV_32F);
            heatmapMat = Mat.zeros(webCamTexture.height, webCamTexture.width, CvType.CV_8UC3);
            zeroMat = Mat.zeros(webCamTexture.height, webCamTexture.width, CvType.CV_8UC3);
            Core.bitwise_not(zeroMat, zeroMat);
            //          heatmapMatPower = new Mat (webCamTexture.height, webCamTexture.width, CvType.CV_8U, new Scalar(100));
            Debug.Log("<unity> analysis size: " + resizeSize.width + "px, " + resizeSize.height + "px");
            locationMat = new Mat(resizeSize, CvType.CV_8UC3, new Scalar(0, 0, 0));
            whiteMat = new Mat(resizeSize, CvType.CV_8UC1, new Scalar(255));
            photoWhiteMat = new Mat(webCamTexture.height, webCamTexture.width, CvType.CV_8UC3, new Scalar(255, 255, 255, 255));
            blackMat = new Mat(resizeSize, CvType.CV_8UC3, new Scalar(0, 0, 0));
            copyMat = new Mat(resizeSize, CvType.CV_8UC3);
            GUImat = new Mat(resizeSize, CvType.CV_8UC1);
            grayMat = new Mat(resizeSize, CvType.CV_8UC1);
            grayFaceMat = new Mat(resizeSize, CvType.CV_8UC1);
            faceMat = new Mat(resizeSize, CvType.CV_8UC1);
            faceRefMat = new Mat(resizeSize, CvType.CV_8UC3);
            photoMat = new Mat(webCamTexture.height, webCamTexture.width, CvType.CV_8UC3);
            kernelMat = new Mat(3, 3, CvType.CV_8U);
            erodeKernelMat = new Mat(1, 1, CvType.CV_8U);
            fileUploadResizedMat = new Mat();

            channels = new List<Mat>();
            heatChannels = new List<Mat>();
            heatChannels.Add(new Mat(webCamTexture.height, webCamTexture.width, CvType.CV_32F));
            //          heatChannels [1] = heatChannels [0].clone();
            //          heatChannels [2] = heatChannels [0].clone();

            //assemble location Mat
            sub = new OpenCVForUnity.CoreModule.Rect(new Point((int)Math.Round(locationMat.width() * rationOfScreen), (int)Math.Round(locationMat.height() * rationOfScreen)),
                new Point((int)Math.Round(locationMat.width() * (1 - rationOfScreen)), (int)Math.Round(locationMat.height() * (1 - rationOfScreen))));
            submat = new Mat(new Size(sub.width, sub.height), CvType.CV_8UC3, white);
            submat.copyTo(locationMat.colRange((int)Math.Round(locationMat.width() * rationOfScreen), (int)Math.Round(locationMat.width() * (1 - rationOfScreen)))
                .rowRange((int)Math.Round(locationMat.height() * rationOfScreen), (int)Math.Round(locationMat.height() * (1 - rationOfScreen))));

            //average center
            averageCenter = new Centers(4, new Point(webCamTexture.width / 2, webCamTexture.height / 2));
            middleOfTheFramePoint = new Point(webCamTexture.width / 2, webCamTexture.height / 2);

            displayCenters.Clear();
            for (int c = 0; c < 3; c++)
            {
                currentCenters.Add(new Centers(c, new Point(webCamTexture.width / 2, webCamTexture.height / 2)));
            }
            displayCenters.Clear();
            for (int c = 0; c < 3; c++)
            {
                displayCenters.Add(new Centers(c, new Point(webCamTexture.width / 2, webCamTexture.height / 2)));
            }

            //faces
            faces = new MatOfRect();
            faceSubmat = new Mat(webCamTexture.height, webCamTexture.width, CvType.CV_8UC4);
            intMaxDetections[0] = 1;
            maxDetections = new MatOfInt(intMaxDetections);
            maxFaceSize = new Size(frameWidth, frameHeight);

            //textures
            if (GUItexture == null || GUItexture.width != resizeSize.width || GUItexture.height != resizeSize.height)
                GUItexture = new Texture2D((int)resizeSize.width, (int)resizeSize.height, TextureFormat.RGBA32, false);

            //          hsvUI.SetActive (false);
            gameObject.GetComponent<Renderer>().material.mainTexture = texture;

            gameObject.transform.localScale = new Vector3(webCamTexture.width, webCamTexture.height, 1);
            Debug.Log("<unity> Screen size: (" + Screen.width + "px, " + Screen.height + "px) Screen.orientation " + Screen.orientation);

            //trackBar UI
            //      Point centerPoint = new Point(rgbMat.width()/2,rgbMat.height()/2);
            totalDistance = (float)Math.Sqrt(((rgbaMat.width() / 2) * (rgbaMat.width() / 2)) + ((rgbaMat.height() / 2) * (rgbaMat.height() / 2)));
            innitialTotalDistance = totalDistance;
            //Debug.Log("max distance from center (trackbar feedback): " + totalDistance + "px\n");
            Point[] trackPointArray = new Point[3] {
                new Point (frameWidth, frameHeight-200),
                new Point (frameWidth - triHight, 170),
                new Point (frameWidth, 170)
            };





            //track text 
            accuracyText = GameObject.FindGameObjectWithTag("accuracyText").GetComponent<UnityEngine.UI.Text>();
            //bar points
            Point bottomLeft = new Point(rgbMat.width(), rgbMat.height());
            Point topRight = bottomLeft;
            Point bottomRight = bottomLeft;
            //barPointsArray = new Point[3];
            barPointsArray = new Point[] { bottomLeft, topRight, bottomRight };

            trackPoints = new MatOfPoint();
            barPoints = new MatOfPoint();
            barPoints = new MatOfPoint(barPointsArray);
            trackPoints = new MatOfPoint(trackPointArray);

            triangleTrack = new List<MatOfPoint>();
            triangleBar = new List<MatOfPoint>();
            triangleTrack.Add(trackPoints);
            triangleBar.Add(barPoints);

            //cross point
            crossPoint = new Point(frameWidth / 2 - 3, frameHeight / 2 + 3);

            //camera position
            float width = rgbaMat.width();
            float height = rgbaMat.height();
            float widthScale = (float)Screen.width / width;
            float heightScale = (float)Screen.height / height;
            Quaternion baseRotation = Camera.main.transform.rotation;

            if (iosBuild)
            {
                if (widthScale < heightScale)
                {
                    Camera.main.transform.rotation = new Quaternion(0, 0, 1, 1);
                    Camera.main.orthographicSize = (height * (float)Screen.height / (float)Screen.width) / 2;
                    Camera.main.transform.position = new Vector3(Camera.main.transform.position.x - 2, Camera.main.transform.position.y, Camera.main.transform.position.z + 10);
                    Debug.Log("widescale smaller");

                }
                else
                {
                    Camera.main.orthographicSize = width / 2;
                    Camera.main.transform.position = new Vector3(Camera.main.transform.position.x - 2, Camera.main.transform.position.y, Camera.main.transform.position.z + 10);
                }
            }
            if (webglBuild)
            {
                if (widthScale > heightScale)
                {
                    Debug.Log("webGL screen width: " + Screen.width + " screen hight: " + Screen.height);
                    Camera.main.orthographicSize = (width * (float)Screen.height / (float)Screen.width) / 2;
                    Camera.main.transform.position = new Vector3(Camera.main.transform.position.x - 2, Camera.main.transform.position.y, Camera.main.transform.position.z);

                }
                else
                {
                    Debug.Log("webGL widthScale !> heightScale - screen width: " + Screen.width + " screen hight: " + Screen.height);

                    Camera.main.orthographicSize = height / 2;
                    Camera.main.transform.position = new Vector3(Camera.main.transform.position.x - 2, Camera.main.transform.position.y, Camera.main.transform.position.z);

                }

            }//camera rotation
            //

            //reset rgbMat if ChangeCamera()
            rgbMat.setTo(new Scalar(255, 255, 255));
            //start processing
            StartCoroutine(processFrame());
        }

        ////////////
        /// DRAWING - update loop - called once per frame
        ///////////

        void Update()
        {
            //if (!downloadingPhoto) { 
            if (allowCrop)
            {

#if UNITY_IOS
                if(actualIosBuild){
//              firstTouch = Input.GetTouch (0);
                    if (Input.touchCount > 0 && Input.GetTouch(0).phase != TouchPhase.Stationary) {
                    Debug.Log ("<unity> touchinggg");
                    iosTouch ();
                    }else{
                        if(Input.touchCount == 0){
                        leftIsDown = false;
                        }
                    }
                }
#endif
            }
            //got a camera frame
            if (hasInitDone && webCamTexture.isPlaying && webCamTexture.didUpdateThisFrame && webCamTexture.width > 100)
            {
                //              if (photoTaken != true) {
                if (frameCount >= photoStartFrame + pauseFrames)
                {

                    //return texture from photo
                    //Utils.matToTexture2D (rgbMat, texture, colors);

                    if (frameProcessingInit)
                    {

                        if (!fileUpload)
                        {

                            if (photoTaken == false)
                            {
                                Utils.webCamTextureToMat(webCamTexture, rgbaMat, colors);
                                Utils.webCamTextureToMat(webCamTexture, rgbMat, colors);
                            }
                            else
                            {

                                //Core.flip(rgbMat, rgbMat, 0);
                                if (wasAtPhoto == true)
                                {
                                    Debug.Log("<debug> was at photo");
                                    Core.flip(rgbMat, rgbMat, 0);
                                        //Core.flip(rgbMat, rgbMat, 0);
                                    //Core.flip(rgbaMat, rgbaMat, 0);

                                    if (photoTaken && faceDetection)
                                    {
                                        Debug.Log("photo taken and face detection");
                                        //                                  Debug.Log ("inside photo taken");
                                        Core.flip(rgbMat, rgbMat, 0);
                                        //                                  Core.flip(rgbCloneMat,rgbCloneMat,0);

                                        //                                  Imgproc.circle (rgbCloneMat, middleOfTheFramePoint, 50, green, 50, Imgproc.LINE_AA, 0);
                                    }

                                    if (insta)
                                    {
                                        rgbCloneMat = rgbMat;
                                    }

                                    //                              Utils.matToTexture2D (rgbaMat, texture, colors);
                                    Utils.matToTexture2D(rgbMat, texture, colors);
                                    wasAtPhoto = false;
                                }

                                //                              Debug.Log( "debug: rbgMat bytes: " + rgbMat.release());
                                //                              rgbMat.;
                                GC.Collect();
                            }


                        }
                        else
                        {
                            //Debug.Log(string.Format(" file upload:  photoTaken: {0}, wasAtPhoto: {1} ", photoTaken, wasAtPhoto));

                            //                          if (initialUpload) {
                            //strech image upload
                            if (photoTaken == false)
                            {

                                if (!allowCrop)
                                {
                                    Imgproc.resize(fileUploadMat, rgbaMat, new Size(rgbaMat.width(), rgbaMat.height()));
                                    Imgproc.resize(fileUploadMat, rgbMat, new Size(rgbMat.width(), rgbMat.height()));
                                }
                            }
                            else
                            {
                                if (wasAtPhoto )
                                {
                                    Core.flip(rgbMat, rgbMat, 0);
                                    wasAtPhoto = false;
                                    //Utils.matToTexture2D (rgbMat, texture, colors);

                                }
                                Core.flip(rgbMat, rgbMat, 0);

                                //if (allowCrop)
                                //{
                                //  Core.flip(rgbMat, rgbMat, 0);

                                //}
                                //if (allowCrop)
                                //{
                                //    //Core.flip(rgbMat, rgbMat, 0);
                                //    //Core.flip(rgbaMat, rgbaMat, 0);

                                //}
                                GC.Collect();

                            }

                            Utils.matToTexture2D(rgbMat, texture, colors);

                            //                              
                            //                          }
                            //                              rgbReferenceMat = rgbMat.clone ();

                        }


                        //green LOCATION rect GUI
                        if (showCalcMats && loactionBias && drawRect)
                        {
                            Point locationPoint1 = new Point((int)Math.Round(rgbaMat.width() * rationOfScreen), (int)Math.Round(rgbaMat.height() * rationOfScreen));
                            Point locationPoint2 = new Point((int)Math.Round(rgbaMat.width() * (1 - rationOfScreen)), (int)Math.Round(rgbaMat.height() * (1 - rationOfScreen)));
                            Imgproc.rectangle(rgbaMat, locationPoint1, locationPoint2, green, 2);
                            Imgproc.putText(rgbaMat, "location bias", locationPoint1, 0, 0.8, green, 2);
                        }
                        if (snapToCenterShowRect)
                        {
                            Imgproc.rectangle(rgbaMat, new Point((rgbaMat.width() / 2) - snapToCenterSize, (rgbaMat.height() / 2) - snapToCenterSize), new Point((rgbaMat.width() / 2) + snapToCenterSize, (rgbaMat.height() / 2) + snapToCenterSize), blue, 2);
                            Imgproc.putText(rgbaMat, "snap to center", new Point((rgbaMat.width() / 2) - snapToCenterSize, (rgbaMat.height() / 2) - snapToCenterSize - 5), 0, 0.8, blue, 2);
                            if (faceDetection == true && frameCount >= 5 && (frameCount - lastFaceFrame <= numberOfFramesWithNoFace))
                            {
                                Imgproc.rectangle(rgbaMat, new Point(faceMiddleX - snapToCenterSizeFace, frameHeight - faceMiddleY - snapToCenterSizeFace), new Point(faceMiddleX + snapToCenterSizeFace, frameHeight - faceMiddleY + snapToCenterSizeFace), blue, 2);

                            }

                        }
                        //device testig stats
                        //                      if (iosBuild) {
                        //                          Imgproc.putText (rgbaMat, " W:" + rgbaMat.width () + " H:" + rgbaMat.height () + " | analysing frame every " + secondsBtwProcessing + "s", new Point (5, rgbaMat.height () - 20), 2, 1.5, UIgreen, 2, Imgproc.LINE_AA, false);
                        //                      }           

                        if (heatmap)
                        {
                            heatmapFlag = true;
                            //actual analysis
                            rgbaMat.create(rgbaMat.size(), CvType.CV_8UC4);

                            heatmapMat = resizeMat.clone();

                            //print pixels
                            for (int indY = 0; indY < heatmapMat.cols(); indY++)
                            {
                                for (int indX = 0; indX < heatmapMat.rows(); indX++)
                                {
                                    //                                  Debug.Log ("(" + Math.Floor (heatmapMat.get (indX, indY) [0]).ToString () + ", " + Math.Floor (heatmapMat.get (indX, indY) [1]).ToString () + ", " + Math.Floor (heatmapMat.get (indX, indY) [2]).ToString () + ")");
                                    double _hue = (float)Math.Floor(heatmapMat.get(indX, indY)[0]);

                                    //without mapping hue value
                                    //                                  heatmapMat.put (indX, indY, new double[3]{_hue,255,255 });
                                    //mapping hue value
                                    heatmapMat.put(indX, indY, new double[3] { map((float)_hue, 0, 360, minHue, hueBar), 255, 255 });
                                }
                            }
                            //normalize values
                            Imgproc.GaussianBlur(heatmapMat, heatmapMat, new Size(19, 19), 0, 0);
                            Imgproc.equalizeHist(heatmapMat, heatmapMat);

                            //      
                            //HLS map
                            //                          Imgproc.cvtColor(resizeMat,heatmapMat,Imgproc.COLOR_HLS2RGB);
                            //HSV map
                            Imgproc.cvtColor(heatmapMat, heatmapMat, Imgproc.COLOR_HSV2RGB);
                            Imgproc.resize(heatmapMat, rgbaMat, rgbaMat.size());

                            //                          Utils.matToTexture2D (rgbaMat, texture, colors);

                        }
                        if (!heatmap && heatmapFlag == true)
                        {
                            heatmapFlag = false;
                            rgbaMat.create(rgbaMat.size(), CvType.CV_8UC4);

                        }
                        if (!allowCrop)
                        {
                            if (snapToCenter)
                            {
                                SnapToCenters();
                            }
                        }
                        checkForCentersData();


                        //if (photoTaken && !downloadingPhoto)
                        if (photoTaken)
                        {
                            //Core.flip(rgbMat, rgbaMat, 0);
                            rgbaMat = rgbMat.clone();
                        }

                        if (showDots)
                        {
                            // draw centers
                            if (!allowCrop)
                            {
                                if (weightedAverage)
                                {
                                    if (!faceDetection || (faceDetection && frameCount >= 5 && (frameCount - lastFaceFrame <= numberOfFramesWithNoFace)))
                                    {
                                        if (iosBuild)
                                        {
                                            Imgproc.circle(rgbaMat, averageCenter.point, 8, averageColor, 13, Imgproc.LINE_AA, 0);
                                        }//webgl build 
                                        else
                                        {
                                            Imgproc.circle(rgbaMat, averageCenter.point, 3, averageColor, 7, Imgproc.LINE_AA, 0);
                                        }
                                        //Imgproc.putText (rgbaMat, "  weighted average", averageCenter.point, 2, 2, averageColor, 2, Imgproc.LINE_AA, false);
                                    }
                                }
                                else
                                {
                                    if (!faceDetection || (faceDetection == true && frameCount >= 5 && (frameCount - lastFaceFrame <= numberOfFramesWithNoFace)))
                                    {

                                        for (int c = 0; c < currentCenters.Count; c++)
                                        {

                                            switch (c)
                                            {
                                                case 0:
                                                    if (iosBuild)
                                                    {
                                                        Imgproc.circle(rgbaMat, currentCenters[c].point, 8, red, 13, Imgproc.LINE_AA, 0);
                                                    }
                                                    else
                                                    {
                                                        Imgproc.circle(rgbaMat, currentCenters[c].point, 3, red, 7, Imgproc.LINE_AA, 0);
                                                    }
                                                    //  Imgproc.putText (rgbaMat, "  red", currentCenters [c].point, 2, 2, red, 2,Imgproc.LINE_AA, false);
                                                    break;
                                                case 1:
                                                    if (iosBuild)
                                                    {
                                                        Imgproc.circle(rgbaMat, currentCenters[c].point, 8, green, 13, Imgproc.LINE_AA, 0);
                                                    }
                                                    else
                                                    {
                                                        Imgproc.circle(rgbaMat, currentCenters[c].point, 3, green, 7, Imgproc.LINE_AA, 0);
                                                    }
                                                    //  Imgproc.putText (rgbaMat, "  green", currentCenters [c].point, 2, 2, green, 2,Imgproc.LINE_AA, false);
                                                    break;
                                                case 2:
                                                    if (iosBuild)
                                                    {
                                                        Imgproc.circle(rgbaMat, currentCenters[c].point, 8, blue, 13, Imgproc.LINE_AA, 0);
                                                    }
                                                    else
                                                    {
                                                        Imgproc.circle(rgbaMat, currentCenters[c].point, 3, blue, 7, Imgproc.LINE_AA, 0);
                                                    }
                                                    //  Imgproc.putText (rgbaMat, "  blue", currentCenters [c].point, 2, 2, blue, 2,Imgproc.LINE_AA, false);
                                                    break;
                                                default:
                                                    //                                      Imgproc.circle (rgbaMat, currentCenters [c].point, 8, red, 13, Imgproc.LINE_AA, 0);
                                                    //  Imgproc.putText (rgbaMat, "  default", currentCenters [c].point, 2, 2, red, 2,Imgproc.LINE_AA, false);
                                                    break;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        //trackBar
                        if (showTrackBar)
                        {
                            if (currentCenters[0] != null)
                            {
                                precentageToCenter = TrackbarDiff(averageCenter.point);
                                if (precentageToCenter < 0)
                                    precentageToCenter = 0;
                                //show green rect
                                //                              Debug.Log("harmony " + precentageToCenter);
                                if (precentageToCenter >= 0.98f)
                                {
                                    harmonyRectFeedback = true;
                                }
                                else
                                {
                                    harmonyRectFeedback = false;
                                }

                                //to ui
                                accuracySlider.value = 1 - precentageToCenter;

                                int intTemp = (int)Math.Floor(precentageToCenter * 100);
                                string aText = intTemp.ToString();
                                aText = aText + '%';
                                accuracyText.text = aText;

                                //                              Imgproc.fillPoly (rgbaMat, TriangleBar (precentageToCenter), 
                                //                                  new Scalar( (int)Math.Round ( ((1 - precentageToCenter*precentageToCenter ) * 500)  ),
                                //                                      (int)Math.Round ((precentageToCenter*precentageToCenter) * 235),                                                
                                //                                      0,
                                //                                      255),
                                //                                  Imgproc.LINE_AA, 0, zeroPoint);
                            }
                        }

                        // show harmony feedback (greater than 95%)
                        if (harmonyRectFeedback)
                        {
                            Imgproc.rectangle(rgbaMat, new Point(0, 0), new Point(frameWidth, frameHeight), green, 6, Imgproc.LINE_AA, 0);
                            //                          Imgproc.circle (rgbaMat, new Point (50, frameHeight - 50), 8, green, 12, Imgproc.LINE_AA,0);
                            //                          Imgproc.putText (rgbaMat, "~100%", new Point (50, frameHeight - 50),0,2, green, 1, Imgproc.LINE_AA,true);
                        }


                        //draw face detection
                        if (faceDetection)
                        {
                            if (iosBuild)
                            {
                                Core.flip(rgbaMat, rgbaMat, 0);
                            }
                            else
                            {
                                Core.flip(rgbaMat, rgbaMat, 1);
                            }

                            if ((rects != null && rects.Length > 0))
                            {


                                checkForFacesData();
                                //                              Camera.main.backgroundColor = faceBackgroundColorGray;

                                //                              //draw faces middle point circle
                                if (!photoTaken)
                                {
                                    Imgproc.circle(rgbaMat, new Point(currentFacePoints[2] + (currentFacePoints[0] - currentFacePoints[2]) / 2, currentFacePoints[1] + (currentFacePoints[3] - currentFacePoints[1]) / 2), 30, crossColorGreen, 1, Imgproc.LINE_AA, 0);
                                }
                                //                              Imgproc.circle (rgbaMat, new Point (currentFacePoints [2],currentFacePoints [3]), 30, red, 2, Imgproc.LINE_AA, 0);
                                //                              Imgproc.putText (rgbaMat, faceMiddleX + ", " + faceMiddleY, currentFacePoints [1], 1, 6, red,2);

                                //                              Debug.Log ("detect faces " + rects [0]);
                                faceSubmat.create(rects[0].size(), CvType.CV_8UC4);

                                //draw faces
                                faceSubmat = rgbaMat.rowRange(currentFacePoints[1], currentFacePoints[3]).colRange(currentFacePoints[0], currentFacePoints[2]);


                                if (!photoTaken)
                                {
                                    faceSubmat = rgbaMat.rowRange(currentFacePoints[1], currentFacePoints[3]).colRange(currentFacePoints[0], currentFacePoints[2]);
                                    faceSubmat -= faceSubmatColor;
                                }
                                //Debug.Log("photoTaken: " + photoTaken);

                                //change rect color
                                //                              faceBackgroundColor.r = (1 - precentageToCenter) - 0.2f;
                                //                              faceBackgroundColor.g = precentageToCenter;
                                //                              Camera.main.backgroundColor = faceBackgroundColor;

                                Core.bitwise_and(rgbaMat.rowRange(currentFacePoints[1], currentFacePoints[3]).colRange(currentFacePoints[0], currentFacePoints[2]), faceSubmat, rgbaMat.rowRange(currentFacePoints[1], currentFacePoints[3]).colRange(currentFacePoints[0], currentFacePoints[2]));

                                if (!photoTaken)
                                {
                                    if (greenRectFeedback)
                                    {
                                        Imgproc.rectangle(rgbaMat, new Point(currentFacePoints[0], currentFacePoints[1]), new Point(currentFacePoints[2], currentFacePoints[3]), green, 10, Imgproc.LINE_AA, 0);
                                    }
                                    else
                                    {
                                        Imgproc.rectangle(rgbaMat, new Point(currentFacePoints[0], currentFacePoints[1]), new Point(currentFacePoints[2], currentFacePoints[3]), red, 1, Imgproc.LINE_AA, 0);

                                    }
                                }
                            }
                            else
                            {
                                if ((frameCount >= 15 && (frameCount - lastFaceFrame <= numberOfFramesWithNoFace)))
                                {
                                    //draw faces middle point circle
                                    Imgproc.circle(rgbaMat, new Point(currentFacePoints[2] + (currentFacePoints[0] - currentFacePoints[2]) / 2, currentFacePoints[1] + (currentFacePoints[3] - currentFacePoints[1]) / 2), 30, crossColor, 1, Imgproc.LINE_AA, 0);

                                    if (!photoTaken)
                                    {
                                        faceSubmat = rgbaMat.rowRange(currentFacePoints[1], currentFacePoints[3]).colRange(currentFacePoints[0], currentFacePoints[2]);
                                        faceSubmat -= faceSubmatColor;
                                    }
                                    //change rect color
                                    //                                  faceBackgroundColor.r = (1 - precentageToCenter) - 0.2f;
                                    //                                  faceBackgroundColor.g = precentageToCenter;
                                    //                                  Camera.main.backgroundColor = faceBackgroundColor;

                                    Core.bitwise_and(rgbaMat.rowRange(currentFacePoints[1], currentFacePoints[3]).colRange(currentFacePoints[0], currentFacePoints[2]), faceSubmat, rgbaMat.rowRange(currentFacePoints[1], currentFacePoints[3]).colRange(currentFacePoints[0], currentFacePoints[2]));

                                    //flicker
                                    //                                  if (greenRectFeedback && frameCount % 2 == 0) {
                                    if (!photoTaken)
                                    {
                                        if (greenRectFeedback)
                                        {
                                            Imgproc.rectangle(rgbaMat, new Point(currentFacePoints[0], currentFacePoints[1]), new Point(currentFacePoints[2], currentFacePoints[3]), green, 10, Imgproc.LINE_AA, 0);
                                        }
                                        else
                                        {
                                            Imgproc.rectangle(rgbaMat, new Point(currentFacePoints[0], currentFacePoints[1]), new Point(currentFacePoints[2], currentFacePoints[3]), red, 1, Imgproc.LINE_AA, 0);
                                        }
                                    }
                                    //opposite
                                    //                                  rgbaMat -= new Scalar (0, 0, 0, 150);
                                    //                                  Core.bitwise_or(rgbaMat.rowRange (vertRange).colRange (horiRange),faceSubmat,rgbaMat.rowRange (vertRange).colRange (horiRange));

                                }
                            }

                        }

                        if (cross)
                        {

                            if (webglBuild)
                            {
                                Imgproc.circle(rgbaMat, crossPoint, 18, crossColor, 2, Imgproc.LINE_AA, 0);
                            }
                            else
                            {
                                Imgproc.circle(rgbaMat, crossPoint, 30, crossColor, 2, Imgproc.LINE_AA, 0);
                            }
                            //                          for (int dotted = 10; dotted <= (frameWidth / 2); dotted += 10) {
                            //                              if (dotted % 40 == 10 && dotted >= 110) {
                            //                                  //top left
                            //                                  Imgproc.line (rgbaMat, new Point ((int)(frameWidth / 2) - (dotted)     , (int)((frameHeight / 2) - ((frameHeight/frameWidth) * (dotted)))  ), 
                            //                                                         new Point ((int)(frameWidth / 2) - (10 + dotted), (int)((frameHeight / 2) - ((frameHeight/frameWidth) * (10 + dotted)))), crossColor, 1, Imgproc.LINE_AA, 0);
                            //                                  //buttom left
                            //                                  Imgproc.line (rgbaMat, new Point ((int)(frameWidth / 2) - (dotted),      (int)((frameHeight / 2) + ((frameHeight / frameWidth) * (dotted)))), 
                            //                                                         new Point ((int)(frameWidth / 2) - (10 + dotted), (int)((frameHeight / 2) + ((frameHeight / frameWidth) * (10 + dotted)))), crossColor, 1, Imgproc.LINE_AA, 0);
                            //                                  //buttom right
                            //                                  Imgproc.line (rgbaMat, new Point ((int)(frameWidth / 2) + (dotted),      (int)((frameHeight / 2) + ((frameHeight / frameWidth) * (dotted)))), 
                            //                                                         new Point ((int)(frameWidth / 2) + (10 + dotted), (int)((frameHeight / 2) + ((frameHeight / frameWidth) * (10 + dotted)))), crossColor, 1, Imgproc.LINE_AA, 0);
                            //                                  //top right
                            //                                  Imgproc.line (rgbaMat, new Point ((int)(frameWidth / 2) + (dotted),      (int)((frameHeight / 2) - ((frameHeight / frameWidth) * (dotted)))), 
                            //                                                         new Point ((int)(frameWidth / 2) + (10 + dotted), (int)((frameHeight / 2) - ((frameHeight / frameWidth) * (10 + dotted)))), crossColor, 1, Imgproc.LINE_AA, 0);
                            //                              }
                            //                          }

                            //top to buttom
                            Imgproc.line(rgbaMat, new Point((int)(frameWidth / 2), (int)0), new Point((int)(frameWidth / 2), (int)(frameHeight * 0.3333)), crossColor, 1, Imgproc.LINE_AA, 0);
                            Imgproc.line(rgbaMat, new Point((int)(frameWidth / 2), (int)frameHeight), new Point((int)(frameWidth / 2), (int)(frameHeight * 0.6666)), crossColor, 1, Imgproc.LINE_AA, 0);

                            //left to right
                            Imgproc.line(rgbaMat, new Point((int)(0), (int)(frameHeight / 2)), new Point((int)(frameWidth * 0.3333), (int)(frameHeight / 2)), crossColor, 1, Imgproc.LINE_AA, 0);
                            Imgproc.line(rgbaMat, new Point((int)(frameWidth), (int)(frameHeight / 2)), new Point((int)(frameWidth * 0.6666), (int)(frameHeight / 2)), crossColor, 1, Imgproc.LINE_AA, 0);

                            //topleft to bottom right
                            Imgproc.line(rgbaMat, new Point((int)(0), (int)(0)), new Point((int)(frameWidth * 0.3333), (int)(frameHeight * 0.3333)), crossColor, 1, Imgproc.LINE_AA, 0);
                            Imgproc.line(rgbaMat, new Point((int)(frameWidth), (int)(frameHeight)), new Point((int)(frameWidth * 0.6666), (int)(frameHeight * 0.6666)), crossColor, 1, Imgproc.LINE_AA, 0);

                            //buttom left to top right
                            Imgproc.line(rgbaMat, new Point((int)(0), (int)(frameHeight)), new Point((int)(frameWidth * 0.3333), (int)(frameHeight * 0.6666)), crossColor, 1, Imgproc.LINE_AA, 0);
                            Imgproc.line(rgbaMat, new Point((int)(frameWidth), (int)(0)), new Point((int)(frameWidth * 0.6666), (int)(frameHeight * 0.3333)), crossColor, 1, Imgproc.LINE_AA, 0);
                            //Debug.Log("cross");

                        }
                        if (centerCross)
                        {
                            //Debug.Log("centercross");
                            // positions comments refers to landscape mode (webGL)


                            Imgproc.line(rgbaMat, new Point((int)(frameWidth * 0.55), (int)(frameHeight * 0.6)), new Point((int)(frameWidth * 0.55), (int)(frameHeight * 0.65)), crossColor, 0, Imgproc.LINE_AA, 0);
                            Imgproc.line(rgbaMat, new Point((int)(frameWidth * 0.45), (int)(frameHeight * 0.6)), new Point((int)(frameWidth * 0.45), (int)(frameHeight * 0.65)), crossColor, 0, Imgproc.LINE_AA, 0);

                            Imgproc.line(rgbaMat, new Point((int)(frameWidth * 0.55), (int)(frameHeight * 0.4)), new Point((int)(frameWidth * 0.55), (int)(frameHeight * 0.35)), crossColor, 0, Imgproc.LINE_AA, 0);
                            Imgproc.line(rgbaMat, new Point((int)(frameWidth * 0.45), (int)(frameHeight * 0.4)), new Point((int)(frameWidth * 0.45), (int)(frameHeight * 0.35)), crossColor, 0, Imgproc.LINE_AA, 0);
                            // positions comments refers to landscape mode (webGL)

                            Imgproc.line(rgbaMat, new Point((int)(frameWidth * 0.55), (int)(frameHeight * 0.35)), new Point((int)(frameWidth * 0.52), (int)(frameHeight * 0.35)), crossColor, 0, Imgproc.LINE_AA, 0);
                            Imgproc.line(rgbaMat, new Point((int)(frameWidth * 0.45), (int)(frameHeight * 0.65)), new Point((int)(frameWidth * 0.48), (int)(frameHeight * 0.65)), crossColor, 0, Imgproc.LINE_AA, 0);

                            Imgproc.line(rgbaMat, new Point((int)(frameWidth * 0.55), (int)(frameHeight * 0.65)), new Point((int)(frameWidth * 0.52), (int)(frameHeight * 0.65)), crossColor, 0, Imgproc.LINE_AA, 0);
                            Imgproc.line(rgbaMat, new Point((int)(frameWidth * 0.45), (int)(frameHeight * 0.35)), new Point((int)(frameWidth * 0.48), (int)(frameHeight * 0.35)), crossColor, 0, Imgproc.LINE_AA, 0);


                            if (webglBuild)
                            {
                                Imgproc.circle(rgbaMat, crossPoint, 18, crossColor, 1, Imgproc.LINE_AA, 0);
                            }
                            else
                            {
                                Imgproc.circle(rgbaMat, crossPoint, 30, crossColor, 2, Imgproc.LINE_AA, 0);
                            }

                        }

                        if (guide)
                        {
                            //                          for (int dotted = 10; dotted <= (frameHeight * 2) ; dotted += 10) {
                            //                              if (dotted % 40 == 10) {
                            //                                  Imgproc.line (rgbaMat, new Point ((int)(frameWidth / 3), dotted), new Point ((int)(frameWidth / 3), (10 + dotted)), crossColor, 0, Imgproc.LINE_AA, 0);
                            //                                  Imgproc.line (rgbaMat, new Point ((int)(frameWidth * 2 / 3), dotted), new Point ((int)(frameWidth * 2 / 3), (10 + dotted)), crossColor, 0, Imgproc.LINE_AA, 0);
                            //                                  Imgproc.line (rgbaMat, new Point (dotted, (int)frameHeight/3), new Point ((10 + dotted), (int)frameHeight/3), crossColor, 0, Imgproc.LINE_AA, 0);
                            //                                  Imgproc.line (rgbaMat, new Point (dotted, (int)frameHeight*2/3), new Point ((10 + dotted), (int)(int)frameHeight*2/3), crossColor, 0, Imgproc.LINE_AA, 0);
                            //                              }
                            //                          
                            //                          }
                            Imgproc.line(rgbaMat, new Point((int)(frameWidth / 3), (int)0), new Point((int)(frameWidth / 3), (int)frameHeight), crossColor, 1, Imgproc.LINE_AA, 0);
                            Imgproc.line(rgbaMat, new Point((int)(frameWidth * 2 / 3), (int)0), new Point((int)(frameWidth * 2 / 3), (int)frameHeight), crossColor, 1, Imgproc.LINE_AA, 0);
                            Imgproc.line(rgbaMat, new Point((int)0, (int)(frameHeight / 3)), new Point((int)(frameWidth), (int)(frameHeight / 3)), crossColor, 1, Imgproc.LINE_AA, 0);
                            Imgproc.line(rgbaMat, new Point((int)0, (int)(frameHeight * 2 / 3)), new Point((int)(frameWidth), (int)(frameHeight * 2 / 3)), crossColor, 1, Imgproc.LINE_AA, 0);
                            //Debug.Log("guide");
                        }


                    }

                    if (allowCrop)
                    {
                        if (secondPoint != null && colsRange != null && deltaCols > 0 && deltaRows > 0)
                        {

                           

                            Imgproc.blur(rgbReferenceMat, rgbaMat, new Size(25, 25));
                            rgbaMat = rgbReferenceMat.clone() - new Scalar(85, 85, 85);
                            //                          rgbaMat = rgbaMat - new Scalar (50, 50, 50) ;




                            rgbReferenceMat.rowRange(rowsRange).colRange(colsRange).copyTo(rgbaMat.rowRange(rowsRange).colRange(colsRange));
                            //crop border rect
                            Imgproc.rectangle(rgbaMat, firstMouseDown, secondPoint, white, 1, Imgproc.LINE_AA, 0);

                            if (showDots)
                            {
                                // draw centers
                                if (weightedAverage)
                                {
                                    if (!faceDetection)
                                    {
                                        if (iosBuild)
                                        {
                                            Imgproc.circle(rgbaMat, averageCenter.point, 8, averageColor, 13, Imgproc.LINE_AA, 0);
                                        }//webgl build 
                                        else
                                        {
                                            Imgproc.circle(rgbaMat, averageCenter.point, 3, averageColor, 7, Imgproc.LINE_AA, 0);
                                        }
                                        //Imgproc.putText (rgbaMat, "  weighted average", averageCenter.point, 2, 2, averageColor, 2, Imgproc.LINE_AA, false);
                                    }
                                }
                                else
                                {
                                    if (!faceDetection)
                                    {

                                        for (int c = 0; c < currentCenters.Count; c++)
                                        {

                                            switch (c)
                                            {
                                                case 0:
                                                    if (iosBuild)
                                                    {
                                                        Imgproc.circle(rgbaMat, currentCenters[c].point, 8, red, 13, Imgproc.LINE_AA, 0);
                                                    }
                                                    else
                                                    {
                                                        Imgproc.circle(rgbaMat, currentCenters[c].point, 3, red, 7, Imgproc.LINE_AA, 0);
                                                    }
                                                    //  Imgproc.putText (rgbaMat, "  red", currentCenters [c].point, 2, 2, red, 2,Imgproc.LINE_AA, false);
                                                    break;
                                                case 1:
                                                    if (iosBuild)
                                                    {
                                                        Imgproc.circle(rgbaMat, currentCenters[c].point, 8, green, 13, Imgproc.LINE_AA, 0);
                                                    }
                                                    else
                                                    {
                                                        Imgproc.circle(rgbaMat, currentCenters[c].point, 3, green, 7, Imgproc.LINE_AA, 0);
                                                    }
                                                    //  Imgproc.putText (rgbaMat, "  green", currentCenters [c].point, 2, 2, green, 2,Imgproc.LINE_AA, false);
                                                    break;
                                                case 2:
                                                    if (iosBuild)
                                                    {
                                                        Imgproc.circle(rgbaMat, currentCenters[c].point, 8, blue, 13, Imgproc.LINE_AA, 0);
                                                    }
                                                    else
                                                    {
                                                        Imgproc.circle(rgbaMat, currentCenters[c].point, 3, blue, 7, Imgproc.LINE_AA, 0);
                                                    }
                                                    //  Imgproc.putText (rgbaMat, "  blue", currentCenters [c].point, 2, 2, blue, 2,Imgproc.LINE_AA, false);
                                                    break;
                                                default:
                                                    //                                      Imgproc.circle (rgbaMat, currentCenters [c].point, 8, red, 13, Imgproc.LINE_AA, 0);
                                                    //  Imgproc.putText (rgbaMat, "  default", currentCenters [c].point, 2, 2, red, 2,Imgproc.LINE_AA, false);
                                                    break;
                                            }
                                        }
                                    }
                                }

                                //crop center point
                                if (allowCrop)
                                {
                                    if (!faceDetection)
                                    {
                                        Imgproc.circle(rgbaMat, new Point(firstMouseDown.x + (secondPoint.x - firstMouseDown.x) / 2, firstMouseDown.y + (secondPoint.y - firstMouseDown.y) / 2), 20, crossColor, 2, Imgproc.LINE_AA, 0);
                                    }
                                }
                            }
                            //                      Imgproc.resize(locationMat,rgbaMat,rgbaMat.size());

                            Utils.matToTexture2D(rgbaMat, texture, colors);
                            //                      Debug.Log ("showing crop display");


                        }

                    }
                    else
                    {
          

                        //Utils.matToTexture2D(rgbaMat, texture, colors);

                        Utils.matToTexture2D(rgbaMat, texture, colors);
                        Core.flip(rgbaMat, rgbaMat, 0);

                        //////////////////////
                        /// rgba mat for drawing - interjection

                    }

                    flippedForPhoto = false;
                }
                else
                {
                    Debug.Log("outside");
                    if (faceDetection && !flippedForPhoto)
                    {
                        //                          Core.flip (rgbCloneMat, rgbCloneMat, 0);
                        Core.flip(rgbMat, rgbMat, 0);
                        Core.flip(rgbaMat, rgbaMat, 0);
                        flippedForPhoto = true;
                        //Utils.matToTexture2D(rgbMat, texture, colors);
                        Debug.Log("there");

                    }
                    else
                    {

                        //                  // photo border
                        //                  photoWhiteMat.colRange (0, 20).copyTo (rgbMat.colRange (0, 20));
                        //                  photoWhiteMat.colRange (photoWhiteMat.cols () - 20, photoWhiteMat.cols ()).copyTo (rgbMat.colRange (photoWhiteMat.cols () - 20, photoWhiteMat.cols ()));
                        //                  photoWhiteMat.rowRange (0, 20).copyTo (rgbMat.rowRange (0, 20));
                        //                  photoWhiteMat.rowRange (photoWhiteMat.rows () - 20, photoWhiteMat.rows ()).copyTo (rgbMat.rowRange (photoWhiteMat.rows () - 20, photoWhiteMat.rows ()));
                        //Debug.Log("here");
                        //Utils.matToTexture2D(rgbMat, texture, colors);
                    }

                }
                frameCount++;
                //recent change flip
                if (allowCrop && fileUpload==null)
                {
                    Core.flip(rgbaMat, rgbaMat, 0);
                }
                if (allowCrop && fileUpload )
                {
                    Core.flip(rgbaMat, rgbaMat, 0);
                }
            }

        }
    
        public void SnapToCenters()
        {
            if (frameCount >= 0 && displayCenters.Count > 0)
            {
                for (int q = 0; q < displayCenters.Count; q++)
                {
                    //                  Debug.Log ("\n displayCenters [q].point.x: " + displayCenters [q].point.x);

                    if (!faceDetection)
                    {
                        //channel center inside rect
                        if (displayCenters[q].point.x >= (frameWidth / 2) - snapToCenterSize
                            && displayCenters[q].point.x <= (frameWidth / 2) + snapToCenterSize
                            && displayCenters[q].point.y >= (frameHeight / 2) - snapToCenterSize
                            && displayCenters[q].point.y <= (frameHeight / 2) + snapToCenterSize)
                        {

                            displayCenters[q].point = middleOfTheFramePoint;
                        }


                    }
                    else
                    {
                        //done (after trackbar)

                        //channel center inside rect
                        if (rects != null && rects.Length > 0 || frameCount >= 5 && (frameCount - lastFaceFrame <= numberOfFramesWithNoFace))
                        {

                            //                          faceMiddleX = (int)Math.Round( frameWidth - (( rects [0].x + rects [0].width/2)/resizeFactor));
                            //                          faceMiddleY = (int)Math.Round( ( rects [0].y + rects [0].height/2)/resizeFactor);

                            //                          Debug.Log ("face middle: " + faceMiddleX + ", " + faceMiddleY);
                            if ((displayCenters[q].point.x) >= (faceMiddleX) - snapToCenterSizeFace
                                && (displayCenters[q].point.x) <= (faceMiddleX) + snapToCenterSizeFace
                                //flip y
                                && (frameHeight - displayCenters[q].point.y) >= (faceMiddleY) - snapToCenterSizeFace
                                && (frameHeight - displayCenters[q].point.y) <= (faceMiddleY) + snapToCenterSizeFace)
                            {

                                displayCenters[q].point = new Point(faceMiddleX, frameHeight - faceMiddleY);
                                greenRectFeedback = true;
                            }
                            else
                            {
                                greenRectFeedback = false;
                            }

                        }
                    }
                }
            }
        }
        public void checkForCentersData()
        {
            //check for first tiem frame processing - Initiate centers - place in the center
            if (displayCenters[0].point.x.ToString() == "NaN")
            {
                displayCenters.Clear();
                for (int o = 0; o <= 2; o++)
                {
                    displayCenters.Add(new Centers(o, zeroPoint));
                }
            }

            if (displayCenters != null && currentCenters.Count == 0 || !centersFlag)
            {
                currentCenters.Clear();

                //initiate currentCenters
                for (int d = 0; d < displayCenters.Count; d++)
                {
                    currentCenters.Add(new Centers(d, new Point(rgbaMat.width() * 0.5, rgbaMat.height() * 0.5)));
                }

            }

            // currentCenters step
            if (displayCenters.Count > 1)
            {

                for (int h = 0; h < displayCenters.Count; h++)
                {

                    currentCenters[h].point.x = (int)Math.Round(speed * (currentCenters[h].point.x - 1) + displayCenters[h].point.x * (1 - speed));
                    currentCenters[h].point.y = (int)Math.Round(speed * (currentCenters[h].point.y + 1) + displayCenters[h].point.y * (1 - speed));

                }

                //centers center - weighted average
                averageCenter.point = WeightedAverageThree(currentCenters[0].point, currentCenters[1].point, currentCenters[2].point);
            }

            centersFlag = true;

        }
        public void checkForFacesData()
        {
            //check for first tiem frame processing - Initiate centers - place in the center
            if (displayFacePoints == null)
            {
                displayFacePoints.Clear();
                //for (int o = 0; o <= 3; o++) {
                displayFacePoints.Add((int)(rects[0].x / resizeFactor));
                displayFacePoints.Add((int)(rects[0].y / resizeFactor));
                displayFacePoints.Add((int)(rects[0].width / resizeFactor));
                displayFacePoints.Add((int)(rects[0].height / resizeFactor));
                //}
            }

            if (displayFacePoints != null && currentFacePoints.Count == 0 || !facesFlag)
            {
                currentFacePoints.Clear();
                //initiate currentCenters
                for (int d = 0; d < displayFacePoints.Count; d++)
                {
                    currentFacePoints.Add((int)(rects[0].x / resizeFactor));
                    currentFacePoints.Add((int)(rects[0].y / resizeFactor));
                    currentFacePoints.Add((int)((rects[0].x + rects[0].width) / resizeFactor));
                    currentFacePoints.Add((int)((rects[0].y + rects[0].height) / resizeFactor));
                }
            }

            // currentCenters step
            if (displayFacePoints.Count > 1)
            {
                for (int h = 0; h < displayFacePoints.Count; h++)
                {
                    currentFacePoints[h] = (int)Math.Round(speed * currentFacePoints[h] + displayFacePoints[h] * (1 - speed));
                    //                  Debug.Log ("currentFacePoints[h]: " + currentFacePoints [h]);
                }


            }
            facesFlag = true;

        }
        public void resizeUploaded()
        {

            float fileUploadRatio;


            //          Debug.Log ("<unity file> file width: " + fileUploadMat.width () + " file height: " + fileUploadMat.height ());

            if (webglBuild)
            {

                fileUploadRatio = fileUploadMat.width();
                fileUploadRatio = fileUploadRatio / fileUploadMat.height();
                //Debug.Log("<unity file> file upload ratio: " + fileUploadRatio);


                if (fileUploadRatio != frameWidth / frameHeight)
                {


                    //Debug.Log("<unity file> ");
                    //Debug.Log("<unity file image> frameWidth / frameHeight= " + (frameWidth / frameHeight));
                    //Debug.Log("<unity file image> frameWidth = " + frameWidth);
                    //Debug.Log("<unity file image> frameHeight = " + frameHeight);
                    //Debug.Log("<unity file image> file upload ratio: " + fileUploadRatio);

                    if (fileUploadRatio <= frameWidth / frameHeight)
                    {
                        //                      Debug.Log ("<unity file> image upload ratio to height (narrower image)");
                        Debug.Log("<unity file image> NARROWER");

                        //resize to frame height/ upload height
                        Imgproc.resize(fileUploadMat, fileUploadMat, new Size(), (frameHeight / fileUploadMat.height()), (frameHeight / fileUploadMat.height()), Imgproc.INTER_NEAREST);

                        //get width diff
                        int frameDelta = (int)Math.Round((float)frameWidth) - fileUploadMat.width();
                        if (frameDelta < 0)
                        {
                            frameDelta = frameDelta - (2 * frameDelta);
                        }

                        //move to frame size mat and back
                        fileUploadResizedMat.create(new Size(frameWidth, frameHeight), CvType.CV_8UC3);
                        OpenCVForUnity.CoreModule.Core.copyMakeBorder(fileUploadMat, fileUploadResizedMat, 0, 0, frameDelta / 2, frameDelta / 2, Core.BORDER_CONSTANT);
                        fileUploadMat = fileUploadResizedMat.clone();
                    }
                    else
                    {
                        //                      Debug.Log ("<unity file> image upload ratio to width (wider image)");
                        //Debug.Log("<unity file image> WIDER");

                        //resize to frame height/ upload height
                        Imgproc.resize(fileUploadMat, fileUploadMat, new Size(), (frameWidth / fileUploadMat.width()), (frameWidth / fileUploadMat.width()), Imgproc.INTER_NEAREST);
                        //Debug.Log("<unity file> fileUploadMat.height: " + fileUploadMat.height() + " fileUploadMat.width: " + fileUploadMat.width());
                        //get width diff
                        int frameDelta = (int)Math.Round((float)frameHeight) - fileUploadMat.height();
                        if (frameDelta < 0)
                        {
                            frameDelta = frameDelta - (2 * frameDelta);
                        }

                        //log
                        //Debug.Log("<unity file image> resize ratio: " + (frameWidth / fileUploadMat.height()));
                        //Debug.Log("<unity file image> frameDelta: " + frameDelta);
                        //Debug.Log("<unity file image> frameHeight: " + frameHeight);
                        //Debug.Log("<unity file image> * frameWidth: " + frameWidth);
                        //Debug.Log("<unity file image> fileUploadMat.width: " + fileUploadMat.width());
                        //Debug.Log("<unity file image> * fileUploadMat.height: " + fileUploadMat.height());
                        //Debug.Log("<unity file image> screens width: " + Screen.width);
                        //Debug.Log("<unity file image> screens height: " + Screen.height);

                        fileUploadResizedMat.create(new Size(frameWidth, frameHeight), CvType.CV_8UC3);
                        OpenCVForUnity.CoreModule.Core.copyMakeBorder(fileUploadMat, fileUploadResizedMat, frameDelta / 2, frameDelta / 2, 0, 0, Core.BORDER_CONSTANT);
                        fileUploadMat = fileUploadResizedMat.clone();
                    }
                }
                else
                {
                    Imgproc.resize(fileUploadMat, fileUploadMat, new Size(frameWidth, frameHeight));
                }
            }

            if (iosBuild)
            {

                fileUploadRatio = fileUploadMat.width();
                fileUploadRatio = fileUploadRatio / fileUploadMat.height();
                Debug.Log("<unity file> ");
                Debug.Log("<unity file> ");
                Debug.Log("<unity file> file upload ratio: " + fileUploadRatio);


                if (fileUploadRatio != frameHeight / frameWidth)
                {
                    Debug.Log("<unity file> frameHeight / frameWidth = " + frameHeight / frameWidth);
                    Debug.Log("<unity file> frameWidth, frameHeight  = " + frameWidth + ", " + frameHeight);
                    Debug.Log("<unity file> before resize: fileUploadMat.height: " + fileUploadMat.height() + " fileUploadMat.width: " + fileUploadMat.width());

                    if (fileUploadRatio < frameHeight / frameWidth)
                    {
                        Debug.Log("<unity file> image upload ratio to height (narrower image)");

                        //resize to frame height/ upload height
                        Imgproc.resize(fileUploadMat, fileUploadMat, new Size(), (frameWidth / fileUploadMat.height()), (frameWidth / fileUploadMat.height()), Imgproc.INTER_NEAREST);
                        Debug.Log("<unity file> after resize: fileUploadMat.height: " + fileUploadMat.height() + " fileUploadMat.width: " + fileUploadMat.width());

                        //get width diff
                        int frameDelta = (int)Math.Round((float)frameHeight) - fileUploadMat.width();

                        if (frameDelta < 0)
                        {
                            frameDelta = frameDelta - (2 * frameDelta);
                        }
                        //rotate upload image
                        Core.transpose(fileUploadMat, fileUploadMat);
                        Core.flip(fileUploadMat, fileUploadMat, 0);
                        Debug.Log("<unity file> rtansposed and flipped 0");

                        //log
                        Debug.Log("<unity file> resize ratio: " + (frameWidth / fileUploadMat.height()));
                        Debug.Log("<unity file> frameDelta: " + frameDelta);
                        Debug.Log("<unity file> frameHeight: " + frameHeight);
                        Debug.Log("<unity file> * frameWidth: " + frameWidth);
                        Debug.Log("<unity file> fileUploadMat.width: " + fileUploadMat.width());
                        Debug.Log("<unity file> * fileUploadMat.height: " + fileUploadMat.height());
                        Debug.Log("screens width: " + Screen.width);
                        Debug.Log("screens height: " + Screen.height);

                        //move to frame size mat and back
                        fileUploadResizedMat.create(new Size(frameWidth, frameHeight), CvType.CV_8UC3);
                        OpenCVForUnity.CoreModule.Core.copyMakeBorder(fileUploadMat, fileUploadResizedMat, frameDelta / 2, frameDelta / 2, 0, 0, Core.BORDER_CONSTANT);
                        fileUploadMat = fileUploadResizedMat.clone();
                    }
                    else
                    {
                        Debug.Log("<unity file> image upload ratio to width (wider image)");

                        //resize to frame height/ upload height
                        Imgproc.resize(fileUploadMat, fileUploadMat, new Size(), (frameHeight / fileUploadMat.width()), (frameHeight / fileUploadMat.width()), Imgproc.INTER_NEAREST);
                        Debug.Log("<unity file> after resize: fileUploadMat.height: " + fileUploadMat.height() + " fileUploadMat.width: " + fileUploadMat.width());
                        //get width diff
                        int frameDelta = (int)Math.Floor((float)frameWidth) - fileUploadMat.height();

                        if (frameDelta < 0)
                        {
                            frameDelta = frameDelta - (2 * frameDelta);
                        }

                        //rotate upload image
                        Core.transpose(fileUploadMat, fileUploadMat);
                        Core.flip(fileUploadMat, fileUploadMat, 0);
                        Debug.Log("<unity file> rtansposed and flipped 0");

                        //log
                        Debug.Log("<unity file> resize ratio: " + (frameHeight / fileUploadMat.width()));
                        Debug.Log("<unity file> frameDelta: " + frameDelta);
                        Debug.Log("<unity file> frameHeight: " + frameHeight);
                        Debug.Log("<unity file> fileUploadMat.width: " + fileUploadMat.width());
                        Debug.Log("<unity file> * fileUploadMat.height: " + fileUploadMat.height());
                        Debug.Log("<unity file> * frameWidth: " + frameWidth);
                        Debug.Log("screens width: " + Screen.width);
                        Debug.Log("screens height: " + Screen.height);

                        fileUploadResizedMat.create(new Size(frameWidth, frameHeight), CvType.CV_8UC3);
                        OpenCVForUnity.CoreModule.Core.copyMakeBorder(fileUploadMat, fileUploadResizedMat, 0, 0, frameDelta / 2, frameDelta / 2, Core.BORDER_CONSTANT);
                        fileUploadMat = fileUploadResizedMat.clone();

                        Imgproc.resize(fileUploadMat, fileUploadMat, new Size(frameWidth, frameHeight));
                    }
                }
                else
                {

                    //rotate upload image
                    Core.transpose(fileUploadMat, fileUploadMat);
                    Core.flip(fileUploadMat, fileUploadMat, 0);
                    Debug.Log("<unity file> rtansposed and flipped 0");

                    Imgproc.resize(fileUploadMat, fileUploadMat, new Size(frameWidth, frameHeight));
                }
            }
        }

        ////////////
        /// A-Sync Frame Processing
        ///////////

        private IEnumerator processFrame()
        {
            while (true && webCamTexture != null && webCamTexture.width > 100)
            {

                //resize down
                if (resizeMat != null)
                {


                    ////////////
                    /// file Uplaod
                    ///////////

                    if (fileUpload != null)
                    {

                        //read image to mat

                        if (iosBuild)
                        {
                            //                          GetImageSizeIos (fileUpload, out assetWidth, out assetHeight);
                        }
                        if (webglBuild)
                        {
                            //                          GetImageSize (fileUpload, out assetWidth, out assetHeight);

                        }
                        //                      Debug.Log ("file upload dimentions: " + assetWidth +", " + assetHeight);


                        if (development)
                        {
#if UNITY_EDITOR
                            //read image to mat
                            //                      Imgcodecs.imread(



#endif
                        }

                        if (webglBuild)
                        {
                            if (fileuploadFirstItiration)
                            {
                                fileuploadFirstItiration = false;

                            fileUploadFlag = true;

                            Debug.Log("<unity> fileupload - FIRST itiration");
                            //                          if (!afterImageEncoding) {


                            //                              if (fileColors == null || fileColors.Length != fileUpload.width * fileUpload.height)
                            //                                  fileColors = new Color32[fileUpload.width * fileUpload.height];
                            //                              if (fileTexture == null || fileTexture.width != fileUpload.width || fileTexture.height != fileUpload.height)
                            //                                  fileTexture = new Texture2D (fileUpload.width, fileUpload.height, TextureFormat.RGBA32, false);

                            //                      fileUploadMat = new Mat (fileTexture.height, fileTexture.width, CvType.CV_8UC3);
                            //
                            //                      Utils.texture2DToMat (fileTexture, fileUploadMat);
                            //                      Imgproc.resize (fileUploadMat, resizeMat, resizeSize, 0.5, 0.5, Core.BORDER_DEFAULT);
                            //                      fileTexture.LoadImage (fileUploadData);
                            fileTexture = fileUpload;

                            //                              Debug.Log ("texture dimentions after resize: " + assetWidth + ", " + assetHeight);

                            fileUploadMat = new Mat(fileTexture.height, fileTexture.width, CvType.CV_8UC3);
                            //Debug.Log("fileTexture.width: " + fileTexture.width + ", fileTexture.height: " + fileTexture.height);
                            OpenCVForUnity.UnityUtils.Utils.texture2DToMat(fileTexture, fileUploadMat);
                            //                              Imgproc.resize (fileUploadMat, fileUploadMat, new Size (assetWidth, assetHeight));

                            //resize to frame
                            resizeUploaded();
                            //                              Imgproc.resize (fileUploadMat, fileUploadMat, new Size (frameWidth, frameHeight), 0.5, 0.5, Core.BORDER_DEFAULT);

                            afterImageEncoding = true;
                                                    }

                            //  Utils.matToTexture2D (fileUploadMat, fileUpload, fileColors);
                            if (allowCrop)
                            {
                                if (initialUpload)
                                {
                                    rgbCloneMat = rgbMat.clone();
                                    initialUpload = false;
                                    Debug.Log("inside initial crop image upload");
                                }
                                else
                                {
                                    //Debug.Log("inside crop image upload");
                                    //                                  resizeMat = new Mat (resizeSize, CvType.CV_8UC3, white);

                                    Imgproc.resize(rgbMat, resizeMat, resizeSize);

                                    //                                  rgbReferenceMat.colRange((int)Math.Round (colsRange.start + (deltaCols * rationOfScreen)), (int)Math.Round (colsRange.start + (deltaCols * (1- rationOfScreen)) ))
                                    //                                                                  .rowRange((int)Math.Round (rowsRange.start + (deltaRows * rationOfScreen)), (int)Math.Round ( rowsRange.start + (deltaRows * (1 - rationOfScreen)))).copyTo(resizeMat.colRange((int)Math.Round (colsRange.start + (deltaCols * rationOfScreen)), (int)Math.Round (colsRange.start + (deltaCols * (1- rationOfScreen)) ))
                                    //                                                                  .rowRange((int)Math.Round (rowsRange.start + (deltaRows * rationOfScreen)), (int)Math.Round ( rowsRange.start + (deltaRows * (1 - rationOfScreen)))));
                                    //                                                              
                                    //                                                                  rgbCloneMat.rowRange (rowsRange).colRange (colsRange).copyTo (resizeMat.rowRange (rowsRange).colRange (colsRange));

                                    //                                                                  Imgproc.resize(rgbCloneMat,rgbCloneMat,resizeSize);

                                    //                                                                  rgbCloneMat.rowRange (rowsRange).colRange (colsRange).copyTo (resizeMat.rowRange (rowsRange).colRange (colsRange));
                                }

                            }
                            else
                            {
                                Imgproc.resize(fileUploadMat, resizeMat, resizeSize, 0.5, 0.5, Core.BORDER_DEFAULT);
                            }

                        }
                        if (iosBuild)
                        {

                            ////
                            /// TO-DO: add image encoding switch
                            /// 

                            fileUploadFlag = true;

                            //xcode correction
                            //

                            if (fileuploadFirstItiration)
                            {
                                fileuploadFirstItiration = false;
                                Debug.Log("<unity> fileupload - FIRST itiration");

                                try
                                {
                                    //initiate texture (output) And it's size acordingto file
                                    //                              Debug.Log ("<unity> path: " + Application.persistentDataPath + "/images-from-ios/image.jpeg");
                                    fileUploadData = File.ReadAllBytes(Application.persistentDataPath + "/images-from-ios/image.jpeg");
                                }
                                catch (IOException e)
                                {
                                    Debug.Log("<unity> CAUGHT - incompatible file exception: " + e);
                                    fileUpload = null;
                                    //reload
                                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                                }
                                //                          Debug.Log ("<unity>  file is: " + fileUpload.GetType ());

                                if (fileColors == null || fileColors.Length != fileUpload.width * fileUpload.height)
                                    fileColors = new Color32[fileUpload.width * fileUpload.height];
                                if (fileTexture == null || fileTexture.width != fileUpload.width || fileTexture.height != fileUpload.height)
                                    fileTexture = new Texture2D(fileUpload.width, fileUpload.height, TextureFormat.RGBA32, false);

                                //                      fileUploadMat = new Mat (fileTexture.height, fileTexture.width, CvType.CV_8UC3);
                                //
                                //                      Utils.texture2DToMat (fileTexture, fileUploadMat);
                                //                      Imgproc.resize (fileUploadMat, resizeMat, resizeSize, 0.5, 0.5, Core.BORDER_DEFAULT);
                                fileTexture.LoadImage(fileUploadData);
                                fileUploadMat = new Mat(fileTexture.height, fileTexture.width, CvType.CV_8UC3);

                                OpenCVForUnity.UnityUtils.Utils.texture2DToMat(fileTexture, fileUploadMat);




                                //resize to frame
                                resizeUploaded();
                                //  Utils.matToTexture2D (fileUploadMat, fileUpload, fileColors);
                            }
                            else
                            {
                                Debug.Log("<unity> fileupload - NOT first itiration");
                                GC.Collect();
                            }
                            //resize to original asset
                            //                          Imgproc.resize (fileUploadMat, fileUploadMat, new Size (assetWidth, assetHeight));
                            //rotate for portrait orientation
                            //                          Core.transpose (fileUploadMat, fileUploadMat);



                            if (allowCrop)
                            {
                                if (initialUpload)
                                {
                                    rgbCloneMat = rgbMat.clone();
                                    initialUpload = false;
                                    Debug.Log("inside initial crop image upload");

                                }
                                else
                                {
                                    Debug.Log("inside crop image upload");
                                    //                                  resizeMat = new Mat (resizeSize, CvType.CV_8UC3, white);
                                    Imgproc.resize(rgbMat, resizeMat, resizeSize);

                                    //                                  rgbReferenceMat.colRange((int)Math.Round (colsRange.start + (deltaCols * rationOfScreen)), (int)Math.Round (colsRange.start + (deltaCols * (1- rationOfScreen)) ))
                                    //                                                                  .rowRange((int)Math.Round (rowsRange.start + (deltaRows * rationOfScreen)), (int)Math.Round ( rowsRange.start + (deltaRows * (1 - rationOfScreen)))).copyTo(resizeMat.colRange((int)Math.Round (colsRange.start + (deltaCols * rationOfScreen)), (int)Math.Round (colsRange.start + (deltaCols * (1- rationOfScreen)) ))
                                    //                                                                  .rowRange((int)Math.Round (rowsRange.start + (deltaRows * rationOfScreen)), (int)Math.Round ( rowsRange.start + (deltaRows * (1 - rationOfScreen)))));
                                    //                                                              
                                    //                                                                  rgbCloneMat.rowRange (rowsRange).colRange (colsRange).copyTo (resizeMat.rowRange (rowsRange).colRange (colsRange));

                                    //                                                                  Imgproc.resize(rgbCloneMat,rgbCloneMat,resizeSize);

                                    //                                                                  rgbCloneMat.rowRange (rowsRange).colRange (colsRange).copyTo (resizeMat.rowRange (rowsRange).colRange (colsRange));
                                }

                            }
                            else
                            {
                                Imgproc.resize(fileUploadMat, resizeMat, resizeSize, 0.5, 0.5, Core.BORDER_DEFAULT);
                            }
                        }


                    }
                    else
                    {
                        //reset encoding flag
                        afterImageEncoding = false;

                        if (fileUploadFlag)
                        {
                            //return texture to camera after file upload
                            gameObject.GetComponent<Renderer>().material.mainTexture = texture;
                            fileUploadFlag = false;
                        }
                        Imgproc.resize(rgbMat, resizeMat, resizeSize, 0.5, 0.5, Core.BORDER_DEFAULT);
                        //                      Imgproc.resize (rgbMat, resizeMat, resizeSize);


                    }

                    ////////////
                    /// Process frame
                    ///////////

                    if (!faceDetection || heatmap)
                    {

                        //flip values
                        if (frameProcessingInit == true)
                        {
                            Core.bitwise_not(resizeMat, resizeMat);
                        }
                        //flip colors position
                        Imgproc.cvtColor(resizeMat, resizeMat, Imgproc.COLOR_RGB2BGR);

                        //edge detection and wights
                        if (edgeBias || heatmap)
                        {

                            grayMat = resizeMat.clone();
                            Imgproc.cvtColor(grayMat, grayMat, Imgproc.COLOR_BGR2GRAY);

                            Imgproc.Canny(grayMat, grayMat, cannyThreshold, cannyThreshold);
                            //                          Imgproc.blur (grayMat, grayMat, new Size (blurSize, blurSize));
                            if (thresh)
                            {
                                Imgproc.threshold(grayMat, grayMat, edgeThreshold, 255, Imgproc.THRESH_BINARY);
                            }

                            //weights
                            Imgproc.cvtColor(grayMat, grayMat, Imgproc.COLOR_GRAY2RGB);
                            //Debug.Log("sample pixel before calc: " + resizeMat.get (100, 100).GetValue(0));
                            Core.addWeighted(resizeMat, (1 - edgeWeight), grayMat, edgeWeight, edgeGamma, resizeMat);
                            //Debug.Log("sample pixel after calc: " + resizeMat.get (100, 100).GetValue(0));
                        }

                        if (loactionBias || heatmap)
                        {

                            if (allowCrop && deltaCols > 0 && deltaRows > 0)
                            {
                                //                              Debug.Log ("creating location mat");
                                //assemble location Mat
                                //                              locationMat.create(resizeMat.size(),CvType.CV_8UC3);
                                Imgproc.resize(locationMat, locationMat, rgbaMat.size());
                                locationMat.setTo(new Scalar(0, 0, 0));

                                //                              Debug.Log ("colsRange: " + colsRange.start + " ," + colsRange.end);
                                //                              Debug.Log ("rowsRange: " + rowsRange.start + " ," + rowsRange.end);
                                //                              Debug.Log ("deltaCols: " + deltaCols);
                                //                              Debug.Log ("deltaRows: " + deltaRows);

                                //                              colsRange.start + (colsRange.end - colsRange.start) * rationOfScreen

                                //                              rowsRange.start + (rowsRange.end - rowsRange.start) * rationOfScreen
                                sub = new OpenCVForUnity.CoreModule.Rect(new Point((int)Math.Round(colsRange.start + deltaCols * rationOfScreen), (int)Math.Round(rowsRange.start + deltaRows * rationOfScreen)),
                                    new Point((int)Math.Round(colsRange.start + (deltaCols * (1 - rationOfScreen))), (int)Math.Round(rowsRange.start + (deltaRows * (1 - rationOfScreen))))
                                );
                                //set values
                                submat = new Mat(new Size(sub.width, sub.height), CvType.CV_8UC3, white);


                                //copyMat to locationmat
                                submat.copyTo(locationMat.colRange((int)Math.Round(colsRange.start + (deltaCols * rationOfScreen)), (int)Math.Round(colsRange.start + (deltaCols * (1 - rationOfScreen))))
                                    .rowRange((int)Math.Round(rowsRange.start + (deltaRows * rationOfScreen)), (int)Math.Round(rowsRange.start + (deltaRows * (1 - rationOfScreen)))));

                                Imgproc.resize(locationMat, locationMat, resizeSize);

                            }

                            Core.addWeighted(resizeMat, (1 - locationWeight), locationMat, locationWeight, 0.0, resizeMat);
                        }

                        //split channels and 
                        Core.split(resizeMat, channels);

                        //clear last cenbters
                        displayCenters.Clear();
                        //center for each channel
                        for (int i = 0; i < channels.Count; i++)
                        {
                            displayCenters.Add(getCenterPointFromMat(channels[i], i));
                        }
                    }

                    //////////
                    // process face detection
                    //////////


                    else
                    {
                        //Mat for detection
                        if (iosBuild)
                        {
                            Core.flip(resizeMat, resizeMat, 1);
                        }
                        else
                        {
                            Core.flip(resizeMat, resizeMat, 3);

                        }
                        Imgproc.cvtColor(resizeMat, grayMat, Imgproc.COLOR_RGB2GRAY);

                        //Hist correction - optional
                        Imgproc.equalizeHist(grayMat, grayMat);

                        if (iosBuild)
                        {

                            Core.rotate(grayMat, rotateMat, 2);
                            // actual cascade face detection // LBS fast dataset 10% less accurate - change to haar cascade dataset at cascade initiation
                            if (cascade != null)
                            {
                                cascade.detectMultiScale2(rotateMat, faces, maxDetections, 1.1, 2, 2, new Size(50, 50), maxFaceSize);
                                //cascade.detectMultiScale (grayMat, faces, 1.1, 2, 2, new Size (20, 20), new Size ());
                            }
                        }
                        else
                        {
                            // actual cascade face detection // LBS fast dataset 10% less accurate - change to haar cascade dataset at cascade initiation
                            if (cascade != null)
                            {
                                cascade.detectMultiScale2(grayMat, faces, maxDetections, 1.1, 2, 2, new Size(50, 50), maxFaceSize);
                                //cascade.detectMultiScale (grayMat, faces, 1.1, 2, 2, new Size (20, 20), new Size ());
                            }
                        }
                        rects = faces.toArray();


                        if (rects.Length > 0)
                        {

                            lastFaceFrame = frameCount;
                            if (iosBuild)
                            {
                                //90 deg clockwise transformation
                                //                              rectsY = rects [0].y;
                                //                              rectsWidth = rects [0].width;
                                //                              rects [0].y = rects [0].width - rects [0].x;
                                //                              rects [0].x = rectsY;
                                //                              rects [0].width = rects [0].height;
                                //                              rects [0].height = rectsWidth;

                                //270 deg clockwise transformation
                                //                              Debug.Log("rect[0]: " +rects[0].ToString());
                                rectsY = rects[0].x;
                                rectsWidth = rects[0].width;
                                rects[0].x = rects[0].y;
                                rects[0].y = (int)resizeSize.height - rectsY - rects[0].width;
                                rects[0].width = rects[0].height;
                                rects[0].height = rectsWidth;

                                //                              Debug.Log ("AFTER rect[0]: " + rects [0].ToString ());


                            }
                            //change calc mats sizes according to face
                            faceRefMat.create(rects[0].size(), CvType.CV_8UC3);
                            grayFaceMat.create(rects[0].size(), CvType.CV_8UC1);

                            faceRefMat = resizeMat.submat(rects[0]);
                            //alternativly
                            //                          resizeMat.submat( rects [0]).copyTo (faceRefMat);

                            //flip values
                            //                          Core.bitwise_not (faceRefMat, faceRefMat);

                            //edge detection and wights
                            if (edgeBias || heatmap)
                            {
                                Imgproc.cvtColor(faceRefMat, grayFaceMat, Imgproc.COLOR_RGB2GRAY);

                                Imgproc.Canny(grayFaceMat, grayFaceMat, cannyThreshold, cannyThreshold);
                                Imgproc.blur(grayFaceMat, grayFaceMat, new Size(blurSize, blurSize));
                                if (thresh)
                                {
                                    Imgproc.threshold(grayFaceMat, grayFaceMat, edgeThreshold, 255, Imgproc.THRESH_BINARY);
                                }

                                // add aweights
                                Imgproc.cvtColor(grayFaceMat, grayFaceMat, Imgproc.COLOR_GRAY2RGB);
                                Core.addWeighted(faceRefMat, (1 - edgeWeight), grayFaceMat, edgeWeight, edgeGamma, faceRefMat);
                            }

                            //Location isn't relevant for face detection mode
                            //                          if (loactionBias) {
                            //                              Core.addWeighted (faceRefMat, (1 - locationWeight), locationMat, locationWeight, 0.0, faceRefMat);
                            //                          }

                            //split channels
                            Core.split(faceRefMat, channels);

                            //clear last cenbters
                            displayCenters.Clear();
                            //center for each channel
                            for (int i = 0; i < channels.Count; i++)
                            {
                                displayCenters.Add(getCenterPointFromMat(channels[i], i));
                            }

                            //clear last faces
                            displayFacePoints.Clear();
                            //add face point
                            displayFacePoints.Add((int)(rects[0].x / resizeFactor));
                            displayFacePoints.Add((int)(rects[0].y / resizeFactor));
                            displayFacePoints.Add((int)(rects[0].x / resizeFactor + rects[0].width / resizeFactor));
                            displayFacePoints.Add((int)(rects[0].y / resizeFactor + rects[0].height / resizeFactor));
                        }
                    }
                    moments.Clear();
                    centersObj.Clear();
                    frameProcessingInit = true;
                }
                yield return new WaitForSeconds(secondsBtwProcessing);
            }
        }

        public Centers getCenterPointFromMat(Mat _mat, int channel)
        {

            if (frameCount <= 1)
            {
                point = middleOfTheFramePoint;
            }
            else
            {

                // 3rd order moment center of mass
                moments.Add(Imgproc.moments(_mat, false));
                point = new Point((int)Math.Round((moments[channel].m10 / moments[channel].m00)), (int)Math.Round((moments[channel].m01 / moments[channel].m00)));

                if (point.x.ToString() == "NaN" || point.x < 0 || point.y < 0)
                {
                    point = middleOfTheFramePoint;
                    Debug.Log("INSIDE CATCH point: " + point);
                }
                //resize point up
                if (!faceDetection)
                {
                    point.x = (int)Math.Round(map((float)point.x, 0, (float)resizeSize.width, (float)webCamTexture.width - (float)webCamTexture.width * exaggerateData, (float)webCamTexture.width * exaggerateData));
                    point.y = (int)Math.Round(map((float)point.y, 0, (float)resizeSize.height, (float)webCamTexture.height - (float)webCamTexture.height * exaggerateData, (float)webCamTexture.height * exaggerateData));
                }
                else
                {
                    if (currentFacePoints.Count > 0)
                    {

                        //map results to frame with exaggeration
                        point.x = (int)Math.Round(map((float)point.x, 0, (float)rects[0].width,
                                  (float)rects[0].width * (exaggerateData + exaggerateDataFace), (float)rects[0].width - (float)rects[0].width * (exaggerateData + exaggerateDataFace)));
                        point.y = (int)Math.Round(map((float)point.y, 0, (float)rects[0].height,
                                  (float)rects[0].height * (exaggerateData + exaggerateDataFace), (float)rects[0].height - (float)rects[0].height * (exaggerateData + exaggerateDataFace)));

                        //                      Debug.Log ("AFTER MAP point: " + point);

                        //flip results horizontaly
                        point.x = (int)Math.Round(((point.x + rects[0].x) / resizeFactor));
                        point.y = (int)Math.Round(webCamTexture.height - (point.y + rects[0].y) / resizeFactor);

                        //                      Debug.Log ("face middle: " +
                        faceMiddleX = (int)Math.Round((rects[0].x + rects[0].width / 2) / resizeFactor);
                        faceMiddleY = (int)Math.Round((rects[0].y + (rects[0].height / 2)) / resizeFactor);
                    }
                }
            }
            centersObj.Add(new Centers(channel, point));

            return centersObj[channel];
        }

        void OnEnable()
        {
            // ImageCroppedEvent.OnCropped += AnalyseImage();

            //resume when active
            Time.timeScale = 1;
            Debug.Log("<unity> resumed view");
            fileUpload = null;

        }
        void OnDisable()
        {
            // ImageCroppedEvent.OnCropped += AnalyseImage();

            //pause when inactive
            Time.timeScale = 0;
            Debug.Log("<unity> paused view");
            fileUpload = null;

        }
        public void OnCrop()
        {
            allowCrop = true;
            exaggerateData = 1.92f;
            cropButton.gameObject.SetActive(false);
            cropRectButton.gameObject.SetActive(false);

            //Utils.matToTexture2D(rgbReferenceMat, texture, colors);

            //if (webglBuild)
            //{
            //    Core.flip(rgbMat, rgbMat, 0);

            //}
            //          Invoke ("photoFromUnityInternal", 10);
            //Toggle heatmap
            //          heatmapToggle.isOn = false;
        }
        public void OnCropRect()
        {
            cropRect = true;
            allowCrop = true;
            exaggerateData = 1.92f;
            cropButton.gameObject.SetActive(false);
            cropRectButton.gameObject.SetActive(false);

            //Toggle heatmap
            //          heatmapToggle.isOn = false;


        }
        public void takePhoto()
        {
            //          Profiler.BeginSample("My Sample");

            Debug.Log("TAKE PHOTO");
            photoStartFrame = frameCount;

            wasAtPhoto = true;
            //TO-DO: reset values
            photoTaken = true;
            takePhotoButton.gameObject.SetActive(false);
            cropButton.gameObject.SetActive(true);
            cropRectButton.gameObject.SetActive(true);
            confirmButton.gameObject.SetActive(true);

            //          Profiler.EndSample();

            //audio
            //          audio.Play ();
            //write to singleton
            //          ImageManager.instance.photo = texture;

            //UnityToXcodePhotoEvent;

            //          photoFromUnity ();


            //          if (false) {
            //              photoFromUnityInternal ();
            //          }
        }
        public void takeInstaPhoto()
        {
            insta = true;
            Debug.Log("TAKE INSTA PHOTO");
            rgbCloneMat = rgbMat;
            photoStartFrame = frameCount;
            wasAtPhoto = true;

            //TO-DO: reset values
            photoTaken = true;
            cropButton.gameObject.SetActive(true);
            cropRectButton.gameObject.SetActive(true);
            confirmButton.gameObject.SetActive(true);

            //PhotoFromUnity ();
            //audio
            //          audio.Play ();
            //write to singleton
            //TO-DO: emmit event for Markus
            //UnityToXcodePhotoEvent;

            if (!faceDetection)
            {
                distanceToEdge = new float[] {
                    (float)averageCenter.point.x,
                    (float)(frameWidth - averageCenter.point.x),
                    (float)averageCenter.point.y,
                    (float)(frameHeight - averageCenter.point.y)
                };
                minDistanceToEdge = (int)Math.Floor(distanceToEdge.Min());
                try
                {
                    rgbCloneMat.colRange(0, (int)Math.Floor(averageCenter.point.x - minDistanceToEdge)).setTo(white);
                    rgbCloneMat.colRange((int)Math.Floor(averageCenter.point.x + minDistanceToEdge), (int)frameWidth).setTo(white);

                    rgbCloneMat.rowRange(0, (int)Math.Floor(averageCenter.point.y - minDistanceToEdge)).setTo(white);
                    rgbCloneMat.rowRange((int)Math.Floor(averageCenter.point.y + minDistanceToEdge), (int)frameHeight).setTo(white);

                }
                catch
                {
                    Debug.Log("Range outo of bounds caught - TO-DO");
                }
            }
            else
            {
                distanceToEdge = new float[] {
                    (float)faceMiddleX,
                    (float)(frameWidth - faceMiddleX),
                    (float)faceMiddleY,
                    (float)(frameHeight - faceMiddleY)
                };
                minDistanceToEdge = (int)Math.Floor(distanceToEdge.Min());
                try
                {
                    rgbCloneMat.colRange(0, (int)Math.Floor((float)faceMiddleX - minDistanceToEdge)).setTo(white);
                    rgbCloneMat.colRange((int)Math.Floor((float)faceMiddleX + minDistanceToEdge), (int)frameWidth).setTo(white);

                    rgbCloneMat.rowRange(0, (int)Math.Floor(frameHeight - (float)faceMiddleY - minDistanceToEdge)).setTo(white);
                    rgbCloneMat.rowRange((int)Math.Floor(frameHeight - (float)faceMiddleY + minDistanceToEdge), (int)frameHeight).setTo(white);
                }
                catch
                {
                    Debug.Log("Range outo of bounds caught - TO-DO");
                }
            }

        }
        /// <summary>
        /// Raises the destroy event.
        /// </summary>
        void OnDestroy()
        {
            Dispose();

        }

        /// <summary>
        /// Raises the back button click event.
        /// </summary>
        public void OnBackButtonClick()
        {
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
            SceneManager.LoadScene("AeStatix");
#else
            Application.LoadLevel ("AeStatix");
#endif
        }

        /// <summary>
        /// Raises the play button click event.
        /// </summary>
        public void OnPlayButtonClick()
        {
            if (hasInitDone)
                webCamTexture.Play();
        }

        /// <summary>
        /// Raises the pause button click event.
        /// </summary>
        public void OnPauseButtonClick()
        {
            if (hasInitDone)
                webCamTexture.Pause();
        }

        /// <summary>
        /// Raises the stop button click event.
        /// </summary>
        public void OnStopButtonClick()
        {
            if (hasInitDone)
                webCamTexture.Stop();
        }

        /// <summary>
        /// Raises the change camera button click event.
        /// </summary>
        public void OnChangeCameraButtonClick()
        {
            //          cameraToggle = !cameraToggle;

            //          if (hasInitDone)
            //              Initialize (null, requestedWidth, requestedHeight, !requestedIsFrontFacing);
        }
        IEnumerator OnMouseDrag()
        {

            //Debug.Log("<unity> insideOnMouseDragEvent");
            //crop

            if (allowCrop)
            {
                if (Input.GetMouseButton(0))
                {


                    float mousePointY = Input.mousePosition.y;
                    float mousePointX = Input.mousePosition.x;
                    //                  Debug.Log ("mouse x: " + mousePointX + ", mouse Y: " + mousePointY);

                    //map point from screen size to mat size and invert
                    if (iosBuild)
                    {
                        secondPoint = new Point(frameWidth - map(mousePointY, 0, Screen.width, 0, frameHeight), frameHeight - map(mousePointX, 0, Screen.height, 0, frameWidth));

                    }
                    if (webglBuild)
                    {
                        secondPoint = new Point(map(mousePointX, 0, Screen.width, 0, frameWidth), frameHeight - map(mousePointY, 0, Screen.height, 0, frameHeight));
                    }



                    //first itiration
                    if (leftIsDown == false)
                    {
  
                        if (afterFirstCropMoseUp)
                        {

                            if (faceDetection)
                            {
                                Core.flip(rgbMat, rgbMat, 0);
                            }
                            rgbMat = rgbReferenceMat.clone();
                        }
                        else
                        {
                            if (faceDetection)
                            {
                                Core.flip(rgbMat, rgbMat, 0);
                            }

                            Debug.Log("flip here");
                            rgbReferenceMat = rgbMat.clone();
                        }
                        //map point from screen size to mat size and invert
                        if (iosBuild)
                        {
                            firstMouseDown = new Point(frameWidth - map(mousePointY, 0, Screen.width, 0, frameHeight), frameHeight - map(mousePointX, 0, Screen.height, 0, frameWidth));

                        }
                        if (webglBuild)
                        {
                            firstMouseDown = new Point(map(mousePointX, 0, Screen.width, 0, frameWidth), frameHeight - map(mousePointY, 0, Screen.height, 0, frameHeight));

                            //                          Debug.Log ("firstMouseDown: " + firstMouseDown);
                            //                          Debug.Log ("secondPoint: " + secondPoint);
                            //

                        }

                        leftIsDown = true;
                        //                      deltaCols = 0;
                        //                      deltaRows = 0;

                    }
                    else
                    {
                        Debug.Log("not first iteration");
                        //                      rgbMat = rgbReferenceMat.clone ();
                        //                      Imgproc.blur (rgbMat, rgbMat, new Size (25, 25));
                        //                      rgbMat = rgbMat - new Scalar (100, 100, 100);
                        //                      
                        //                      if (faceDetection && !afterFirstCropMoseUp) {
                        //                          Core.flip (rgbMat, rgbMat, 0);
                        //                          Core.flip (rgbaMat, rgbaMat, 0);
                        //                      }
                    }

                    //if not the same point
                    if (Math.Abs(secondPoint.x - firstMouseDown.x) > 0 && Math.Abs(secondPoint.y - firstMouseDown.y) > 0)
                    {
                        exaggerateData = 1.2f * totalDistance / innitialTotalDistance;

                        //arrange values

                        //catch cropping cases
                        if (secondPoint.x < 0)
                        {
                            //                          Debug.Log ("frame width: " + frameWidth);
                            secondPoint.x = 0;
                        }
                        if (secondPoint.y < 0)
                        {
                            //                          Debug.Log ("frame width: " + frameWidth);
                            secondPoint.y = 0;
                        }
                        //                      Debug.Log ("frame width: " + frameWidth);
                        //                      Debug.Log ("frame height: " + frameHeight);

                        if (secondPoint.y > frameHeight)
                        {
                            //                          Debug.Log ("frame height: " + frameHeight);
                            secondPoint.y = frameHeight;
                        }
                        if (secondPoint.x > frameWidth)
                        {
                            //                          Debug.Log ("frame width: " + frameWidth);
                            secondPoint.x = frameWidth;
                        }



                        ///////////////
                        /// TO-DO: crop rect
                        /// //////////

                        //negative x range
                        if (firstMouseDown.x > secondPoint.x)
                        {
                            if (firstMouseDown.y > secondPoint.y)
                            {

                                //first moust y is bigger & first mouse x is bigger

                                //                              if (cropRect) {
                                //                                  if ( Math.Abs(secondPoint.x - firstMouseDown.x) > Math.Abs(secondPoint.y - firstMouseDown.y)  ) {
                                //                                      //width is bigger
                                //                                      firstMouseDown.x = secondPoint.x + Math.Abs(secondPoint.y - firstMouseDown.y);
                                //                                      Imgproc.circle (rgbReferenceMat, firstMouseDown, 10, green, 10, Imgproc.LINE_AA,0);
                                //                                      Imgproc.circle (rgbReferenceMat, secondPoint, 10, red, 10, Imgproc.LINE_AA,0);
                                //                                      Debug.Log ("state 0");
                                //                                      Debug.Log ("firstmouse: " + firstMouseDown + " , second point: " + secondPoint);
                                //                                  } 
                                //                                  if ( Math.Abs(secondPoint.x - firstMouseDown.x) < Math.Abs(secondPoint.y - firstMouseDown.y) ) {
                                //                                      //height is bigger
                                ////                                        firstMouseDown.y = secondPoint.y + Math.Abs(secondPoint.x - firstMouseDown.x);
                                //                                      Imgproc.circle (rgbReferenceMat, firstMouseDown, 10, green, 10, Imgproc.LINE_AA,0);
                                //                                      Imgproc.circle (rgbReferenceMat, secondPoint, 10, red, 10, Imgproc.LINE_AA,0);
                                //                                      Debug.Log ("state 1");
                                //                                      Debug.Log ("firstmouse: " + firstMouseDown + " , second point: " + secondPoint);
                                //
                                //
                                //                                  }
                                //                              }

                                colsRange = new OpenCVForUnity.CoreModule.Range((int)Math.Floor(secondPoint.x), (int)Math.Floor(firstMouseDown.x));
                                rowsRange = new OpenCVForUnity.CoreModule.Range((int)Math.Floor(secondPoint.y), (int)Math.Floor(firstMouseDown.y));
                            }
                            else
                            {

                                //first moust y is smaller & first mouse x is bigger

                                //                              if (cropRect) {
                                //                                  if ( Math.Abs(secondPoint.x - firstMouseDown.x) >= Math.Abs(secondPoint.y - firstMouseDown.y)) {
                                //                                      //width is bigger
                                //                                      secondPoint.x = firstMouseDown.x + Math.Abs(secondPoint.y - firstMouseDown.y);
                                //                                      Debug.Log ("state 2");
                                //
                                //                                  } else {
                                //                                      //height is bigger
                                //                                      secondPoint.y = firstMouseDown.y + Math.Abs(secondPoint.x - firstMouseDown.x);
                                //                                      Debug.Log ("state 3");
                                //
                                //                                  }
                                //                              }

                                colsRange = new OpenCVForUnity.CoreModule.Range((int)Math.Floor(secondPoint.x), (int)Math.Floor(firstMouseDown.x));
                                rowsRange = new OpenCVForUnity.CoreModule.Range((int)Math.Floor(firstMouseDown.y), (int)Math.Floor(secondPoint.y));
                            }

                        }
                        else
                        {

                            if (firstMouseDown.y > secondPoint.y)
                            {

                                //first moust y is bigger & first mouse x is smaller

                                //                              if (cropRect) {
                                //                                  if ( Math.Abs(secondPoint.x - firstMouseDown.x) >= Math.Abs(secondPoint.y - firstMouseDown.y)) {
                                //                                      //width is bigger
                                ////                                        secondPoint.x = firstMouseDown.x + Math.Abs(secondPoint.y - firstMouseDown.y);
                                //                                      Debug.Log ("state 4");
                                //
                                //                                  } else {
                                //                                      //height is bigger
                                ////                                        firstMouseDown.y = secondPoint.y + Math.Abs(secondPoint.x - firstMouseDown.x);
                                //                                      Debug.Log ("state 5");
                                //
                                //                                  }
                                //                              }

                                colsRange = new OpenCVForUnity.CoreModule.Range((int)Math.Floor(firstMouseDown.x), (int)Math.Floor(secondPoint.x));
                                rowsRange = new OpenCVForUnity.CoreModule.Range((int)Math.Floor(secondPoint.y), (int)Math.Floor(firstMouseDown.y));
                            }
                            else
                            {

                                //first moust y is smaller & first mouse x is smaller

                                //                              if (cropRect) {
                                //                                  if ( Math.Abs(secondPoint.x - firstMouseDown.x) >= Math.Abs(secondPoint.y - firstMouseDown.y)) {
                                //                                      //width is bigger
                                //                                      secondPoint.x = firstMouseDown.x + Math.Abs(secondPoint.y - firstMouseDown.y);
                                //                                      Debug.Log ("state 6");
                                //
                                //                                  } else {
                                //                                      //height is bigger
                                //                                      secondPoint.y = firstMouseDown.y + Math.Abs(secondPoint.x - firstMouseDown.x);
                                //                                      Debug.Log ("state 7");
                                //
                                //                                  }
                                //                              }

                                colsRange = new OpenCVForUnity.CoreModule.Range((int)Math.Floor(firstMouseDown.x), (int)Math.Floor(secondPoint.x));
                                rowsRange = new OpenCVForUnity.CoreModule.Range((int)Math.Floor(firstMouseDown.y), (int)Math.Floor(secondPoint.y));
                            }
                        }

                        // x range larger than frame
                        //                      if (firstMouseDown.x < 0) {
                        //                          Debug.Log ("frame width: " + frameWidth);
                        //                          firstMouseDown.x = 0;
                        //                      }

                        deltaRows = rowsRange.end - rowsRange.start;
                        deltaCols = colsRange.end - colsRange.start;
                        //                  Debug.Log ("cols range: " + colsRange.ToString() + " rows range: " + rowsRange);
                        //                  Debug.Log ("leftIsDown: " + leftIsDown);
                        //clear mat

                        zeroMat.copyTo(rgbMat);

                        rgbReferenceMat.rowRange(rowsRange).colRange(colsRange).copyTo(rgbMat.rowRange(rowsRange).colRange(colsRange));
                        //                      //for processing
                        //                      Imgproc.resize(resizeMat,resizeMat, new Size((int)Math.Floor(deltaRows-1),(int)Math.Floor(deltaRows)));
                        //                      rgbReferenceMat.rowRange (rowsRange).colRange (colsRange).copyTo(resizeMat);

                    }

                    if (deltaRows != 0 && deltaCols != 0)
                    {

                        deltaRows = rowsRange.end - rowsRange.start;
                        deltaCols = colsRange.end - colsRange.start;
                        //                      Debug.Log ("cols range: " + colsRange.ToString() + " rows range: " + rowsRange);

                    }

                    //rect crop
                    //                  if (cropRect == true) {
                    //                      Debug.Log ("inside crop rect");
                    //                      if (deltaRows >= deltaCols) {
                    //                          deltaRows = deltaCols;
                    //                      } else {
                    //                          deltaCols = deltaRows;
                    //                      }
                    //                  }


                }
                leftIsDown = true;

            }
            yield return 0;
        }

        void OnMouseUp()
        {
            if (allowCrop)
            {
                if (!iosBuild)
                {
                    leftIsDown = false;
                }
                afterFirstCropMoseUp = true;
                //              cropRect = false;
                Debug.Log("end of drag");
                Debug.Log(Input.mousePosition);
                //return analysis to full mat
                //                  rgbMat = rgbReferenceMat.clone ();

                //                  deltaRows = null;
                //                  deltaCols = null;

            }
        }

        //      void OnMouseDown () {
        //          //start recording the time when a key is pressed and held.
        //          downTime = Time.time;
        //          isHandled = false;
        //
        //          //look for a double click
        //          if(Time.time-lastClick < 0.3){
        //              // do something
        //
        //              firstMouseDown = new Point (0, 0);
        //              secondPoint = new Point ( (int)Math.Floor(frameWidth - 1),  (int)Math.Floor(frameHeight -1));
        //              colsRange = new Range (0, (int)Math.Floor( frameWidth - 1));
        //              rowsRange = new Range (0, (int)Math.Floor( frameHeight - 1));
        //          }
        //          lastClick = Time.time;
        //      }

        //      void OnMouseDown () {
        //          if (Input.GetButtonDown ("confirm")) {
        //              photoFromUnityInternal ();
        //          }
        //      }

        public void iosTouch()
        {


            //crop

            if (allowCrop)
            {

                if (Input.touchCount > 0)
                {

                    //                  //////////////
                    //                  //double click - cancel crop state
                    //                  //////
                    //
                    //                  //start recording the time when a key is pressed and held.
                    //                  downTime = Time.time;
                    //                  isHandled = false;
                    //
                    //                  //look for a double click
                    //                  if(Time.time-lastClick < 0.3){
                    //                      // do something
                    //
                    //                      firstMouseDown = new Point (0, 0);
                    //                      secondPoint = new Point ( (int)Math.Floor(frameWidth - 1),  (int)Math.Floor(frameHeight -1));
                    //                      colsRange = new Range (0, (int)Math.Floor( frameWidth - 1));
                    //                      rowsRange = new Range (0, (int)Math.Floor( frameHeight - 1));
                    //                  }
                    //                  lastClick = Time.time;
                    //
                    //                  //
                    //                  //
                    //                  ////////////


                    //                  Touch[] myTouches = Input.touches;
                    //                  for(int i = 0; i < Input.touchCount; i++)
                    //                  {
                    //                      //Do something with the touches
                    //                  }

                    secondTouch = Input.GetTouch(0);
                    float mousePointY = secondTouch.position.y;
                    float mousePointX = secondTouch.position.x;
                    //                  Debug.Log ("mouse x: " + mousePointX + ", mouse Y: " + mousePointY);

                    //map point from screen size to mat size and invert

                    secondPoint = new Point(frameWidth - map(mousePointY, 0, Screen.width, 0, frameHeight), frameHeight - map(mousePointX, 0, Screen.height, 0, frameWidth));



                    //first itiration
                    if (leftIsDown == false)
                    {

                        if (afterFirstCropMoseUp)
                        {

                            rgbMat = rgbReferenceMat.clone();

                        }
                        else
                        {
                            if (faceDetection)
                            {
                                Core.flip(rgbMat, rgbMat, 0);
                            }
                            rgbReferenceMat = rgbMat.clone();
                        }
                        //map point from screen size to mat size and invert
                        //                          if (iosBuild) {
                        firstMouseDown = new Point(frameWidth - map(mousePointY, 0, Screen.width, 0, frameHeight), frameHeight - map(mousePointX, 0, Screen.height, 0, frameWidth));
                        Debug.Log("<unity crop> first iteration, firstPoint: " + firstMouseDown);
                        //                          }
                        //                          if (webglBuild) {
                        //                              firstMouseDown = new Point (map (mousePointX, 0, Screen.width, 0, frameWidth), frameHeight - map (mousePointY, 0, Screen.height, 0, frameHeight)); 
                        //
                        //                              //                          Debug.Log ("firstMouseDown: " + firstMouseDown);
                        //                              //                          Debug.Log ("secondPoint: " + secondPoint);
                        //                              //
                        //
                        //                          }

                        leftIsDown = true;
                        deltaCols = 0;
                        deltaRows = 0;
                    }
                    //                  else {
                    //                      rgbMat = rgbReferenceMat.clone ();
                    //                      Imgproc.blur (rgbMat, rgbMat, new Size (25, 25));
                    //                      rgbMat = rgbMat - new Scalar (100, 100, 100);
                    //                  }

                    //if not the same point
                    if (firstMouseDown.x != secondPoint.x && firstMouseDown.y != secondPoint.y)
                    {
                        exaggerateData = 1.2f * totalDistance / innitialTotalDistance;

                        //arrange values

                        //catch cropping cases
                        if (secondPoint.x < 0)
                        {
                            //                          Debug.Log ("frame width: " + frameWidth);
                            secondPoint.x = 0;
                        }
                        if (secondPoint.y < 0)
                        {
                            //                          Debug.Log ("frame width: " + frameWidth);
                            secondPoint.y = 0;
                        }
                        //                      Debug.Log ("frame width: " + frameWidth);
                        //                      Debug.Log ("frame height: " + frameHeight);

                        if (secondPoint.y > frameHeight)
                        {
                            //                          Debug.Log ("frame height: " + frameHeight);
                            secondPoint.y = frameHeight;
                        }
                        if (secondPoint.x > frameWidth)
                        {
                            //                          Debug.Log ("frame width: " + frameWidth);
                            secondPoint.x = frameWidth;
                        }
                        Debug.Log("<unity crop> no the same point, first point: " + firstMouseDown + " second: " + secondPoint);
                        //negative x range
                        if (firstMouseDown.x > secondPoint.x)
                        {
                            if (firstMouseDown.y > secondPoint.y)
                            {
                                colsRange = new OpenCVForUnity.CoreModule.Range((int)Math.Floor(secondPoint.x), (int)Math.Floor(firstMouseDown.x));
                                rowsRange = new OpenCVForUnity.CoreModule.Range((int)Math.Floor(secondPoint.y), (int)Math.Floor(firstMouseDown.y));
                                Debug.Log("<unity crop> state 0, colRange:" + colsRange + " rowRange: " + rowsRange);


                            }
                            else
                            {
                                colsRange = new OpenCVForUnity.CoreModule.Range((int)Math.Floor(secondPoint.x), (int)Math.Floor(firstMouseDown.x));
                                rowsRange = new OpenCVForUnity.CoreModule.Range((int)Math.Floor(firstMouseDown.y), (int)Math.Floor(secondPoint.y));
                                Debug.Log("<unity crop> state 1, colRange:" + colsRange + " rowRange: " + rowsRange);

                            }

                        }
                        else
                        {
                            if (firstMouseDown.y > secondPoint.y)
                            {
                                colsRange = new OpenCVForUnity.CoreModule.Range((int)Math.Floor(firstMouseDown.x), (int)Math.Floor(secondPoint.x));
                                rowsRange = new OpenCVForUnity.CoreModule.Range((int)Math.Floor(secondPoint.y), (int)Math.Floor(firstMouseDown.y));
                                Debug.Log("<unity crop> state 2, colRange:" + colsRange + " rowRange: " + rowsRange);


                            }
                            else
                            {
                                colsRange = new OpenCVForUnity.CoreModule.Range((int)Math.Floor(firstMouseDown.x), (int)Math.Floor(secondPoint.x));
                                rowsRange = new OpenCVForUnity.CoreModule.Range((int)Math.Floor(firstMouseDown.y), (int)Math.Floor(secondPoint.y));
                                Debug.Log("<unity crop> state 3, colRange:" + colsRange + " rowRange: " + rowsRange);

                            }
                        }

                        // x range larger than frame
                        //                      if (firstMouseDown.x < 0) {
                        //                          Debug.Log ("frame width: " + frameWidth);
                        //                          firstMouseDown.x = 0;
                        //                      }

                        deltaRows = rowsRange.end - rowsRange.start;
                        deltaCols = colsRange.end - colsRange.start;
                        Debug.Log("<unity crop> bfr zeroing cols range: " + colsRange.ToString() + " rows range: " + rowsRange);

                        //clear mat
                        zeroMat.copyTo(rgbMat);

                        //cleared
                        //                      if (deltaRows <= 2 || deltaCols <= 2) {
                        //                          rowsRange = new Range(0, (int)Math.Floor( frameHeight-1));
                        //                          colsRange = new Range(0, (int)Math.Floor(frameWidth-1));
                        //                          deltaRows = rowsRange.end - rowsRange.start;
                        //                          deltaCols = colsRange.end - colsRange.start;
                        //                      }
                        rgbReferenceMat.rowRange(rowsRange).colRange(colsRange).copyTo(rgbMat.rowRange(rowsRange).colRange(colsRange));

                        //                  rgbReferenceMat.rowRange (rowsRange).colRange (colsRange).copyTo(resizeMat.rowRange (rowsRange).colRange (colsRange));

                    }

                    if (deltaRows != 0 && deltaCols != 0)
                    {

                        deltaRows = rowsRange.end - rowsRange.start;
                        deltaCols = colsRange.end - colsRange.start;
                        Debug.Log("<unity crop> not zerocols range: " + colsRange.ToString() + " rows range: " + rowsRange);

                    }
                }
                leftIsDown = true;
            }
            //              yield return 0;

        }

        public float map(float s, float a1, float a2, float b1, float b2)
        {
            return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
        }
        void OnGUI()
        {
            if (showCalcMats && frameCount >= 1)
            {

                //only black rect
                unityRect = new UnityEngine.Rect(5f, 5f, (float)resizeSize.width, (float)resizeSize.height);

                if (!loactionBias && !edgeBias)
                {
                    //TO-DO: switch between face detectiuon ui and regular
                    //                  GUImat = faceRefMat.clone ();
                    GUImat = blackMat.clone();

                }
                if (loactionBias && !edgeBias && GUItexture != null)
                {
                    //GUImat = blackMat.clone ();

                    Core.addWeighted(locationMat, locationWeight, blackMat, (1 - locationWeight), 0.0, GUImat);


                }
                if (edgeBias && loactionBias)
                {
                    Core.addWeighted(blackMat, (1 - edgeWeight), grayMat, (edgeWeight), edgeGamma, GUImat);
                    Core.addWeighted(locationMat, locationWeight, GUImat, (1 - locationWeight), 0.0, GUImat);


                }
                if (!loactionBias && edgeBias)
                {

                    GUImat = grayMat.clone();
                    Core.addWeighted(blackMat, (1 - edgeWeight), grayMat, (edgeWeight), edgeGamma, GUImat);

                }

                //Imgproc.resize(resize, GUImat, GUImat.size());
                //Imgproc.resize(photoMat, GUImat, GUImat.size());

                Utils.matToTexture2D(GUImat, GUItexture);
                GUI.DrawTexture(unityRect, GUItexture);

            }

        }

        //ui controls
        public void showFace()
        {
            faceDetection = !faceDetection;
            if (cameraIndexUI == 0)
            {
                cameraIndexUI = 1;
                //cross ui
                if (cross)
                {
                    crossToogleUi = GameObject.FindGameObjectWithTag("cross").GetComponent<Toggle>();
                    //                  cross = false;
                    crossToogleUi.isOn = false;
                }
                if (centerCross)
                {
                    centerCrossToogleUi = GameObject.FindGameObjectWithTag("centerCross").GetComponent<Toggle>();
                    //                  centerCross = false; 
                    centerCrossToogleUi.isOn = false;
                    //                  Debug.Log ("centerCross: "+ centerCross);
                }
                //back to back camera
            }
            else { cameraIndexUI = 0; };
            if (frameCount > 0)
                ChangeCamera(cameraIndexUI);


        }
        public void showHeatmap()
        {
            if (iosBuild)
            {
                if (faceDetection)
                {

                    faceToogleUi = GameObject.FindGameObjectWithTag("facedetection").GetComponent<Toggle>();
                    faceToogleUi.isOn = false;
                    showFace();
                    faceDetection = false;
                }
            }
            //          hsvUIBool = !hsvUIBool;
            //          hsvUI = GameObject.FindGameObjectWithTag ("hsv");
            //          hsvUI.SetActive (hsvUIBool);

            heatmap = !heatmap;
            if (heatmap == true)
            {
                //              disclaimerUi = GameObject.FindGameObjectWithTag ("heatmap-disclaimer").GetComponent<Text> ();
                if (disclaimerUi)
                {
                    disclaimerUi.enabled = true;
                    disclaimerUiChild.enabled = true;
                    disclaimerBackground.enabled = true;
                }
            }
            else
            {
                if (disclaimerUi)
                {

                    //                  disclaimerUi = GameObject.FindGameObjectWithTag ("heatmap-disclaimer").GetComponent<Text> ();
                    disclaimerUi.enabled = false;
                    disclaimerUiChild.enabled = false;
                    disclaimerBackground.enabled = false;
                }

            }
        }
        public void showEdge()
        {
            edgeBias = !edgeBias;
        }
        public void showCenter()
        {
            loactionBias = !loactionBias;
        }
        public void colorDotsOn()
        {
            weightedAverage = false;
        }
        public void colorDotsOff()
        {
            weightedAverage = true;
        }
        public void webGLtoggleDots()
        {
            weightedAverage = !weightedAverage;
        }
        public void showGuide()
        {
            guide = !guide;
        }
        public void showCross()
        {
            cross = !cross;
        }
        public void showCenterCross()
        {
            centerCross = !centerCross;
        }
        public void dotsOff()
        {
            showDots = false;
        }
        public void dotsOn()
        {
            showDots = true;
        }

        public void ExaggerationSlider()
        {
            //to-do : init up 
            float sliderGet = GameObject.Find("exaggeration").GetComponent<Slider>().value;
            exaggerateData = sliderGet;

            Text _text = GameObject.Find("exaggerationText").GetComponent<UnityEngine.UI.Text>();
            _text.text = exaggerateData.ToString();
        }

        //calculate the trackbar bar
        public List<MatOfPoint> TriangleBar(float _percentToCenter)
        {
            barPointsArray = new Point[]{ new Point (rgbMat.width(), rgbMat.height() - rgbMat.height() * 0.85),
                new Point (rgbaMat.width() - (triHight * _percentToCenter),rgbaMat.height()-( (rgbaMat.height() * _percentToCenter ))+0),
                new Point(rgbaMat.width() , rgbaMat.height()-( (rgbaMat.height() * _percentToCenter ))+0)};

            //          foreach (Point _point in barPointsArray) {
            //              Debug.Log ("point = " + _point);
            //          }

            barPoints = new MatOfPoint(barPointsArray);

            triangleBar.Clear();
            triangleBar.Add(barPoints);

            return triangleBar;
            //either wnough kto sync or clear and add array to mat and to list

        }
        public float TrackbarDiff(Point _current)
        {

            totalDistance = (float)Math.Sqrt(((frameWidth / 2) * (frameWidth / 2)) + ((frameHeight / 2) * (frameHeight / 2)));

            //TO-DO: optimize initiation with flag condition 
            if (!faceDetection)
            {
                //              totalDistance = (float)Math.Sqrt (((frameWidth / 2) * (frameWidth / 2)) + ((frameHeight / 2) * (frameHeight / 2)));

                if (!allowCrop)
                {
                    trackbarDiffFloat = (float)(Math.Sqrt((
                        (_current.x - (frameWidth / 2)) * (_current.x - (frameWidth / 2))) + ((_current.y - (frameHeight / 2)) * (_current.y - (frameHeight / 2)))));
                }
                else
                {
                    if (deltaCols > 0 && deltaRows > 0)
                    {
                        trackbarDiffFloat = (float)(Math.Sqrt((
                            (_current.x - (colsRange.start + deltaCols / 2)) * (_current.x - (colsRange.start + deltaCols / 2))) + ((_current.y - (rowsRange.start + deltaRows / 2)) * (_current.y - (rowsRange.start + deltaRows / 2)))));
                        //                      Debug.Log ("cropped trackbar diff: " + trackbarDiffFloat);
                    }
                }
                if (trackbarDiffFloat > totalDistance - 10)
                    trackbarDiffFloat = totalDistance;
                if (trackbarDiffFloat < 10)
                    trackbarDiffFloat = 0;
            }
            else
            {
                //face detection 

                if (rects != null && rects.Length > 0)
                {

                    //                  Debug.Log ("BFR: _current: " + _current);
                    //                  Debug.Log ("rects[0].x: " + rects [0].x/resizeFactor);
                    //                  Debug.Log ("rects[0].y: " + rects [0].y/resizeFactor);

                    //map point to face rect
                    totalDistance = (float)Math.Sqrt(((rects[0].width / 2 / resizeFactor) * (rects[0].width / 2 / resizeFactor) + (rects[0].height / 2 / resizeFactor) * (rects[0].height / 2 / resizeFactor)));
                    _current.x = (float)((_current.x) - (rects[0].x / resizeFactor));
                    _current.y = (float)(frameHeight - _current.y - (rects[0].y / resizeFactor));
                    //                  Debug.Log ("_current: " + _current);
                    //                  Imgproc.circle (rgbaMat, _current, 8, green, 13, Imgproc.LINE_AA, 0);


                    trackbarDiffFloat = (float)(Math.Sqrt((
                        (_current.x - (rects[0].width / 2 / resizeFactor)) * (_current.x - (rects[0].width / 2 / resizeFactor))) + ((_current.y - (rects[0].height / 2 / resizeFactor)) * (_current.y - (rects[0].height / 2 / resizeFactor)))));


                    //                  Debug.Log ("total disstance: " + totalDistance);
                    //                  Debug.Log ("p: " + (1 - (trackbarDiffFloat/totalDistance)));

                    if (greenRectFeedback)
                    {

                        return 1;
                    }

                }
                else
                {
                    return 0;
                }
            }
            //          Debug.Log ("1 - trackbarDiffFloat/totalDistance: " + (1 - trackbarDiffFloat/totalDistance));
            //          Debug.Log ("trackbarDiffFloat: " + (trackbarDiffFloat ));
            //          Debug.Log ("return: " + (1 - trackbarDiffFloat / totalDistance));
            if (1 - trackbarDiffFloat / totalDistance >= 0.97)
            {
                return (1 - trackbarDiffFloat / totalDistance);
            }
            else
            {

                return (1 - trackbarDiffFloat / totalDistance);
            }
        }
        public Point WeightedAverageThree(Point _redPoint, Point _greenPoint, Point _bluePoint)
        {
            return new Point((int)Math.Round(((_redPoint.x * redCoeficiente) + (_greenPoint.x * greenCoeficiente) + (_bluePoint.x * blueCoeficiente)) / (redCoeficiente + greenCoeficiente + blueCoeficiente)),
                              (int)Math.Round(((_redPoint.y * redCoeficiente) + (_greenPoint.y * greenCoeficiente) + (_bluePoint.y * blueCoeficiente)) / (redCoeficiente + greenCoeficiente + blueCoeficiente)));
        }
        public void ChangeCamera(int cameraNumber)
        {
            //pasuse
            //          Time.timeScale = 0;
            //          Debug.Log ("<unity> paused for change camera");

            if (cameraNumber == 0)
            {
                Application.LoadLevel(Application.loadedLevel);
            }
            //reset face detection feedback (on the camera background)
            Camera.main.backgroundColor = faceBackgroundColorGray;

            cameraIndexUI = cameraNumber;

            StopCoroutine(processFrame());
            StopCoroutine(_Initialize());

            //          Dispose ();
            //          StopCoroutine (_Initialize ());

            if (hasInitDone)
                Initialize(null, requestedWidth, requestedHeight, Convert.ToBoolean(cameraIndexUI));

            //          //TO-FIX PROPERLY!!
            //          takePhoto();
            //      
        }

        //msg from xcode
        public void AnalyseImage(string msg)
        {
            Debug.Log("<unity> message recieved :" + msg);
            Debug.Log("<unity> persistant path: " + Application.persistentDataPath);
            fileUpload = new Texture2D(2, 2);
        }

        public void OnCancel()
        {
            ChangeCamera(0);
        }
        public void OnNoCrop()
        {
            //          allowCrop = false;
            //
            //          leftIsDown = false;
            //          afterFirstCropMoseUp = true;
            //          firstMouseDown.x = 0;
            //          firstMouseDown.y = 0;
            //          secondPoint = firstMouseDown;
            ////            deltaCols = 0;
            ////            deltaCols = 0;
            ////
            //          rgbMat = rgbReferenceMat.clone();
            ////            rgbaMat =  rgbReferenceMat.clone();
            ////            rgbReferenceMat = rgbMat.clone ();
        }

        ////////
        /// on xcode build
        ////////

        [DllImport("__Internal")]
        //  static class  PhotoFromUnity ();
        private extern static void photoFromUnity();

        public void photoFromUnityInternal()
        {
            downloadingPhoto = true;

            if ((allowCrop && deltaCols > 0 && deltaRows > 0) || (actualIosBuild && allowCrop))
            {
                Debug.Log("<unity crop> cropped image");
                Debug.Log("<unity crop> deltaCols: " + deltaCols + ", delta rows: " + deltaRows + ", allowCrop: " + allowCrop);
                //get cropped
                Imgproc.resize(resizeMat, resizeMat, rgbMat.size());

                if (faceDetection)
                {
                    Core.flip(resizeMat, resizeMat, 3);
                    //                  Core.flip( rgbaMat, rgbaMat,0);

                }
                Debug.Log("<unity crop> bfr else first point: " + firstMouseDown + ", second: " + secondPoint);
                //Core.flip(resizeMat, resizeMat, 0);

                resizeMat = resizeMat.submat(new OpenCVForUnity.CoreModule.Rect(firstMouseDown, secondPoint));

                rgbaMat = rgbaMat.submat(new OpenCVForUnity.CoreModule.Rect(firstMouseDown, secondPoint));
                rgbMat = rgbMat.submat(new OpenCVForUnity.CoreModule.Rect(firstMouseDown, secondPoint));

                Imgproc.resize(rgbMat, photoMat, rgbMat.size());
                Debug.Log("<unity crop> in cropped");
            }
            else
            {
                Debug.Log("<unity crop> aftr else first point: " + firstMouseDown + ", second: " + secondPoint);

                Debug.Log("<unity crop>not in cropped");
                Debug.Log("<unity crop> deltaCols: " + deltaCols + ", delta rows: " + deltaRows + ", allowCrop: " + allowCrop);


                Imgproc.resize(rgbaMat, photoMat, rgbMat.size());

            }
            if (faceDetection)
            {
                if (allowCrop)
                {
                    Core.flip(resizeMat, resizeMat, 3);
                    Core.flip(rgbaMat, rgbaMat, 0);
                    //camera to Jpeg rgba to bgr
                    Imgproc.cvtColor(rgbMat, photoMat, Imgproc.COLOR_RGBA2BGR);
                    Debug.Log("<unity crop> state 0");

                }
                else
                {
                    Debug.Log("<unity crop> state 1");

                    //camera to Jpeg rgba to bgr
                    Imgproc.cvtColor(rgbaMat, photoMat, Imgproc.COLOR_RGBA2BGR);
                }
            }
            else
            {
                if (allowCrop)
                {
                    Debug.Log("<unity crop> state 2");

                    //camera to Jpeg rgba to bgr
                    Imgproc.cvtColor(rgbaMat, photoMat, Imgproc.COLOR_RGBA2BGR);

                }
                else
                {
                    //camera to Jpeg rgba to bgr
                    Debug.Log("<unity crop> state 3");

                    Imgproc.cvtColor(rgbaMat, photoMat, Imgproc.COLOR_RGBA2BGR);
                }
                //camera to Jpeg rgba to bgr
            }


            //write image
#if !UNITY_IOS

            Debug.Log("post start");
            OpenCVForUnity.ImgcodecsModule.Imgcodecs.imwrite("snapshot-photo-with-data.jpeg", photoMat);
            photoDataCopy = photoMat.clone();
            //Core.flip(photoDataCopy, photoDataCopy, 1);

            Imgproc.cvtColor(rgbMat, photoMat, Imgproc.COLOR_RGB2BGR);
            OpenCVForUnity.ImgcodecsModule.Imgcodecs.imwrite("snapshot-photo.jpeg", photoMat);
            photoCopy = photoMat.clone();
            //heatmap analysis
            //          rgbaMat.create (rgbaMat.size (), CvType.CV_8UC4);

            heatmapMat = resizeMat.clone();

            //print pixels
            for (int indY = 0; indY < heatmapMat.cols(); indY++)
            {
                for (int indX = 0; indX < heatmapMat.rows(); indX++)
                {
                    //                                  Debug.Log ("(" + Math.Floor (heatmapMat.get (indX, indY) [0]).ToString () + ", " + Math.Floor (heatmapMat.get (indX, indY) [1]).ToString () + ", " + Math.Floor (heatmapMat.get (indX, indY) [2]).ToString () + ")");
                    double _hue = (float)Math.Floor(heatmapMat.get(indX, indY)[0]);

                    //without mapping hue value
                    heatmapMat.put(indX, indY, new double[3] { _hue, 255, 255 });
                    //mapping hue value
                    //                  heatmapMat.put (indX, indY, new double[3]{map( (float)_hue,0,360,minHue,hueBar),255,255 });
                }
            }
            //normalize values
            Imgproc.GaussianBlur(heatmapMat, heatmapMat, new Size(19, 19), 0, 0);
            Imgproc.equalizeHist(heatmapMat, heatmapMat);

            //      
            //HLS map
            //                          Imgproc.cvtColor(resizeMat,heatmapMat,Imgproc.COLOR_HLS2RGB);
            //HSV map
            Imgproc.cvtColor(heatmapMat, heatmapMat, Imgproc.COLOR_HSV2RGB);
            Imgproc.resize(heatmapMat, heatmapMat, rgbaMat.size());

            OpenCVForUnity.ImgcodecsModule.Imgcodecs.imwrite("snapshot-photo-heatmap.jpeg", heatmapMat);
            photoHeatmapCopy = heatmapMat.clone();

            //          StartCoroutine(Post());
            UploadHandlerPost();

#else
            Debug.Log("\n<unity> saving to path: " + Application.persistentDataPath + "/images-from-unity/image-data.jpeg");
            OpenCVForUnity.ImgcodecsModule.Imgcodecs.imwrite (Application.persistentDataPath + "/images-from-unity/image-data.jpeg", photoMat);

            Imgproc.cvtColor (rgbMat, photoMat, Imgproc.COLOR_RGB2BGR);
            if(faceDetection){
            Core.flip(photoMat,photoMat,0);
            Debug.Log("<unity> image flipped code 0");
            }

            OpenCVForUnity.ImgcodecsModule.Imgcodecs.imwrite (Application.persistentDataPath + "/images-from-unity/image.jpeg", photoMat);
            Debug.Log("\n<unity> saved image to path: " + Application.persistentDataPath + "/images-from-unity/image.jpeg");

            //heatmap analysis
            //          rgbaMat.create (rgbaMat.size (), CvType.CV_8UC4);

            heatmapMat = resizeMat.clone();

            //print pixels
            for(int indY = 0; indY < heatmapMat.cols();indY++){
            for (int indX = 0; indX < heatmapMat.rows(); indX++) {
            //                                  Debug.Log ("(" + Math.Floor (heatmapMat.get (indX, indY) [0]).ToString () + ", " + Math.Floor (heatmapMat.get (indX, indY) [1]).ToString () + ", " + Math.Floor (heatmapMat.get (indX, indY) [2]).ToString () + ")");
            double _hue = (float)Math.Floor (heatmapMat.get (indX, indY) [0]);

            //without mapping hue value
                                                heatmapMat.put (indX, indY, new double[3]{_hue,255,255 });
            //mapping hue value
//          heatmapMat.put (indX, indY, new double[3]{map( (float)_hue,0,360,minHue,hueBar),255,255 });
            }
            }
            //normalize values
            Imgproc.GaussianBlur (heatmapMat, heatmapMat, new Size(19,19), 0,0);
            Imgproc.equalizeHist (heatmapMat, heatmapMat);

            //      
            //HLS map
            //                          Imgproc.cvtColor(resizeMat,heatmapMat,Imgproc.COLOR_HLS2RGB);
            //HSV map
            Imgproc.cvtColor(heatmapMat,heatmapMat,Imgproc.COLOR_HSV2RGB);
            Imgproc.resize(heatmapMat,rgbaMat, rgbaMat.size());

            if(faceDetection){
            Core.flip(rgbaMat,rgbaMat,1);
            Core.flip(rgbaMat,rgbaMat,0);
            Debug.Log("<unity> image flipped code 1");
            }

            OpenCVForUnity.ImgcodecsModule.Imgcodecs.imwrite (Application.persistentDataPath + "/images-from-unity/image-heatmap.jpeg", rgbaMat);
#endif

            ///////
            /// xcode function
            /// ///
            if (actualIosBuild)
            {
                photoFromUnity();

                //reset after photo
                //////
                /// 
                fileUpload = null;
                Debug.Log("<unity> inside photoFromUnityInternal");

                if (allowCrop)
                    allowCrop = false;

                deltaCols = 0;
                deltaRows = 0;

                afterFirstCropMoseUp = false;

                wasAtPhoto = false;
                //TO-DO: reset values
                photoTaken = false;
                cropButton.gameObject.SetActive(false);
                cropRectButton.gameObject.SetActive(true);

                confirmButton.gameObject.SetActive(false);

            }

            //////


            //last change memory
            //          resizeMat = new Mat (resizeSize, CvType.CV_8UC3);


            //          ChangeCamera (0);
        }

        public void ImageFromWebServer(string msg)
        {
            //          url = "https://sci-crop.com/uploads/";
            Debug.Log("messege (filename) recieved from server: " + msg);
            url = msg;
            Debug.Log(url);
            fileuploadFirstItiration = true;

            StartCoroutine(ProcessPhoto());

        }
        public void ImageFromXcode(string msg)
        {
            url = "file://";
            Debug.Log("<unity xcode> messege (filename) recieved from xcode: " + msg);
            url = url + msg;
            Debug.Log(url);
            fileuploadFirstItiration = true;
            StartCoroutine(ProcessPhoto());
        }
        public void ClearImageFromWebServer()
        {
            Debug.Log("called ClearImageFromWebServer() from server");
            fileUpload = null;
        }

        IEnumerator ProcessPhoto()
        {
            if (iosBuild)
            {
                var www = new WWW(url);
                Debug.Log("<unity xcode> bfr yield");
                yield return www;

                fileUpload = www.texture;
                StopCoroutine(ProcessPhoto());

                if (fileUpload == null)
                {
                    Debug.Log("<unity xcode> Failed to load texture url:" + url);
                }
            }

            //webGL build
            else
            {
                UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.Log(www.error);
                    StopCoroutine(ProcessPhoto());
                }
                else
                {
                    fileUpload = DownloadHandlerTexture.GetContent(www) as Texture2D;
                    StopCoroutine(ProcessPhoto());
                }
            }
        }


        public void UploadHandlerPost()
        {

            //setup byte array for all 3 outputs
            photoBuffer = new MatOfByte();
            photoDataBuffer = new MatOfByte();
            photoHeatmapBuffer = new MatOfByte();

            //          Mat photoCopyClone = photoCopy.clone ();
            //          Mat photoDataCopyClone = photoDataCopy.clone ();
            //          Mat photoHeatmapCopyClone = photoHeatmapCopy.clone ();

            OpenCVForUnity.ImgcodecsModule.Imgcodecs.imencode(".jpeg", photoCopy, photoBuffer);
            OpenCVForUnity.ImgcodecsModule.Imgcodecs.imencode(".jpeg", photoDataCopy, photoDataBuffer);
            OpenCVForUnity.ImgcodecsModule.Imgcodecs.imencode(".jpeg", photoHeatmapCopy, photoHeatmapBuffer);

            payload1 = new byte[photoBuffer.toArray().Length];
            payload1 = photoBuffer.toArray();
            //          Debug.Log ("data: " + payload);

            payload2 = new byte[photoDataBuffer.toArray().Length];
            payload2 = photoDataBuffer.toArray();

            payload3 = new byte[photoHeatmapBuffer.toArray().Length];
            payload3 = photoHeatmapBuffer.toArray();

            StartCoroutine(Upload());
        }

        IEnumerator Upload()
        {
            //          byte[] myData = System.Text.Encoding.UTF8.GetBytes("This is some test data");
            UnityWebRequest www1 = UnityWebRequest.Put("https://sci-crop.com/unityUpload", payload1);
            //          UnityWebRequest www1 = UnityWebRequest.Put("http://localhost:3000/unityUpload", payload1);
            yield return www1.SendWebRequest();

            UnityWebRequest www2 = UnityWebRequest.Put("https://sci-crop.com/unityUploadData", payload2);
            //          UnityWebRequest www2 = UnityWebRequest.Put("http://localhost:3000/unityUploadData", payload2);
            yield return www2.SendWebRequest();

            UnityWebRequest www3 = UnityWebRequest.Put("https://sci-crop.com/unityUploadHeatmap", payload3);
            //          UnityWebRequest www3 = UnityWebRequest.Put("http://localhost:3000/unityUploadHeatmap", payload3);
            yield return www3.SendWebRequest();



            if (www1.isNetworkError || www1.isHttpError)
            {
                Debug.Log(www1.error);
            }
            else
            {
                Debug.Log("Upload complete!");

                downloadImageUrl = www1.GetResponseHeader("imageName");
                Debug.Log(www1.GetResponseHeader("imageName"));

            }

            if (www2.isNetworkError || www2.isHttpError)
            {
                Debug.Log(www2.error);
            }
            else
            {
                Debug.Log("Upload complete!");
                downloadImageDataUrl = www2.GetResponseHeader("imageName");

            }


            if (www3.isNetworkError || www3.isHttpError)
            {
                Debug.Log(www3.error);
            }
            else
            {
                Debug.Log("Upload complete!");
                downloadImageHeatmapUrl = www3.GetResponseHeader("imageName");
                responseDone();

            }
        }

        public void responseDone()
        {

            string[] urlsRes = new string[3];
            urlsRes[0] = downloadImageUrl;
            urlsRes[1] = downloadImageDataUrl;
            urlsRes[2] = downloadImageHeatmapUrl;

            Application.ExternalCall("download", urlsRes);
            Debug.Log("download called " + urlsRes);

            StopCoroutine(Upload());

            //reset after photo
            //////
            /// 
            fileUpload = null;
            Debug.Log("<unity> inside photoFromUnityInternal");

            if (allowCrop)
                allowCrop = false;

            deltaCols = 0;
            deltaRows = 0;

            afterFirstCropMoseUp = false;

            wasAtPhoto = false;
            //TO-DO: reset values
            photoTaken = false;
            cropButton.gameObject.SetActive(false);
            cropRectButton.gameObject.SetActive(true);

            confirmButton.gameObject.SetActive(false);
            downloadingPhoto = false;

            ChangeCamera(0);

        }


        //  //access texture2D asset file original dimentions 
        //      public static bool GetImageSize(Texture2D asset, out int width, out int height) {
        //          if (asset != null) {
        //              string assetPath;
        //              assetPath = AssetDatabase.GetAssetPath (asset);
        //              TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        //
        //              if (importer != null) {
        //                  object[] args = new object[2] { 0, 0 };
        //                  MethodInfo mi = typeof(TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);
        //                  mi.Invoke(importer, args);
        //
        //                  width = (int)args[0];
        //                  height = (int)args[1];
        //
        //                  return true;
        //              }
        //          }
        //
        //          height = width = 0;
        //          return false;
        //      }
        //      public static bool GetImageSizeIos(Texture2D asset, out int width, out int height) {
        //          if (asset != null) {
        //              string assetPath;
        //              assetPath = url;
        //
        //              TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        //
        //              if (importer != null) {
        //                  object[] args = new object[2] { 0, 0 };
        //                  MethodInfo mi = typeof(TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);
        //                  mi.Invoke(importer, args);
        //
        //                  width = (int)args[0];
        //                  height = (int)args[1];
        //
        //                  return true;
        //              }
        //          }
        //
        //          height = width = 0;
        //          return false;
        //      }

    }
}
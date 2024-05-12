using OpenCvSharp;
using OpenCvSharp.Demo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CountorFinder : WebCamera
{
    //can change these values live in Unity
    [SerializeField] private FlipMode ImageFlip; //flip images over desired axis(es)
    [SerializeField] private float Threshold = 96.4f; //limit to decide if black/white
    [SerializeField] private bool ShowProcessingImage = true;
    [SerializeField] private float CurveAccuracy = 10f; //higher value makes it more low-poly, lower value makes the lines more tightfitting. Less points is better for colliders.
    [SerializeField] private float MinArea = 5000f; //raise this until objects you don't want to be recognized are not highlighted in white.
    [SerializeField] private PolygonCollider2D PolygonCollider;


    private Mat image;
    private Mat processImage = new Mat();
    private Point[][] contours;
    private HierarchyIndex[] hierarchy;
    private Vector2[] vectorList;
    protected override bool ProcessTexture(WebCamTexture input, ref Texture2D output)
    {
        image = OpenCvSharp.Unity.TextureToMat(input);

        //flip the image based on the axis, convert to greyscale, apply a threshold to it.
        //Currently, darker colors are inverted to white in the processed image. Possibly because of ThresholdTypes.BinaryInv

        Cv2.Flip(image, image, ImageFlip);
        Cv2.CvtColor(image, processImage, ColorConversionCodes.BGR2GRAY);//convert to grayscale image
        Cv2.Threshold(processImage, processImage, Threshold, 255, ThresholdTypes.BinaryInv);
        Cv2.FindContours(processImage, out contours, out hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple, null);

        PolygonCollider.pathCount = 0; //assume there are no colliders in the scene first before creating contours

        foreach (Point[] contour in contours)
        {
            Point[] points = Cv2.ApproxPolyDP(contour, CurveAccuracy, true);
            var area = Cv2.ContourArea(contour);

            if(area > MinArea)
            {
                drawContour(processImage, new Scalar(127, 127, 127), 2, points);

                PolygonCollider.pathCount++;//If the area meets our requirement to be considered, we increment the pathcount.
                PolygonCollider.SetPath(PolygonCollider.pathCount - 1, toVector2(points));           
            }
        }

        //take the image and shove it into the already existing output otherwise you'll run out of memory

        if (output == null)
            output = OpenCvSharp.Unity.MatToTexture(ShowProcessingImage ? processImage : image); // if ShowProcessingImage, return processImage else return image
        else
            OpenCvSharp.Unity.MatToTexture(ShowProcessingImage ? processImage :image, output);

        return true;
    }

    //Point is openCV's way to recognize 2D point while Vector2 is Unity's - basically same thing but need conversion

    private Vector2[] toVector2(Point[] points)
    {
        vectorList = new Vector2[points.Length];
        for(int i=0;  i<points.Length; i++) 
        {
            vectorList[i] = new Vector2(points[i].X, points[i].Y);
        }
        return vectorList;
    }

    //Take the points, draw it onto the image using the color and thickness of lines
    private void drawContour(Mat Image, Scalar Color, int Thickness, Point[] Points)
    {
        for(int i =1;i<Points.Length; i++) 
        {
            //this draws the contour over the image. Can be circles, rectangles, possibly text? and more. Lines used below
            Cv2.Line(Image, Points[i - 1], Points[i], Color, Thickness);
        }
        Cv2.Line(Image, Points[Points.Length - 1], Points[0], Color, Thickness);//completes the loop 
    }
}

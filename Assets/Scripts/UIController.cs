using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Toolbelt_OJ
{

    public class UIController : MonoBehaviour
    {
        public bool randomizeTimer;
        [SerializeField] private bool hover;
        [SerializeField] private float horizontalHoverDistance = 2f, verticalHoverDistance = 10f;

        [SerializeField] public bool sizeLerp;
        [SerializeField] private float sizeMultiplier = 2f;

        [SerializeField] private bool rotate;
        [SerializeField] private float rotationMinTilt = 0.5f, rotationMaxTilt = 1f;

        [SerializeField] private bool fadeIn;
        [SerializeField] private bool fadeOut;

        [SerializeField, Range (0.1f, 1f)] private float pingPongDuration = 0.5f;
        [SerializeField] private float pingPongTimer; //set if you want to desync from other UI effects

        [SerializeField] private bool move;
        [SerializeField] private Vector3[] movementPoints;
        [SerializeField] private int movementCount;

        [HideInInspector] public bool isText, isImage, pingPongTimerDecrease;
        private TextMeshProUGUI text;
        private Image image;

        //private float startSize;
        /*[HideInInspector]*/ public Vector3 textStartScale, textStartPos, imageStartScale,imageStartPos;
        private Quaternion textStartRotation, imageStartRotation;

        private Color imageColour, invisibleImageColour;

        private void Awake()
        {

            if (gameObject.GetComponent<TextMeshProUGUI>())
            {
                text = gameObject.GetComponent<TextMeshProUGUI>();
                textStartPos = text.rectTransform.position;
                textStartScale = text.rectTransform.localScale;
                textStartRotation = text.rectTransform.localRotation;
                //movementCount = movementPoints.Length;
                isText = true;
            }

            if (gameObject.GetComponent<Image>())
            {
                image = gameObject.GetComponent<Image>();
                imageStartPos = image.rectTransform.rect.position;
                imageStartScale = image.rectTransform.localScale;
                imageStartRotation = image.rectTransform.localRotation;
                imageColour = image.color;
                invisibleImageColour = new Color(imageColour.r, imageColour.g, imageColour.b, 0f);
                isImage = true;
            }

            if (!gameObject.GetComponent<TextMeshProUGUI>() && !gameObject.GetComponent<Image>())
            {
                enabled = false;
            }

            if (randomizeTimer) pingPongTimer = Random.Range(-pingPongDuration, pingPongDuration);
        }

        private void Update()
        {
            if (isText)
            {
                if (hover)
                {
                    UIHover(text, textStartPos, pingPongTimer);
                }

                //if (bounce)
                //{
                //    UIBounce(text, speed, textStartPos, timer);
                //}

                if (sizeLerp)
                {
                    UISizeLerp(text, textStartScale, pingPongTimer);
                }

                if (rotate)
                {
                    UIRotate(text, textStartRotation, pingPongTimer);
                }

                if (fadeIn)
                {
                    UIFadeIn(text);
                }

                if (fadeOut)
                {
                    UIFadeOut(text);
                }

                if (move)
                {
                    UIMovePosition(text);
                }
            }
            
            if (isImage)
            {
                if (hover)
                {
                    UIHover(image, imageStartPos, pingPongTimer);
                }

                //if (bounce)
                //{
                //    UIBounce(image, speed);
                //}

                if (sizeLerp)
                {
                    UISizeLerp(image, imageStartScale, pingPongTimer);
                }

                if (rotate)
                {
                    UIRotate(image, imageStartRotation, pingPongTimer);
                }

                if (fadeIn)
                {
                    UIFadeIn(image);
                }

                if (fadeOut)
                {
                    UIFadeOut(image);
                }

                if (move)
                {
                    UIMovePosition(image);
                }
            }

            if (pingPongTimer <= -pingPongDuration)
            {
                pingPongTimerDecrease = false;
            }

            if (pingPongTimer >= pingPongDuration)
            {
                pingPongTimerDecrease = true;
            }

            if (pingPongTimerDecrease)
            {
                pingPongTimer -= Time.deltaTime;
            }
            else
            {
                pingPongTimer += Time.deltaTime;
            }
            
        }

        //private void OnRectTransformDimensionsChange()
        //{
        //    if (isText)
        //    {
        //        textStartPos = new Vector3(Screen.width / 2, (Screen.height / 2 + (Screen.height / 4)), textStartPos.z);
        //    }
        //    if (isImage)
        //    {
        //        imageStartPos = new Vector3(Screen.width / 2, (Screen.height / 2 + (Screen.height / 4)), imageStartPos.z);
        //    }
        //}

        public void UIHover(TextMeshProUGUI text, Vector3 startPosition, float timer)
        {
            text.rectTransform.position = Vector3.Lerp(new Vector3(startPosition.x + horizontalHoverDistance, startPosition.y - verticalHoverDistance, text.rectTransform.position.z), new Vector3(startPosition.x - horizontalHoverDistance, startPosition.y + verticalHoverDistance, text.rectTransform.position.z), Mathf.PingPong(timer, pingPongDuration));
        }

        public void UIHover(Image image, Vector3 startPosition, float timer)
        {
            image.rectTransform.position = Vector3.Lerp(new Vector3(startPosition.x + horizontalHoverDistance, startPosition.y - verticalHoverDistance, image.rectTransform.position.z), new Vector3(startPosition.x - horizontalHoverDistance, startPosition.y + verticalHoverDistance, image.rectTransform.position.z), Mathf.PingPong(timer, pingPongDuration));
        }

        //public void UIBounce(TextMeshProUGUI text, float speed, Vector3 startPosition, float Timer)
        //{
        //    text.rectTransform.position = Vector3.Lerp(startPosition, new Vector3(startPosition.x, startPosition.y + 10, text.rectTransform.position.z), Mathf.PingPong(timer, pingPongDuration));

        //}
        //public void UIBounce(Image image, float speed)
        //{
        //    image.transform.position = Vector3.Lerp(image.transform.position, new Vector3(image.transform.position.x, image.transform.position.y + 1f, image.transform.position.z), Mathf.PingPong(Time.deltaTime, Mathf.Lerp(0f, 1f, speed * Time.deltaTime)));

        //}

        public void UISizeLerp(TextMeshProUGUI text, Vector3 startSize, float timer)
        {
            Vector3 sizeMin = startSize / sizeMultiplier;
            Vector3 sizeMax = startSize * sizeMultiplier;
            text.rectTransform.localScale = Vector3.Lerp(sizeMin, sizeMax, Mathf.PingPong(timer, pingPongDuration));
        }

        public void UISizeLerp(Image image, Vector3 startSize, float timer)
        {
            Vector3 sizeMin = startSize / sizeMultiplier;
            Vector3 sizeMax = startSize * sizeMultiplier;
            image.rectTransform.localScale = Vector3.Lerp(sizeMin, sizeMax, Mathf.PingPong(timer, pingPongDuration));
        }

        public void ResetSize(TextMeshProUGUI text, Vector3 startSize)
        {
            text.rectTransform.localScale = startSize;
        }

        public void ResetSize(Image image, Vector3 startSize)
        {
            image.rectTransform.localScale = startSize;
        }

        public void UIRotate(TextMeshProUGUI text, Quaternion startRotation, float timer)
        {
            Quaternion rotationMin = new Quaternion(startRotation.x, startRotation.y, startRotation.z - rotationMinTilt, startRotation.w);
            Quaternion rotationMax = new Quaternion(startRotation.x, startRotation.y, startRotation.z + rotationMaxTilt, startRotation.w);
            text.rectTransform.rotation = Quaternion.Lerp(rotationMin, rotationMax, Mathf.PingPong(timer, pingPongDuration));

        }
        
        public void UIRotate(Image image, Quaternion startRotation, float timer)
        {
            Quaternion rotationMin = new Quaternion(startRotation.x, startRotation.y, startRotation.z - rotationMinTilt, startRotation.w);
            Quaternion rotationMax = new Quaternion(startRotation.x, startRotation.y, startRotation.z + rotationMaxTilt, startRotation.w);
            image.rectTransform.rotation = Quaternion.Lerp(rotationMin, rotationMax, Mathf.PingPong(timer, pingPongDuration));

        }

        public void UIFadeIn(TextMeshProUGUI text)
        {
            text.alpha = Mathf.Lerp(1f, 0f, Time.deltaTime);
        }

        public void UIFadeIn(Image image)
        {
            image.color = Color.Lerp(invisibleImageColour, imageColour,  Time.deltaTime);

        }
        public void UIFadeOut(TextMeshProUGUI text)
        {
            text.alpha = Mathf.Lerp(0f, 1f, Time.deltaTime);

        }

        public void UIFadeOut(Image image)
        {
            image.color = Color.Lerp(imageColour, invisibleImageColour, Time.deltaTime);
        }

        public void UIMovePosition(TextMeshProUGUI text)
        {
            text.transform.position += movementPoints[movementCount];

            if (text.transform.position.x > (Screen.width /2) || text.transform.position.y > (Screen.height / 2) || text.transform.position.x < (-Screen.width / 2) || text.transform.position.y < (-Screen.height / 2))
            {
                if (movementCount < movementPoints.Length)
                {
                    movementCount += 1;
                }
                else if (movementCount >= movementPoints.Length)
                {
                    movementCount = 0;
                }
            }
        }
        public void UIMovePosition(Image image)
        {
            image.transform.position += movementPoints[movementCount] * Time.deltaTime;
            Debug.Log("distance = " + Vector3.Distance(image.transform.position, movementPoints[movementCount]).ToString());

            if (image.transform.position.x > (Screen.width / 2) || image.transform.position.y > (Screen.height / 2) || image.transform.position.x < (-Screen.width / 2) || image.transform.position.y < (-Screen.height / 2))
            {
                if (movementCount < movementPoints.Length - 1)
                {
                    movementCount += 1;
                }
                else if (movementCount >= movementPoints.Length - 1)
                {
                    movementCount = 0;
                }
            }
        }
    }
}

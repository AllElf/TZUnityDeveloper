using UnityEngine;
using UnityEngine.UI;

public class BrushPixelColorAutoSampler : MonoBehaviour
{
    [Header("Компонент кисти")]
    public Image brushImage;
    [SerializeField] Color colorDefaulf;
    [SerializeField] bool _default;
    [SerializeField] string tagColor = "Color";
    [SerializeField] string tagGameobject = "Brush";
    public string currentColorName;
    [Header("Сколько секунд кисть должна находиться внутри цвета")]
    public float requiredStayTime = 2f;

    private GameObject currentColorObject;
    private float stayTimer = 0f;
    [Header("Возвращение в дефолтное состояние")]
    [SerializeField] GrabAndMove grabAndMove;


    [System.Obsolete]
    private void Start()
    {
        if (gameObject.tag == tagGameobject)
        {
            _default = false;
            colorDefaulf = brushImage.color;
            grabAndMove = FindObjectOfType<GrabAndMove>();
        }
            
    }
    public void DefaultColor()
    {
        brushImage.color = colorDefaulf;
    }
    void Update()
    {
        if (gameObject.tag == tagGameobject)
        {
            if (grabAndMove.hasTakenObject == false) { DefaultColor(); }
            if (_default) { DefaultColor(); _default = false; }
            if (brushImage == null)
            {
                Debug.LogWarning("brushImage не назначен в инспекторе.");
                return;
            }

            GameObject detected = DetectCurrentColorObject();

            if (detected == currentColorObject)
            {
                stayTimer += Time.deltaTime;

                if (stayTimer >= requiredStayTime)
                {
                    ApplyPixelColor(detected);
                    stayTimer = 0f;
                }
            }
            else
            {
                currentColorObject = detected;
                stayTimer = 0f;
            }
        }
        else if (gameObject.tag != tagGameobject)
        {
            currentColorName = gameObject.name;
        }
    }

    GameObject DetectCurrentColorObject()
    {
        GameObject[] colorObjects = GameObject.FindGameObjectsWithTag(tagColor);

        foreach (GameObject obj in colorObjects)
        {
            RectTransform targetRT = obj.GetComponent<RectTransform>();
            if (targetRT == null) continue;

            if (RectTransformUtility.RectangleContainsScreenPoint(
                targetRT,
                brushImage.rectTransform.position,
                GetUICamera()))
            {
                return obj;
            }
        }

        return null;
    }

    void ApplyPixelColor(GameObject colorObj)
    {
        if (brushImage == null || brushImage.rectTransform == null)
        {
            Debug.LogWarning("Компонент brushImage или его RectTransform не найден.");
            return;
        }

        if (colorObj != null)
        {
            Image colorImage = colorObj.GetComponent<Image>();
            if (colorImage == null || colorImage.rectTransform == null)
            {
                Debug.LogWarning($"Объект '{colorObj.name}' не имеет Image или RectTransform.");
                return;
            }

            Sprite sprite = colorImage.sprite;
            if (sprite == null)
            {
                Debug.LogWarning($"Image у '{colorObj.name}' не содержит Sprite.");
                return;
            }

            Texture2D texture = sprite.texture;
            if (texture == null || !texture.isReadable)
            {
                Debug.LogWarning($"Текстура '{sprite.name}' недоступна для чтения. Включи 'Read/Write Enabled' в импорте.");
                return;
            }

            Vector2 localPos;
            bool valid = RectTransformUtility.ScreenPointToLocalPointInRectangle(
                colorImage.rectTransform,
                brushImage.rectTransform.position,
                GetUICamera(),
                out localPos);

            if (!valid)
            {
                Debug.LogWarning("Не удалось получить локальную точку внутри объекта цвета.");
                return;
            }

            Rect spriteRect = sprite.rect;
            Vector2 normalized = new Vector2(
                (localPos.x + colorImage.rectTransform.rect.width * 0.5f) / colorImage.rectTransform.rect.width,
                (localPos.y + colorImage.rectTransform.rect.height * 0.5f) / colorImage.rectTransform.rect.height
            );

            int texX = Mathf.Clamp(Mathf.FloorToInt(spriteRect.x + normalized.x * spriteRect.width), 0, texture.width - 1);
            int texY = Mathf.Clamp(Mathf.FloorToInt(spriteRect.y + normalized.y * spriteRect.height), 0, texture.height - 1);

            Color pixelColor = texture.GetPixel(texX, texY);
            brushImage.color = pixelColor;
            currentColorName = colorObj.name; 
        }
    }

    Camera GetUICamera()
    {
        Canvas canvas = brushImage.canvas;
        if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceCamera)
            return canvas.worldCamera;

        return null;
    }
}
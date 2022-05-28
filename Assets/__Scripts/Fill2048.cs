using UnityEngine;
using UnityEngine.UI;

namespace __Scripts
{
    public class Fill2048 : MonoBehaviour
    {
        [HideInInspector] public int value;
    
        [SerializeField] private Text valueDisplay;
        [SerializeField] private float speed;

        private bool _hasCombine;
        private const float BoostTimer = 5.0f;

        private Image _image;

        private readonly Vector3 _startScale = new Vector3(0.1f, 0.1f, 0.1f);
        private readonly Vector3 _requiredScale = Vector3.one;
    
    
        public void FillValueUpdate(int valueIn)
        {
            value = valueIn;
            valueDisplay.text = value.ToString();

            int colorIndex = GetColorIndex(value);
        
            _image = GetComponent<Image>();
            _image.color = GameController.Instance.fillColors[colorIndex];

        }

        private int GetColorIndex(int valueIn)
        {
            int index = 0;
            while (valueIn != 1)
            {
                index++;
                valueIn /= 2;
            }

            index--;
            return index;

        }

        private void Start()
        {
            transform.localScale = _startScale;
        }

        private void Update()
        {
        
            if (transform.localScale != Vector3.one)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, _requiredScale, Time.deltaTime * BoostTimer);
            }
        
            if (transform.localPosition != Vector3.zero)
            {
                _hasCombine = false;
                transform.localPosition =
                    Vector3.MoveTowards(transform.localPosition, Vector3.zero, speed * Time.deltaTime);
            }
            else if (_hasCombine == false)
            {
                if (transform.parent.GetChild(0) != this.transform)
                {
                    Destroy(transform.parent.GetChild(0).gameObject);
                }

            
                _hasCombine = true;
            }
        }

        public void DoubleValue()
        {
            value *= 2;
            GameController.Instance.ScoreUpdate(value);
            valueDisplay.text = value.ToString();
        
            int colorIndex = GetColorIndex(value);
        
        
            _image.color = GameController.Instance.fillColors[colorIndex];
        
            GameController.Instance.WinningCheck(value);
        
        }

    }
}

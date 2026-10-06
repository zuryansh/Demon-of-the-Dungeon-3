using UnityEngine;
using UnityEngine.Pool;
public class PopupTextManager : Singleton<PopupTextManager> 
{
        

        [SerializeField] private PopupText popupTextPRefab;
        [SerializeField] private int initialPoolSize = 20;
        
        private ObjectPool<PopupText> pool;

        protected override void Awake()
        {
            base.Awake();
            pool = new ObjectPool<PopupText>(
                Create,
                OnGet,
                OnRelease,
                OnKill,
                false,
                initialPoolSize,
                100
            );
        }

        private PopupText Create()
        {
            PopupText popup = Instantiate(popupTextPRefab, transform);
            popup.Initialize(this);
            return popup;
        }

        private void OnGet(PopupText text)
        {
            text.gameObject.SetActive(true);
            text.ResetState();
    }

        private void OnRelease(PopupText text)
        {
            text.gameObject.SetActive(false);
        }



        private void OnKill(PopupText text)
        {
            Destroy(text.gameObject);
        }

    public void Show(string text, Vector3 position, Color color,float scale = 1f, float fadeDuration = 0.5f, float activeDuration = 1f)
        {
            PopupText popupText = pool.Get();
            popupText.Show(text, position, color,scale, fadeDuration,activeDuration);
        }

        public void Release(PopupText text)
        {
            pool.Release(text);
        }
    
}

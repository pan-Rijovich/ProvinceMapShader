namespace UnityEngine
{
    public static class ComponentExtension
    {
        public static void Activate(this Component component)
        {
            component.gameObject.SetActive(true);
        }

        public static void Disactivate(this Component component)
        {
            component.gameObject.SetActive(false);
        }

        public static void SetActive(this Component component, bool value)
        {
            component.gameObject.SetActive(value);
        }

        public static bool IsActive(this Component component)
        {
            return component.gameObject.activeInHierarchy;
        }
    }
}
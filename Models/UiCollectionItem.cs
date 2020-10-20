namespace Proggy.Models
{
    public class UiCollectionItem<T>
    {
        public string Title { get; }
        public T Value { get; }

        public UiCollectionItem(string title, T value)
        {
            Title = title;
            Value = value;
        }

        public override string ToString()
        {
            return Title;
        }
    }
}

namespace Proggy.ViewModels.CollectionItems
{
    public class ListItem<T>
    {
        public string Title { get; }
        public T Value { get; }

        public ListItem(string title, T value)
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

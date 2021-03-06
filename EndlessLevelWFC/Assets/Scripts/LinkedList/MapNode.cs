namespace LinkedList
{
    public class MapNode<T>
    {
        private string s;

        public T Data { get; set; }
        public MapNode<T> Left { get; set; }
        public MapNode<T> Right { get; set; }
        public MapNode<T> Top { get; set; }
        public MapNode<T> Bottom { get; set; }

        public MapNode(T data)
        {
            Data = data;
        }

        public MapNode(string s)
        {
            this.s = s;
        }
    }
}

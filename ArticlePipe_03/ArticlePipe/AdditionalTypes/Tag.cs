namespace ArticlePipe.AdditionalTypes
{
    public struct Tag
    {
        private readonly string _tag;

        public string TagName => _tag;

        public Tag(string tag)
        {
            _tag = tag;
        }


        public static implicit operator Tag(string tag)
        {
            var res = new Tag(tag);
            return res;
        }


        public override string ToString()
        {
            return _tag;
        }
    }
}
using System.Collections.Generic;
using System.Text;
using ArticlePipe.AdditionalTypes;
using ArticlePipe.Extensions;
using ArticlePipe.interfaces;

namespace ArticlePipe.Implementations.HabraHabrImplementation
{
    public sealed class HabrahabrArticle : IArticle<string>
    {
        private readonly HashSet<Tag> _tags;
        private readonly string _data;
        private readonly string _title;
        private readonly string _date;
        
        
        #region Constructors

        public HabrahabrArticle(string data, string title, IEnumerable<Tag> tags, string date)
        {
            _data = data;
            _title = title;
            _tags = tags.ToHashSet();
            _date = date;
        }

        #endregion


        #region Properties
        public string Title => _title;
        public string Data => _data;
        public string Date => _date;

        #endregion



        #region IArticle
        public HashSet<Tag> Tags => _tags;

        public string GetArticle()
        {
            var outSb = new StringBuilder();

            outSb.Append(Title + "\n");
            outSb.Append(Date+"\n");
            outSb.Append(Data);

            return outSb.ToString();
        } 
        #endregion
    }
}

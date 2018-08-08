using System;
using System.Collections.Generic;
using ArticlePipe.AdditionalTypes;
using ArticlePipe.GenericImplementation;
using ArticlePipe.interfaces;
using HtmlAgilityPack;
using Microsoft.Win32;

namespace ArticlePipe.Implementations.HabraHabrImplementation
{
    public sealed class HabraHabrPublisher : IPublisher<string>
    {
        private int _postsNumber;
        
        #region Constructors

        public HabraHabrPublisher()
        {
            _postsNumber = GetNumberOfPosts();
        }

        public HabraHabrPublisher(GenericSubscriptionManager<string> manager)
        {
            manager.AddPublisher(this);
            CreateNew();
        }

        #endregion


        #region Create Methods

        public void CreateNew()
        {
            _postsNumber = GetNumberOfPosts();
            const string userRoot = "HKEY_CURRENT_USER";
            const string subkey = "ArticlePipe";
            const string keyName = userRoot + "\\" + subkey;
            const string fieldName = "NumberOfHabrPost";

            var lastValueObj = Registry.GetValue(keyName, fieldName, null);


            if (lastValueObj == null)
            {
                Registry.SetValue(keyName, fieldName, _postsNumber);
            }
            else
            {
                var lastValue = (int)lastValueObj;
                var isEqual = lastValue == _postsNumber;
                if (!isEqual)
                {
                    Registry.SetValue(keyName, fieldName, _postsNumber);
                    CreateFromRange(lastValue, _postsNumber);
                }
            }
        }

        public void CreateFrom(string url)
        {
            try
            {
                var habrArticle = HabrParcer(url);
                Created?.Invoke(this, habrArticle);
            }
            catch (Exception)
            {
                Console.WriteLine($"#Error: url - [{url}] is can't parsed");
            }
        }

        public void CreateFromRange(int min, int max)
        {
            
            if(max < min)
                throw new Exception("Uncorrect range");

            if (max > _postsNumber)
                max = _postsNumber;

            for (var i = min; i <= max; ++i)
            {
                CreateFromArticleNumber(i);
            }
        }

        public bool CreateFromArticleNumber(int articleNumber)
        {
            var url = "https://habr.com/post/" + articleNumber + "/";

            try
            {
                var habrArticle = HabrParcer(url);
                Created?.Invoke(this, habrArticle);
                return true;
            }
            catch
            {
                // ignored
            }

            return false;
        }

        public void CreateRandom()
        {
            var isCreate = false;

            while (!isCreate)
            {
                var random = new Random();
                var randomNumber = random.Next(1, _postsNumber);

                isCreate = CreateFromArticleNumber(randomNumber);
            }


        }

        #endregion


        #region Parser 

        private static HabrahabrArticle HabrParcer(string url)
        {
            var web = new HtmlWeb();
            var doc = web.Load(url);

            var postNode = doc.DocumentNode.SelectSingleNode("//*[@class=\"post__body post__body_full\"]");

            var dataNode = postNode.SelectSingleNode("//*[@data-io-article-url]");
            var data = dataNode.InnerText;

            var titleNode = postNode.SelectSingleNode("//*[@class=\"post__title-text\"]");
            var title = titleNode.InnerText;

            var tagsNode = postNode.SelectSingleNode("//*[@class=\"post__tags\"]");
            var tags = tagsNode.InnerText;
            var nodeSeparators = new[] {'\n'};
            var separatedTags = tags.Split(nodeSeparators, StringSplitOptions.RemoveEmptyEntries);

            var processedTagsList = new List<Tag>();

            for (var i = 1; i < separatedTags.Length - 2; ++i)
            { 
                var trimTag = separatedTags[i].Trim(' ');
                if (trimTag.Length != 0)
                        processedTagsList.Add(trimTag);
            }

            var dateNode = tagsNode.SelectSingleNode("//*[@class=\"post__time\"]");
            var date = dateNode.InnerText;

            return new HabrahabrArticle(data, title, processedTagsList, date);
        }

        private static int GetNumberOfPosts()
        {
            var web = new HtmlWeb();
            var doc = web.Load("https://habr.com/all/");

            var lastPostNode = doc.DocumentNode.SelectSingleNode("//*[@class=\"content-list__item content-list__item_post shortcuts_item\"]");
            var idInClass = lastPostNode.Id;
            var normalId = idInClass.Remove(0, 5);

            int.TryParse(normalId, out var result);

            return result;
        }

        #endregion


        #region IPublisher

        public event EventHandler<IArticle<string>> Created;

        #endregion
    }

}

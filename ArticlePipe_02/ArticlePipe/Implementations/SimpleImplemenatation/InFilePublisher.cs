using System;
using System.IO;
using System.Linq;
using ArticlePipe.AdditionalTypes;
using ArticlePipe.GenericImplementation;
using ArticlePipe.interfaces;

namespace ArticlePipe.Implementations.SimpleImplemenatation
{
    public sealed class InFilePublisher : IPublisher<string>
    {

        public void Create(string inputPath)
        {
            var sr = new StreamReader(inputPath);
            var tagsFromFile = sr.ReadLine();

            var separators = new[] {' '};

            var tags = tagsFromFile?.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            var data = sr.ReadToEnd();
            var realTags = tags?.Select(x=>new Tag(x)).ToList();
            var newArticle = new GenericArticle<string>(realTags, data);
            sr.Dispose();
            Created?.Invoke(this, newArticle);

        }

        public event EventHandler<IArticle<string>> Created;
    }
}

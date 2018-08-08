using System;
using System.Collections.Generic;
using ArticlePipe.AdditionalTypes;
using ArticlePipe.GenericImplementation;
using ArticlePipe.Implementations.HabraHabrImplementation;
using ArticlePipe.Implementations.SimpleImplemenatation;

namespace ArticlePipeLauncher
{
    class Program
    {
        static void Main(string[] args)
        {
            var manager = new GenericSubscriptionManager<string>();

            const int iterationCount = 2;

            var tagList = new HashSet<Tag>() {"2018", "java"};
            var subscriber = new GenericSubscriber<string>($"subscriber 1",tagList);
            manager.AddSubscriber(subscriber);

            var tagList2 = new HashSet<Tag>() {"2018", "путин", "splunk"};
            var subscriber2 = new GenericSubscriber<string>($"subscriber 2", tagList2);
            manager.AddSubscriber(subscriber2);

            var tagList3 = new HashSet<Tag>() {"путин", "утечка памяти", "linux kernel" };
            var subscriber3 = new GenericSubscriber<string>($"subscriber 3", tagList3);
            manager.AddSubscriber(subscriber3);

            var publisher = new InFilePublisher();
            manager.AddPublisher(publisher);
            //publisher.Create("input1.txt");

            var habrPublisher = new HabraHabrPublisher();
            manager.AddPublisher(habrPublisher);
            //habrPublisher.CreateRandom();
            habrPublisher.CreateFrom("https://habr.com/company/piter/blog/354532/");
            habrPublisher.CreateFrom("https://habr.com/company/spbau/blog/224589/");


            Console.ReadKey();
        }
    }
}

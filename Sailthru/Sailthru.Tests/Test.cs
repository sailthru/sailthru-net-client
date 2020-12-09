using NUnit.Framework;
using System;
using Sailthru.Tests.Mock;
using Sailthru.Models;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using static Sailthru.Models.UserRequest;
using Newtonsoft.Json;

namespace Sailthru.Tests
{
    [TestFixture]
    public class Test
    {
        private static ApiServer server = new ApiServer();
        private SailthruClient client = new SailthruClient("3386", "3386", server.ApiUrl);

        [Test]
        public void PostListEraseJob()
        {
            Hashtable jobParams = new Hashtable()
            {
                ["lists"] = new string[] { "foo", "bar" }
            };

            SailthruResponse response = client.ProcessJob("list_erase", null, null, jobParams);
            Assert.IsTrue(response.IsOK());
            Assert.AreEqual(response.HashtableResponse["name"], "Bulk List Erase: 2 requested");
        }

        [Test]
        public void PostUser()
        {
            UserRequest request = new UserRequest();
            request.Id = "test@sailthru.com";
            request.Login = "test@sailthru.com";
            request.OptoutEmail = OptoutStatus.Basic;
            request.Vars = new Hashtable()
            {
                ["foobar"] = "test"
            };
            request.Fields = new Hashtable()
            {
                ["vars"] = 1,
                ["optout_email"] = 1
            };

            SailthruResponse response = client.SetUser(request);
            Assert.IsTrue(response.IsOK());

            Assert.AreEqual(response.HashtableResponse["optout_email"], "basic");

            Hashtable vars = response.HashtableResponse["vars"] as Hashtable;
            Assert.AreEqual(vars["foobar"], "test");
        }

        [Test]
        public void UnicodeResponse()
        {
            string unicodeString = "ä好😃⇔";

            UserRequest request = new UserRequest();
            request.Id = "test@sailthru.com";
            request.Vars = new Hashtable()
            {
                ["unicode"] = unicodeString
            };
            request.Fields = new Hashtable()
            {
                ["vars"] = 1,
            };

            SailthruResponse response = client.SetUser(request);
            Assert.IsTrue(response.IsOK());

            Hashtable vars = response.HashtableResponse["vars"] as Hashtable;
            Assert.AreEqual(vars["unicode"], unicodeString);
        }

        [Test]
        public void PostEvent()
        {
            EventRequest request = new EventRequest();
            request.Id = "test@sailthru.com";
            request.Event = "hello";
            request.Vars = new Hashtable()
            {
                ["var1"] = "yes"
            };

            SailthruResponse response = client.PostEvent(request);
            Assert.IsTrue(response.IsOK());
        }

        [Test]
        public void PostImportJob()
        {
            List<String> emails = new List<String>() {
                "user1@example.com",
                "user2@example.com",
                "user3@example.com"
            };

            SailthruResponse response = client.ProcessImportJob("test_list", emails);
            Assert.IsTrue(response.IsOK());
            Assert.AreEqual(response.HashtableResponse["name"], "List Import: test_list");
        }

        [Test]
        [Ignore("mock api server missing support for form data")]
        public void PostUpdateJob()
        {
            string filename = Path.GetTempFileName();

            using (StreamWriter writer = new StreamWriter(filename))
            {
                writer.WriteLine("{\"id\":\"user1@example.com\", \"signup_date\":\"1987-08-01\"}");
                writer.WriteLine("{\"id\":\"user2@example.com\", \"signup_date\":\"1987-08-01\"}");
                writer.WriteLine("{\"id\":\"user3@example.com\", \"signup_date\":\"1987-08-01\"}");
            }

            Hashtable updateParams = new Hashtable()
            {
                ["file"] = filename
            };

            SailthruResponse response = client.ProcessJob("update", null, null, updateParams);
            Assert.IsTrue(response.IsOK());
            Assert.AreEqual(response.HashtableResponse["name"], "Bulk Update");
        }

        [Test]
        public void GetListWithFields()
        {
            Hashtable request = new Hashtable();
            request["list"] = "List With 2 Users";
            request["fields"] = new Dictionary<string, object>()
            {
                ["vars"] = 1
            };

            Hashtable parameters = new Hashtable()
            {
                ["json"] = JsonConvert.SerializeObject(request)
            };

            SailthruResponse response = client.ApiGet("list", parameters);
            Assert.IsTrue(response.IsOK());
        }

        [Test]
        [Timeout(10000)]
        public void Timeout()
        {
            EventRequest request = new EventRequest();
            request.Id = "test@example.com";
            request.Event = "trigger_timeout";

            SailthruClient clientWithTimeout = new SailthruClient("3386", "3386", server.ApiUrl);
            clientWithTimeout.Timeout = 1000;
            SailthruResponse response = clientWithTimeout.PostEvent(request);
            Assert.IsFalse(response.IsOK());
        }

        [Test]
        public void PostPurchase()
        {
            PurchaseRequest request = new PurchaseRequest();
            request.Email = "test@example.com";
            request.PurchaseKeys = new Hashtable()
            {
                ["extid"] = "12345"
            };

            Hashtable item1 = new Hashtable();
            item1.Add("qty", 1);
            item1.Add("id", "abc");
            item1.Add("title", "This new product");
            item1.Add("price", 1099);
            item1.Add("url", "http://www.example.com/thisnewproduct.html");

            Hashtable item2 = new Hashtable();
            item2.Add("qty", 2);
            item2.Add("id", "def");
            item2.Add("title", "water bottle");
            item2.Add("price", 199);
            item2.Add("url", "http://www.example.com/water.html");

            ArrayList items = new ArrayList();
            items.Add(item1);
            items.Add(item2);
            request.Items = items;

            SailthruResponse response = client.Purchase(request);
            Assert.IsTrue(response.IsOK());

            Hashtable purchaseResult = (Hashtable)response.HashtableResponse["purchase"];
            Assert.AreEqual(purchaseResult["price"], 1497);
            Assert.AreEqual(purchaseResult["qty"], 3);

            Hashtable purchaseKeys = purchaseResult["purchase_keys"] as Hashtable;
            Assert.AreEqual(purchaseKeys["extid"], "12345");
        }

        [Test]
        public void PostDraftBlast()
        {
            BlastRequest request = new BlastRequest
            {
                Name = "test",
                List = "list",
                Subject = "test"
            };

            SailthruResponse response = client.ScheduleBlast(request);
            Assert.IsTrue(response.IsOK());
            Assert.AreEqual(response.HashtableResponse["status"], "created");
            Assert.AreEqual(response.HashtableResponse["name"], "test");
        }

        [Test]
        public void SetContentWithUrl()
        {
            ContentRequest request = new ContentRequest();
            request.Url = "http://example.com/product";

            SailthruResponse response = client.SetContent(request);
            Assert.IsTrue(response.IsOK());

            ArrayList contents = response.HashtableResponse["content"] as ArrayList;
            Hashtable content = contents[0] as Hashtable;
            Assert.AreEqual(content["url"], request.Url);
        }

        [Test]
        public void SetContentWithId()
        {
            ContentRequest request = new ContentRequest();
            request.Id = "http://example.com/product";
            request.Key = "url";
            request.Keys = new Hashtable
            {
                ["sku"] = "123abc"
            };
            request.Title = "Product Name Here";
            request.Description = "Product info text goes here.";
            request.Date = DateTime.Now.ToString();
            request.Tags = new string[] { "blue", "jeans", "size-m" };
            request.Vars = new Hashtable
            {
                ["var1"] = "var1 value"
            };
            request.Images = new Hashtable
            {
                ["full"] = new Hashtable
                {
                    ["url"] = "http://example.com/images/product.jpg"
                }
            };
            request.SiteName = "Store";

            SailthruResponse response = client.SetContent(request);
            Assert.IsTrue(response.IsOK());

            ArrayList contents = response.HashtableResponse["content"] as ArrayList;
            Hashtable content = contents[0] as Hashtable;
            Assert.AreEqual("123abc", content["sku"]);
        }
    }
}

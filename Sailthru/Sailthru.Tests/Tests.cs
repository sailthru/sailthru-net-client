// Licensed to the .NET Foundation under one or more agreements. The .NET Foundation licenses this
// file to you under the MIT license.

using System.Collections;
using Newtonsoft.Json;
using Sailthru.Models;
using Sailthru.Tests.Mock;
using static Sailthru.Models.BlastRequest;
using static Sailthru.Models.ContentRequest;
using static Sailthru.Models.UserRequest;

namespace Sailthru.Tests
{
    [TestClass]
    public class Tests
    {
        private static readonly ApiServer s_server = new();
        private readonly SailthruClient _client = new("3386", "3386", s_server.ApiUrl);

        [TestMethod]
        public void PostListEraseJob()
        {
            Hashtable jobParams = new()
            {
                ["lists"] = new string[] { "foo", "bar" }
            };

            SailthruResponse response = _client.ProcessJob("list_erase", null, null, jobParams);
            Assert.IsTrue(response.IsOK());
            Assert.AreEqual(response.HashtableResponse["name"], "Bulk List Erase: 2 requested");
        }

        [TestMethod]
        public void PostUser()
        {
            UserRequest request = new()
            {
                Id = "test@sailthru.com",
                Login = "test@sailthru.com",
                OptoutEmail = OptoutStatus.Basic,
                Vars = new Hashtable()
                {
                    ["foobar"] = "test"
                },
                Fields = new Hashtable()
                {
                    ["vars"] = 1,
                    ["optout_email"] = 1
                }
            };

            SailthruResponse response = _client.SetUser(request);
            Assert.IsTrue(response.IsOK());

            Assert.AreEqual(response.HashtableResponse["optout_email"], "basic");

            Hashtable vars = response.HashtableResponse["vars"] as Hashtable;
            Assert.AreEqual(vars["foobar"], "test");
        }

        [TestMethod]
        public void UnicodeResponse()
        {
            string unicodeString = "ä好😃⇔";

            UserRequest request = new()
            {
                Id = "test@sailthru.com",
                Vars = new Hashtable()
                {
                    ["unicode"] = unicodeString
                },
                Fields = new Hashtable()
                {
                    ["vars"] = 1,
                }
            };

            SailthruResponse response = _client.SetUser(request);
            Assert.IsTrue(response.IsOK());

            Hashtable vars = response.HashtableResponse["vars"] as Hashtable;
            Assert.AreEqual(vars["unicode"], unicodeString);
        }

        [TestMethod]
        public void PostEvent()
        {
            EventRequest request = new()
            {
                Id = "test@sailthru.com",
                Event = "hello",
                Vars = new Hashtable()
                {
                    ["var1"] = "yes"
                }
            };

            SailthruResponse response = _client.PostEvent(request);
            Assert.IsTrue(response.IsOK());
        }

        [TestMethod]
        public void PostImportJob()
        {
            List<string> emails = new() {
                "user1@example.com",
                "user2@example.com",
                "user3@example.com"
            };

            SailthruResponse response = _client.ProcessImportJob("test_list", emails);
            Assert.IsTrue(response.IsOK());
            Assert.AreEqual(response.HashtableResponse["name"], "List Import: test_list");
        }

        [TestMethod]
        [Ignore("mock api server missing support for form data")]
        public void PostUpdateJob()
        {
            string filename = Path.GetTempFileName();

            using (StreamWriter writer = new(filename))
            {
                writer.WriteLine("{\"id\":\"user1@example.com\", \"signup_date\":\"1987-08-01\"}");
                writer.WriteLine("{\"id\":\"user2@example.com\", \"signup_date\":\"1987-08-01\"}");
                writer.WriteLine("{\"id\":\"user3@example.com\", \"signup_date\":\"1987-08-01\"}");
            }

            Hashtable updateParams = new()
            {
                ["file"] = filename
            };

            SailthruResponse response = _client.ProcessJob("update", null, null, updateParams);
            Assert.IsTrue(response.IsOK());
            Assert.AreEqual(response.HashtableResponse["name"], "Bulk Update");
        }

        [TestMethod]
        public void GetListWithFields()
        {
            Hashtable request = new()
            {
                ["list"] = "List With 2 Users",
                ["fields"] = new Dictionary<string, object>()
                {
                    ["vars"] = 1
                }
            };

            Hashtable parameters = new()
            {
                ["json"] = JsonConvert.SerializeObject(request)
            };

            SailthruResponse response = _client.ApiGet("list", parameters);
            Assert.IsTrue(response.IsOK());
        }

        [TestMethod]
        [Timeout(10000)]
        public void Timeout()
        {
            EventRequest request = new()
            {
                Id = "test@example.com",
                Event = "trigger_timeout"
            };

            SailthruClient clientWithTimeout = new("3386", "3386", s_server.ApiUrl)
            {
                Timeout = 1000
            };
            SailthruResponse response = clientWithTimeout.PostEvent(request);
            Assert.IsFalse(response.IsOK());
        }

        [TestMethod]
        public void PostPurchase()
        {
            PurchaseRequest request = new()
            {
                Email = "test@example.com",
                PurchaseKeys = new Hashtable()
                {
                    ["extid"] = "12345"
                }
            };

            Hashtable item1 = new()
            {
                { "qty", 1 },
                { "id", "abc" },
                { "title", "This new product" },
                { "price", 1099 },
                { "url", "http://www.example.com/thisnewproduct.html" }
            };

            Hashtable item2 = new()
            {
                { "qty", 2 },
                { "id", "def" },
                { "title", "water bottle" },
                { "price", 199 },
                { "url", "http://www.example.com/water.html" }
            };

            ArrayList items = new()
            {
                item1,
                item2
            };
            request.Items = items;

            SailthruResponse response = _client.Purchase(request);
            Assert.IsTrue(response.IsOK());

            Hashtable purchaseResult = (Hashtable)response.HashtableResponse["purchase"];
            Assert.AreEqual(1497d, purchaseResult["price"]);
            Assert.AreEqual(3d, purchaseResult["qty"]);

            Hashtable purchaseKeys = purchaseResult["purchase_keys"] as Hashtable;
            Assert.AreEqual(purchaseKeys["extid"], "12345");
        }

        [TestMethod]
        public void PostDraftBlast()
        {
            BlastRequest request = new()
            {
                Name = "test",
                List = "list",
                Subject = "test",
                ScheduleTime = "+3 hours",
                Status = StatusType.Draft
            };

            SailthruResponse response = _client.ScheduleBlast(request);
            Assert.IsTrue(response.IsOK());
            Assert.AreEqual(response.HashtableResponse["status"], "created");
            Assert.AreEqual(response.HashtableResponse["name"], "test");
        }

        [TestMethod]
        public void PostRetargetingBlastMissingId()
        {
            BlastRequest request = new()
            {
                Name = "test",
                List = "list",
                Subject = "test",
                ScheduleTime = "+3 hours",
                MessageCriteria = MessageCriteriaType.NotOpened,
                PreviousBlastId = 500
            };

            SailthruResponse response = _client.ScheduleBlast(request);
            Assert.IsFalse(response.IsOK());
            Assert.AreEqual("No blast found with id: 500", response.HashtableResponse["errormsg"]);
        }

        [TestMethod]
        public void PostScheduledBlast()
        {
            string content = @"Lorem ipsum dolor sit amet, consectetuer adipiscing elit.
            Aenean commodo ligula eget dolor.Aenean massa.
            Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus.
            Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem.
            Nulla consequat massa quis enim.Donec pede justo, fringilla vel, aliquet nec, vulputate eget, arcu.
            In enim justo, rhoncus ut, imperdiet a, venenatis vitae, justo.";

            BlastRequest request = new()
            {
                Name = "Blast Name1",
                ContentHtml = content,
                ContentText = content,
                List = "List With 2 Users",
                Subject = "Sample Subject",
                ScheduleTime = "+3 hours",
                FromName = "C# Client",
                FromEmail = "danny+fake@sailthru.com",
                SeedEmails = new string[] { "seed1@example.com", "seed2@example.com" },
                Labels = new Dictionary<string, LabelType>
                {
                    ["AddLabel"] = LabelType.Add,
                    ["RemoveLabel"] = LabelType.Remove
                }
            };

            SailthruResponse response = _client.ScheduleBlast(request);
            Assert.IsTrue(response.IsOK());
            Assert.AreEqual("Blast Name1", response.HashtableResponse["name"]);
            Assert.AreEqual("scheduled", response.HashtableResponse["status"]);
            CollectionAssert.AreEquivalent(request.SeedEmails, (ICollection)response.HashtableResponse["seed_emails"]);
            CollectionAssert.AreEquivalent(new string[] { "AddLabel" }, (ICollection)response.HashtableResponse["labels"]);
        }

        [TestMethod]
        public void PostScheduledBlastNoScheduleTime()
        {
            BlastRequest request = new()
            {
                Name = "test",
                List = "list",
                Subject = "test",
                Status = StatusType.Scheduled
            };

            SailthruResponse response = _client.ScheduleBlast(request);
            Assert.IsFalse(response.IsOK());
            Assert.AreEqual(2d, response.HashtableResponse["error"]);
        }

        [TestMethod]
        public void Send()
        {
            SendRequest request = new()
            {
                Email = "test+123@sailthru.com",
                Template = "Sandbox Image Template",
                DataFeedUrl = "https://feed.sailthru.com/ws/feed?id=59010e2dade9c209738b456a",
                Vars = new Hashtable
                {
                    ["hello"] = "world"
                }
            };

            SailthruResponse response = _client.Send(request);
            Assert.IsTrue(response.IsOK());
            Assert.AreEqual(request.Email, response.HashtableResponse["email"]);
            Assert.AreEqual(request.Template, response.HashtableResponse["template"]);
        }

        [TestMethod]
        public void SendMissingTemplate()
        {
            SendRequest request = new()
            {
                Email = "test+123@sailthru.com",
                Template = "Not Exist"
            };

            SailthruResponse response = _client.Send(request);
            Assert.IsFalse(response.IsOK());
            Assert.AreEqual(14d, response.HashtableResponse["error"]);
        }

        [TestMethod]
        public void GetContent()
        {
            SailthruResponse response = _client.GetContent("http://www.sailthru.com/welcome-emails-your-first-hand-shake-with-your-new-user");
            Assert.IsTrue(response.IsOK());

            Assert.AreEqual("Marketing Team", response.HashtableResponse["author"]);
            Assert.AreEqual("Sailthru", response.HashtableResponse["site_name"]);
        }

        [TestMethod]
        public void GetMissingContent()
        {
            SailthruResponse response = _client.GetContent("http://example.com/missing");
            Assert.IsFalse(response.IsOK());
            Assert.IsTrue(((string)response.HashtableResponse["errormsg"]).StartsWith("Content not found", StringComparison.Ordinal));
        }

        [TestMethod]
        public void GetContents()
        {
            SailthruResponse response = _client.GetContents(3);
            Assert.IsTrue(response.IsOK());

            ArrayList contents = response.HashtableResponse["content"] as ArrayList;
            Assert.AreEqual(3, contents.Count);
        }

        [TestMethod]
        public void SetContentWithUrl()
        {
            ContentRequest request = new()
            {
                Url = "http://example.com/product"
            };

            SailthruResponse response = _client.SetContent(request);
            Assert.IsTrue(response.IsOK());

            ArrayList contents = response.HashtableResponse["content"] as ArrayList;
            Hashtable content = contents[0] as Hashtable;
            Assert.AreEqual(content["url"], request.Url);
        }

        [TestMethod]
        public void SetContentWithId()
        {
            ContentRequest request = new()
            {
                Id = "http://example.com/product",
                Key = "url",
                Keys = new Hashtable
                {
                    ["sku"] = "123abc"
                },
                Title = "Product Name Here",
                Description = "Product info text goes here.",
                Date = DateTime.Now.ToString(),
                Tags = new string[] { "blue", "jeans", "size-m" },
                Vars = new Hashtable
                {
                    ["var1"] = "var1 value"
                },
                Images = new Hashtable
                {
                    ["full"] = new Hashtable
                    {
                        ["url"] = "http://example.com/images/product.jpg"
                    }
                },
                SiteName = "Store",
                Price = 1299,
                Inventory = 100,
                OverrideExclude = OverrideExcludeType.Enabled
            };

            SailthruResponse response = _client.SetContent(request);
            Assert.IsTrue(response.IsOK());

            ArrayList contents = response.HashtableResponse["content"] as ArrayList;
            Hashtable content = contents[0] as Hashtable;
            Assert.AreEqual("123abc", content["sku"]);
            Assert.AreEqual(1299d, content["price"]);
            Assert.AreEqual(100d, content["inventory"]);
        }
    }
}

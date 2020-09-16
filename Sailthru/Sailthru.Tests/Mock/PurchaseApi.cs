using Newtonsoft.Json.Linq;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sailthru.Tests.Mock
{
    public class PurchaseApi
    {
        public static Dictionary<string, object> ProcessPost(JObject request)
        {
            JObject purchaseKeys = (JObject)request["purchase_keys"];
            string extid = purchaseKeys != null ? purchaseKeys["extid"].Value<string>() : null;

            ObjectId purchaseId = ObjectId.GenerateNewId();
            string date = DateTime.Now.ToLocalTime().ToString("ddd, dd MMM yyyy HH:mm:ss zzz");

            int totalQty = 0;
            int totalPrice = 0;
            foreach (JObject item in request["items"])
            {
                int qty = item["qty"].Value<int>();
                int price = item["price"].Value<int>();
                totalQty += qty;
                totalPrice += qty * price;
            }

            Dictionary<string, object> purchase = new Dictionary<string, object>
            {
                ["_id"] = purchaseId.ToString(),
                ["items"] = request["items"],
                ["price"] = totalPrice,
                ["qty"] = totalQty,
                ["time"] = date,
                ["unique_id"] = purchaseId.ToString(),
                ["channel"] = "online",
            };

            if (extid != null)
            {
                purchase.Add("purchase_keys", new Dictionary<string, object>()
                {
                    ["extid"] = extid
                });
            }

            return new Dictionary<string, object>()
            {
                ["purchase"] = purchase
            };
        }
    }
}
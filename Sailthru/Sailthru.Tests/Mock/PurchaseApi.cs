using MongoDB.Bson;
using Newtonsoft.Json.Linq;

namespace Sailthru.Tests.Mock
{
    public class PurchaseApi
    {
        public static Dictionary<string, object> ProcessPost(JObject request)
        {
            JObject purchaseKeys = (JObject)request["purchase_keys"];
            string? extid = purchaseKeys["extid"]?.Value<string>();

            ObjectId purchaseId = ObjectId.GenerateNewId();

            int totalQty = 0;
            int totalPrice = 0;
            foreach (JObject item in request["items"].Cast<JObject>())
            {
                int qty = item["qty"].Value<int>();
                int price = item["price"].Value<int>();
                totalQty += qty;
                totalPrice += qty * price;
            }

            string? date = DateTime.Now.ToLocalTime().ToString("ddd, dd MMM yyyy HH:mm:ss zzz");
            Dictionary<string, object> purchase = new()
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

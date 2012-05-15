# A simple client library to remotely access the Sailthru REST API.

## [Sailthu API Docs](http://docs.sailthru.com/)

## Code Examples

``` C#
string apiKey = "***************";
string secret = "***************";
SailthruClient client = new SailthruClient(apiKey, secret);
SailthruResponse response;
```

### [Send](http://docs.sailthru.com/api/send)

### Send
	//Send
    SailthruResponse response;
    response = client.Send("default2", "abc@sailthru.com");
	
	//recommended to check response from server
	if (response.IsOK())
	{
		//do something
	}
    else
    {
        String errorrResponse = response.RawResponse;
    }
	
### Multi Send
	//multi send
	String[] emails = {"abc@sailthru.com", "xyz@gmail.com"};
	response = client.Multisend("default", emails);
	
### Send Cancel
	//send cancel
	//response = client.CancelSend("TTimJmdj2apLAAPM");

### Get Send
	//get send
	String sendId = "TTizbmdj2YheAAUu";
    response = client.GetSend(sendId);

## [Template](http://docs.sailthru.com/api/template)
### Get Template
	String templateId = "default2";
	response = client.GetTemplate(templatedId);
	
### Save Template
``` C#
TemplateRequest templateRequest = new TemplateRequest();
templateRequest.FromEmail = "praj@sailthru.com";
templateRequest.FromName = "Sailthru";
templateRequest.GoogleAnalytics = TemplateRequest.GoogleAnalyticsType.Disabled;
templateRequest.Template = "default2555";

response = client.SaveTemplate(templateRequest);
```

## [Email](http://docs.sailthru.com/api/email)

### Get Email
	String email = "bill@gates.com";
	response = client.GetEmail(email);
	
### Set Email
	Hashtable fieldsVar = new Hashtable();	
	fieldsVar.Add("name", "prajwal tuladhar");
	String email = "praj@sailthru.com";
	response = client.SetEmail(email, fieldsVar);

## [Blast](http://docs.sailthru.com/api/blast)	

### Set Blast

``` C#
BlastRequest blastRequest = new BlastRequest();
blastRequest.Name = "Blast Name1";
blastRequest.ContentHtml = "<p>Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim. Donec pede justo, fringilla vel, aliquet nec, vulputate eget, arcu. In enim justo, rhoncus ut, imperdiet a, venenatis vitae, justo. Nullam dictum felis eu pede mollis pretium. Integer tincidunt. Cras dapibus. Vivamus elementum semper nisi. Aenean vulputate eleifend tellus. Aenean leo ligula, porttitor eu, consequat vitae, eleifend ac, enim. Aliquam lorem ante, dapibus in, viverra quis, feugiat a, tellus. Phasellus viverra nulla ut metus varius laoreet. Quisque rutrum. Aenean imperdiet. Etiam ultricies nisi vel augue. Curabitur ullamcorper ultricies nisi. Nam eget dui. Etiam rhoncus. Maecenas tempus, tellus eget condimentum rhoncus, sem quam semper libero, sit amet adipiscing sem neque sed ipsum. Nam quam nunc, blandit vel, luctus pulvinar, hendrerit id, lorem. Maecenas nec odio et ante tincidunt tempus. Donec vitae sapien ut libero venenatis faucibus. Nullam quis ante. Etiam sit amet orci eget eros faucibus tincidunt. Duis leo. Sed fringilla mauris sit amet nibh. Donec sodales sagittis magna. Sed consequat, leo eget bibendum sodales, augue velit cursus nunc,</p>";
blastRequest.List = "Sample List";
blastRequest.ContentText = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim. Donec pede justo, fringilla vel, aliquet nec, vulputate eget, arcu. In enim justo, rhoncus ut, imperdiet a, venenatis vitae, justo. Nullam dictum felis eu pede mollis pretium. Integer tincidunt. Cras dapibus. Vivamus elementum semper nisi. Aenean vulputate eleifend tellus. Aenean leo ligula, porttitor eu, consequat vitae, eleifend ac, enim. Aliquam lorem ante, dapibus in, viverra quis, feugiat a, tellus. Phasellus viverra nulla ut metus varius laoreet. Quisque rutrum. Aenean imperdiet. Etiam ultricies nisi vel augue. Curabitur ullamcorper ultricies nisi. Nam eget dui. Etiam rhoncus. Maecenas tempus, tellus eget condimentum rhoncus, sem quam semper libero, sit amet adipiscing sem neque sed ipsum. Nam quam nunc, blandit vel, luctus pulvinar, hendrerit id, lorem. Maecenas nec odio et ante tincidunt tempus. Donec vitae sapien ut libero venenatis faucibus. Nullam quis ante. Etiam sit amet orci eget eros faucibus tincidunt. Duis leo. Sed fringilla mauris sit amet nibh. Donec sodales sagittis magna. Sed consequat, leo eget bibendum sodales, augue velit cursus nunc,";
blastRequest.Subject = "Sample Subject";
blastRequest.ScheduleTime = "+3 hours";
blastRequest.FromName = "C# Client";
blastRequest.FromEmail = "no-reply@sailthru.com";

response = client.ScheduleBlast(blastRequest);
```

### Get Blast

``` C#
String blastId = "252525";
response = client.GetBlast(blastId);
```	
	
## [Stats](http://docs.sailthru.com/api/stats)

### Get Stat
response = client.GetStat("list");
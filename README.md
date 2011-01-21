# A simple client library to remotely access the Sailthru REST API.

## Code Examples
		
	string apiKey = "***************";
	string secret = "***************";
	SailthruClient client = new SailthruClient(apiKey, secret);
	SailthruResponse response;
	
### Send
	//Send
    SailthruResponse response;
    response = client.Send("default2", "abc@sailthru.com");
	
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
	
### Get Template
	String templateId = "default2";
	response = client.GetTemplate(templatedId);
	
### Save Template
	Hashtable fields = new Hashtable();
	String fromName = "Barack Obama";
	String fromEmail = "barack@obama.com";
	String templateName = "default2";
	String subject = "What's up Sarah Palin!";
	fields.Add("from_name", fromName);
	fields.Add("from_email", fromEmail);
	fields.Add("subject", subject);
	response = client.SaveTemplate(templateName, fields);
	
### Get Email
	String email = "bill@gates.com";
	response = client.GetEmail(email);
	
### Set Email
	Hashtable fieldsVar = new Hashtable();	
	fieldsVar.Add("name", "prajwal tuladhar");
	String email = "praj@sailthru.com";
	response = client.SetEmail(email, fieldsVar);
	
### Set Blast
	String blastName = "xyz_blast";
	String listName = "xyz_list";
	String date = "+3 hours";
	String fromName = "prajwal";
	String fromEmail = "praj@infynyxx.com";
	String subject = "Hello World!";
	String contentWithHTML = "<p>Lorem Ispum</p>";
	String contentWithoutHTML = "Lorem Ispum";
	response = client.ScheduleBlast(blastName, listName, date, fromName, fromEmail, subject, contentWithHTML, contentWithoutHTML);
	
### Get Blast
	String blastName = "xyz_blast";
    response = client.GetBlast(blastName);
	
### Import Contacts
	String email = "praj@sailthru.com";
	String password = "p@ssw0rd";
	Boolean includeName = true;
	response = client.ImportContacts(email, password, includeName);
	
## Stats (http://docs.sailthru.com/api/stats)
### Get Stat
	response = client.GetStat("list");
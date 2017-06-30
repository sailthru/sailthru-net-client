using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Sailthru.Models;

namespace Sailthru
{
    public interface ISailthruClient
    {
        /// <summary>
        /// Receive the output of a Post.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        bool ReceiveOptoutPost(NameValueCollection parameters);

        /// <summary>
        /// Receive and verify the output of a Post.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        bool ReceiveVerifyPost(NameValueCollection parameters);

        /// <summary>
        /// Save Template
        /// </summary>
        /// <param name="strTemplateName"></param>
        /// <param name="fields"></param>
        /// <seealso cref="http://docs.sailthru.com/api/template"/>
        /// <returns></returns>
        SailthruResponse SaveTemplate(string strTemplateName, Hashtable fields = null);

        /// <summary>
        /// Save Template
        /// </summary>
        /// <param name="request">TemplateRequest parameters.</param>
        /// <seealso cref="http://docs.sailthru.com/api/template"/>
        /// <returns></returns>
        SailthruResponse SaveTemplate(TemplateRequest request);

        /// <summary>
        /// Get Template
        /// </summary>
        /// <param name="strTemplateName"></param>
        /// <seealso cref="http://docs.sailthru.com/api/template"/>
        /// <returns></returns>
        SailthruResponse GetTemplate(string templateName);

        /// <summary>
        /// Fetch email contacts from an address book at one of the major email providers (aol/gmail/hotmail/yahoo) 
        /// </summary>
        /// <param name="strEmail">Email String</param>
        /// <param name="strPassword">Password String</param>
        /// <param name="boolIncludeNames">Boolean</param>
        /// <seealso cref="http://docs.sailthru.com/api/template"/>
        /// <returns>SailthruResponse Object</returns>
        SailthruResponse ImportContacts(string strEmail, string strPassword, bool boolIncludeNames);

        /// <summary>
        /// Create, update, and/or schedule a blast.
        /// </summary>
        /// <param name="strName"></param>
        /// <param name="strList"></param>
        /// <param name="strScheduleTime"></param>
        /// <param name="strFromName"></param>
        /// <param name="strFromEmail"></param>
        /// <param name="strSubject"></param>
        /// <param name="strContentHtml"></param>
        /// <param name="strContentText"></param>
        /// <param name="htOptions"></param>
        /// <seealso cref="http://docs.sailthru.com/api/blast"/>
        /// <returns></returns>
        SailthruResponse ScheduleBlast(string strName, string strList, string strScheduleTime, string strFromName, string strFromEmail, string strSubject, string strContentHtml, string strContentText, Hashtable htOptions = null);

        /// <summary>
        /// Create, update, and/or schedule a blast.
        /// </summary>
        /// <param name="request">BlastRequest parameters.</param>
        /// <seealso cref="http://docs.sailthru.com/api/blast"/>
        /// <returns></returns>
        SailthruResponse ScheduleBlast(BlastRequest request);

        /// <summary>
        /// Get Blast
        /// </summary>
        /// <param name="strBlastId"></param>
        /// <seealso cref="http://docs.sailthru.com/api/blast"/>
        /// <returns></returns>
        SailthruResponse GetBlast(string blastId);

        /// <summary>
        /// Get information about one of your users.
        /// </summary>
        /// <param name="strEmail"></param>
        /// <returns></returns>
        /// <seealso cref="http://docs.sailthru.com/api/email"/>
        SailthruResponse GetEmail(string email);

        /// <summary>
        /// Get information about one of your users.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <seealso cref="http://docs.sailthru.com/api/email"/>
        SailthruResponse GetEmail (EmailRequest request);

        /// <summary>
        /// Update information about one of your users, including adding and removing the user from lists.
        /// </summary>
        /// <param name="strEmail"></param>
        /// <param name="htVars"></param>
        /// <param name="htLists"></param>
        /// <param name="htTemplates"></param>
        /// <param name="verified"></param>
        /// <param name="optout"></param>
        /// <param name="send"></param>
        /// <param name="sendVars"></param>
        /// <returns></returns>
        /// <seealso cref="http://docs.sailthru.com/api/email"/>
        SailthruResponse SetEmail(string strEmail, 
            Hashtable htVars = null, 
            Hashtable htLists = null, 
            Hashtable htTemplates = null, 
            int verified = 0, 
            String optout = null, 
            string send = null, 
            Hashtable sendVars = null,
            String sms = null,
            String twitter = null,
            String changeEmail = null);

        /// <summary>
        /// Update information about one of your users, including adding and removing the user from lists.
        /// </summary>
        /// <param name="request">EmailRequest parameters.</param>
        /// <returns></returns>
        /// <seealso cref="http://docs.sailthru.com/api/email"/>
        SailthruResponse SetEmail(EmailRequest request);

        /// <summary>
        /// Send a transactional email for multiple users
        /// </summary>
        /// <param name="strTemplateName"></param>
        /// <param name="strEmail"></param>
        /// <param name="htVars"></param>
        /// <param name="htOptions"></param>
        /// <returns></returns>
        /// <seealso cref="http://docs.sailthru.com/api/send"/>
        SailthruResponse Multisend(string strTemplateName, string[] strEmail, Hashtable htVars = null, Hashtable htOptions = null);

        /// <summary>
        /// Send a transactional Email for a single user
        /// </summary>
        /// <param name="strTemplateName"></param>
        /// <param name="strEmail"></param>
        /// <param name="htVars"></param>
        /// <param name="htOptions"></param>
        /// <returns></returns>
        /// <seealso cref="http://docs.sailthru.com/api/send"/>
        SailthruResponse Send(string strTemplateName, string strEmail, Hashtable htVars = null, Hashtable htOptions = null);

        /// <summary>
        /// Send a transactional Email for a single or multiple users.
        /// </summary>
        /// <param name="request">SendRequest parameters.</param>
        /// <returns></returns>
        SailthruResponse Send(SendRequest request);

        /// <summary>
        /// cancel a future send before it goes out.
        /// </summary>
        /// <param name="sendId"></param>
        /// <returns></returns>
        /// <seealso cref="http://docs.sailthru.com/api/send"/>
        SailthruResponse CancelSend(string sendId);

        /// <summary>
        /// check on the status of a send
        /// </summary>
        /// <param name="sendId"></param>
        /// <returns></returns>
        /// <seealso cref="http://docs.sailthru.com/api/send"/>
        SailthruResponse GetSend(string sendId);

        /// <summary>
        /// Submit a Purchase to Sailthru
        /// </summary>
        /// <param name="request">Purchaserequest parameters.</param>
        /// <returns></returns>
        SailthruResponse Purchase(PurchaseRequest request);

        SailthruResponse ProcessJob(String jobType, String reportEmail, String postbackUrl, Hashtable parameters);

        SailthruResponse ProcessImportJob(String listName, List<String> emails);

        SailthruResponse ProcessImportJob(String reportEmail, String postbackUrl, String listName, List<String> emails);

        SailthruResponse ProcessImportJob(String listName, String filePath);

        SailthruResponse ProcessImportJob(String reportEmail, String postbackUrl, String listName, String filePath);

        /// <summary>
        /// Request various stats from Sailthru.
        /// </summary>
        /// <param name="stat"></param>
        /// <param name="list"></param>
        /// <param name="date"></param>
        /// <param name="htOptions"></param>
        /// <returns></returns>
        /// <seealso cref="http://docs.sailthru.com/api/stats"/>
        SailthruResponse GetStat(String stat, String list = null, String date = null, Hashtable htOptions = null);

        /// <summary> 
        /// Request various stats from Sailthru.
        /// </summary>
        /// <param name="stat"></param>
        /// <param name="template"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="htOptions"></param>
        /// <returns></returns>
        /// <seealso cref="http://docs.sailthru.com/api/stats"/>
        SailthruResponse GetStat(String stat, String template = null, String startDate = null, String endDate = null, Hashtable htOptions = null);

        /// <summary>
        /// Request various stats from Sailthru.
        /// </summary>
        /// <param name="stat"></param>
        /// <param name="htOptions"></param>
        /// <returns></returns>
        /// <seealso cref="http://docs.sailthru.com/api/stats"/>
        SailthruResponse GetStat(String stat, Hashtable htOptions);

        /// <summary>
        /// Set information about one of your users. Users are referenced by multiple keys.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <seealso cref="http://docs.sailthru.com/api/user"/>
        SailthruResponse SetUser (UserRequest request);

        /// <summary>
        /// Get information about one of your users. Users are referenced by multiple keys.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <seealso cref="http://docs.sailthru.com/api/user"/>
        SailthruResponse GetUser (UserRequest request);

        /// <summary>
        /// Get information about one of your urls.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <seealso cref="http://docs.sailthru.com/api/content"/>
        SailthruResponse GetContent (string url);

        /// <summary>
        /// Set information about one of your urls.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <seealso cref="http://docs.sailthru.com/api/content"/>
        SailthruResponse SetContent (ContentRequest request);

        /// <summary>
        /// Get rate limit information for last API call
        /// </summary>
        /// <param name="action">API endpoint</param>
        /// <param name="method">HTTP method</param>
        /// <returns>Hashtable|null</returns>
        Hashtable getLastRateLimitInfo(string action, string method);

        /// For custom API calls that wrappers above don't cover, you can use the below:
        ///
        /// <summary>
        /// For making  API GET Request
        /// </summary>
        /// <param name="action">API Method String</param>
        /// <param name="parameters">API Parameter Hashtable</param>
        /// <returns>SailthruResponse Object</returns>
        SailthruResponse ApiGet(String action, Hashtable parameters);

        /// <summary>
        /// For making  API DELETE Request
        /// </summary>
        /// <param name="action"></param>
        /// <param name="strParams"></param>
        /// <returns>SailthruResponse Object</returns>
        SailthruResponse ApiDelete(string action, Hashtable parameters);

        /// <summary>
        /// For making  API POST Request
        /// </summary>
        /// <param name="action"></param>
        /// <param name="parameters"></param>
        /// <returns>SailthruResponse Object</returns>
        SailthruResponse ApiPost(string action, Hashtable parameters);
    }
}
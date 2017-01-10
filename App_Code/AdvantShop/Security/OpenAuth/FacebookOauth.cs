//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using System.IO;
using Newtonsoft.Json;

namespace AdvantShop.Security.OpenAuth
{
    public class FacebookOauth
    {
        class FacebookUser
        {
            public string id { get; set; }
            public string name { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public string link { get; set; }
            public string gender { get; set; }
            public string email { get; set; }
        }

        public class StatusFacebook
        {
            public enum StatusValue
            {
                Error,
                Success,
                NotFacebook
            }

            public StatusValue Status;
            public string ErrorReason;
            public string Error;
            public string ErrorDescription;
        }

        public struct FacebookUserParams
        {
            //Provides access to the "About Me" section of the profile in the about property
            public const string UserAboutMe = "user_about_me";

            //Provides access to the user's list of activities as the activities connection
            public const string UserActivities = "user_activities";

            //Provides access to the birthday with year as the birthday property
            public const string UserBirthday = "user_birthday";

            //Provides read access to the authorized user's check-ins or a friend's check-ins that the user can see.
            public const string UserCheckins = "user_checkins";

            //Provides access to education history as the education property
            public const string UserEducationHistory = "user_education_history";

            //Provides access to the list of events the user is attending as the events connection
            public const string UserEvents = "user_events";

            //Provides access to the list of groups the user is a member of as the groups connection
            public const string UserGroups = "user_groups";

            //Provides access to the user's hometown in the hometown property
            public const string UserHometown = "user_hometown";

            //Provides access to the user's list of interests as the interests connection
            public const string UserInterests = "user_interests";

            //Provides access to the list of all of the pages the user has liked as the likes connection
            public const string UserLikes = "user_likes";

            //Provides access to the user's current location as the location property
            public const string UserLocation = "user_location";

            //Provides access to the user's notes as the notes connection
            public const string UserNotes = "user_notes";

            //Provides access to the user's online/offline presence
            public const string UserOnlinePresence = "user_online_presence";

            /* Deprecated; not supported after November 22, 2011. 
             * Provides access to the photos and videos the user has uploaded, and photos and videos the user has been tagged in; 
             * this permission is equivalent to requesting both user_photos and user_videos, or friends_photos and friends_videos. */
            public const string UserPhotoVideoTags = "user_photo_video_tags";

            //Provides access to the photos the user has uploaded, and photos the user has been tagged in
            public const string UserPhotos = "user_photos";

            //Provides access to the questions the user or friend has asked
            public const string UserQuestions = "user_questions";

            //Provides access to the user's family and personal relationships and relationship status
            public const string UserRelationships = "user_relationships";

            //Provides access to the user's relationship preferences
            public const string UserRelationshipDetails = "user_relationship_details";

            //Provides access to the user's religious and political affiliations
            public const string UserReligionPolitics = "user_religion_politics";

            //Provides access to the user's most recent status message
            public const string UserStatus = "user_status";

            //Provides access to the videos the user has uploaded, and videos the user has been tagged in
            public const string UserVideos = "user_videos";

            //Provides access to the user's web site URL
            public const string UserWebsite = "user_website";
            //Provides access to work history as the work property
            public const string UserWorkHistory = "user_work_history";

            //Provides access to the user's primary email address in the email property. Do not spam users. Your use of email must comply both with Facebook policies and with the CAN-SPAM Act.
            public const string Email = "email";
        }

        public static IList<string> ScopeParameters;

        public static void ShowAuthDialog(string redirectUrl)
        {
            if (ScopeParameters != null && ScopeParameters.Count > 0)
            {
                var scope = string.Empty;
                for (int i = 0; i < ScopeParameters.Count; ++i)
                {
                    if (i < ScopeParameters.Count - 1)
                        scope += ScopeParameters[i] + ",";
                    else
                        scope += ScopeParameters[i];
                }
                HttpContext.Current.Response.Redirect(string.Format("https://www.facebook.com/dialog/oauth?client_id={0}&redirect_uri={1}&scope={2}"
                    , SettingsOAuth.FacebookClientId
                    , HttpContext.Current.Server.UrlEncode(redirectUrl)
                    , scope));
            }
            else
            {
                HttpContext.Current.Response.Redirect(string.Format("https://www.facebook.com/dialog/oauth?client_id={0}&redirect_uri={1}&scope={2}", SettingsOAuth.FacebookClientId, HttpContext.Current.Server.UrlEncode(redirectUrl), "email,user_about_me"));
            }
        }

        /// <summary>
        /// запрос к facebook на получение access_token ( нужен для запроса данных пользователя )
        /// </summary>
        /// <param name="authorizationCode"></param>
        /// <param name="redirectUrl"></param>
        public static void SendFacebookRequest(string authorizationCode, string redirectUrl)
        {
            try
            {
                var request = WebRequest.Create(string.Format("https://graph.facebook.com/oauth/access_token?client_id={0}&redirect_uri={1}&client_secret={2}&code={3}",
                                                                SettingsOAuth.FacebookClientId,
                                                                HttpContext.Current.Server.UrlEncode(redirectUrl),
                                                                SettingsOAuth.FacebookApplicationSecret,
                                                              authorizationCode));

                request.Method = "GET";

                var response = request.GetResponse();
                if (response != null)
                {
                    var responseStr = (new StreamReader(response.GetResponseStream()).ReadToEnd()).Split(new[] { '&', '=' }, StringSplitOptions.RemoveEmptyEntries);
                    if (responseStr.Length == 4)
                    {
                        GetFacebookUser(responseStr[1]);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message + "authorizationCode: " + authorizationCode + "   redirectUrl: " + redirectUrl + "    request :" + string.Format("https://graph.facebook.com/oauth/access_token?client_id={0}&redirect_uri={1}&client_secret={2}&code={3}",
                                                                SettingsOAuth.FacebookClientId,
                                                                HttpContext.Current.Server.UrlEncode(redirectUrl),
                                                                SettingsOAuth.FacebookApplicationSecret,
                                                              authorizationCode));
            }
            return;
        }

        /// <summary>
        /// получение данных пользователя
        /// </summary>
        /// <param name="accessToken"></param>
        private static void GetFacebookUser(string accessToken)
        {
            var request = WebRequest.Create("https://graph.facebook.com/me?access_token=" + accessToken);
            var response = request.GetResponse();
            if (response != null)
            {
                var facebookUser = JsonConvert.DeserializeObject<FacebookUser>(
                    (new StreamReader(response.GetResponseStream()).ReadToEnd()));
                AuthOrRegCustomer(new Customer
                                      {
                                          FirstName = facebookUser.first_name,
                                          LastName = facebookUser.last_name,
                                          EMail = facebookUser.email,
                                          CustomerGroupId = 1,
                                          Password = Guid.NewGuid().ToString()
                                      });
            }
            return;
        }

        public static void AuthOrRegCustomer(Customer customer)
        {
            OAuthResponce.AuthOrRegCustomer(customer);
        }
    }
}
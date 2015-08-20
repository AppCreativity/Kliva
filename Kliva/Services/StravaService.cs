using Kliva.Models;
using Kliva.Services.Interfaces;
using System;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;

namespace Kliva.Services
{
    public class StravaService : IStravaService
    {
        //private async Task GetAccessToken(string authorizationCode)
        //{
        //}

        public async Task GetAuthorizationCode()
        {
            string authenticationURL = string.Format("{0}?client_id={1}&response_type=code&redirect_uri={2}&scope=view_private&state=mystate&approval_prompt=force", Constants.STRAVA_AUTHORITY_AUTHORIZE_URL, StravaIdentityConstants.STRAVA_AUTHORITY_CLIENT_ID, Constants.STRAVA_AUTHORITY_REDIRECT_URL);

            try
            {
                WebAuthenticationResult webAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, new Uri(authenticationURL), new Uri(Constants.STRAVA_AUTHORITY_REDIRECT_URL));
                if (webAuthenticationResult.ResponseStatus == WebAuthenticationStatus.Success)
                {
                    var responseData = webAuthenticationResult.ResponseData.ToString();
                    //var temporaryAuthCode = ParseCode(responseData);
                    //await GetAccessToken(temporaryAuthCode);
                }
            }
            catch(Exception ex)
            {

            }
        }
    }
}

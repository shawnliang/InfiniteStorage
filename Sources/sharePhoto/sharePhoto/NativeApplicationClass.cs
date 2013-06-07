using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotNetOpenAuth.OAuth2;

namespace Wpf_testHTTP
{
    public class NativeApplicationClient : UserAgentClient
    {
        /// <summary>
        /// Represents a callback URL which points to a special out of band page 
        /// used for native OAuth2 authorization. This URL will cause the authorization 
        /// code to appear in the title of the window.
        /// </summary>
        /// <remarks>
        /// See http://code.google.com/apis/accounts/docs/OAuth2.html
        /// </remarks>
        public const string OutOfBandCallbackUrl = "urn:ietf:wg:oauth:2.0:oob";

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAgentClient"/> class.
        /// </summary>
        /// <param name="authorizationServer">The token issuer.</param>
        /// <param name="clientIdentifier">The client identifier.</param>
        /// <param name="clientSecret">The client secret.</param>
        public NativeApplicationClient(
            AuthorizationServerDescription authorizationServer,
            string clientIdentifier,
            string clientSecret)
            : base(authorizationServer, clientIdentifier, clientSecret)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAgentClient"/> class.
        /// </summary>
        /// <param name="authorizationServer">The token issuer.</param>
        public NativeApplicationClient(
            AuthorizationServerDescription authorizationServer)
            : this(authorizationServer, null, null)
        {
        }

        /// <summary>
        /// Creates the URL which should be used by the user to request the initial 
        /// authorization. Uses the default Out-of-band-URI as a callback.
        /// </summary>
        /// <param name="scope">Set of requested scopes</param>
        /// <returns>URI pointing to the authorization server</returns>
        public Uri RequestUserAuthorization(IEnumerable<string> scope)
        {
            var state = new AuthorizationState(scope);
            state.Callback = new Uri(OutOfBandCallbackUrl);
            return RequestUserAuthorization(state, false, null);
        }

        /// <summary>
        /// Uses the provided authorization code to create an authorization state.
        /// </summary>
        /// <param name="authCode">The authorization code for getting an access token.</param>
        /// <param name="authorizationState">The authorization.  Optional.</param>
        /// <returns>The granted authorization, or <c>null</c> if the authorization was null or rejected.</returns>
        public IAuthorizationState ProcessUserAuthorization(
            string authCode,
            IAuthorizationState authorizationState)
        {
            if (authorizationState == null)
            {
                authorizationState = new AuthorizationState(null);
                authorizationState.Callback = new Uri(OutOfBandCallbackUrl);
            }

            // Build a generic URL containing the auth code.
            // This is done here as we cannot modify the DotNetOpenAuth library 
            // and the underlying method only allows parsing an URL as a method 
            // of retrieving the AuthorizationState.
            string url = "http://example.com/?code=" + authCode;
            return ProcessUserAuthorization(new Uri(url), authorizationState);
        }

        /// <summary>
        /// Uses the provided authorization code to create an authorization state.
        /// </summary>
        /// <param name="authCode">The authorization code for getting an access token.</param>
        /// <returns>The granted authorization, or <c>null</c> if the authorization was null or rejected.</returns>
        public IAuthorizationState ProcessUserAuthorization(string authCode)
        {
            return ProcessUserAuthorization(authCode, null);
        }
    };


}
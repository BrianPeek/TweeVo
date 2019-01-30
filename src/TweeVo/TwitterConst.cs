using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TweeVo
{
    internal static class TwitterConst
    {
        public const string CONSUMER_KEY = "4pTSNMfUNod06joAZXCUjw";
        public const string CONSUMER_SECRET = "OG635c7lPjhDfTYzsazwD5OlEfma1HIq5tAYZPcdBmM";
        public const string AUTHORIZATION = "Authorization";
        public const string CONTENT_TYPE = "application/x-www-form-urlencoded";
        public const string USER_AGENT = "@BrianPeek TweeVo";
        public const string VERIFY_URL = "https://api.twitter.com/1.1/account/verify_credentials.json";
        public const string UPDATE_URL = "https://api.twitter.com/1.1/statuses/update.json";
    }
}

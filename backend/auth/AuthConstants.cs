using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackendApp.Auth
{
    public class AuthConstants
    {
        public static class PolicyNames
        {
            public const string HasIdEqualToUserIdParamPolicyName = "HasIdEqualToUserIdParamPolicy";
            public const string HasIdEqualToSenderIdPolicyName = "HasIdEqualToSenderIdPolicy";
            public const string HasIdEqualToIdParamPolicyName = "HasIdEqualToIdPolicy";
            public const string HasNotificationPolicyName = "HasNotificationPolicy";
            public const string SentMessagePolicyName = "SentMessagePolicy";
            public const string SentConnectionRequestPolicyName = "SentConnectionRequestPolicy";
            public const string ReceivedConnectionRequestPolicyName = "ReceivedConnectionRequestPolicy";
            public const string CreatedJobPolicyName = "CreatedJobPolicy";
            public const string CreatedPostPolicyName = "CreatedPostPolicy";
            public const string IsAdminPolicyName = "IsAdminPolicy";
            public const string IsMemberOfConversationPolicyName = "IsMemberOfConversationPolicy";
            public const string IsMemberOfConnectionPolicyName = "IsMemberOfConnectionPolicy";
        }

        public static class ClaimTypes
        {
            public const string Email = "email";
            public const string Subject = "sub";
            public const string Role = "role";
        }
    }
}
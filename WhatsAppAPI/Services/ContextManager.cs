using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhatsAppAPI.Models;

namespace WhatsAppAPI.Services
{

   
    public class ContextManager
    {
        private static IDictionary<string, UserContext> userStates = new Dictionary<string, UserContext>();


        public UserContext GetUserContext(string userId)
        {
            bool state = userStates.TryGetValue(userId, out UserContext c);

            if (!state) return UserContext.Begin;

            return c;
        }

        public void SetUserContext(string userId, UserContext c)
        {
            userStates[userId] = c;
        }

    }
}

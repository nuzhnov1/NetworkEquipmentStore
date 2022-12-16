﻿using NetworkEquipmentStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace NetworkEquipmentStore.Pages.Helpers
{
    public enum SessionKey
    {
        USER, CART
    }

    public static class SessionHelper
    {
        private static void Set(HttpSessionState session, SessionKey key, object value)
        {
            session[Enum.GetName(typeof(SessionKey), key)] = value;
        }

        private static void Remove(HttpSessionState session, SessionKey key)
        {
            session[Enum.GetName(typeof(SessionKey), key)] = null;
        }

        public static T Get<T>(HttpSessionState session, SessionKey key)
        {
            object dataValue = session[Enum.GetName(typeof(SessionKey), key)];
            
            if (dataValue != null && dataValue is T t)
            {
                return t;
            }
            else
            {
                return default;
            }
        }

        public static void AuthorizeUser(HttpSessionState session, User user)
        {
            Set(session, SessionKey.USER, user);
            
            if (user.Level == PermissionsLevel.CLIENT)
            {
                Set(session, SessionKey.CART, new Cart());
            }
        }

        public static void RemoveUser(HttpSessionState session)
        {
            Remove(session, SessionKey.USER);
            Remove(session, SessionKey.CART);
        }

        public static User GetUser(HttpSessionState session)
        {
            return Get<User>(session, SessionKey.USER);
        }

        public static Cart GetCart(HttpSessionState session)
        {
            return Get<Cart>(session, SessionKey.CART);
        }
    }
}
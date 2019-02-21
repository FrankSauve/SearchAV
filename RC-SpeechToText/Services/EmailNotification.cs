using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RC_SpeechToText.Services
{
    /// <summary>
    /// The 'EmailNotification' abstract class
    /// </summary>
    abstract class EmailNotification
    {
        private List<IUser> _editors = new List<IUser>();

        public void addUser(IUser user)
        {
            _editors.Add(user);
        }

        public void removeUser(IUser user)
        {
            _editors.Remove(user);
        }

        public void Notify()
        {
            foreach (IUser u in _editors)
            {
                u.Update();
                Console.WriteLine("test");

            }
        }
    }
}

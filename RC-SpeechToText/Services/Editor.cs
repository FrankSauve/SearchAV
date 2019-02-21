using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RC_SpeechToText.Services
{
    /// <summary>
    /// The 'Concrete Observer' class
    /// </summary>
    public class Editor : IUser
    {
        private string _name;
        private string _email;

        // Constructor
        public Editor(string name, string email)
        {
            this._name = name;
            this._email = email;
        }

        public void Update()
        {
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdzerSchedulingSystem.Models
{
    public class User
    {
        private int _userID;
        public int userID
        {
            get { return this._userID; }
            set { _userID = value; }
        }

        private string _username;
        public string username
        {
            get { return this._username; }
            set { _username = value; }
        }

        private string _password;
        public string password
        {
            get { return this._password; }
            set { _password = value; }
        }

        private bool _isAdmin;
        public bool isAdmin
        {
            get { return this._isAdmin; }
            set { _isAdmin = value; }
        }

        public User(int userID, string username, string password, bool isAdmin)
        {
            _userID = userID;
            _username = username;
            _password = password;
            _isAdmin = isAdmin;
        }
    }
}

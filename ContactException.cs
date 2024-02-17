using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContactList
{
    public class ContactsException : Exception
    {
        public ContactsException()
        {

        }

        public ContactsException(string message) : base (message)
        {
            
        }

        public ContactsException(string message, Exception inner)
        : base(message, inner)
    {
    }
    }
}
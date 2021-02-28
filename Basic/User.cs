using System;
using System.Text;

namespace Basic
{
    class User
    {
        protected String guid;
        protected String name;
        protected String pubKey;

        public String GUID { get { return guid; } }
        public String Name { get { return name; } }
        public String PubKey { get { return pubKey; } }

        public User(String guid, String name, String pubKey)
        {
            this.guid = guid;
            this.name = name;
            this.pubKey = pubKey;
        }

        public User()
        {

        }
    }

    class CurrentUser : User
    {
        String privKey;

        public String PrivKey { get { return privKey; } }


        public CurrentUser(String guid, String name, String pubKey, String privKey)
        {
            this.guid = guid;
            this.name = name;
            this.pubKey = pubKey;
            this.privKey = privKey;
        }
    }
}
